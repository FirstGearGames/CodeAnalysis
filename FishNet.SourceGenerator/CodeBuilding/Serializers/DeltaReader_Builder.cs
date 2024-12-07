using System;
using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.FishNet.SyncTypes;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using Roslyn.FishNet.CodeBuilding;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding
{
    public class DeltaReader_Builder
    {
        private const string InitializeOnLoad_Method_Name = DeltaWriter_Builder.InitializeOnLoad_Method_Name;
        private const string Generated_Class_Name = "Generated_DeltaReaders";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedReaderPrefix}ReadDelta";
        private const string Generated_ReaderParameter_Name = "reader";
        private const string Generated_DeltaSerializerOption_Name = DeltaWriter_Builder.Generated_DeltaSerializerOption_Name;

        private static readonly StringBuilder _stringBuilder = new();

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Serializers _serializers => _generator.Serializers;
        private SerializableGenerator _generator;
        private GeneratorExecutionContext _context;
        private GeneratorSyntaxReceiver _rootSyntaxReceiver;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, SerializableGenerator generator)
        {
            Log("");
            Log("Initialize.");

            _context = context;
            _rootSyntaxReceiver = rootSyntaxReceiver;
            _generator = generator;
        }

        public void CreateEmptySerializerMethods() => CreateEmptySerializerMethods(_context, _rootSyntaxReceiver);
        public void CreateSerializerBodies() => CreateSerializerBodies(_context, _rootSyntaxReceiver);
        public void CreateGeneratedSerializersClass() => CreateGeneratedSerializersClass(_context);

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableReceiver.TypesNeedingSerializers)
                CreateEmptyDeltaSerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty serializer method for a type.
        /// </summary>
        private void CreateEmptyDeltaSerializerMethod(GeneratorExecutionContext context, SerializableType serializableType, int recursiveCount = 1)
        {
            string typeFullName = serializableType.FullName;
            Log($"{recursiveCount.ToIndent()}Trying to create writer for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetReadMethod(serializableType.TypeSymbol, GetSerializerType.Delta, metadataName: false, out _).IsValid())
                return;

            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
                return;
            if (!_serializers.CanCreateDeltaSerializer(namedTypeSymbol))
                return;

            List<IFieldSymbol> serializableFields = _serializers.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                ITypeSymbol typeSymbol = item.Type;
                CreateEmptyDeltaSerializerMethod(context, new SerializableType(typeSymbol), recursiveCount + 1);
            }

            string header = GetMethodHeader(out string methodName);
            //Add to readers.
            _serializers.AddReadMethod(new GeneratedDeltaSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Delta);

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                sb.Append(2, $"public static {typeFullName} {mName}" + $"(this {FishNetConstants.Reader_FullName} {Generated_ReaderParameter_Name}," + $" {typeFullName} {_serializers.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetReadDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                Log($"Creating serializer body for {item.Value.TypeFullName}.");

                const int bodyIndent = 3;
                StringBuilder sb = new();

                //Make a new instance of the type to return.
                const string resultVariableName = "result";
                sb.AppendLine(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(item.Key, resultVariableName, "new()"));

                /* ulong totalFlags = reader.ReadPackedWhole(); */
                string totalFlagsVariableName = "totalFlags";
                sb.Append(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariableName, string.Empty, false));
                sb.AppendLine($" = {RoslynCodeBuilder.CallMethod(FishNetConstants.Reader_ReadUnsignedPackedWhole_Name, Generated_ReaderParameter_Name)}{NativeConstants.LineFeed}");

                /* DeltaSerializerOption options = (DeltaSerializerOption)totalFlags;
                 * if (options.FastContains(DeltaSerializerOption.FullSerializer))
                 *      return reader.Read<type>(); */
                CreateReadFullIf();

                void CreateReadFullIf()
                {
                    ITypeSymbol typeSymbol = item.Value.TypeSymbol;
                    SerializerMethod sm = _serializers.GetReadMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);

                    string optionsVariableName = $"options";
                    sb.Append(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(FishNetConstants.DeltaSerializerOption_FullName, optionsVariableName, closeLine: false));
                    sb.AppendLine($" = ({FishNetConstants.DeltaSerializerOption_FullName}){totalFlagsVariableName};");

                    sb.AppendLine(bodyIndent, $"if ({optionsVariableName}.{FishNetConstants.FastContains_Name}({FishNetConstants.DeltaSerializerOption_FullSerialize_FullName}))");
                    sb.AppendLine(bodyIndent, "{");

                    if (!sm.IsValid())
                        sb.AppendLine(bodyIndent + 1, CodeBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol));
                    else
                        sb.AppendLine(bodyIndent + 1, $"{resultVariableName} = {Generated_ReaderParameter_Name}.{sm.MethodName}();{NativeConstants.LineFeed}");

                    sb.AppendLine(bodyIndent + 1, $"return {resultVariableName};");
                    sb.AppendLine(bodyIndent, "}" + NativeConstants.LineFeed);
                }

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.TypeSymbol);

                ulong fieldFlag = (FishNetConstants.DeltaSerializerOption_MaxValue * 2);
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;
                    Debugg.Log("");
                    Log($"Getting reader for field name {fieldSymbol.Name}");
                    SerializerMethod sm = _serializers.GetReadMethod(typeSymbol, GetSerializerType.FavorDelta, metadataName: false, out _);

                    //Neither delta nor full could be found.
                    if (!sm.IsValid())
                    {
                        sb.AppendLine(bodyIndent, CodeBuilder.GetMissingSerializerComment(deltaSerializer: false, item.Value.TypeSymbol, fieldSymbol));
                        continue;
                    }

                    //if ((totalFlags & flag) == flag)
                    sb.AppendLine(bodyIndent, ($"if (({totalFlagsVariableName} & {fieldFlag}) == {fieldFlag})"));

                    //If is a full serializer.
                    if (!sm.IsDeltaSerializer())
                    {
                        sb.AppendLine(bodyIndent + 1, CodeBuilder.GetMissingSerializerComment(deltaSerializer: true, item.Value.TypeSymbol, fieldSymbol));
                        sb.AppendLine(bodyIndent + 1, _generator.ReaderBuilder.GetReadCall(sm, resultVariableName, Generated_ReaderParameter_Name, fieldSymbol, closeCall: true));
                    }
                    else
                    {
                        //If here then is a delta serializer.
                        DeltaSerializerMethod dsm = sm as DeltaSerializerMethod;
                        sb.AppendLine(bodyIndent + 1, GetReadDeltaCall(dsm, resultVariableName, Generated_ReaderParameter_Name, fieldSymbol, closeCall: true));
                    }

                    /* else
                     *      result.FieldName = previousResult.FieldName; */
                    sb.AppendLine(bodyIndent, ("else"));
                    sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {_serializers.GetValueParameterName(0)}.{fieldSymbol.Name};{NativeConstants.LineFeed}");
                    fieldFlag *= 2;
                }

                sb.Append(bodyIndent, $"return {resultVariableName};");

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedSerializersClass(GeneratorExecutionContext context)
        {
            StringBuilder sb = new();

            string clsText = RoslynCodeBuilder.CreatePublicStaticClass(Generated_Class_Name, out string footer, FishNetConstants.Serializing_Namespace);
            sb.AppendLine(clsText);

            const int initializeIndent = 2;
            MethodContent initializeMethod = CodeBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);

            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetReadDeltaMethods())
            {
                if (item.Value is not GeneratedDeltaSerializerMethod dsm) continue;

                sb.AppendLine(dsm.MethodContent.ToString(2));

                //Add return if body already contains value. This is just to make neater formatting.
                if (initializeMethod.Body.Length > 0)
                    initializeMethod.Body.AppendLine();

                initializeMethod.Body.Append(initializeIndent + 1, CreateInitializeFunction(dsm));
            }

            sb.Append(initializeMethod.ToString(initializeIndent));
            sb.AppendLine(footer);

            context.AddSource($"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs", sb.ToString());
        }

        /// <summary>
        /// Creates a call to the set method for generic serializers.
        /// </summary>
        private string CreateInitializeFunction(GeneratedDeltaSerializerMethod dsm)
        {
            StringBuilder sb = new();
            //GenericDeltaSerializer<Type>.SetWrite(
            sb.Append($"{FishNetConstants.GenericDeltaReader_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericDeltaReader_SetRead_Name}(");
            sb.Append($"{RoslynCodeBuilder.CreateFunction(dsm.TypeFullName, FishNetConstants.Reader_FullName, dsm.TypeFullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        /// <summary>
        /// Returns a SerializerMethod for a DefaultWriter of a type. Optionally returns a call to Write<T> if a DefaultWriter is not found.
        /// </summary>
        private SerializerMethod GetFullReader(string typeFullName, out bool found)
        {
            SerializerMethod sm = _serializers.GetReadMethod(typeFullName, GetSerializerType.Full);

            found = sm.IsValid();

            return sm;
        }

        private string GetReadDeltaCall(DeltaSerializerMethod dsm, string resultVariableName, string readerVariableName, IFieldSymbol fieldSymbol, bool closeCall)
        {
            if (!dsm.IsValid())
                return string.Empty;

            ITypeSymbol fieldSymbolType = fieldSymbol.Type;
            string fieldName = fieldSymbol.Name;

            string arguments;
            if (!dsm.AreGenericsNamed && dsm.GenericArguments.Count > 0)
                arguments = $"{fieldSymbolType.GetGenericArgumentsString(GenericArgumentType.PreferNamed).GetCombinedGenericArguments(fieldSymbolType)}";
            else
                arguments = string.Empty;

            // if (dsm.IsGenerated())
            //     return $"{resultVariableName}.{fieldName} = {RoslynCodeBuilder.CallMethod($"ReadDelta{arguments}", readerVariableName, closeCall,
            //         $"{_serializers.GetValueParameterName(0)}.{fieldName}")}";
            // else
            return $"{resultVariableName}.{fieldName} = {RoslynCodeBuilder.CallMethod($"{dsm.MethodName}{arguments}", readerVariableName, closeCall,
                $"{_serializers.GetValueParameterName(0)}.{fieldName}")}";
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [DeltaReader_Builder] {txt}");
        }
    }
}