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
    internal class DeltaSerializer
    {
        private Serializers _serializers;
        private const string GeneratedClass_Name = "Generated_Delta_Serializers";

        public void Initialize(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver, Serializers serializers)
        {
            _serializers = serializers;

            StringBuilder generatedWritersSb = CreateDeltaWriters(context, rootSyntaxReceiver);
            CreateGeneratedDeltaSerializersClass(context, generatedWritersSb);
        }


        /// <summary>
        /// Creates delta writers for container types.
        /// </summary>
        private StringBuilder CreateDeltaWriters(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            StringBuilder sb = new();

            foreach (string item in rootSyntaxReceiver.SerializableTypes)
                CreateEmptyDeltaWriter(item);

            void CreateEmptyDeltaWriter(string typeFullName)
            {
                //Already exist either in FishNet or already created.
                if (_serializers.GetDeltaWriter(typeFullName).IsValid())
                    return;

                string header = GetMethodHeader(out string methodName);
                string footer = GetMethodFooter();

                INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(typeFullName);
                if (namedTypeSymbol == null || (!namedTypeSymbol.IsUserDefinedStruct() && !namedTypeSymbol.IsUserDefinedClass()))
                    return;
                else
                    sb.AppendLine($"   // Type {typeFullName} is supported.");

                if (namedTypeSymbol.MemberNames.Count() >= 63)
                    throw new Exception(
                        $"Type {namedTypeSymbol.GetTypeFullName()} exceeds the maximum of 63 field members. Reduce the amount of field members or encapsulate members.");

                foreach (ISymbol item in namedTypeSymbol.GetMembers())
                {
                    if (item is IFieldSymbol fieldSymbol)
                    {
                        ITypeSymbol typeSymbol = fieldSymbol.Type;
                        CreateEmptyDeltaWriter(typeSymbol.GetTypeFullName());
                        //sb.Indent(3)
                        sb.AppendLine($"       //Member fullName is {typeSymbol.GetTypeFullName()}, {item.Name}");
                    }
                }

                //Add to writers.
                _serializers.AddDeltaWriter(new SerializerMethod(namedTypeSymbol, typeFullName, methodName, true));

                string GetMethodHeader(out string mName)
                {
                    tmpSb.Clear();
                    mName = $"WriteDelta_{typeFullName.RemovePeriods("_")}";

                    tmpSb.Indent(2).AppendLine($"public static bool {mName}" +
                                               $"(this {Writer_FullName} {WriteDelta_WriterParameter_FullName}," +
                                               $" in {typeFullName} {WriteDelta_ParameterA_Name}, in {typeFullName} {WriteDelta_ParameterB_Name}" +
                                               $", bool writeFull = false, bool rootWriter = true)");
                    tmpSb.Indent(2).Append('{');

                    return tmpSb.ToString();
                }

                string GetMethodFooter()
                {
                    tmpSb.Clear();
                    tmpSb.Indent(2).AppendLine("}");

                    return tmpSb.ToString();
                }
            }

            foreach (KeyValuePair<string, DeltaWriterMethod> item in createdMethods)
            {
                sb.AppendLine(item.Value.Header);

                //If to write full then call Write on type and exit.
                sb.AppendLine(3, "if (writeFull)");
                sb.AppendLine(3, "{");
                sb.AppendLine(4, $"{WriteDelta_WriterParameter_FullName}.Write({WriteDelta_ParameterB_Name});");
                sb.AppendLine(4, "return true;");
                sb.AppendLine(3, "}\r\n");

                //Starting flag for each modified field.
                ulong fieldFlag = 2;
                ulong totalFlags = 0;

                string totalFlagsVariable = "totalFlags";
                sb.AppendLine(3, CodeBuilder.CreateLocalVariable(UInt64_FullName, totalFlagsVariable, "0"));
                sb.AppendLine(3, CodeBuilder.CallGetPooledWriter(out string tmpWriterVariable) + "\r\n");

                foreach (ISymbol symbol in item.Value.NamedTypeSymbol.GetMembers())
                {
                    if (symbol is not IFieldSymbol fieldSymbol) continue;

                    ITypeSymbol typeSymbol = fieldSymbol.Type;
                    string typeFullName = typeSymbol.GetTypeFullName();
                    string writeMethodName = GetWriteDeltaMethodName(typeFullName);

                    if (writeMethodName == string.Empty)
                    {
                        sb.AppendLine(3, $"//WriteMethodName is empty for {item.Key}");
                        continue;
                    }

                    //Check if symbol is a user struct/class.
                    string inText;
                    if (typeSymbol.IsUserDefinedClassOrStruct())
                        inText = "in ";
                    else
                        inText = string.Empty;

                    sb.AppendLine(3, CodeBuilder.SingleLineIf(
                        CodeBuilder.CallMethod(writeMethodName, tmpWriterVariable, false,
                            $"{inText}{WriteDelta_ParameterA_Name}.{fieldSymbol.Name}",
                            $"{inText}{WriteDelta_ParameterB_Name}.{fieldSymbol.Name}")));
                    sb.AppendLine(4, $"{totalFlagsVariable} += {fieldFlag};");

                    // );
                    // sb.AppendLine(3, CodeBuilder.CallMethod(writeMethodName, tmpWriter,
                    //     $"{WriteDelta_ParameterA_Name}.{fieldSymbol.Name}",
                    //     $"{WriteDelta_ParameterB_Name}.{fieldSymbol.Name}"));

                    fieldFlag *= 2;
                }

                //Write the changed flags.
                string changedVariable = "changed";
                sb.AppendLine(3, $"bool {changedVariable} = ({totalFlagsVariable} != 0) || rootWriter;");

                sb.AppendLine(3, CodeBuilder.SingleLineIf(changedVariable));
                sb.AppendLine(3, "{");
                sb.AppendLine(4,
                    CodeBuilder.CallMethod(Writer_WritePackedWhole_Name, WriteDelta_WriterParameter_FullName, true,
                        totalFlagsVariable));
                //Write tmpWriter.
                sb.AppendLine(4,
                    CodeBuilder.CallWriteBytes(WriteDelta_WriterParameter_FullName, tmpWriterVariable));
                sb.AppendLine(3, "}");
                //store tmpWriter.
                sb.AppendLine(3, CodeBuilder.CallStorePooledWriter(tmpWriterVariable) + "\r\n");
                /* Struct/class writers must always return true. This is so if they are being encapsulated
                 * the flags written will be read, even if that flag is 0. */
                sb.AppendLine(3, $"return {changedVariable};");

                sb.AppendLine(item.Value.Footer);
            }
        }
        
        
        /// <summary>
        /// Creates a class containing generated delta writers.
        /// </summary>
        private void CreateGeneratedDeltaSerializersClass(GeneratorExecutionContext context, StringBuilder content)
        {
            StringBuilder classSb = new();

            string clsText = CodeBuilder.CreatePublicStaticClass(GeneratedClass_Name, out string footer, FishNetConstants.Serializing_Namespace);
            classSb.AppendLine(clsText);
            classSb.AppendLine(content.ToString());
            classSb.AppendLine(footer);

            context.AddSource($"{FishNetConstants.Serializing_Namespace}_{GeneratedClass_Name}.g.cs", classSb.ToString());
        }
    }
}