using System.Runtime.CompilerServices;

namespace FishNet.Serializing
{
    public partial class Reader
    {
        [Reader]
        public int ReadInt32() => default;

        [Reader]
        public string ReadString() => default;

        [Reader]
        public bool ReadBoolean() => default;

        [Reader]
        public float ReadSingle() => default;
        
        public ulong ZigZagDecode(ulong value) => default;

        public long ReadSignedPackedWhole() => default;

        public ulong ReadUnsignedPackedWhole() => default;

        public T Read<T>() => default;

        [DeltaReader]
        public bool ReadDeltaBoolean(bool valueA) => default;

        [DeltaReader]
        public int ReadDeltaInt32(int valueA) => default;

        [DeltaReader]
        public float ReadDeltaSingle(float valueA) => default;
    }
}

