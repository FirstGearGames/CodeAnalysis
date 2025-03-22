using System;
using System.Collections.Generic;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.RemoteProcedureCalls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.FishNet.CodeAnalysis.Helpers.RemoteProcedureCalls
{
    public class RpcFinder
    {
        public RpcFinder(GeneratorSyntaxReceiver generatorSyntaxReceiver)
        {
            _generatorSyntaxReceiver = generatorSyntaxReceiver;
        }

        /// <summary>
        /// Invoked during analysis when a class is not partial.
        /// </summary>
        public event Action<SyntaxNodeAnalysisContext> OnClassIsNotPartial;

        /// <summary>
        /// All RPC methods which need to be generated.
        /// </summary>
        public readonly HashSet<RpcMethodDatas> RpcMethodDatas = new();
        
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
        /// Checks if a method is a RPC and builds data for it if so.
        /// </summary>
        private void CheckRpcMethod(object context, IMethodSymbol methodSymbol)
        {
            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return;

            //Check scope to ensure class holding method is partial.
            if (!methodSymbol.ContainingType.HasPartialModifier())
            {
                if (context is SyntaxNodeAnalysisContext analysisContext)
                    OnClassIsNotPartial?.Invoke(analysisContext);
                else
                    Log($"Class {methodSymbol.ContainingType.GetTypeSymbolFullName(metadataName: false)} must be partial to create RPC serializers.");
                
                return;
            }

            //Iterate each rpcAttribute and make method data for it.
            foreach (RpcAttributeData data in results)
            {
                List<IParameterSymbol> serializables = _generatorSyntaxReceiver.SerializableFinder.GetRpcSerializableParameters(context, methodSymbol, data);
                
                //Build RpcMethodData here.
                string defaultChannelValue = RpcHelper.GetDefaultChannelValue(methodSymbol, data.RPCType);
                RpcMethodDatas rmd = new(methodSymbol, defaultChannelValue, serializables, data);
             
                RpcMethodDatas.Add(rmd);
            }
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [RpcFinder] {txt}");
        }
    }
}
