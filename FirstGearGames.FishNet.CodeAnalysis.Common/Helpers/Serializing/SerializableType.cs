using FirstGearGames.CodeAnalysis.Extensions;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing
{
    public struct SerializableType
    {
        // public TypeExposure Exposure;
        /// <summary>
        /// Type the serializable is for.
        /// </summary>
        public INamedTypeSymbol NamedTypeSymbol;
        /// <summary>
        /// Full name of the type.
        /// </summary>
        public string FullName;
        /// <summary>
        /// Full meta name of the type.
        /// </summary>
        public string FullMetadataName;

        public SerializableType(INamedTypeSymbol namedTypeSymbol)
        {
            NamedTypeSymbol = namedTypeSymbol;
            FullName = namedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
            FullMetadataName = namedTypeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: true);
        }
    }
}