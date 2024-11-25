using System;
using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet;
using FirstGearGames.Roslyn.FishNet.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace Roslyn.FishNet.CodeBuilding
{
    public class DeltaWriter_Builder
    {
        public const string InitializeOnLoad_Method_Name = Writer_Builder.InitializeOnLoad_Method_Name;
        public const string Generated_DeltaSerializerOption_Name = "options";
        private const string Generated_Class_Name = "Generated_DeltaWriters";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedWriterPrefix}WriteDelta";
        private const string Generated_WriterParameter_Name = Writer_Builder.Generated_WriterParameter_Name;

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
            Log($"Initialize.");
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
            if (_serializers.GetWriteMethod(serializableType.TypeSymbol, GetSerializerType.Delta, metadataName: false, out _).IsValid())
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
            //Add to writers.
            _serializers.AddWriteMethod(new GeneratedDeltaSerializerMethod(namedTypeSymbol, methodName, header, string.Empty), AddSerializerType.Delta);

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                sb.Append(2, $"public static bool {mName}" + $"(this {FishNetConstants.Writer_FullName} {Generated_WriterParameter_Name}," + $" {typeFullName} {_serializers.GetValueParameterName(0)}, {typeFullName} {_serializers.GetValueParameterName(1)}" + $", {FishNetConstants.DeltaSerializerOption_FullName} {Generated_DeltaSerializerOption_Name} = {FishNetConstants.DeltaSerializerOption_Unset_FullName})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver SyntaxReceiver)
        {
            //Iterate all serializers and if they are generated then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                Log($"Creating serializer body for {item.Value.TypeFullName}.");

                const int bodyIndent = 3;
                StringBuilder sb = new();

                //totalFlags and pooledWriter local variables.
                string totalFlagsVariableName = "totalFlags";
                sb.AppendLine(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariableName, $"(ulong){Generated_DeltaSerializerOption_Name};{NativeConstants.LineFeed}", closeLine: false));

                /* If (options.FastContains(DeltaSerializerOption.FullSerialize))
                 * {
                 *      writer.Write(type);
                 *      return;
                 * } */
                CreateWriteFullIf();

                void CreateWriteFullIf()
                {
                    ITypeSymbol typeSymbol = item.Value.TypeSymbol;
                    SerializerMethod sm = _serializers.GetWriteMethod(typeSymbol, GetSerializerType.Full, metadataName: false, out _);

                    sb.AppendLine(bodyIndent, $"if ({Generated_DeltaSerializerOption_Name}.{FishNetConstants.FastContains_Name}({FishNetConstants.DeltaSerializerOption_FullSerialize_FullName}))");
                    sb.AppendLine(bodyIndent, "{");

                    //writer.WriteUnsignedPackedWhole(flags); */
                    sb.AppendLine(bodyIndent + 1, RoslynCodeBuilder.CallMethod(FishNetConstants.Writer_WriteUnsignedPackedWhole_Name, Generated_WriterParameter_Name, true, totalFlagsVariableName));

                    if (!sm.IsValid())
                        sb.AppendLine(bodyIndent + 1, $"//Serializer not found for {typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false)}. Type will not be serialized.{NativeConstants.LineFeed}");
                    else
                        sb.AppendLine(bodyIndent + 1, $"{Generated_WriterParameter_Name}.{sm.MethodName}({_serializers.GetValueParameterName(1)});");

                    sb.Append(bodyIndent + 1, $"return true;{NativeConstants.LineFeed}");
                    sb.AppendLine(bodyIndent, "}" + NativeConstants.LineFeed);
                }

                //Starting flag for each modified field.
                ulong fieldFlag = (FishNetConstants.DeltaSerializerOption_MaxValue * 2);

                //PooledWriter local variable.
                sb.AppendLine(bodyIndent, CodeBuilder.CallGetPooledWriter(out string tmpWriterVariableName) + NativeConstants.LineFeed);

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.TypeSymbol);

                //Call write for all members.
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    ITypeSymbol typeSymbol = fieldSymbol.Type;
                    SerializerMethod sm = _serializers.GetWriteMethod(typeSymbol, GetSerializerType.FavorDelta, metadataName: false, out _);

                    //Neither delta nor full could be found.
                    if (!sm.IsValid())
                    {
                        sb.AppendLine(bodyIndent, $"//Serializer not found for type {typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false)}. Type will not be serialized.{NativeConstants.LineFeed}");
                        continue;
                    }

                    //If is a full serializer.
                    if (!sm.IsDeltaSerializer())
                    {
                        sb.AppendLine(bodyIndent, $"//Delta serializer not found for type {typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false)}. Full serializer will be used.");
                        sb.AppendLine(bodyIndent, _generator.WriterBuilder.GetWriteCall(sm, tmpWriterVariableName, fieldSymbol.Name, closeCall: true));
                    }
                    else
                    {
                        //If here then is a delta serializer.
                        DeltaSerializerMethod dsm = sm as DeltaSerializerMethod;

                        /* if (writer.WriteDeltaXYZ(p0, p1))
                            totalFlags += x */
                        string writeDeltaText = GetWriteDeltaCall(dsm, tmpWriterVariableName, fieldSymbol.Name, closeCall: false);
                        sb.AppendLine(bodyIndent, $"if ({writeDeltaText})");
                    }

                    AppendIncreaseTotalFlags(4);

                    sb.AppendLine();

                    void AppendIncreaseTotalFlags(int indent)
                    {
                        sb.AppendLine(indent, $"{totalFlagsVariableName} += {fieldFlag};");
                    }

                    fieldFlag *= 2;
                }

                string changedVariable = "changed";
                //System.Boolean changed = (totalFlags != 0) || rootSerializer;
                sb.AppendLine(bodyIndent, $"{NativeConstants.Boolean_FullName} {changedVariable} = ({totalFlagsVariableName} != 0 || {Generated_DeltaSerializerOption_Name} == {FishNetConstants.DeltaSerializeOption_RootSerialize_FullName});");

                /* if (changed)
                 {
                    writer.WritePackedWhole(totalFlags); */
                sb.AppendLine(bodyIndent, $"if ({changedVariable})");
                sb.AppendLine(bodyIndent, "{");
                sb.AppendLine(bodyIndent + 1, RoslynCodeBuilder.CallMethod(FishNetConstants.Writer_WriteUnsignedPackedWhole_Name, Generated_WriterParameter_Name, true, totalFlagsVariableName));
                /*  writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
                 } */
                sb.AppendLine(bodyIndent + 1, CodeBuilder.CallWriteArraySegment(Generated_WriterParameter_Name, tmpWriterVariableName));
                sb.AppendLine(bodyIndent, "}");
                //store tmpWriter.
                sb.AppendLine(bodyIndent, CodeBuilder.CallStorePooledWriter(tmpWriterVariableName) + NativeConstants.LineFeed);
                /* Struct/class writers must always return true. This is so if they are being encapsulated
                 * the flags written will be read, even if that flag is 0. */
                sb.Append(bodyIndent, $"return {changedVariable};");

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
            sb.AppendLine($"//FishNet generated file.");
            sb.AppendLine(clsText);

            const int initializeIndent = 2;
            MethodContent initializeMethod = CodeBuilder.CreatePublicRuntimeInitializeOnLoadMethod(initializeIndent, InitializeOnLoad_Method_Name);

            int addedSerializers = 0;

            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                if (item.Value is not GeneratedDeltaSerializerMethod dsm) continue;

                sb.AppendLine(dsm.MethodContent.ToString(2));

                //Add return if body already contains value. This is just to make neater formatting.
                if (initializeMethod.Body.Length > 0)
                    initializeMethod.Body.AppendLine();

                initializeMethod.Body.Append(initializeIndent + 1, CreateInitializeFunction(dsm));

                addedSerializers++;
            }

            sb.Append(initializeMethod.ToString(initializeIndent));
            sb.AppendLine(footer);

            string fileName = $"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs";
            context.AddSource($"{fileName}", sb.ToString());

            Log($"Added class {fileName}. Added serializer count is {addedSerializers}.");
        }

        /// <summary>
        /// Creates a call to the set method for generic serializers.
        /// </summary>
        private string CreateInitializeFunction(GeneratedDeltaSerializerMethod dsm)
        {
            StringBuilder sb = new();
            // //GenericDeltaSerializer<Type>.SetWrite(
            sb.Append($"{FishNetConstants.GenericDeltaWriter_FullName}<{dsm.TypeFullName}>.{FishNetConstants.GenericDeltaWriter_SetWrite_Name}(");
            sb.Append($"{RoslynCodeBuilder.CreateFunction(NativeConstants.Boolean_FullName, FishNetConstants.Writer_FullName, dsm.TypeFullName, dsm.TypeFullName, FishNetConstants.DeltaSerializerOption_FullName)}");
            sb.Append($"({dsm.MethodName}));");

            return sb.ToString();
        }

        public string GetWriteDeltaCall(DeltaSerializerMethod dsm, string writerVariableName, string fieldName, bool closeCall)
        {
            if (!dsm.IsValid())
                return string.Empty;
            
            if (dsm.IsGenerated())
                return RoslynCodeBuilder.CallMethod($"WriteDelta", writerVariableName, closeCall, $"{_serializers.GetValueParameterName(0)}.{fieldName}", $"{_serializers.GetValueParameterName(1)}.{fieldName}", $"{FishNetConstants.DeltaSerializerOption_Unset_FullName}");
            else
                return RoslynCodeBuilder.CallMethod($"{dsm.MethodName}", writerVariableName, closeCall, $"{_serializers.GetValueParameterName(0)}.{fieldName}", $"{_serializers.GetValueParameterName(1)}.{fieldName}");
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [DeltaWriter_Builder] {txt}");
        }
    }
}