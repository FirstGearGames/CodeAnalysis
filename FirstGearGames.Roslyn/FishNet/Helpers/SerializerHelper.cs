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
        public static bool HasRpcAttribute(this ISymbol symbol) => symbol.HasRpcAttributes(out _);

        /// <summary>
        /// True if the symbol implements IBroadcast. 
        /// </summary>
        public static bool HasIBroadcastInterface(this ISymbol symbol)
        {
            INamedTypeSymbol namedTypeSymbol = symbol.GetUserDefinedNamedTypeSymbol();
            if (namedTypeSymbol == null) return false;

            return namedTypeSymbol.ImplementsInterface(FishNetConstants.BroadcastsInterface_FullName);
        }
        
        /// <summary>
        /// True if the symbol implements any prediction interfaces.
        /// </summary>
        /// <returns></returns>
        public static bool HasPredictionInterface(this ISymbol symbol)
        {
            INamedTypeSymbol namedTypeSymbol = symbol.GetUserDefinedNamedTypeSymbol();
            if (namedTypeSymbol == null) return false;

            return namedTypeSymbol.ImplementsInterface(FishNetConstants.IReplicateInterface_FullName) || namedTypeSymbol.ImplementsInterface(FishNetConstants.IReconcileInterface_FullName);
        }


        /// <summary>
        /// True if symbol has the IncludeSerialization attribute.
        /// </summary>
        public static bool HasIncludeSerializationAttribute(this ISymbol symbol)
        {
            INamedTypeSymbol namedTypeSymbol = symbol.GetUserDefinedNamedTypeSymbol();
            if (namedTypeSymbol == null) return false;

            return  namedTypeSymbol.HasAttribute(FishNetConstants.IncludeSerializationAttribute_FullName, isMetadataName: false, out _);
        }

        /// <summary>
        /// True if the symbol will have serializable types. This would be true for RPCs, IBroadcast, Replicates, and more.
        /// </summary>
        public static bool HasAnySerializable(this ISymbol symbol)
        {
            if (symbol.HasIBroadcastInterface())
                return true;
            if (symbol.HasRpcAttribute())
                return true;
            if (symbol.HasPredictionInterface())
                return true;
            if (symbol.HasIncludeSerializationAttribute())
                return true;
            
            return false;
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