namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers.Extensions
{
    public static class StringExtensions
    {
        public static string RemovePeriods(this string value, string newValue = "") => value.Replace(".", newValue);

        public static string RemoveGlobalAlias(this string value)
        {
            if (value.StartsWith("global::")) value = value.Substring(8);
            else if (value.StartsWith("<global namespace>")) value = value.Substring(18);
            return value;
        }
    }
}
