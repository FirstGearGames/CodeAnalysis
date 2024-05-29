using FishNet.CodeAnalysis.Extensions;
using FishNet.Serializing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynLearning.Helpers;
using SourceGenerating.SyntaxReceivers;
using SourceGenerator.Extensions;
using SourceGenerator.SyntaxReceiver.SyntaxProcessor;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace SourceGenerating
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private struct WriteMethod
        {
            public string MethodFullName;
            public string TypeFullName;

            public WriteMethod(string methodFullName, string typeFullName)
            {
                MethodFullName = methodFullName;
                TypeFullName = typeFullName;
            }
        }

        private RootSyntaxReceiver _rootReceiver = new();
        private Dictionary<string, WriteMethod> _writeMethods = new();

        private INamedTypeSymbol _generatedWriters_Symbol;

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => _rootReceiver);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Debugg.Log($"- Execute Start.");
            FindFishNetDependencies(context);

            foreach (string item in _rootReceiver.SerializerProcessor.SerializableTypes)
            {
                string writeMethodFullName = string.Empty;
                if (_writeMethods.TryGetValue(item, out WriteMethod writeMethod))
                {
                    writeMethodFullName = writeMethod.MethodFullName;
                }
                else
                {
                    Debugg.Log($"-- Write method not found. Here are all... ");
                    foreach (KeyValuePair<string, WriteMethod> i2 in _writeMethods)
                        Debugg.Log($" --- Key '{i2.Key}'. MethodName '{i2.Value.MethodFullName}'");
                }
                Debugg.Log($"SerializableType: '{item}'. WriteMethod '{writeMethodFullName}'");
            }

            WriteStubSerializers(context);

            Debugg.Log($"- Execute End.");
            Debugg.Send();
        }

        private void WriteStubSerializers(GeneratorExecutionContext context)
        {
            Debugg.Log("- WriteStubSerializers Start.");


            StringBuilder sb = new();

            sb.AppendLine("namespace FishNet.Serializing");
            sb.AppendLine("{");
            sb.Append('\t').AppendLine("public static class GeneratedWriters");
            sb.Append('\t').AppendLine("{");

            foreach (KeyValuePair<string, WriteMethod> i2 in _writeMethods)
            {
                WriteMethod value = i2.Value;
                //MethodDeclarationSyntax method = GetMethodDeclarationSyntax(typeof(void).FullName, i2.Key, new string[] { i2.Value.TypeFullName }, new string[] { $"p{i2.Value.TypeFullName}" });
                Debugg.Log($" ---Key '{i2.Key}'.  MethodName '{value.MethodFullName}'");

                string methodSignature = $"public static void Write_" +
                    $"{value.TypeFullName}(this {SerializerProcessor.Writer_FullName} writer, {value.TypeFullName} value)";
                sb.Append("\t\t").AppendLine(methodSignature);
                sb.Append("\t\t").AppendLine("{");
                sb.Append("\t\t\t").AppendLine("// Write System.Int32 ");
                sb.Append("\t\t").AppendLine("}");
            }

            sb.Append('\t').AppendLine("}");

            sb.AppendLine("}");

            context.AddSource("GeneratedWriters.g.cs", sb.ToString());

            Debugg.Log(sb.ToString());


            //ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            //if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            //string fullName = namedTypeSymbol.GetFullName();
            //if (fullName == typeof(FishNet.Serializing.GeneratedWriters).FullName)
            //    GeneratedWriter_Class = classDeclarationSyntax;

            Debugg.Log("- WriteStubSerializers End. ");
        }

        private void FindFishNetDependencies(GeneratorExecutionContext context)
        {
            if (context.Compilation == null || context.Compilation.SourceModule == null || context.Compilation.SourceModule.ReferencedAssemblySymbols == null)
                return;

            ImmutableArray<IAssemblySymbol> assemblySymbols = context.Compilation.SourceModule.ReferencedAssemblySymbols;

            IAssemblySymbol? fishnetSymbol = null;
            foreach (IAssemblySymbol item in assemblySymbols)
            {
                if (item.Name == "FishNet.Runtime")
                {
                    fishnetSymbol = item;
                    break;
                }
            }

            if (fishnetSymbol == null)
            {
                Debugg.Log($"Could not find FishNet assembly .");
                return;
            }

            FindWriterMethods();

            void FindWriterMethods()
            {
                string writerFullName = typeof(Writer).FullName;
                INamedTypeSymbol? writerSymbol = fishnetSymbol.GetTypeByMetadataName(writerFullName);
                if (writerSymbol == null)
                {
                    Debugg.Log($"Could not find writer. FullName checked is {writerFullName}.");
                    return;
                }

                Debugg.Log($"Writer found. FullName checked is  {writerFullName}");
                foreach (IMethodSymbol methodSymbol in writerSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    //Writers will always have at least 1 parameter.
                    if (methodSymbol.Parameters.Length == 0) continue;
                    //Does not have writer attribute.
                    if (!methodSymbol.HasAttribute<WriterAttribute>(out _)) continue;

                    //Type will always be the first parameter.
                    string typeFullName = methodSymbol.Parameters.First().Type.GetFullName();

                    if (_writeMethods.TryGetValue(typeFullName, out _))
                        Debugg.Log($"__ERROR__ Type {typeFullName} already added.");
                    else
                        _writeMethods.Add(typeFullName, new WriteMethod(methodSymbol.GetFullName(), typeFullName));

                    Debugg.Log($"Write Method Name: {methodSymbol.GetFullName()}. Par ameters are: {typeFullName}");
                }
            }
        }
    }
}
