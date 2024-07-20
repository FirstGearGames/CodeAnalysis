using System.Runtime.CompilerServices;

namespace FishNet.Serializing
{

    /// <summary>
    /// Used for write references to generic types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class GenericDeltaWriter<T>
    {
        public static Func<Writer, T, T, DeltaSerializerOption, bool> Write { get; private set; }

        public static void SetWrite(Func<Writer, T, T, DeltaSerializerOption, bool> value)
        {
            Write = value;
        }
    }

}
