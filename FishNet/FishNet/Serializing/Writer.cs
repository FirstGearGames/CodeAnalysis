
using System.Runtime.CompilerServices;
namespace FishNet.Serializing
{
    public  struct  TestStruct
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

        [Writer]
        public void WriteInt32(int value) { }
        [Writer]
        public void WriteString(string value) { }
        [Writer]
        public void WriteBoolean(bool value) { }
        [Writer]
        public void WriteTestStruct(TestStruct value) { }
        [Writer]
        public void WriteTestClass(TestClass value) { }
    }
}
public class NoNameSpaceA { }