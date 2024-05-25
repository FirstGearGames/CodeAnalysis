using FishNet.CodeAnalysis.Extensions;
using FishNet.Serializing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynLearning.Helpers;
using SourceGenerating.SyntaxReceivers;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => _rootReceiver);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Debugg.Log($"- Execute Start.");
            FindWriters(context);

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


            Debugg.Log($"- Execute End.");
            Debugg.Send();
        }

        private void FindWriters(GeneratorExecutionContext context)
        {
            Debugg.Log($"--- Start FindWriters");

            if (context.Compilation == null || context.Compilation.SourceModule == null || context.Compilation.SourceModule.ReferencedAssemblySymbols == null)
            {
                Debugg.Log($"A value we need is null.");
                return;
            }

            ImmutableArray<IAssemblySymbol> assemblySymbols = context.Compilation.SourceModule.ReferencedAssemblySymbols;
            Debugg.Log($"Symbols count: {assemblySymbols.Count()}");

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
            }
            else
            {
                string writerFullName = typeof(Writer).FullName;
                Debugg.Log($"Found FishNet assembly.");
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


                    Debugg.Log($"Write Method Name: {methodSymbol.GetFullName()}. Parameters are: {typeFullName}");

                }

            }

            //ImmutableArray<AssemblyIdentity> assemblyIdentifiers = context.Compilation.SourceModule.ReferencedAssemblies;
            //Debugg.Log($"Identities count: {assemblyIdentifiers.Count()}");


            Debugg.Log($"--- End FindWriters.");
        }
    }
}
