using System;
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
        internal Serializers Serializers;
        internal DeltaWriter_Builder DeltaWriterBuilder;
        internal DeltaReader_Builder DeltaReaderBuilder;
        internal Writer_Builder WriterBuilder;
        internal Reader_Builder ReaderBuilder;

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new GeneratorSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            Log("");
            string assemblyName = context.Compilation.AssemblyName;
            //Ignore unity assemblies.
            if (assemblyName.StartsWith("Unity.", StringComparison.OrdinalIgnoreCase))
                return;

            Debugg.SetAssemblyName(assemblyName);

            if (context.SyntaxContextReceiver is not GeneratorSyntaxReceiver syntaxReceiver)
            {
                Log($"Excepted receiver to be our own, it was not.");
                Debugg.Send();
                return;
            }
            else
            {
                Log($"Iteration begin for assembly {context.Compilation.AssemblyName}.");
            }

            IAssemblySymbol? fishnetRuntimeAssemblySymbol = GetFishNetRuntimeAssemblySymbol(context);
            if (fishnetRuntimeAssemblySymbol == null)
            {
                Debugg.Log($"FishNet assembly {FishNetConstants.Runtime_Assembly_Name} could not be found.");
                Debugg.Send();
                return;
            }

            /* All objects which might be referenced need to be found
             first. The serialization generation process will rely on
             default serializers for types native to fishnet. */
            Serializers = new();
            Serializers.Initialize(fishnetRuntimeAssemblySymbol);
            //
            // DeltaWriterBuilder = new();
            // DeltaWriterBuilder.Initialize(context, syntaxReceiver, this);
            //
            // DeltaReaderBuilder = new();
            // DeltaReaderBuilder.Initialize(context, syntaxReceiver, this);

            WriterBuilder = new();
            WriterBuilder.Initialize(context, syntaxReceiver, this);

            ReaderBuilder = new();
            ReaderBuilder.Initialize(context, syntaxReceiver, this);

//            DeltaWriterBuilder.Execute();
//            DeltaReaderBuilder.Execute();
            WriterBuilder.Execute();
            ReaderBuilder.Execute();

            Debugg.Log($"Iteration complete for assembly {context.Compilation.AssemblyName}.");

            Debugg.Send();
        }

        private IAssemblySymbol? GetFishNetRuntimeAssemblySymbol(in GeneratorExecutionContext context)
        {
            IAssemblySymbol? fishNetSymbol = null;

            if (context.Compilation.SourceModule.ContainingAssembly.Name == FishNetConstants.Runtime_Assembly_Name)
            {
                fishNetSymbol = context.Compilation.SourceModule.ContainingAssembly;
            }
            else
            {
                ImmutableArray<IAssemblySymbol> assemblySymbols = context.Compilation.SourceModule.ReferencedAssemblySymbols;
                fishNetSymbol = assemblySymbols.FirstOrDefault(x => x.Name == FishNetConstants.Runtime_Assembly_Name);
            }

            return fishNetSymbol;
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"[SerializableGenerator] {txt}");
        }
    }
}