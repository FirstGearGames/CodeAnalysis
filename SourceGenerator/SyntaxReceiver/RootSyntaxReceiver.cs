using System.Collections.Generic;
using System.Linq;
using FishNet.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynLearning.Helpers;
using SourceGenerating.Constants;
using SourceGenerator.CodeBuilding.Serializers;

namespace SourceGenerating.SyntaxReceivers
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
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                FindRpcSerializables(context, methodDeclaration);
        }

        private void FindClassSerializables(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;
            if (!namedTypeSymbol.HasAttribute(FishNetConstants.IncludeSerializationAttribute_FullName)) return;

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

                string metadataName = theSymbol.MetadataName;
                SerializableTypes.Add(new SerializableType(fullName, metadataName));

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
            Debugg.Log("Parameter count is  " + parametersCount);

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
                string metadataName = parameter.Type.MetadataName;
                SerializableTypes.Add(new SerializableType(fullName, metadataName));
            }
        }
    }
}
