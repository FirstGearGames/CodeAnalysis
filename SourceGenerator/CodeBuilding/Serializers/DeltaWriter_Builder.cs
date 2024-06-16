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
    internal class DeltaWriter_Builder
    {
        private Serializers _serializers;
        private const string Generated_Class_Name = "Generated_DeltaWriters";
        private const string Generated_Method_Prefix = "WriteDelta";
        private const string Generated_WriterParameter_Name = "writer";
        private const string Generated_WriteFullParameter_Name = "writeFull";
        private const string Generated_RootCallParameter_Name = "rootCall";
        
        public void Initialize(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver, Serializers serializers)
        {
            _serializers = serializers;

            //Create all stub(empty) delta methods.
            CreateEmptySerializerMethods(context, rootSyntaxReceiver);

            //Create all bodies for delta methods.
            CreateSerializerBodies(context, rootSyntaxReceiver);

            //Create delta serializers class adding generated serializers.
            CreateGeneratedDeltaSerializersClass(context);
        }

        /// <summary>
        /// Creates SerializerMethod for each type in need.
        /// </summary>
        private void CreateEmptySerializerMethods(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            foreach (string item in rootSyntaxReceiver.SerializableTypes)
                CreateEmptyDeltaSerializerMethod(context, item);
        }

        /// <summary>
        /// Creates bodies for generated delta serializers.
        /// </summary>
        private void CreateSerializerBodies(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            CreateDeltaSerializerBodies(context, rootSyntaxReceiver);
        }


        /// <summary>
        /// Creates an empty delta serializer method for a type.
        /// </summary>
        private void CreateEmptyDeltaSerializerMethod(GeneratorExecutionContext context, string typeFullName)
        {
            Debugg.Log($"Trying to create writer for {typeFullName}.");
            //Already exist either in FishNet or already created.
            if (_serializers.GetWriteMethod(typeFullName, GetSerializerType.Delta).IsValid())
            {
                Debugg.Log($"- Writer already exists.");
                return;
            }

            INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(typeFullName);
            //Not a supported type. Must be a user defined struct or class.
            if (namedTypeSymbol == null || !namedTypeSymbol.IsUserDefinedClassOrStruct())
            {
                bool isNull = (namedTypeSymbol == null);
                Debugg.Log($"- NamedSymbol is null: {isNull}, or not user defined.");
                return;
            }
            
            //Too many parameters to process as a delta writer due to not enough flags.
            if (namedTypeSymbol.MemberNames.Count() >= 63)
            {
                Debugg.Log($"- Too many members for delta writer.");
                //throw new Exception($"Type {namedTypeSymbol.GetTypeFullName()} exceeds the maximum of 63 field members. Reduce the amount of field members or encapsulate members.");
                return;
            }

            //Create empty writers for any nested types which would need to be serialized.
            foreach (ISymbol item in namedTypeSymbol.GetMembers())
            {
                if (!_serializers.CanGenerateFieldSerializer(item, out IFieldSymbol? fieldSymbol))
                {
                    Debugg.Log($"- Cannot generate serializer for field {item.GetSymbolFullName()}");
                    continue;
                }

                ITypeSymbol typeSymbol = fieldSymbol!.Type;
                CreateEmptyDeltaSerializerMethod(context, typeSymbol.GetTypeFullName());
            }

            Debugg.Log("- Creating header and footer.");
            string header = GetMethodHeader(out string methodName);
            string footer = GetMethodFooter();
            //Add to writers.

            _serializers.AddWriteMethod(new GeneratedDeltaSerializerMethod(namedTypeSymbol, typeFullName, methodName, header, footer, ""), AddSerializerType.Delta);
            Debugg.Log($"- Added for type {typeFullName}.");

            string GetMethodHeader(out string mName)
            {
                mName = $"{Generated_Method_Prefix}{typeFullName.RemovePeriods("_")}";
                StringBuilder sb = new();
                string inText = namedTypeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                sb.Indent(2).AppendLine($"public static bool {mName}" +
                                        $"(this {FishNetConstants.Writer_FullName} {Generated_WriterParameter_Name}," +
                                        $" {inText}{typeFullName} {_serializers.GetValueParameterName(0)}, {inText}{typeFullName} {_serializers.GetValueParameterName(1)}" +
                                        $", bool {Generated_WriteFullParameter_Name} = false, bool {Generated_RootCallParameter_Name} = true)");
                sb.Indent(2).Append('{');

                return sb.ToString();
            }

            string GetMethodFooter()
            {
                StringBuilder sb = new();
                sb.AppendLine(2, "}");
                return sb.ToString();
            }
        }

        /// <summary>
        /// Creates bodies for empty delta serializer methods.
        /// </summary>
        private void CreateDeltaSerializerBodies(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            StringBuilder sb = new();

            //Iterate all serializers and if they are generated delta writers then complete them.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                if (!item.Value.IsValid() || item.Value is not GeneratedDeltaSerializerMethod gsm)
                {
                    Debugg.Log($"Skipping {item.Key} because it's type is {item.Value.GetType()}");
                    continue;
                }
                else
                {
                    Debugg.Log($"- Creating body for {gsm.TypeFullName}");
                }

                sb.Clear();

                //Write full block.
                SerializerMethod fullSerializerMethod = _serializers.GetWriteMethod(item.Key, GetSerializerType.Full);
                if (!fullSerializerMethod.IsValid())
                {
                    sb.AppendThrowLine(3, $"Full Writer could not be found for type {item.Key}. This is normal until added. Continuing...");
                    //continue;
                    fullSerializerMethod = new SerializerMethod(item.Key, $"Write");
                }

                CreateWriteFullIf();
                void CreateWriteFullIf()
                {
                    StringBuilder ifBody = new();
                    ifBody.AppendLine(4, $"{Generated_WriterParameter_Name}.{fullSerializerMethod.MethodName}({_serializers.GetValueParameterName(1)});");
                    ifBody.Append(4, "return true;");
                    sb.AppendLine(CodeBuilder.CreateMultiLineIf(3, $"{Generated_WriteFullParameter_Name}", ifBody));
                }
                
                //Starting flag for each modified field.
                ulong fieldFlag = 2;

                //totalFlags and pooledWriter local variables.
                string totalFlagsVariable = "totalFlags";
                sb.AppendLine(3, CodeBuilder.CreateLocalVariable(NativeConstants.UInt64_FullName, totalFlagsVariable, "0"));
                sb.AppendLine(3, CodeBuilder.CallGetPooledWriter(out string tmpWriterVariable) + NativeConstants.LineFeed);

                //Call write for all members.
                foreach (ISymbol symbol in gsm.NamedTypeSymbol.GetMembers())
                {
                    if (!_serializers.CanGenerateFieldSerializer(symbol, out IFieldSymbol? fieldSymbol)) continue;

                    ITypeSymbol typeSymbol = fieldSymbol!.Type;
                    string typeFullName = typeSymbol.GetTypeFullName();

                    //Get delta writer method for the field.
                    DeltaSerializerMethod? dsm = _serializers.GetWriteMethod(typeFullName, GetSerializerType.Delta) as DeltaSerializerMethod;
                    if (!dsm.IsValid())
                    {
                        sb.AppendThrowLine(3, $"Delta writer could not be found for type {typeFullName}.");
                        continue;
                    }

                    //If a user defined struct then use the in keyword.
                    string inText = typeSymbol.IsUserDefinedStruct() ? "in " : string.Empty;
                    /* if (writer.WriteDeltaXYZ(p0, p1))
                        totalFlags += x */
                    sb.AppendLine(3, CodeBuilder.CreateSingleLineIf(
                        CodeBuilder.CallMethod(dsm!.MethodName, tmpWriterVariable, false,
                            $"{inText}{_serializers.GetValueParameterName(0)}.{fieldSymbol.Name}",
                            $"{inText}{_serializers.GetValueParameterName(1)}.{fieldSymbol.Name}")));
                    sb.AppendLine(4, $"{totalFlagsVariable} += {fieldFlag};");

                    fieldFlag *= 2;
                }

                string changedVariable = "changed";
                sb.AppendLine(""); //simple line feed for formatting.
                //bool changed = (totalFlags != 0) || rootWriter;
                sb.AppendLine(3, $"bool {changedVariable} = ({totalFlagsVariable} != 0) || {Generated_RootCallParameter_Name};");

                /* if (changed)
                 {
                    writer.WritePackedWhole(totalFlags); */
                sb.AppendLine(3, CodeBuilder.CreateSingleLineIf(changedVariable));
                sb.AppendLine(3, "{");
                sb.AppendLine(4,
                    CodeBuilder.CallMethod(FishNetConstants.Writer_WriteUnsignedPackedWhole_Name, Generated_WriterParameter_Name,
                        true, totalFlagsVariable));
                /*  writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
                 } */
                sb.AppendLine(4,
                    CodeBuilder.CallWriteBytes(Generated_WriterParameter_Name, tmpWriterVariable));
                sb.AppendLine(3, "}");
                //store tmpWriter.
                sb.AppendLine(3, CodeBuilder.CallStorePooledWriter(tmpWriterVariable) + NativeConstants.LineFeed);
                /* Struct/class writers must always return true. This is so if they are being encapsulated
                 * the flags written will be read, even if that flag is 0. */
                sb.AppendLine(3, $"return {changedVariable};");

                gsm.Body = sb.ToString();
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

            //Delta writers.
            foreach (KeyValuePair<string, SerializerMethod> item in _serializers.GetWriteDeltaMethods())
            {
                if (item.Value is not GeneratedDeltaSerializerMethod dsm) continue;

                sb.AppendLine(dsm.Header);
                sb.AppendLine(dsm.Body);
                sb.AppendLine(dsm.Footer);
            }

            sb.AppendLine(footer);

            context.AddSource($"{FishNetConstants.Serializing_Namespace}_{Generated_Class_Name}.g.cs", sb.ToString());
        }
    }
}