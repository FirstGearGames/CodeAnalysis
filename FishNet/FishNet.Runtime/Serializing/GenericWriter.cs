
namespace FishNet.Serializing
{
    /// <summary>
    /// Used to write generic types.
    /// </summary>
    public static class GenericWriter<T>
    {
        public static Action<Writer, T> Write { get; private set; }

        public static void SetWrite(Action<Writer, T> value)
        {
        }
    }
}