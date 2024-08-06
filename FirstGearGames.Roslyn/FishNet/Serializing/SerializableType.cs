using System;
using FirstGearGames.Roslyn.Extensions;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.FishNet.Serializing
{
    public struct SerializableType : IEquatable<SerializableType>
    {
        public enum TypeExposure
        {
            Unset = 0,
            Public = 1,
            Internal = 2,
            Partial = 3,
        }

        // public TypeExposure Exposure;
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
        // /// <summary>
        // /// Containing class of the type. When set serializers should be written as a partial of this class.
        // /// </summary>
        // public readonly string ContainingClassNamespace;
        // /// <summary>
        // /// Containing class of the type. When set serializers should be written as a partial of this class.
        // /// </summary>
        // public readonly string ContainingClass;

        public SerializableType(ITypeSymbol typeSymbol, TypeExposure exposure)
        {
            TypeSymbol = typeSymbol;
            FullName = typeSymbol.GetSymbolFullName();
            FullMetadataName = typeSymbol.GetSymbolFullMetaName();

            // if (exposure == TypeExposure.NestedWithinPartial)
            // {
            //     ContainingClassNamespace = typeSymbol.GetNamespace();
            //     ContainingClass = typeSymbol.Name;
            // }
        }

        public SerializableType(string fullName, string fullMetadataName, string containingClassNamespace, string containingClass)
        {
            FullName = fullName;
            FullMetadataName = fullMetadataName;
            // ContainingClassNamespace = containingClassNamespace;
            // ContainingClass = containingClass;
        }

        public bool Equals(SerializableType other) => (other.FullName == this.FullName);
    }
}