using FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.RemoteProcedureCalls;
using FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.Serializers;
using FirstGearGames.FishNet.CodeAnalysis.Constants;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.CodeAnalysis.Helpers;

namespace FirstGearGames.FishNet.CodeAnalysis.SourceGenerators
{
    [Generator]
    public sealed class SerializableGenerator : ISourceGenerator
    {
        public GeneratorSyntaxReceiver GeneratorSyntaxReceiver;

        public SerializableMethods SerializerMethods;

        public GeneratedDeltaWriter_Builder GeneratedDeltaWriterBuilder;
        public GeneratedDeltaReader_Builder GeneratedDeltaReaderBuilder;
        public GeneratedWriter_Builder GeneratedWriterBuilder;
        public GeneratedReader_Builder GeneratedReaderBuilder;

        public RpcWriter_Builder RpcWriterBuilder;

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

            if (context.SyntaxContextReceiver is GeneratorSyntaxReceiver syntaxReceiver)
            {
                GeneratorSyntaxReceiver = syntaxReceiver;
                Log($"Iteration begin for assembly {context.Compilation.AssemblyName}.");
            }
            else
            {
                Log($"Excepted receiver to be our own, it was not.");
                Debugg.Send();
                return;
            }
            
            if (!FindSerializers(context)) return;
            
            if (!CreateGeneratedSerializers(context)) return;

            if (!CreateRpcSerializerMethods(context)) return;

            Log($"Iteration complete for assembly {context.Compilation.AssemblyName}.");

            Debugg.Send();
        }

        private bool FindSerializers(GeneratorExecutionContext context)
        {
            IAssemblySymbol? fishnetRuntimeAssemblySymbol = GetFishNetRuntimeAssemblySymbol(context);
            if (fishnetRuntimeAssemblySymbol == null)
            {
                Log($"FishNet assembly {FishNetConstants.Runtime_Assembly_Name} could not be found.");
                Debugg.Send();
                return false;
            }

            /* Initialize the serializers class which finds
             * default serializers and is used by most builders. */
            SerializerMethods = new();
            SerializerMethods.Initialize(fishnetRuntimeAssemblySymbol);

            return true;
        }

        private bool CreateGeneratedSerializers(GeneratorExecutionContext context)
        {
            /* Initialize each builder. This mostly
             * just sets references. */
            GeneratedDeltaWriterBuilder = new();
            GeneratedDeltaWriterBuilder.Initialize(context, GeneratorSyntaxReceiver, this);

            GeneratedDeltaReaderBuilder = new();
            GeneratedDeltaReaderBuilder.Initialize(context, GeneratorSyntaxReceiver, this);

            GeneratedWriterBuilder = new();
            GeneratedWriterBuilder.Initialize(context, GeneratorSyntaxReceiver, this);

            GeneratedReaderBuilder = new();
            GeneratedReaderBuilder.Initialize(context, GeneratorSyntaxReceiver, this);

            /* Create the method template of each generated serializer. These need
             * to be done before any bodies are generated so that
             * generated bodies can call stubs. */
            GeneratedDeltaWriterBuilder.CreateEmptySerializerMethods();
            GeneratedDeltaReaderBuilder.CreateEmptySerializerMethods();
            GeneratedWriterBuilder.CreateEmptySerializerMethods();
            GeneratedReaderBuilder.CreateEmptySerializerMethods();

            /* Make method bodies. This is done separately from
             * creating the class with the empty serializer methods
             * so it is easier to debug sections of code where one builder
             * might be causing problems with another. */
            GeneratedDeltaWriterBuilder.CreateSerializerBodies();
            GeneratedDeltaReaderBuilder.CreateSerializerBodies();
            GeneratedWriterBuilder.CreateSerializerBodies();
            GeneratedReaderBuilder.CreateSerializerBodies();

            /* Create the class containing generated serializers. */
            GeneratedDeltaWriterBuilder.CreateGeneratedSerializersClass();
            GeneratedDeltaReaderBuilder.CreateGeneratedSerializersClass();
            GeneratedWriterBuilder.CreateGeneratedSerializersClass();
            GeneratedReaderBuilder.CreateGeneratedSerializersClass();

            return true;
        }

        private bool CreateRpcSerializerMethods(GeneratorExecutionContext context)
        {
            RpcWriterBuilder = new();
            RpcWriterBuilder.Initialize(context, GeneratorSyntaxReceiver, this);

            RpcWriterBuilder.CreateEmptyRpcMethods();

            return true;
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
