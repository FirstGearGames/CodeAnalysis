using System;

namespace FishNet.SourceGenerating.CodeBuilding
{
    internal struct SerializableType : IEquatable<SerializableType>
    {
        public readonly string FullName;
        public readonly string FullMetadataName;

        public SerializableType(string fullName, string fullMetadataName)
        {
            FullName = fullName;
            FullMetadataName = fullMetadataName;
        }

        public bool Equals(SerializableType other) => (other.FullName == this.FullName);
    }
}
