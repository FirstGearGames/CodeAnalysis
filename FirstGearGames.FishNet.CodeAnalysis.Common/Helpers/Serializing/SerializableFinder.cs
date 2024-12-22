using FirstGearGames.FishNet.CodeAnalysis.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using FirstGearGames.CodeAnalysis.Constants;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Receivers;
using FirstGearGames.FishNet.CodeAnalysis.RemoteProcedureCalls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing
{
    public class SerializableFinder
    {
        public SerializableFinder(GeneratorSyntaxReceiver generatorSyntaxReceiver)
        {
            _generatorSyntaxReceiver = generatorSyntaxReceiver;
        }

        public event Action<SyntaxNodeAnalysisContext> OnIsNotSerializableAccessible;

        public readonly HashSet<SerializableType> TypesNeedingSerializers = new();

        private GeneratorSyntaxReceiver _generatorSyntaxReceiver;

        public void CheckStructSerializables(GeneratorSyntaxContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            CheckNamedTypeSymbolSerializables(context, namedTypeSymbol, checkForSerializableIdentifier: false);
        }

        public void CheckStructSerializables(SyntaxNodeAnalysisContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            CheckNamedTypeSymbolSerializables(context, namedTypeSymbol, checkForSerializableIdentifier: false);
        }

        public void CheckClassSerializables(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            CheckClassSerializables(context, namedTypeSymbol, context.SemanticModel);
        }

        public void CheckClassSerializables(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            CheckClassSerializables(context, namedTypeSymbol, context.SemanticModel);
        }

        /// <summary>
        /// Finds all possible serializables in a class which are not covered by specalized finders such as RPCs, [GenerateSerializer].
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namedTypeSymbol"></param>
        /// <param name="semanticModel"></param>
        private void CheckClassSerializables(object context, INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            //Find syncTypes. This only runs if not a NetworkBehaviour.
            CheckSyncTypeSerializables(context, namedTypeSymbol, semanticModel);
        }

        /// <summary>
        /// Finds SyncType serializables within a namedTypeSymbol that inherits NetowrkBehaviour.
        /// </summary>
        public void CheckSyncTypeSerializables(object context, INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            //Named type must inherit NetworkBehaviour to look for SyncTypes.
            if (!namedTypeSymbol.InheritsClass(FishNetConstants.NetworkBehaviour_FullName))
                return;

            List<IFieldSymbol> fieldSymbols = namedTypeSymbol.GetFieldMembers();

            //Check all field types to see if they inherit SyncBase.
            foreach (IFieldSymbol fieldSymbol in fieldSymbols)
            {
                //This is returning false (not synctype) on synctypes.
                if (!fieldSymbol.IsSyncType()) continue;

                //This should always pass given this is checked within 'IsSyncType'.
                if (fieldSymbol.Type is not INamedTypeSymbol fieldNamedTypeSymbol) continue;

                //Get SyncType.
                SyncTypeType stt = fieldNamedTypeSymbol.GetSyncType();

                //No syncType.
                if (stt == SyncTypeType.Unset)
                    continue;

                //Custom.
                if (stt == SyncTypeType.Custom)
                    CheckCustomSyncTypeSerializable(fieldNamedTypeSymbol);
                //SyncType is built into FishNet with generics.
                else
                    CheckIncludedGenericSyncTypeSerializable(fieldNamedTypeSymbol);
            }

            //Finds serializables for generic synctypes such as SyncVar<T>.
            void CheckIncludedGenericSyncTypeSerializable(INamedTypeSymbol fieldNamedTypeSymbol)
            {
                List<ITypeSymbol> genericArgumentTypeSymbols = fieldNamedTypeSymbol.GetGenericArgumentsOfNamedTypeSymbol();
                /* FullNames added this iteration.
                 * This is used to prevent endless loops. */
                HashSet<string> addedFullNames = new();

                foreach (ITypeSymbol typeSymbol in genericArgumentTypeSymbols)
                {
                    //Must be named to be added as a serializable.
                    if (typeSymbol is not INamedTypeSymbol genericArgumentNamedTypeSymbol)
                        continue;

                    RecursivelyAddSerializables(context, genericArgumentNamedTypeSymbol, addedFullNames);
                }
            }

            //Finds serializables on types which implement ICustomSync by reading the GetSerializedType method.
            void CheckCustomSyncTypeSerializable(INamedTypeSymbol fieldNamedTypeSymbol)
            {
                IMethodSymbol methodSymbol = fieldNamedTypeSymbol.GetMethod(FishNetConstants.ICustomSync_GetSerializedType_Name, metadataName: false);
                if (methodSymbol == null) return;

                //Default return type should be System.Object, exit if not the case.
                if (methodSymbol.ReturnType.GetTypeSymbolFullName(metadataName: false) != NativeConstants.Object_FullName) return;

                List<ExpressionSyntax> returnExpressions = methodSymbol.GetReturnedExpressionSyntaxes();
                //No entries, or first entry is not named.
                if (returnExpressions.Count == 0 || returnExpressions[0] is not TypeOfExpressionSyntax typeOfExpressionSyntax) return;

                ITypeSymbol returnedTypeSymbol = typeOfExpressionSyntax.GetTypeIdentifier(semanticModel);
                if (returnedTypeSymbol == null || returnedTypeSymbol is not INamedTypeSymbol returnedNamedTypeSymbol) return;
                /* FullNames added this iteration.
                 * This is used to prevent endless loops. */
                HashSet<string> addedFullNames = new();
                //Add first entry.
                RecursivelyAddSerializables(context, returnedNamedTypeSymbol, addedFullNames);
            }
        }

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        public void CheckRpcSerializables(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);

            if (symbol is not IMethodSymbol methodSymbol) return;

            CheckRpcSerializables(context, methodSymbol);
        }

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        public void CheckRpcSerializables(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);

            if (symbol is not IMethodSymbol methodSymbol) return;

            CheckRpcSerializables(context, methodSymbol);
        }

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        private void CheckRpcSerializables(object context, IMethodSymbol methodSymbol)
        {
            List<IParameterSymbol> serializables = GetRpcSerializableParameters(context, methodSymbol);

            foreach (IParameterSymbol parameterSymbol in serializables)
            {
                if (parameterSymbol.Type is INamedTypeSymbol namedSymbol)
                    AddSerializableType(context, namedSymbol);
            }
        }

        /// <summary>
        /// Returns possible serializables in over all RPC attributes for a method.
        /// </summary>
        /// <remarks>If there are multiple attributes on the RPC then serializers will be added accordingly for each attribute.</remarks>
        public List<IParameterSymbol> GetRpcSerializableParameters(object context, IMethodSymbol methodSymbol)
        {
            List<IParameterSymbol> serializableResults = new();

            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return serializableResults;

            List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
            int parametersCount = parameters.Count;

            const bool metadataName = false;

            foreach (RpcAttributeData item in results)
                serializableResults.AddRange(GetRpcSerializableParameters(context, methodSymbol, item));

            return parameters;
        }

        public List<IParameterSymbol> GetRpcSerializableParameters(object context, IMethodSymbol methodSymbol, RpcAttributeData rpcAttributeData)
        {
            List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
            int parametersCount = parameters.Count;

            const bool metadataName = false;

            //ServerRpc.
            if (rpcAttributeData.RPCType == RPCType.Server)
                RemoveTrailingNetworkConnection();
            //TargetRpc.
            else if (rpcAttributeData.RPCType == RPCType.Target)
                RemoveLeadingNetworkConnection();
            
            //All Rpcs support optional channel.
            RemoveTrailingChannel();
            
            //Removes networkConnection if the first parameter.
            void RemoveLeadingNetworkConnection()
            {
                if (parametersCount == 0) return;
                //Remove channel from serializable.
                if (parameters[0].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.NetworkConnection_FullName)
                    parameters.RemoveAt(--parametersCount);
            }

            //Removes networkConnection if the last parameter.
            void RemoveTrailingNetworkConnection()
            {
                if (parametersCount == 0) return;
                //Remove channel from serializable.
                if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.NetworkConnection_FullName)
                    parameters.RemoveAt(--parametersCount);
            }

            //Removes channel if the last parameter.
            void RemoveTrailingChannel()
            {
                if (parametersCount == 0) return;
                //Remove channel from serializable.
                if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.Channel_FullName)
                    parameters.RemoveAt(--parametersCount);
            }

            for (int i = 0; i < parameters.Count; i++)
            {
                if (parameters[i].Type is not INamedTypeSymbol)
                    parameters.RemoveAt(i--);
            }

            return parameters;
        }

        /// <summary>
        /// Finds serializables for an INamedTypeSymbol which may not be bound to specific mechanics, such as RPC.
        /// This method will ignore symbols which inherit NetworkBehaviour.
        /// </summary>
        public void CheckNamedTypeSymbolSerializables(object context, INamedTypeSymbol namedTypeSymbol, bool checkForSerializableIdentifier)
        {
            if (checkForSerializableIdentifier && !namedTypeSymbol.HasSerializableIdentifier())
                return;

            /* FullNames added this iteration.
             * This is used to prevent endless loops. */
            HashSet<string> addedFullNames = new();
            RecursivelyAddSerializables(context, namedTypeSymbol, addedFullNames);
        }

        /// <summary>
        /// Adds a type to serializableTypes.
        /// Returns true if added, false if already existed.
        /// </summary>
        private bool AddSerializableType(object context, INamedTypeSymbol namedTypeSymbol)
        {
            const bool isMetadataName = false;

            //Has exclude serialization attribute.
            if (namedTypeSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName, isMetadataName)) return false;

            string fullName = namedTypeSymbol.GetTypeSymbolFullName(isMetadataName);
            //Few other checks for types we want to ignore.
            if (fullName == typeof(System.ValueType).FullName) return false;
            if (fullName == typeof(System.Object).FullName) return false;

            //Check if already added.
            foreach (SerializableType st in TypesNeedingSerializers)
            {
                if (st.TypeSymbol == namedTypeSymbol)
                    return false;
            }

            if (!namedTypeSymbol.HasPublicAccessibility())
            {
                if (context is SyntaxNodeAnalysisContext analysisContext)
                    OnIsNotSerializableAccessible?.Invoke(analysisContext);
                return false;
            }

            if (TypesNeedingSerializers.Add(new SerializableType(namedTypeSymbol)))
                Log($"Added {namedTypeSymbol.GetTypeSymbolFullName(isMetadataName)} to types needing serializers.");

            return true;
        }

        /// <summary>
        /// Recursively iterates a namedTypeSymbol adding serializable types within.
        /// </summary>
        /// <param name="foundNames">A collection reference to store already found serializables during the iteration.</param>
        private void RecursivelyAddSerializables(object context, INamedTypeSymbol namedTypeSymbol, HashSet<string> foundNames)
        {
            while (true)
            {
                string fullName = namedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
                //Already added.
                if (foundNames.Contains(fullName))
                    break;

                /* The method indicated it could not add some reason.
                 * Maybe the type had an attribute. */
                if (!AddSerializableType(context, namedTypeSymbol))
                    break;
                else
                    foundNames.Add(fullName);

                if (namedTypeSymbol.BaseType is not INamedTypeSymbol nts)
                    break;
                else
                    namedTypeSymbol = nts;
            }
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