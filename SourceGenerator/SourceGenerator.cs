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
            public readonly string MethodFullName;
            public readonly string TypeFullName;

            public WriteMethod(string methodFullName, string typeFullName)
            {
                MethodFullName = methodFullName;
                TypeFullName = typeFullName;
            }
        }

        #endregion

        private const string FishNetAssemblyName = "FishNet";

        private const string WriterFullName = "FishNet.Serializing.Writer";

        private const string WriterAttributeFullName = "FishNet.Serializing.WriterAttribute";

        private readonly Dictionary<string, WriteMethod> _fishNetWriteMethods = new();

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RootSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not RootSyntaxReceiver rootSyntaxReceiver) return;

            Debugg.Log($"- Execute Start. ");

            FindFishNetDependencies(context);

            WriteStubSerializers(context);

            Debugg.Log($"- Execute End.");

            Debugg.Send();
        }

        private struct DeltaWriterMethod
        {
            public DeltaWriterMethod(string fullName, string header, string footer)
            {
                FullName = fullName;
                Header = header;
                Footer = footer;
            }

            public string FullName;
            public string Header;
            public string Footer;
        }

        private void WriteStubSerializers(GeneratorExecutionContext context)
        {
            Debugg.Log("- WriteStubSerializers Start.");

            StringBuilder classSb = new();
            StringBuilder tmpSb = new();

            classSb.AppendLine("namespace GenerateTest");
            classSb.AppendLine("{");
            classSb.Indent().AppendLine("public static class GeneratedWriters");
            classSb.Indent().AppendLine("{");

            Dictionary<string, DeltaWriterMethod> writeDeltaMethods = new();

            //First generate all the delta writer stubs.
            foreach (KeyValuePair<string, WriteMethod> entry in _fishNetWriteMethods)
            {
                WriteMethod writeMethod = entry.Value;
                CreateEmptyDeltaWriter(entry.Key);
            }

            void CreateEmptyDeltaWriter(string typeFullName)
            {
                if (writeDeltaMethods.ContainsKey(typeFullName)) return;
                
                string header = GetMethodHeader();
                string footer = GetMethodFooter();

                INamedTypeSymbol? namedTypeSymbol = context.Compilation.GetTypeByMetadataName(typeFullName);
                if (namedTypeSymbol == null || (!namedTypeSymbol.IsUserDefinedStruct() &&
                    !namedTypeSymbol.IsUserDefinedClass()))
                    return;

                foreach (ISymbol item in namedTypeSymbol.GetMembers())
                {
                    if (item is IFieldSymbol fieldSymbol)
                    {
                        ITypeSymbol typeSymbol = fieldSymbol.Type;
                        CreateEmptyDeltaWriter(typeSymbol.GetFullTypeName());
                        //classSb.Indent(3)
                        //.AppendLine($"//MemberX fullName is {typeSymbol.GetFullTypeName()}, {item.Name}");
                    }
                }
                
                //Add to deltaWriters.
                writeDeltaMethods.Add(typeFullName, new DeltaWriterMethod(typeFullName, header, footer));
                
                string GetMethodHeader()
                {
                    tmpSb.Clear();
                    tmpSb.Indent(2).AppendLine($"public static void WriteDelta_" +
                                               $"{typeFullName.RemovePeriods("_")}(this {WriterFullName} writer," +
                                               $" {typeFullName} valueA,  {typeFullName} valueB)");
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

            
            //Add all generated delta writers.
            foreach (var VARIABLE in writeDeltaMethods)
            {
                classSb.AppendLine(VARIABLE.Value.Header);
                classSb.AppendLine(VARIABLE.Value.Footer);
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
                assemblySymbols.FirstOrDefault(assemblySymbols => assemblySymbols.Name == FishNetAssemblyName);

            if (fishNetSymbol == null)
            {
                Debugg.Log($"Could not find FishNet assembly .");
                return;
            }

            FindWriterMethods();

            void FindWriterMethods()
            {
                if (fishNetSymbol.GetTypeByMetadataName(WriterFullName) is not INamedTypeSymbol writerTypeSymbol)
                {
                    Debugg.Log($"Could not find writer.");
                    return;
                }

                foreach (IMethodSymbol methodSymbol in writerTypeSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    // Writers will always have at least 1 parameter.
                    if (methodSymbol.Parameters.Length == 0) continue;
                    // Does not have writer attribute.
                    if (!methodSymbol.HasAttribute(WriterAttributeFullName, out _)) continue;

                    //Type will always be the first parameter.
                    string typeFullName = methodSymbol.Parameters.First().Type.GetFullTypeName();

                    if (_fishNetWriteMethods.ContainsKey(typeFullName))
                        Debugg.Log($"__ERROR__ Type {typeFullName} already added.");
                    else
                        _fishNetWriteMethods.Add(typeFullName,
                            new WriteMethod(methodSymbol.GetFullName(), typeFullName));

                    Debugg.Log($"Write Method Name: {methodSymbol.GetFullName()}. Parameters are: {typeFullName}");
                }
            }
        }
    }
}