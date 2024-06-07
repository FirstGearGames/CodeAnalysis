using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynLearning.Helpers;
using SourceGenerating.SyntaxReceivers;
using SourceGenerator.Extensions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SourceGenerating
{
    [Generator]
    public sealed class SourceGenerator : ISourceGenerator
    {
        #region Types.

        private readonly struct WriteMethod
        {
            public readonly string MethodName;
            public readonly string TypeFullName;

            public WriteMethod(string methodFullName, string typeFullName)
            {
                MethodName = methodFullName;
                TypeFullName = typeFullName;
            }
        }

        #endregion

        private const string WriteDelta_WriterParameter_FullName = "writer";
        private const string WriteDelta_ParameterA_Name = "valueA";
        private const string WriteDelta_ParameterB_Name = "valueB";
        
        private const string FishNetAssembly_Name = "FishNet.Runtime";
        private const string Writer_FullName = "FishNet.Serializing.Writer";
        private const string WriterAttribute_FullName = "FishNet.Serializing.WriterAttribute";
        private const string DeltaWriterAttribute_FullName = "FishNet.Serializing.DeltaWriterAttribute";

        private readonly Dictionary<string, WriteMethod> _fishNetDeltaWriteMethods = new();


        private struct DeltaWriterMethod
        {
            public INamedTypeSymbol NamedTypeSymbol;
            public string FullName;
            public string MethodName;
            public string Header;
            public string Footer;

            public DeltaWriterMethod(INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName,
                string header, string footer)
            {
                NamedTypeSymbol = namedTypeSymbol;
                FullName = typeFullName;
                MethodName = methodName;
                Header = header;
                Footer = footer;
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RootSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not RootSyntaxReceiver rootSyntaxReceiver) return;

            Debugg.Log($"- Execute Start. ");

            FindFishNetDependencies(context);

            WriteStubSerializers(context, rootSyntaxReceiver);

            Debugg.Log($"- Execute End.");

            Debugg.Send();
        }

        private void WriteStubSerializers(GeneratorExecutionContext context, RootSyntaxReceiver rootSyntaxReceiver)
        {
            Debugg.Log("- WriteStubSerializers Start.");

            StringBuilder classSb = new();
            StringBuilder tmpSb = new();

            classSb.AppendLine("namespace GenerateTest");
            classSb.AppendLine("{");
            classSb.Indent().AppendLine("public static class GeneratedWriters");
            classSb.Indent().AppendLine("{");

            Dictionary<string, DeltaWriterMethod> writeDeltaMethods = new();

            // //First generate all the delta writer stubs.
            // foreach (KeyValuePair<string, WriteMethod> entry in _fishNetWriteMethods)
            // {
            //     WriteMethod writeMethod = entry.Value;
            //     CreateEmptyDeltaWriter(entry.Key);
            // }

            foreach (string variable in rootSyntaxReceiver.SerializableTypes)
            {
                classSb.AppendLine("// Creating DeltaWriter for " + variable);
                CreateEmptyDeltaWriter(variable);
            }

            void CreateEmptyDeltaWriter(string typeFullName)
            {
                if (GetWriteDeltaMethodName(typeFullName) != string.Empty)
                {
                    classSb.AppendLine($"   // Already created {typeFullName}");
                    return;
                }

                string header = GetMethodHeader(out string methodName);
                string footer = GetMethodFooter();

                INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(typeFullName);
                if (namedTypeSymbol == null || (!namedTypeSymbol.IsUserDefinedStruct() &&
                                                !namedTypeSymbol.IsUserDefinedClass()))
                {
                    return;
                }
                else
                {
                    classSb.AppendLine($"   // Type {typeFullName} is supported.");
                }

                foreach (ISymbol item in namedTypeSymbol.GetMembers())
                {
                    if (item is IFieldSymbol fieldSymbol)
                    {
                        ITypeSymbol typeSymbol = fieldSymbol.Type;
                        CreateEmptyDeltaWriter(typeSymbol.GetTypeFullName());
                        //classSb.Indent(3)
                        classSb.AppendLine($"       //Member fullName is {typeSymbol.GetTypeFullName()}, {item.Name}");
                    }
                }

                //Add to deltaWriters.
                writeDeltaMethods.Add(typeFullName,
                    new DeltaWriterMethod(namedTypeSymbol, typeFullName, methodName, header, footer));

                string GetMethodHeader(out string mName)
                {
                    tmpSb.Clear();
                    mName = $"WriteDelta_{typeFullName.RemovePeriods("_")}";

                    tmpSb.Indent(2).AppendLine($"public static void {mName}" +
                                               $"(this {Writer_FullName} {WriteDelta_WriterParameter_FullName}," +
                                               $" {typeFullName} {WriteDelta_ParameterA_Name},  {typeFullName} {WriteDelta_ParameterB_Name})");
                    tmpSb.Indent(2).AppendLine("{");

                    return tmpSb.ToString();
                }

                string GetMethodFooter()
                {
                    tmpSb.Clear();
                    tmpSb.Indent(2).AppendLine("}");

                    return tmpSb.ToString();
                }
            }

            foreach (KeyValuePair<string, DeltaWriterMethod> item in writeDeltaMethods)
            {
                classSb.AppendLine(item.Value.Header);

                foreach (ISymbol symbol in item.Value.NamedTypeSymbol.GetMembers())
                {
                    if (symbol is not IFieldSymbol fieldSymbol) continue;
                    ;
                    ITypeSymbol typeSymbol = fieldSymbol.Type;
                    string typeFullName = typeSymbol.GetTypeFullName();
                    string writeMethodName = GetWriteDeltaMethodName(typeFullName);

                    if (writeMethodName == string.Empty)
                    {
                        classSb.AppendLine($"           //WriteMethodName is empty for {item.Key}");
                        continue;
                    }
                    
                    classSb.AppendLine($"           //Member fullName is {typeFullName}, {item.Key}");
                    string callMethodText = $"          {WriteDelta_WriterParameter_FullName}.{writeMethodName}(" +
                                            $"{WriteDelta_ParameterA_Name}, {WriteDelta_ParameterB_Name});";
                    classSb.AppendLine(callMethodText);
                }


          

                classSb.AppendLine(item.Value.Footer);
            }

            string GetWriteDeltaMethodName(string typeFullName)
            {
                if (_fishNetDeltaWriteMethods.TryGetValue(typeFullName, out WriteMethod wm))
                    return wm.MethodName;
                if (writeDeltaMethods.TryGetValue(typeFullName, out DeltaWriterMethod dwm))
                    return dwm.MethodName;

                //Fallthrough/failure.
                return string.Empty;
            }

            classSb.Indent().AppendLine("}");
            classSb.AppendLine("}");

            context.AddSource("GeneratedWriters.g.cs", classSb.ToString());

            Debugg.Log(classSb.ToString());
            Debugg.Log("- WriteStubSerializers End.");
        }

        private void FindFishNetDependencies(in GeneratorExecutionContext context)
        {
            ImmutableArray<IAssemblySymbol>
                assemblySymbols = context.Compilation.SourceModule.ReferencedAssemblySymbols;
            IAssemblySymbol? fishNetSymbol =
                assemblySymbols.FirstOrDefault(assemblySymbols => assemblySymbols.Name == FishNetAssembly_Name);

            if (fishNetSymbol == null)
            {
                Debugg.Log($"Could not find FishNet.Runtime assembly .");
                return;
            }

            FindWriterMethods();

            void FindWriterMethods()
            {
                if (fishNetSymbol.GetTypeByMetadataName(Writer_FullName) is not INamedTypeSymbol writerTypeSymbol)
                {
                    Debugg.Log($"Could not find writer.");
                    return;
                }

                foreach (IMethodSymbol methodSymbol in writerTypeSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    // Writers will always have at least 1 parameter.
                    if (methodSymbol.Parameters.Length == 0) continue;
                    // Does not have writer attribute.
                    if (!methodSymbol.HasAttribute(DeltaWriterAttribute_FullName, out _)) continue;

                    //Type will always be the first parameter.
                    string typeFullName = methodSymbol.Parameters.First().Type.GetTypeFullName();

                    if (_fishNetDeltaWriteMethods.ContainsKey(typeFullName))
                        Debugg.Log($"__ERROR__ Type {typeFullName} already added.");
                    else
                        _fishNetDeltaWriteMethods.Add(typeFullName,
                            new WriteMethod(methodSymbol.Name, typeFullName));

                    Debugg.Log(
                        $"Write Method Name: {methodSymbol.GetSymbolFullName()}. Parameters are: {typeFullName}");
                }
            }
        }
    }
}