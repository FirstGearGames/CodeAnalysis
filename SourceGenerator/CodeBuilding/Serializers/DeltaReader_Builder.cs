using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using RoslynLearning.CodeBuilding;
using RoslynLearning.Helpers;
using SourceGenerating.Constants;
using SourceGenerating.SyntaxReceivers;
using SourceGenerator.Extensions;

namespace SourceGenerator.CodeBuilding.Serializers
{
    internal class DeltaReader_Builder
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private Serializers _serializers;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private const string Generated_Class_Name = "Generated_DeltaReaders";
        private const string Generated_Method_Prefix = "ReadDelta";
        private const string Generated_ReaderParameter_Name = "reader";
        private string Generated_DeltaSerializerOption_Name => DeltaWriter_Builder.Generated_DeltaSerializerOption_Name;

        public void Initialize(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver, Serializers serializers)
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
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            foreach (SerializableType item in rootSyntaxReceiver.SerializableTypes)
                CreateEmptyDeltaSerializerMethod(context, item);
        }

        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptyDeltaSerializerMethod(GeneratorExecutionContext context, SerializableType serializableType)
        {
            string typeFullName = serializableType.FullName;
            Debugg.Log($"Trying to create reader for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetReadMethod(typeFullName, GetSerializerType.Delta).IsValid())
            {
                Debugg.Log($"Read method is not valid.");
                return;
            }

            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(serializableType.FullMetadataName);
            if (namedTypeSymbol == null)
            {
                Debugg.Log($"namedType not resolved.");
                return;
            }
            if (!_serializers.CanCreateDeltaSerializer(namedTypeSymbol, true))
            {
                Debugg.Log($"Cannot create serializer.");
                return;
            }

            Debugg.Log($"continuing...");
            List<IFieldSymbol> serializableFields = _serializers.GetSerializableFieldSymbols(namedTypeSymbol);
            foreach (IFieldSymbol item in serializableFields)
            {
                ITypeSymbol typeSymbol = item.Type;
                CreateEmptyDeltaSerializerMethod(context, new SerializableType(typeSymbol.GetTypeFullName(), typeSymbol.GetSymbolFullMetaName()));
            }

            string header = GetMethodHeader(out string methodName);
            
            //Add to readers.
            _serializers.AddReadMethod(new GeneratedDeltaSerializerMethod(2, namedTypeSymbol, typeFullName, methodName, header, ""), AddSerializerType.Delta);

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                sb.AppendLine($"public static {typeFullName} {mName}" +
                                        $"(this {FishNetConstants.Reader_FullName} {Generated_ReaderParameter_Name}," +
                                        $" {inText}{typeFullName} {_serializers.GetValueParameterName(0)})");

                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateDeltaSerializerBodies(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            //Iterate all serializers and if they are generated delta writers then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetReadDeltaMethods())
            {
                //Skip built in serializers.
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                    continue;

                StringBuilder sb = new();

                //Make a new instance of the type to return.
                const string resultVariableName = "result";
                sb.AppendLine(3, CodeBuilder.CreateLocalVariable(item.Key, resultVariableName, "new()"));

                string totalFlagsVariable = "totalFlags";
                sb.Append(3, CodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariable, string.Empty, false));
                sb.AppendLine($" = {CodeBuilder.CallMethod(FishNetConstants.Reader_ReadUnsignedPackedWhole_Name, Generated_ReaderParameter_Name)}");
                sb.AppendLine();

                /* DeltaSerializerOption options = (DeltaSerializerOption)totalFlags;
                 * if (options.FastContains(DeltaSerializerOption.FullSerializer))
                *      return reader.Read<type>(); */
                CreateReadFullIf();
                void CreateReadFullIf()
                {
                    string optionsVariable = DeltaWriter_Builder.Generated_DeltaSerializerOption_Name;
                    sb.AppendLine(3, $"{FishNetConstants.DeltaSerializerOption_FullName} {optionsVariable} = ({FishNetConstants.DeltaSerializerOption_FullName}){totalFlagsVariable};");

                    SerializerMethod fullSerializerMethod = GetFullReader(item.Key, true, out _);
                    sb.AppendLine(3, $"if ({optionsVariable}.{FishNetConstants.FastContains_Name}({FishNetConstants.DeltaSerializerOption_FullSerialize_FullName}))");
                    sb.AppendLine(4, $"return {Generated_ReaderParameter_Name}.{fullSerializerMethod.MethodName}();");
                    sb.AppendLine();
                }

                List<IFieldSymbol> serializableFieldSymbols = _serializers.GetSerializableFieldSymbols(gsm.NamedTypeSymbol);

                ulong fieldFlag = (FishNetConstants.DeltaSerializerOption_MaxValue * 2);
                foreach (IFieldSymbol fieldSymbol in serializableFieldSymbols)
                {
                    //if ((totalFlags & flag) == flag)
                    sb.AppendLine(3, ($"if (({totalFlagsVariable} & {fieldFlag}) == {fieldFlag})"));

                    //Get information on which read method to call.
                    string typeFullName = fieldSymbol.Type.GetTypeFullName();
                    //Get delta writer method for the field.
                    DeltaSerializerMethod? dsm = _serializers.GetReadMethod(typeFullName, GetSerializerType.Delta) as DeltaSerializerMethod;
                    //Delta reader not found.
                    if (!dsm.IsValid())
                    {
                        SerializerMethod sm = GetFullReader(typeFullName, true, out bool _);
                        sb.AppendLine(3, $"//Delta reader could not be found for type {typeFullName}. Please report this note.");
                        sb.AppendLine(4, $"{resultVariableName}.{fieldSymbol.Name} = {Generated_ReaderParameter_Name}.{sm.MethodName}();");
                    }
                    //Delta reader found.
                    else
                    {
                        sb.AppendLine(4, $"{resultVariableName}.{fieldSymbol.Name} = {CodeBuilder.CallMethod(dsm!.MethodName, Generated_ReaderParameter_Name, true,
                            $"{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}")}");
                    }

                    /* else
                     *      result.FieldName = previousResult.FieldName; */
                    sb.AppendLine(3, ($"else"));
                    sb.AppendLine(4, $"{resultVariableName}.{fieldSymbol.Name} = {_serializers.GetValueParameterName(0)}.{fieldSymbol.Name};");
                    sb.AppendLine();
                    fieldFlag *= 2;
                }

                sb.Append(3, $"return {resultVariableName};");

                gsm.MethodContent.Body = sb;
            }
        }

        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedDeltaSerializersClass(GeneratorExecutionContext context)
        {
            StringBuilder sb = new();

            string clsText = CodeBuilder.CreatePublicStaticClass(Generated_Class_Name, out string footer, FishNetConstants.Serializing_Namespace);
            sb.AppendLine(clsText);

            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetReadDeltaMethods())
            {
                if (item.Value is not GeneratedDeltaSerializerMethod dsm) continue;

                sb.AppendLine(dsm.MethodContent.ToString());
            }

            sb.AppendLine(footer);

            context.AddSource($"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs", sb.ToString());
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