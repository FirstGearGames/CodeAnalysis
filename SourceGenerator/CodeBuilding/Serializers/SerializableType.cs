using System;

namespace SourceGenerator.CodeBuilding.Serializers
{
    internal struct SerializableType : IEquatable<SerializableType>
    {
        public readonly string FullName;
        public readonly string MetadataName;

        public SerializableType(string fullName, string metadataName)
        {
            FullName = fullName;
            MetadataName = metadataName;
        }

        public bool Equals(SerializableType other) => (other.FullName == this.FullName);
    }
}
