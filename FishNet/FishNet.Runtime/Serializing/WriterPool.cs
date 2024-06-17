namespace FishNet.Serializing
{
    /// <summary>
    /// Writer which is reused to save on garbage collection and performance.
    /// </summary>
    public sealed class PooledWriter : Writer
    {
        public void Store()
        {
            
        }
    }

    /// <summary>
    /// Collection of PooledWriter. Stores and gets PooledWriter.
    /// </summary>
    public static class WriterPool
    {
        /// <summary>
        /// Gets a writer from the pool.
        /// </summary>
        public static PooledWriter Retrieve() => new();
    }
}