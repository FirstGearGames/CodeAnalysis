
using System.Runtime.CompilerServices;

namespace FishNet.Serializing
{
    public partial class Writer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [Writer]
        public void WriteInt32(int value) { }
    }
}
public class NoNameSpaceA { }