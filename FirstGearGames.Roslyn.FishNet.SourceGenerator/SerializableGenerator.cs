using System;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Receivers;
using FirstGearGames.Roslyn.FishNet.SyncTypes;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding.Serializers
{
    [Generator]
    public sealed class SerializableGenerator : ISourceGenerator
    {
        internal Methods SerializerMethods;
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
                Log($"FishNet assembly {FishNetConstants.Runtime_Assembly_Name} could not be found.");
                Debugg.Send();
                return;
            }

            /* Initialize the serializers class which finds
             * default serializers and is used by most builders. */
            SerializerMethods = new();
            SerializerMethods.Initialize(fishnetRuntimeAssemblySymbol);


            /* Initialize each builder. This mostly
             * just sets references. */
            DeltaWriterBuilder = new();
            DeltaWriterBuilder.Initialize(context, syntaxReceiver, this);

            DeltaReaderBuilder = new();
            DeltaReaderBuilder.Initialize(context, syntaxReceiver, this);

            WriterBuilder = new();
            WriterBuilder.Initialize(context, syntaxReceiver, this);

            ReaderBuilder = new();
            ReaderBuilder.Initialize(context, syntaxReceiver, this);

            /* Create the method template of each generated serializer. These need
             * to be done before any bodies are generated so that
             * generated bodies can call stubs. */
            DeltaWriterBuilder.CreateEmptySerializerMethods();
            DeltaReaderBuilder.CreateEmptySerializerMethods();
            WriterBuilder.CreateEmptySerializerMethods();
            ReaderBuilder.CreateEmptySerializerMethods();

            /* Make method bodies. This is done separately from
             * creating the class with the empty serializer methods
             * so it is easier to debug sections of code where one builder
             * might be causing problems with another. */
            DeltaWriterBuilder.CreateSerializerBodies();
            DeltaReaderBuilder.CreateSerializerBodies();
            WriterBuilder.CreateSerializerBodies();
            ReaderBuilder.CreateSerializerBodies();
            
            /* Create the class containing generated serializers. */
            DeltaWriterBuilder.CreateGeneratedSerializersClass();
            DeltaReaderBuilder.CreateGeneratedSerializersClass();
            WriterBuilder.CreateGeneratedSerializersClass();
            ReaderBuilder.CreateGeneratedSerializersClass();

            Log($"Iteration complete for assembly {context.Compilation.AssemblyName}.");

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