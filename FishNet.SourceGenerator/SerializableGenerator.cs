using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.Roslyn.FishNet.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Receivers;
using Roslyn.FishNet.CodeBuilding;

namespace FirstGearGames.Roslyn.FishNet
{
    [Generator]
    public sealed class SerializableGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new GeneratorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not GeneratorSyntaxReceiver syntaxReceiver) return;
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
            deltaWriterBuilder.Initialize(context, syntaxReceiver, serializers);
            DeltaReader_Builder deltaReaderBuilder = new();
            deltaReaderBuilder.Initialize(context, syntaxReceiver, serializers);
            
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