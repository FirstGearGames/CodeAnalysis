using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using FishNet.SourceGenerating.CodeBuilding;
using FishNet.SourceGenerating.SyntaxReceivers;
using FishNet.SourceGenerating.Constants;
using FishNet.SourceGenerating.Helpers;

namespace FishNet.SourceGenerating
{
    [Generator]
    public sealed class MainGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RootSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (!context.Compilation.AssemblyName.Contains("FishNet.Test")) return;

            if (context.SyntaxContextReceiver is not RootSyntaxReceiver rootSyntaxReceiver) return;
            Debugg.Log($"- Execute Start for {context.Compilation.AssemblyName}.");

            IAssemblySymbol? fishnetRuntimeAssemblySymbol = GetFishNetRuntimeAssemblySymbol(context);
            if (fishnetRuntimeAssemblySymbol == null)
            {
                Debugg.Log($"Assembly {FishNetConstants.Runtime_Assembly_Name} could not be found.");
                return;
            }

            /* All objects which might be referenced need to be found
             first. The serialization generation process will rely on
             default serializers for types native to fishnet. */
            Serializers serializers = new();
            serializers.Initialize(fishnetRuntimeAssemblySymbol);

            DeltaWriter_Builder deltaWriterBuilder = new();
            deltaWriterBuilder.Initialize(context, rootSyntaxReceiver, serializers);
            DeltaReader_Builder deltaReaderBuilder = new();
            deltaReaderBuilder.Initialize(context, rootSyntaxReceiver, serializers);
            
            Debugg.Log($"- Execute End for {context.Compilation.AssemblyName}.");

            Debugg.Send();
        }


        private IAssemblySymbol? GetFishNetRuntimeAssemblySymbol(in GeneratorExecutionContext context)
        {
            ImmutableArray<IAssemblySymbol> assemblySymbols = context.Compilation.SourceModule.ReferencedAssemblySymbols;
            IAssemblySymbol? fishNetSymbol = assemblySymbols.FirstOrDefault(x => x.Name == FishNetConstants.Runtime_Assembly_Name);
            return fishNetSymbol;
        }

  
    }
}