namespace FishNet.SourceAnaylze.Constants
{
    internal static class DiagnosticIds
    {
        /// <summary>
        /// Network serializable types must be declared internal or public, or the declaring class must be partial.
        /// </summary>
        public const string FN0001 = nameof(FN0001);
    }

    internal static class DiagnosticCategories
    {
        /// <summary>
        /// Indicates a change in usage is required.
        /// </summary>
        public const string Usage = "Usage";
    }
}