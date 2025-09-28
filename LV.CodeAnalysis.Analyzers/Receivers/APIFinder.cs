using System;
using System.Collections.Generic;
using System.Linq;
using FirstGearGames.CodeAnalysis.Constants;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing
{
    public class APIFinder: ISyntaxContextReceiver
    {
        public const string GENERATE_SHELL_ATTRIBUTE = "GenerateShellAttribute";

        private List<MethodDeclarationSyntax> _methods;
        private List<StructDeclarationSyntax> _structs;
        private List<EnumDeclarationSyntax> _enums;
        private List<ClassDeclarationSyntax> _classes;

        private bool _collectionsInstantiated;
        
        /// <summary>
        /// Instantiates collections if they have not already been created.
        /// </summary>
        private void InstantiateCollectionsIfNeeded()
        {
            if (_collectionsInstantiated)
                return;

            _methods = new();
            _structs = new();
            _enums = new();
            _classes = new();
            
            _collectionsInstantiated = true;
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            InstantiateCollectionsIfNeeded();
            
            SyntaxNode syntaxNode = context.Node;

            if (SerializableFinder is null)
                SerializableFinder = new();

            if (RpcFinder is null)
                RpcFinder = new(this);

            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                LogVisit();
                SerializableFinder.AddClassSerializables(context, classDeclaration);
            }
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
            {
                LogVisit();
                SerializableFinder.AddStructSerializables(context, structDeclaration);
            }
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            {
                LogVisit();
                SerializableFinder.AddRpcSerializables(context, methodDeclaration);
                RpcFinder.CheckRpcMethod(context, methodDeclaration);
            }

            void LogVisit()
            {
                //Log($"OnVisitSyntaxNode type {syntaxNode.GetType().Name}");
            }
        }
        
        public void AddStructSerializables(object context, StructDeclarationSyntax structDeclarationSyntax)
        {
            if (context.GetSemanticModel() is not SemanticModel sm)
                return;

            if (sm.GetDeclaredSymbol(structDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                return;

            AddNamedTypeSymbolSerializablesWithIdentifier(context, namedTypeSymbol, null);
        }

        public void AddClassSerializables(object context, ClassDeclarationSyntax classDeclarationSyntax)
        {
            if (context.GetSemanticModel() is not SemanticModel sm)
                return;

            if (sm.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol namedTypeSymbol)
                return;

            //Find syncTypes. This only runs if is a NetworkBehaviour.
            CheckSyncTypeSerializables(context, namedTypeSymbol, sm);
        }

        /// <summary>
        /// Finds SyncType serializables within a namedTypeSymbol that inherits NetowrkBehaviour.
        /// </summary>
        private void CheckSyncTypeSerializables(object context, INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            //Named type must inherit NetworkBehaviour to look for SyncTypes.
            if (!namedTypeSymbol.InheritsClass(FishNetConstants.NetworkBehaviour_FullName))
                return;

            List<IFieldSymbol> fieldSymbols = namedTypeSymbol.GetFieldMembers();

            //Check all field types to see if they inherit SyncBase.
            foreach (IFieldSymbol fieldSymbol in fieldSymbols)
            {
                //This is returning false (not synctype) on synctypes.
                if (!fieldSymbol.IsSyncType())
                    continue;

                //This should always pass given this is checked within 'IsSyncType'.
                if (fieldSymbol.Type is not INamedTypeSymbol fieldNamedTypeSymbol)
                    continue;

                //Get SyncType.
                SyncTypeType stt = fieldNamedTypeSymbol.GetSyncType();

                //No syncType.
                if (stt == SyncTypeType.Unset)
                    continue;

                //Custom.
                if (stt == SyncTypeType.Custom)
                    CheckCustomSyncTypeSerializable(fieldNamedTypeSymbol, fieldSymbol);
                //SyncType is built into FishNet with generics.
                else
                    CheckIncludedGenericSyncTypeSerializable(fieldNamedTypeSymbol, fieldSymbol);
            }

            //Finds serializables for generic synctypes such as SyncVar<T>.
            void CheckIncludedGenericSyncTypeSerializable(INamedTypeSymbol fieldNamedTypeSymbol, IFieldSymbol fieldSymbol)
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

                    AddSelfAndBaseTypeSerializables(context, genericArgumentNamedTypeSymbol, addedFullNames, fieldSymbol);
                }
            }

            //Finds serializables on types which implement ICustomSync by reading the GetSerializedType method.
            void CheckCustomSyncTypeSerializable(INamedTypeSymbol fieldNamedTypeSymbol, IFieldSymbol fieldSymbol)
            {
                IMethodSymbol methodSymbol = fieldNamedTypeSymbol.GetMethod(FishNetConstants.ICustomSync_GetSerializedType_Name, metadataName: false);
                if (methodSymbol is null)
                    return;

                //Default return type should be System.Object, exit if not the case.
                if (methodSymbol.ReturnType.GetTypeSymbolFullName(metadataName: false) != NativeConstants.Object_FullName)
                    return;

                List<ExpressionSyntax> returnExpressions = methodSymbol.GetReturnedExpressionSyntaxes();
                //No entries, or first entry is not named.
                if (returnExpressions.Count == 0 || returnExpressions[0] is not TypeOfExpressionSyntax typeOfExpressionSyntax)
                    return;

                ITypeSymbol returnedTypeSymbol = typeOfExpressionSyntax.GetTypeIdentifier(semanticModel);
                if (returnedTypeSymbol is null || returnedTypeSymbol is not INamedTypeSymbol returnedNamedTypeSymbol)
                    return;
                /* FullNames added this iteration.
                 * This is used to prevent endless loops. */
                HashSet<string> addedFullNames = new();
                //Add first entry.
                AddSelfAndBaseTypeSerializables(context, returnedNamedTypeSymbol, addedFullNames, fieldSymbol);
            }
        }

        /// <summary>
        /// Finds serializables for methods which implement RPC attributes.
        /// </summary>
        public void AddRpcSerializables(object context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            SemanticModel sm = context.GetSemanticModel();

            ISymbol symbol = sm?.GetDeclaredSymbol(methodDeclarationSyntax);

            if (symbol is not IMethodSymbol methodSymbol)
                return;

            List<IParameterSymbol> serializables = GetRpcSerializableParameters(context, methodSymbol);

            foreach (IParameterSymbol parameterSymbol in serializables)
            {
                if (parameterSymbol.Type is INamedTypeSymbol namedSymbol)
                    AddSerializableType(context, namedSymbol, methodSymbol);
            }
        }

        /// <summary>
        /// Returns possible serializables in over all RPC attributes for a method.
        /// </summary>
        /// <remarks>If there are multiple attributes on the RPC then serializers will be added accordingly for each attribute.</remarks>
        public List<IParameterSymbol> GetRpcSerializableParameters(object context, IMethodSymbol methodSymbol)
        {
            List<IParameterSymbol> serializableResults = new();

            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results))
                return serializableResults;

            List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();

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
                if (parametersCount == 0)
                    return;
                //Remove channel from serializable.
                if (parameters[0].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.NetworkConnection_FullName)
                    parameters.RemoveAt(--parametersCount);
            }

            //Removes networkConnection if the last parameter.
            void RemoveTrailingNetworkConnection()
            {
                if (parametersCount == 0)
                    return;
                //Remove channel from serializable.
                if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.NetworkConnection_FullName)
                    parameters.RemoveAt(--parametersCount);
            }

            //Removes channel if the last parameter.
            void RemoveTrailingChannel()
            {
                if (parametersCount == 0)
                    return;
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
        /// </summary>
        private void AddNamedTypeSymbolSerializablesWithIdentifier(object context, INamedTypeSymbol namedTypeSymbol, ISymbol source)
        {
            if (!namedTypeSymbol.HasSerializableIdentifier())
                return;

            /* FullNames added this iteration.
             * This is used to prevent endless loops. */
            HashSet<string> addedFullNames = new();
            AddSelfAndBaseTypeSerializables(context, namedTypeSymbol, addedFullNames, source);
        }

        /// <summary>
        /// Adds a type to serializableTypes.
        /// Returns true if added, false if already existed.
        /// </summary>
        private bool AddSerializableType(object context, INamedTypeSymbol namedTypeSymbol, ISymbol source)
        {
            const bool isMetadataName = false;

            //Has exclude serialization attribute.
            if (namedTypeSymbol.HasAttribute(FishNetConstants.ExcludeSerializationAttribute_FullName, isMetadataName))
                return false;

            string fullName = namedTypeSymbol.GetTypeSymbolFullName(isMetadataName);
            //Few other checks for types we want to ignore.
            if (fullName == typeof(ValueType).FullName)
                return false;
            if (fullName == typeof(Object).FullName)
                return false;

            //Check if already added.
            foreach (SerializableType st in TypesNeedingSerializers)
            {
                if (st.NamedTypeSymbol == namedTypeSymbol)
                    return false;
            }

            if (!namedTypeSymbol.HasPublicAccessibility()) // && !namedTypeSymbol.ContainingType.HasPartialModifier())
            {
                if (context is SyntaxNodeAnalysisContext analysisContext)
                    OnIsNotSerializableAccessible?.Invoke(analysisContext, source, string.Empty);

                return false;
            }

            if (TypesNeedingSerializers.Add(new(namedTypeSymbol)))
                Log($"Added {namedTypeSymbol.GetTypeSymbolFullName(isMetadataName)} to types needing serializers.");

            return true;
        }

        /// <summary>
        /// Iterates up the base types of a named symbol and adds them to serializables.
        /// </summary>
        /// <param name = "foundNames">A collection reference to store already found serializables during the iteration.</param>
        private void AddSelfAndBaseTypeSerializables(object context, INamedTypeSymbol namedTypeSymbol, HashSet<string> foundNames, ISymbol source)
        {
            while (true)
            {
                if (!TryAdd(namedTypeSymbol))
                    return;

                namedTypeSymbol = namedTypeSymbol?.BaseType;
            }

            bool TryAdd(INamedTypeSymbol lNamedTypeSymbol)
            {
                //No more base types, or cannot be serialized.s
                if (lNamedTypeSymbol is not INamedTypeSymbol)
                    return false;

                string fullName = lNamedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);

                //Already added.
                if (foundNames.Contains(fullName))
                    return false;

                /* The method indicated it could not add some reason.
                 * Maybe the type had an attribute. */
                if (!AddSerializableType(context, lNamedTypeSymbol, source))
                    return false;

                foundNames.Add(fullName);

                return true;
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
