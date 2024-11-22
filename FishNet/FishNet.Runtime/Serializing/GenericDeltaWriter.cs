
namespace FishNet.Serializing
{

    /// <summary>
    /// Used to write generic types.
    /// </summary>
    public static class GenericDeltaWriter<T>
    {
        public static Func<Writer, T, T, DeltaSerializerOption, bool> Write { get; internal set; }
        
        public static void SetWrite(Func<Writer, T, T, DeltaSerializerOption, bool> value) { }
    }

}