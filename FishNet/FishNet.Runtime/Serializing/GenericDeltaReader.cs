namespace FishNet.Serializing
{

    /// <summary>
    /// Used to read generic types.
    /// </summary>
    public static class GenericDeltaReader<T>
    {
        public static Func<Reader, T, T> Read { get; private set; }
        /// <summary>
        /// True if this type has a custom writer.
        /// </summary>
        private static bool _hasCustomSerializer;

        public static void SetReader(Func<Reader, T, T> value)
        {
            Read = value;
        }
    }

}