using System;
using System.Collections.Generic;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.RemoteProcedureCalls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers.RemoteProcedureCalls
{
    public class RpcFinder
    {
        public RpcFinder(GeneratorSyntaxReceiver generatorSyntaxReceiver)
        {
            _generatorSyntaxReceiver = generatorSyntaxReceiver;
        }

        public event Action<SyntaxNodeAnalysisContext> OnClassIsNotPartial;

        public readonly HashSet<RpcMethodData> MethodsNeedingSerializers = new();

        private GeneratorSyntaxReceiver _generatorSyntaxReceiver;

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        public void CheckRpcMethod(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);

            if (symbol is not IMethodSymbol methodSymbol) return;

            CheckRpcMethod(context, methodSymbol);
        }

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        public void CheckRpcMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);

            if (symbol is not IMethodSymbol methodSymbol) return;

            CheckRpcMethod(context, methodSymbol);
        }

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        private void CheckRpcMethod(object context, IMethodSymbol methodSymbol)
        {
            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return;

            //Check scope to ensure class holding method is partial.
            if (!methodSymbol.ContainingType.IsPartial())
            {
                if (context is SyntaxNodeAnalysisContext analysisContext)
                    OnClassIsNotPartial?.Invoke(analysisContext);

                return;
            }

            //TODO this should be using SerializerFinder
            List<IParameterSymbol> serializables = _generatorSyntaxReceiver.SerializableFinder.GetRpcSerializableParameters(context, methodSymbol);

            for (int i = 0; i < serializables.Count; i++)
            {
                if (serializables[i].Type is not INamedTypeSymbol)
                    serializables.RemoveAt(i--);
            }

            //Build RpcMethodData here.
        }


        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [SerializerReceiver] {txt}");
        }
    }
}
