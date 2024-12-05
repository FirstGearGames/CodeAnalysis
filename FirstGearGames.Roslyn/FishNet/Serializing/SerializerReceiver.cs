using System;
using System.Collections.Generic;
using System.Linq;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.Native.Constants;
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
        public readonly HashSet<SerializableType> TypesNeedingSerializers = new();

        public void FindStructSerializables(GeneratorSyntaxContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structDeclaration);
            if (symbol == null) return;

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindNamedTypeSymbolSerializables(context, namedTypeSymbol);
        }

        public void FindStructSerializables(SyntaxNodeAnalysisContext context, StructDeclarationSyntax structDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, structDeclaration);
            if (symbol == null) return;

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindNamedTypeSymbolSerializables(context, namedTypeSymbol);
        }

        public void FindClassSerializables(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);
            if (symbol == null) return;

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindClassSerializables(context, namedTypeSymbol, context.SemanticModel);
        }

        public void FindClassSerializables(SyntaxNodeAnalysisContext context, ClassDeclarationSyntax classDeclaration)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclaration);
            if (symbol == null) return;

            if (symbol is not INamedTypeSymbol namedTypeSymbol) return;

            FindClassSerializables(context, namedTypeSymbol, context.SemanticModel);
        }

        private void FindClassSerializables(object context, INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            //Find syncTypes. This only runs if not a NetworkBehaviour. 
            FindSyncTypeSerializables(context, namedTypeSymbol, semanticModel);

            /* Find serializable fields if namedType can be serializers.
             * This only runs if not inheriting NetworkBehaviour. */

            // This is trying to serialize literally all classes, including anything* that inherits syncBase and so on.Is this really needed since we target* types for serialization
            // such as RPcs and so on ? */
            //FindNamedTypeSymbolSerializables(context, namedTypeSymbol);
        }

        /// <summary>
        /// Finds SyncType serializables within a namedTypeSymbol.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="namedTypeSymbol"></param>
        public void FindSyncTypeSerializables(object context, INamedTypeSymbol namedTypeSymbol, SemanticModel semanticModel)
        {
            //Named type must inherit NetworkBehaviour to look for SyncTypes.
            if (!namedTypeSymbol.InheritsClass(FishNetConstants.NetworkBehaviour_FullName))
                return;

            List<IFieldSymbol> fieldSymbols = namedTypeSymbol.GetFieldMembers();

            //Check all field types to see if they inherit SyncBase.
            foreach (IFieldSymbol fieldSymbol in fieldSymbols)
            {
                ITypeSymbol fieldType = fieldSymbol.Type;

                //SyncTypes can only be named type. Continue if it's not namedType.
                if (fieldType is not INamedTypeSymbol fieldNamedTypeSymbol)
                    continue;

                //Get SyncType.
                SyncTypeType stt = fieldNamedTypeSymbol.GetSyncType();

                //No syncType.
                if (stt == SyncTypeType.Unset)
                    continue;

                //Custom.
                if (stt == SyncTypeType.Custom)
                    FindCustomSyncTypeSerializable(fieldNamedTypeSymbol, semanticModel);
                //SyncType is built into FishNet with generics.
                else
                    FindIncludedGenericSyncTypeSerializable(fieldNamedTypeSymbol);
            }

            void FindIncludedGenericSyncTypeSerializable(INamedTypeSymbol fieldNamedTypeSymbol)
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

            /* Our cecil logic reviews the IL instructions in
             * the method to determine what named type is being returned
             * in the 'object GetSerializableType' method.
             *
             * This is not possible in Roslyn. Users instead will just
             * need to put the 'GenerateSerializers' attribute on their
             * type. */
            void FindCustomSyncTypeSerializable(INamedTypeSymbol fieldNamedTypeSymbol, SemanticModel semanticModel)
            {
                IMethodSymbol methodSymbol = fieldNamedTypeSymbol.GetMethod(FishNetConstants.ICustomSync_GetSerializedType_Name, metadataName: false);
                if (methodSymbol == null) return;

                //Default return type should be System.Object, exit if not the case.
                if (methodSymbol.ReturnType.GetTypeSymbolFullName(metadataName: false) != NativeConstants.Object_FullName) return;

                List<ExpressionSyntax> returnExpressions = methodSymbol.GetReturnedExpressionSyntaxes();
                //No entries, or first entry is not named.
                if (returnExpressions.Count == 0) return;
                if (returnExpressions[0] is not TypeOfExpressionSyntax typeOfExpressionSyntax) return;
                
                ITypeSymbol returnedTypeSymbol = typeOfExpressionSyntax.GetTypeOfInner(semanticModel);
                if (returnedTypeSymbol == null || returnedTypeSymbol is not INamedTypeSymbol returnedNamedTypeSymbol) return;
                /* FullNames added this iteration.
                 * This is used to prevent endless loops. */
                HashSet<string> addedFullNames = new();
                //Add first entry.
                RecursivelyAddSerializables(context, returnedNamedTypeSymbol, addedFullNames);
            }
        }

        public void FindRpcSerializables(GeneratorSyntaxContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);
            if (symbol == null)
                return;

            if (symbol is not IMethodSymbol methodSymbol) return;

            FindRpcSerializables(context, methodSymbol);
        }

        public void FindRpcSerializables(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(context.SemanticModel, methodDeclarationSyntax);
            if (symbol == null)
                return;

            if (symbol is not IMethodSymbol methodSymbol) return;

            FindRpcSerializables(context, methodSymbol);
        }

        private void FindRpcSerializables(object context, IMethodSymbol methodSymbol)
        {
            if (!methodSymbol.HasRpcAttributes(out List<RpcAttributeData> results)) return;

            List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
            int parametersCount = parameters.Count;

            const bool metadataName = false;

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
                    if (parameters[0].Type.GetTypeSymbolFullName(metadataName) == NetworkConnection_FullName)
                        parameters.RemoveAt(--parametersCount);
                }

                //Removes networkConnection if the last parameter.
                void RemoveTrailingNetworkConnection()
                {
                    if (parametersCount == 0) return;
                    //Remove channel from serializable.
                    if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == NetworkConnection_FullName)
                        parameters.RemoveAt(--parametersCount);
                }

                //Removes channel if the last parameter.
                void RemoveTrailingChannel()
                {
                    if (parametersCount == 0) return;
                    if (!parameters[parametersCount - 1].IsOptional) return;
                    //Remove channel from serializable.
                    if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == Channel_FullName)
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
            /* Named type cannot inherit NetworkBehaviour as normal fields
             * within a NetworkBehaviour should not be targeted for serialization. */
            if (namedTypeSymbol.InheritsClass(FishNetConstants.NetworkBehaviour_FullName))
                return;

            if (!namedTypeSymbol.HasAnySerializable())
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

            TypeScope typeScope = GetTypeScope(context, namedTypeSymbol);
            if (typeScope == TypeScope.Unset)
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

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [SerializerReceiver] {txt}");
        }
    }
}