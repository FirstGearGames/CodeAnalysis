namespace FishNet.SourceAnaylze.Constants
{
    public static class DiagnosticIds
    {
        /// <summary>
        /// Network serializable types must be declared public. This limitation will be removed in a later release.
        /// </summary>
        public const string FN0001 = nameof(FN0001);
    }

    public static class DiagnosticCategories
    {
        /// <summary>
        /// Indicates a change in usage is required.
        /// </summary>
        public const string Usage = "Usage";
    }
}