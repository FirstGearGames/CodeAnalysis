
namespace FishNet.Serializing
{

    /// <summary>
    /// Used to read generic types.
    /// </summary>
    public static class GenericDeltaReader<T>
    {
        public static Func<Reader, T, T> Read { get; internal set; }

        public static void SetRead(Func<Reader, T, T> value)
        {
        }
    }

}