using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.Serializing;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;
using Roslyn.FishNet.CodeBuilding;
using RoslynCodeBuilder = FirstGearGames.Roslyn.CodeBuilding.CodeBuilder;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding
{
    public class DeltaReader_Builder
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Serializers _serializers;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private const string Generated_Class_Name = "Generated_DeltaReaders";
        private const string Generated_Method_Prefix = $"{FishNetConstants.GeneratedReaderPrefix}ReadDelta";
        private const string Generated_ReaderParameter_Name = "reader";
        private const string Generated_DeltaSerializerOption_Name = DeltaWriter_Builder.Generated_DeltaSerializerOption_Name;
        public const string InitializeOnLoad_Method_Name = DeltaWriter_Builder.InitializeOnLoad_Method_Name;

        public void Initialize(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver, Serializers serializers)
        {
            _serializers = serializers;

            //Create all stub(empty) delta methods.
            CreateEmptySerializerMethods(context, rootSyntaxReceiver);
            //Create all bodies for delta methods.
            CreateDeltaSerializerBodies(context, rootSyntaxReceiver);
            //Create delta serializers class adding generated serializers.
            CreateGeneratedDeltaSerializersClass(context);
        }

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, GeneratorSyntaxReceiver syntaxReceiver)
        {
            foreach (SerializableType item in syntaxReceiver.SerializableReceiver.SerializableTypes)
                CreateEmptyDeltaSerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptyDeltaSerializerMethod(GeneratorExecutionContext context, SerializableType serializableType, int recursiveCount = 1)
        {
            string typeFullName = serializableType.FullName;
            Debugg.Log($"{recursiveCount.ToIndent()}Trying to create reader for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetReadMethod(typeFullName, GetSerializerType.Delta).IsValid())
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
                CreateEmptyDeltaSerializerMethod(context, new SerializableType(typeSymbol, SerializableType.TypeExposure.Public), recursiveCount + 1);
            }

            string header = GetMethodHeader(out string methodName);

            //Add to readers.
            _serializers.AddReadMethod(new GeneratedDeltaSerializerMethod(2, namedTypeSymbol, typeFullName, methodName, header, ""), AddSerializerType.Delta);

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                inText = "";
                sb.Append(2, $"public static {typeFullName} {mName}" + $"(this {FishNetConstants.Reader_FullName} {Generated_ReaderParameter_Name}," + $" {inText}{typeFullName} {_serializers.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateDeltaSerializerBodies(GeneratorExecutionContext context, GeneratorSyntaxReceiver rootSyntaxReceiver)
        {
            //Iterate all serializers and if they are generated delta writers then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetReadDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                const int bodyIndent = 3;
                StringBuilder sb = new();

                //Make a new instance of the type to return.
                const string resultVariableName = "result";
                sb.AppendLine(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(item.Key, resultVariableName, "new()"));

                string totalFlagsVariable = "totalFlags";
                sb.Append(bodyIndent, RoslynCodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariable, string.Empty, false));
                sb.AppendLine($" = {RoslynCodeBuilder.CallMethod(FishNetConstants.Reader_ReadUnsignedPackedWhole_Name, Generated_ReaderParameter_Name)}");
                sb.AppendLine();

                /* DeltaSerializerOption options = (DeltaSerializerOption)totalFlags;
                 * if (options.FastContains(DeltaSerializerOption.FullSerializer))
                 *      return reader.Read<type>(); */
                CreateReadFullIf();

                void CreateReadFullIf()
                {
                    string optionsVariable = Generated_DeltaSerializerOption_Name;
                    sb.AppendLine(bodyIndent, $"{FishNetConstants.DeltaSerializerOption_FullName} {optionsVariable} = ({FishNetConstants.DeltaSerializerOption_FullName}){totalFlagsVariable};");

                    SerializerMethod fullSerializerMethod = GetFullReader(item.Key, true, out _);
                    sb.AppendLine(bodyIndent, $"if ({optionsVariable}.{FishNetConstants.FastContains_Name}({FishNetConstants.DeltaSerializerOption_FullSerialize_FullName}))");
                    sb.AppendLine(bodyIndent + 1, $"return {Generated_ReaderParameter_Name}.{fullSerializerMethod.MethodName}();");
                    sb.AppendLine();
                }

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.NamedTypeSymbol);

                ulong fieldFlag = (FishNetConstants.DeltaSerializerOption_MaxValue * 2);
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    //if ((totalFlags & flag) == flag)
                    sb.AppendLine(bodyIndent, ($"if (({totalFlagsVariable} & {fieldFlag}) == {fieldFlag})"));

                    //Get information on which read method to call.
                    string typeFullName = fieldSymbol.Type.GetTypeFullName();
                    //Get delta writer method for the field.
                    DeltaSerializerMethod? dsm = _serializers.GetReadMethod(typeFullName, GetSerializerType.Delta) as DeltaSerializerMethod;
                    //Delta reader not found.
                    if (!dsm.IsValid())
                    {
                        SerializerMethod sm = GetFullReader(typeFullName, true, out bool _);
                        sb.AppendLine(bodyIndent, $"//Delta reader could not be found for type {typeFullName}. Please report this note.");
                        sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {Generated_ReaderParameter_Name}.{sm.MethodName}();");
                    }
                    //Delta reader found.
                    else
                    {
                        // sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {RoslynCodeBuilder.CallMethod(dsm!.MethodName, Generated_ReaderParameter_Name, true,
                        //     $"{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}")}");

                        //TODO get working to call method directly once delta serializers are fully supported.
                        if (dsm!.IsGenerated())
                        {
                            sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {RoslynCodeBuilder.CallMethod("ReadDelta", Generated_ReaderParameter_Name, true,
                                $"{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}")}");
                        }
                        else 
                        {
                            sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {RoslynCodeBuilder.CallMethod(dsm!.MethodName, Generated_ReaderParameter_Name, true,
                                $"{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}")}");
                        }

                    }

                    /* else
                     *      result.FieldName = previousResult.FieldName; */
                    sb.AppendLine(bodyIndent, ($"else"));
                    sb.AppendLine(bodyIndent + 1, $"{resultVariableName}.{fieldSymbol.Name} = {_serializers.GetValueParameterName(0)}.{fieldSymbol.Name};");
                    sb.AppendLine();
                    fieldFlag *= 2;
                }

                sb.Append(bodyIndent, $"return {resultVariableName};");

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedDeltaSerializersClass(GeneratorExecutionContext context)
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
        private SerializerMethod GetFullReader(string typeFullName, bool callReadIfNull, out bool found)
        {
            SerializerMethod sm = _serializers.GetReadMethod(typeFullName, GetSerializerType.Full) as SerializerMethod;
            found = sm.IsValid();
            if (callReadIfNull && !sm.IsValid())
                sm = new SerializerMethod(typeFullName, $"{FishNetConstants.Reader_Read_Name}<{typeFullName}>");

            return sm;
        }
    }
}