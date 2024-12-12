using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Extensions;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers.Serializing
{
    public struct SerializableType
    {
        // public TypeExposure Exposure;
        /// <summary>
        /// Type the serializable is for.
        /// </summary>
        public ITypeSymbol TypeSymbol;
        /// <summary>
        /// Full name of the type.
        /// </summary>
        public string FullName;
        /// <summary>
        /// Full meta name of the type.
        /// </summary>
        public string FullMetadataName;

        public SerializableType(ITypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
            FullName = typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
            FullMetadataName = typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: true);
        }
    }
}
