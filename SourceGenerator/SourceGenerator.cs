using System;
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
using RoslynLearning.CodeBuilding;
using SourceGenerating.Constants;
using SourceGenerator.CodeBuilding.Serializers;

namespace SourceGenerating
{
    [Generator]
    public sealed class SourceGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new RootSyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxContextReceiver is not RootSyntaxReceiver rootSyntaxReceiver) return;
            Debugg.Log($"- Execute Start. ");

            IAssemblySymbol? fishnetRuntimeAssemblySymbol = GetFishNetRuntimeAssemblySymbol(context);
            if (fishnetRuntimeAssemblySymbol == null)
            {
                Debugg.Log($"Assembly {FishNetConstants.Runtime_Assembly_Name} could not be found.");
                return;
            }

            /* All objects which might be referenced need to be found
             first. The serialization generation process will rely on
             default serializers for types native to fishnet. */
            Serializers serializerses = new();
            serializerses.Initialize(fishnetRuntimeAssemblySymbol);

            DeltaSerializer deltaSerializer = new();
            deltaSerializer.Initialize(rootSyntaxReceiver);
            WriteStubSerializers(context, rootSyntaxReceiver);

            Debugg.Log($"- Execute End.");

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