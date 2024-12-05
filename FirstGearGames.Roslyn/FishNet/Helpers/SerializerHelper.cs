using System.Collections.Generic;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.FishNet.Helpers
{
    public static class SerializerHelper
    {
        /// <summary>
        /// True if the symbol has any RPC attribute. Use HasRpcAttributes(List<RpcAttributeData> for RPC details.
        /// </summary>
        public static bool HasRpcAttribute(this IMethodSymbol symbol) => symbol.HasRpcAttributes(out _);

        /// <summary>
        /// True if the symbol implements IBroadcast. 
        /// </summary>
        public static bool ImplementsIBroadcastInterface(this INamedTypeSymbol symbol)
        {
            INamedTypeSymbol namedTypeSymbol = symbol.GetUserDefinedNamedTypeSymbol();
            if (namedTypeSymbol == null) return false;

            return namedTypeSymbol.ImplementsInterface(FishNetConstants.IBroadcasts_FullName);
        }

        /// <summary>
        /// True if the symbol implements any prediction interfaces.
        /// </summary>
        /// <returns></returns>
        public static bool ImplementsPredictionInterface(this INamedTypeSymbol symbol)
        {
            INamedTypeSymbol namedTypeSymbol = symbol.GetUserDefinedNamedTypeSymbol();
            if (namedTypeSymbol == null) return false;

            return namedTypeSymbol.ImplementsInterface(FishNetConstants.IReplicateInterface_FullName) || namedTypeSymbol.ImplementsInterface(FishNetConstants.IReconcileInterface_FullName);
        }

        /// <summary>
        /// True if symbol has the IncludeSerialization attribute.
        /// </summary>
        public static bool HasIncludeSerializationAttribute(this INamedTypeSymbol symbol)
        {
            INamedTypeSymbol namedTypeSymbol = symbol.GetUserDefinedNamedTypeSymbol();
            if (namedTypeSymbol == null) return false;

            return namedTypeSymbol.HasAttribute(FishNetConstants.GenerateSerializersAttribute_FullName, isMetadataName: false, out _);
        }

        /// <summary>
        /// True if symbol inherits SyncBase anywhere within it's hierarchy.
        /// </summary>
        public static bool InheritsSyncBase(this INamedTypeSymbol symbol)
        {
            if (symbol is not INamedTypeSymbol namedTypeSymbol)
                return false;

            return namedTypeSymbol.InheritsClass(FishNetConstants.SyncBase_FullName);
        }

        /// <summary>
        /// True if symbol has any identifiers which mark it for serialization.
        /// This can include IBroadcast, RPC methods, SyncTypes, prediction interfaces, GenerateSerializer attribute.
        /// </summary>
        public static bool HasSerializableIdentifier(this ISymbol symbol)
        {
            //Methods.
            if (symbol is IMethodSymbol methodSymbol)
            {
                return methodSymbol.HasRpcAttribute();
            }
            //Field members.
            else if (symbol is IFieldSymbol fieldSymbol)
            {
                return fieldSymbol.IsSyncType();
            }
            //Named types.
            else if (symbol is INamedTypeSymbol namedTypeSymbol)
            {
                if (namedTypeSymbol.ImplementsIBroadcastInterface())
                    return true;
                if (namedTypeSymbol.ImplementsPredictionInterface())
                    return true;
                if (namedTypeSymbol.HasIncludeSerializationAttribute())
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the generic arguments of a namedTypeSymbol.
        /// </summary>
        public static List<ITypeSymbol> GetGenericArgumentsOfNamedTypeSymbol(this INamedTypeSymbol namedTypeSymbol)
        {
            List<ITypeSymbol> results = new();

            //Not generic.
            if (!namedTypeSymbol.IsGenericType)
                return results;

            foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                results.Add(typeArgument);

            return results;
        }

        /// <summary>
        /// Returns INamedTypeSymbol if symbol is a user define typed. Otherwise returns null.
        /// </summary>
        private static INamedTypeSymbol? GetUserDefinedNamedTypeSymbol(this ISymbol symbol)
        {
            //If not named it cannot be class or struct.
            if (symbol is not INamedTypeSymbol namedTypeSymbol)
                return null;
            //If not user defined then it won't implement the interface.
            if (!namedTypeSymbol.IsUserDefinedClassOrStruct())
                return null;

            return namedTypeSymbol;
        }
    }
}