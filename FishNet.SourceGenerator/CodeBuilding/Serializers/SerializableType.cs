using System;
using Microsoft.CodeAnalysis;

namespace FishNet.SourceGenerating.CodeBuilding
{
    internal struct SerializableType : IEquatable<SerializableType>
    {
        /// <summary>
        /// FullName of the serializable.
        /// </summary>
        public readonly string FullName;
        /// <summary>
        /// FullName as Metadata of the serializable.
        /// </summary>
        public readonly string FullMetadataName;

        public SerializableType(string fullName, string fullMetadataName)
        {
            FullName = fullName;
            FullMetadataName = fullMetadataName;
        }

        public bool Equals(SerializableType other) => (other.FullName == this.FullName);
    }
}
