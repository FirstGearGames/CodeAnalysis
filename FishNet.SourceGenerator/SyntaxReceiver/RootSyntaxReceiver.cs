using System.Collections.Generic;
using System.Linq;
using FishNet.SourceGenerating.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using FishNet.SourceGenerating.Helpers;
using FishNet.SourceGenerating.CodeBuilding;
using SourceGenerating.Extensions;

namespace FishNet.SourceGenerating.SyntaxReceivers
{
    internal class RootSyntaxReceiver : ISyntaxContextReceiver
    {
        public const string NetworkConnection_FullName = "FishNet.Connection.NetworkConnection";
        public const string Channel_FullName = "FishNet.Transporting.Channel";
        public HashSet<SerializableType> SerializableTypes = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode syntaxNode = context.Node;

            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                FindClassSerializables(context, classDeclaration);
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
                FindStructSerializables(context, structDeclaration);
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                FindRpcSerializables(context, methodDeclaration);
        }


        private void FindStructSerializables(GeneratorSyntaxContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(structDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;
            if (!namedTypeSymbol.HasAttribute(FishNetConstants.IncludeSerializationAttribute_FullName))
            {
               // Debugg.Log($"       No include serialization for {namedTypeSymbol.GetSymbolFullName()}");
                return;
            }

            Debugg.Log($"Found includeSerialization for {namedTypeSymbol.GetSymbolFullName()}");

            /* FullNames added this iteration.
             * This is used to prevent endless loops. */
            HashSet<string> addedFullNames = new();

            AddSerializableType(namedTypeSymbol);
            while (namedTypeSymbol.BaseType is INamedTypeSymbol nestedNamedTypeSymbol)
                /* The method indicated it could not add some reason.
                 * Maybe the type had an attribute to not serialize or
                 * its the previously checked type. */
                if (!AddSerializableType(nestedNamedTypeSymbol)) break;

            bool AddSerializableType(INamedTypeSymbol theSymbol)
            {
                //Has exclude serialization attribute.
                if (theSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName)) return false;

                string fullName = theSymbol.GetTypeFullName();

                if (!addedFullNames.Add(fullName)) return false;

                if (SerializableTypes.Add(new SerializableType(fullName, theSymbol.GetSymbolFullMetaName())))
                    Debugg.Log($"   Added {theSymbol.GetSymbolFullName()} to serializable types.");

                return true;
            }
        }


        private void FindClassSerializables(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;
            if (!namedTypeSymbol.HasAttribute(FishNetConstants.IncludeSerializationAttribute_FullName))
            {
               // Debugg.Log($"       No include serialization for {namedTypeSymbol.GetSymbolFullName()}");
                return;
            }

            Debugg.Log($"Found includeSerialization for {namedTypeSymbol.GetSymbolFullName()}");

            /* FullNames added this iteration.
             * This is used to prevent endless loops. */
            HashSet<string> addedFullNames = new();

            AddSerializableType(namedTypeSymbol);
            while (namedTypeSymbol.BaseType is INamedTypeSymbol nestedNamedTypeSymbol)
                /* The method indicated it could not add some reason.
                 * Maybe the type had an attribute to not serialize or
                 * its the previously checked type. */
                if (!AddSerializableType(nestedNamedTypeSymbol)) break;

            bool AddSerializableType(INamedTypeSymbol theSymbol)
            {
                //Has exclude serialization attribute.
                if (theSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName)) return false;

                string fullName = theSymbol.GetTypeFullName();

                if (!addedFullNames.Add(fullName)) return false;

                if (SerializableTypes.Add(new SerializableType(fullName, theSymbol.GetSymbolFullMetaName())))
                    Debugg.Log($"   Added {theSymbol.GetSymbolFullName()} to serializable types.");

                return true;
            }
        }

        private void FindRpcSerializables(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax);
            if (symbol is not IMethodSymbol methodSymbol) return;
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
                    if (parameters[0].Type.GetTypeFullName() == NetworkConnection_FullName)
                        parameters.RemoveAt(--parametersCount);
                }

                //Removes networkConnection if the last parameter.
                void RemoveTrailingNetworkConnection()
                {
                    if (parametersCount == 0) return;
                    //Remove channel from serializable.
                    if (parameters[parametersCount - 1].Type.GetTypeFullName() == NetworkConnection_FullName)
                        parameters.RemoveAt(--parametersCount);
                }

                //Removes channel if the last parameter.
                void RemoveTrailingChannel()
                {
                    if (parametersCount == 0) return;
                    if (!parameters[parametersCount - 1].IsOptional) return;
                    //Remove channel from serializable.
                    if (parameters[parametersCount - 1].Type.GetTypeFullName() == Channel_FullName)
                        parameters.RemoveAt(--parametersCount);
                }
            }

            //Anything left in parameters is serializable.
            foreach (IParameterSymbol parameter in parameters)
            {
                string fullName = parameter.Type.GetTypeFullName();
                string metadataName = parameter.Type.GetSymbolFullMetaName();
                SerializableTypes.Add(new SerializableType(fullName, metadataName));
            }
        }
    }
}
