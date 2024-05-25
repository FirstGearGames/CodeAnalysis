using FishNet.CodeAnalysis.Extensions;
using FishNet.Serializing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynLearning.Helpers;
using SourceGenerating.SyntaxReceivers;
using System.Collections.Immutable;
using System.Linq;

namespace SourceGenerating
{
    [Generator]
    public class SourceGenerator : ISourceGenerator
    {
        private RootSyntaxReceiver _rootReceiver = new();
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => _rootReceiver);
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Debugg.Log($"- Execute Start.");
            foreach (string item in _rootReceiver.SerializerProcessor.SerializableTypes)
                Debugg.Log($"SerializableType: {item}.");

            FindWriters(context);
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
                Debugg.Log($"Could not find FishNet assembly.");
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

                Debugg.Log($"Writer found. FullName checked is {writerFullName}");
                foreach (IMethodSymbol methodSymbol in writerSymbol.GetMembers().OfType<IMethodSymbol>())
                {
                    if (methodSymbol.HasAttribute<WriterAttribute>(out _))
                    {
                        string paramters = string.Empty;
                        foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
                        {
                            paramters += parameterSymbol.Type.GetFullName() + ", ";
                        }
                        Debugg.Log($"Write Method Name: {methodSymbol.Name}. Parameters are: {paramters}");

                    }
                } 

            }

            //ImmutableArray<AssemblyIdentity> assemblyIdentifiers = context.Compilation.SourceModule.ReferencedAssemblies;
            //Debugg.Log($"Identities count: {assemblyIdentifiers.Count()}");


            Debugg.Log($"--- End FindWriters.");
        }
    }
}
