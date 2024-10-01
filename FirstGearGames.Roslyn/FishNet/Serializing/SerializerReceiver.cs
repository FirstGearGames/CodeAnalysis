using System;
using System.Collections.Generic;
using System.Linq;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.Roslyn.FishNet.Serializing
{
    public class SerializableReceiver
    {
        public enum TypeScope 
        {
            Unset,
            Public,
            Private,
        }

        public event Action<SyntaxNodeAnalysisContext> OnIsNotSerializableAccessible;

        public const string NetworkConnection_FullName = "FishNet.Connection.NetworkConnection";
        public const string Channel_FullName = "FishNet.Transporting.Channel";
        public HashSet<SerializableType> SerializableTypes = new();

        public void FindStructSerializables(GeneratorSyntaxContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structDeclaration);
            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindNamedTypeSymbolSerializables(null, namedTypeSymbol);
        }

        public void FindStructSerializables(SyntaxNodeAnalysisContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structDeclaration);
            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindNamedTypeSymbolSerializables(context, namedTypeSymbol);
        }

        public void FindClassSerializables(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);
            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindNamedTypeSymbolSerializables(null, namedTypeSymbol);
        }

        public void FindClassSerializables(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);
            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindNamedTypeSymbolSerializables(context, namedTypeSymbol);
        }

        public void FindRpcSerializables(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);
            if (symbol is not IMethodSymbol methodSymbol) return;
            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return;

            FindRpcSerializables(null, methodSymbol);
        }


        public void FindRpcSerializables(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);
            if (symbol is not IMethodSymbol methodSymbol) return;
            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return;

            FindRpcSerializables(context, methodSymbol);
        }

        private void FindRpcSerializables(object context, IMethodSymbol methodSymbol)
        {
            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return;

            List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
            int parametersCount = parameters.Count;

            foreach (RpcAttributeData item in results)
            {
                //ServerRpc.
                if (item.RPCType == RPCType.Server)
                {
                    RemoveTrailingNetworkConnection();
                    RemoveTrailingChannel();
                }
                //TargetRpc.
                else if (item.RPCType == RPCType.Target)
                {
                    RemoveTrailingChannel();
                    RemoveLeadingNetworkConnection();
                }
                //ObserversRpc.
                else if (item.RPCType == RPCType.Observers)
                {
                    RemoveTrailingChannel();
                    RemoveLeadingNetworkConnection();
                }

                //Removes networkConnection if the first parameter.
                void RemoveLeadingNetworkConnection()
                {
                    if (parametersCount == 0) return;
                    //Remove channel from serializable.
                    if (parameters[0].Type.GetTypeSymbolFullName() == NetworkConnection_FullName)
                        parameters.RemoveAt(--parametersCount);
                }

                //Removes networkConnection if the last parameter.
                void RemoveTrailingNetworkConnection()
                {
                    if (parametersCount == 0) return;
                    //Remove channel from serializable.
                    if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName() == NetworkConnection_FullName)
                        parameters.RemoveAt(--parametersCount);
                }

                //Removes channel if the last parameter.
                void RemoveTrailingChannel()
                {
                    if (parametersCount == 0) return;
                    if (!parameters[parametersCount - 1].IsOptional) return;
                    //Remove channel from serializable.
                    if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName() == Channel_FullName)
                        parameters.RemoveAt(--parametersCount);
                }
            }

            //Anything left in parameters is serializable.
            foreach (IParameterSymbol parameter in parameters)
            {
                if (parameter is not INamedTypeSymbol namedSymbol) continue;

                AddSerializableType(context, namedSymbol);
            }
        }

        /// <summary>
        /// Finds serializables for an INamedTypeSymbol which may not be bound to specific mechanics, such as RPC.
        /// </summary>
        public void FindNamedTypeSymbolSerializables(object context, INamedTypeSymbol namedTypeSymbol)
        {
            //Manually excluding serialization.
            if (namedTypeSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName)) return;

            bool canSerialize = ImplementsPredictionInterface(namedTypeSymbol);

            if (!canSerialize)
                canSerialize |= namedTypeSymbol.HasAttribute(FishNetConstants.IncludeSerializationAttribute_FullName);

            //Nothing indicates value can be serialized.
            if (!canSerialize)
                return;

            Debugg.Log($"Found type to be serialized: {namedTypeSymbol.GetSymbolFullName()}");

            /* FullNames added this iteration.
             * This is used to prevent endless loops. */
            HashSet<string> addedFullNames = new();

            AddSerializableType(context, namedTypeSymbol);
            while (namedTypeSymbol.BaseType is INamedTypeSymbol nestedNamedTypeSymbol)
                /* The method indicated it could not add some reason.
                 * Maybe the type had an attribute to not serialize or
                 * its the previously checked type. */
                if (!AddSerializableType(context, nestedNamedTypeSymbol))
                    break;

            //Returns if a symbol implements prediction interfaces.
            bool ImplementsPredictionInterface(INamedTypeSymbol theSymbol)
            {
                if (theSymbol.ImplementsInterface(FishNetConstants.IReplicate_FullName)) return true;
                if (theSymbol.ImplementsInterface(FishNetConstants.IReconcile_FullName)) return true;

                return false;
            }
        }

        /// <summary>
        /// Adds a type to serializableTypes.
        /// Returns true if added, false if already existed.
        /// </summary>
        private bool AddSerializableType(object context, INamedTypeSymbol namedTypeSymbol)
        {
            //Has exclude serialization attribute.
            if (namedTypeSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName)) return false;

            //Check if already added.
            foreach (SerializableType st in SerializableTypes)
            {
                if (st.TypeSymbol == namedTypeSymbol)
                    return false;
            }

            TypeScope typeScope = GetTypeScope(context, namedTypeSymbol);
            if (typeScope == TypeScope.Unset)
            {
                if (context is SyntaxNodeAnalysisContext analysisContext)
                    OnIsNotSerializableAccessible?.Invoke(analysisContext);
                return false;
            }

            if (SerializableTypes.Add(new SerializableType(namedTypeSymbol)))
                Debugg.Log($"   Added {namedTypeSymbol.GetSymbolFullName()} to serializable types.");

            return true;
        }

        /// <summary>
        /// Returns if a type is accessible for serialization.
        /// This returns true if scope is internal or public, or if the class it's nested within is partial.
        /// </summary>
        private TypeScope GetTypeScope(object context, INamedTypeSymbol typeSymbol)
        {
            //Public.
            if (typeSymbol.DeclaredAccessibility is Accessibility.Public) return TypeScope.Public;
            ////Internal.
            //if (typeSymbol.DeclaredAccessibility is Accessibility.Internal or Accessibility.ProtectedAndInternal) return SerializableType.TypeExposure.Internal;

            /* If here type is not exposed enough. See if containing type is partial which will allow us
             * to put the generated serializer in the containing type. */

            if (typeSymbol.ContainingType is not INamedTypeSymbol baseNamedType) return TypeScope.Unset;

            if (baseNamedType.DeclaringSyntaxReferences.First() is not SyntaxReference syntaxReference) return TypeScope.Unset;

            SyntaxNode node = syntaxReference.GetSyntax();
            if (node is not TypeDeclarationSyntax typeDeclaration) return TypeScope.Unset;

            // if (typeDeclaration.Modifiers.Any(SyntaxKind.PartialKeyword))
            //     return SerializableType.TypeExposure.NestedWithinPartial;

            //Not partial.
            return TypeScope.Unset;
        }
    }
}