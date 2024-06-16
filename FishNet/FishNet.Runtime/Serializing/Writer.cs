using System.Runtime.CompilerServices;

namespace FishNet.Serializing
{
    public partial class Writer
    {
        public int Length;

        public byte[] GetBuffer() => default;

        [Writer]
        public void WriteInt32(int value)
        {
        }

        [Writer]
        public void WriteString(string value)
        {
        }

        [Writer]
        public void WriteBoolean(bool value)
        {
        }


        public void WriteBytes(byte[] value, int offset, int count)
        {
        }

        /// <summary>
        /// ZigZag encodes a signed integer and maps it to a unsigned integer.
        /// </summary>

        public ulong ZigZagEncode(ulong value) => 0;

        public void WriteSignedPackedWhole(long value)
        {
            WriteUnsignedPackedWhole(ZigZagEncode((ulong)value));
        }
        public void WriteUnsignedPackedWhole(ulong value)
        {
        }

        [Writer]
        public void WriteSingle(float value)
        {
        }

  
        public void Write<T>(T value)
        {
        }

        [DeltaWriter]
        public bool WriteDeltaBoolean(bool valueA, bool valueB)
        {
            return true;
        }
        [DeltaWriter]
        public bool WriteDeltaInt32(int valueA, int valueB)
        {
            int next = (valueB - valueA);
            if (next != 0)
                WriteInt32(next);

            return (next != 0);
        }
        
        
        [DeltaWriter]
        public bool WriteDeltaSingle(float valueA, float valueB)
        {
            float next = (valueB - valueA);
            if (next != 0f)
                WriteSingle(next);

            return (next != 0);
        }
        // [Writer]
        // public void WriteTestStruct(TestStruct value) { }
        // [Writer]
        // public void WriteTestClass(TestClass value) { }
    }
}
