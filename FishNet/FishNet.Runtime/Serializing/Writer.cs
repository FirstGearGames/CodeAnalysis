using System.Runtime.CompilerServices;

namespace FishNet.Serializing
{
    public struct TestStruct
    {
        public string A;
        public bool B;
        public int C;
    }

    public class TestClass
    {
        public string A;
        public bool B;
        public int C;
    }

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

        [Writer]
        public void WriteFloat(float value)
        {
            
        }
        public void WriteBytes(byte[] value, int offset, int count)
        {
        }

        public void WritePackedWhole(ulong value)
        {
        }

        public void Write<T>(T value)
        {
        }

        [DeltaWriter]
        public bool WriteDeltaInt32(int valueA, int valueB)
        {
            int next = (valueB - valueA);
            if (next != 0)
                WriteInt32(next);

            return (next != 0);
        }
        // [Writer]
        // public void WriteTestStruct(TestStruct value) { }
        // [Writer]
        // public void WriteTestClass(TestClass value) { }
    }
}

public class NoNameSpaceA
{
}