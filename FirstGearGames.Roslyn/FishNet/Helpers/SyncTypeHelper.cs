using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;

namespace FirstGearGames.Roslyn.FishNet.Helpers
{
    public enum SyncTypeType
    {
        Unset,
        SyncDictionary,
        SyncHashSet,
        SyncList,
        SyncVar,
    }
    
    public static class SyncTypeHelper
    {
        /// <summary>
        /// True if symbol is a SyncType.
        /// </summary>
        public static bool IsSyncType(this ISymbol symbol) => symbol.InheritsSyncBase();

        /// <summary>
        /// SyncType symbol is.
        /// </summary>
        public static SyncTypeType GetSyncType(this ISymbol symbol)
        {
            //Only named can be syncTypes.
            if (symbol is not INamedTypeSymbol namedTypeSymbol)
                return SyncTypeType.Unset;
            
            if (!symbol.InheritsSyncBase())
                return SyncTypeType.Unset;

            string symbolFullName = namedTypeSymbol.GetTypeSymbolFullName(metadataName: false);

            if (symbolFullName == FishNetConstants.SyncDictionary_FullName)
                return SyncTypeType.SyncDictionary;
            if (symbolFullName == FishNetConstants.SyncList_FullName)
                return SyncTypeType.SyncList;
            if (symbolFullName == FishNetConstants.SyncVar_FullName)
                return SyncTypeType.SyncVar;
            if (symbolFullName == FishNetConstants.SyncHashSet_FullName)
                return SyncTypeType.SyncHashSet;            
            
            //Fall through or unhandle, such as custom synctypes.
            return SyncTypeType.Unset;
        }

    }
}