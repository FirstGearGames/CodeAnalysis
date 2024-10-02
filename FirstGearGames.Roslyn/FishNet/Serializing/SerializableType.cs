using FirstGearGames.Roslyn.Extensions;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.FishNet.Serializing
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
            FullName = typeSymbol.GetTypeSymbolFullNameWithGenericArguments();
            FullMetadataName = typeSymbol.GetTypeSymbolFullMetadataName();
        }
    }
}