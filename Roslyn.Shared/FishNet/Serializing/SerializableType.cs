using System;
using Microsoft.CodeAnalysis;
using Roslyn.Extensions;

namespace Roslyn.FishNet.Serializing
{
    public struct SerializableType : IEquatable<SerializableType>
    {
        public enum TypeExposure
        {
            Unset = 0,
            PublicOrInternal = 1,
            NestedWithinPartial = 2,
        }

        public TypeExposure Exposure;
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

        public SerializableType(ITypeSymbol typeSymbol, TypeExposure exposure)
        {
            TypeSymbol = typeSymbol;
            FullName = typeSymbol.GetSymbolFullName();
            FullMetadataName = typeSymbol.GetSymbolFullMetaName();
            Exposure = exposure;
        }
        public SerializableType(ITypeSymbol typeSymbol, string fullName, string fullMetadataName, TypeExposure exposure)
        {
            TypeSymbol = typeSymbol;
            FullName = fullName;
            FullMetadataName = fullMetadataName;
            Exposure = exposure;
        }

        public bool Equals(SerializableType other) => (other.FullName == this.FullName);
    }
}
