
namespace FishNet.Serializing
{
    /// <summary>
    /// Used to read generic types.
    /// </summary>
    public static class GenericReader<T>
    {
        public static Func<Reader, T> Read { get; set; }        

        public static void SetRead(Func<Reader, T> value)
        {
        }

    }
}