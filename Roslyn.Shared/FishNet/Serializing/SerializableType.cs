using System;
using Microsoft.CodeAnalysis;
using Roslyn.Extensions;

namespace Roslyn.FishNet.Serializing
{
    public struct SerializableType : IEquatable<SerializableType>
    {
        /// <summary>
        /// Type the serializable is for.
        /// </summary>
        public ITypeSymbol TypeSymbol;
        /// <summary>
        /// FullName of the serializable.
        /// </summary>
        public readonly string FullName;
        /// <summary>
        /// FullName as Metadata of the serializable.
        /// </summary>
        public readonly string FullMetadataName;

        public SerializableType(ITypeSymbol typeSymbol)
        {
            TypeSymbol = typeSymbol;
            FullName = typeSymbol.GetSymbolFullName();
            FullMetadataName = typeSymbol.GetSymbolFullMetaName();
        }
        public SerializableType(ITypeSymbol typeSymbol, string fullName, string fullMetadataName)
        {
            TypeSymbol = typeSymbol;
            FullName = fullName;
            FullMetadataName = fullMetadataName;
        }

        public bool Equals(SerializableType other) => (other.FullName == this.FullName);
    }
}
