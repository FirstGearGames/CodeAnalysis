using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Constants;
using Microsoft.CodeAnalysis;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Extensions;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers.Serializing;

namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers
{
    public enum SyncTypeType
    {
        Unset,
        SyncDictionary,
        SyncHashSet,
        SyncList,
        SyncVar,
        Custom,
    }

    public static class SyncTypeHelper
    {
        /// <summary>
        /// True if symbol is a SyncType.
        /// </summary>
        public static bool IsSyncType(this IFieldSymbol symbol)
        {
            if (symbol.Type is not INamedTypeSymbol namedTypeSymbol) return false;

            return (namedTypeSymbol.GetSyncType() != SyncTypeType.Unset);
        }

        /// <summary>
        /// SyncType symbol is.
        /// </summary>
        public static SyncTypeType GetSyncType(this INamedTypeSymbol symbol)
        {
            if (!symbol.InheritsSyncBase())
                return SyncTypeType.Unset;

            string symbolFullName = symbol.GetTypeSymbolFullName(metadataName: false);

            if (symbolFullName == FishNetConstants.SyncDictionary_FullName)
                return SyncTypeType.SyncDictionary;
            if (symbolFullName == FishNetConstants.SyncList_FullName)
                return SyncTypeType.SyncList;
            if (symbolFullName == FishNetConstants.SyncVar_FullName)
                return SyncTypeType.SyncVar;
            if (symbolFullName == FishNetConstants.SyncHashSet_FullName)
                return SyncTypeType.SyncHashSet;
            if (symbol.ImplementsInterface(FishNetConstants.ICustomSync_FullName))
                return SyncTypeType.Custom;

            //Fall through or unhandle, such as custom synctypes.
            return SyncTypeType.Unset;
        }

    }
}
