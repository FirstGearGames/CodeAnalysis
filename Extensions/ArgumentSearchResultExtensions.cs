using CodeAnalysis.Finding;

namespace CodeAnalysis.Extensions;


/// <summary>
/// Extension methods for inspecting <see cref="ArgumentSearchResult"/> values.
/// </summary>
public static class ArgumentSearchResultExtensions
{
    /// <summary>
    /// Returns whether the supplied result represents an error for the requested search type.
    /// </summary>
    /// <param name="thisArgumentSearchResult">Result value to inspect.</param>
    /// <returns>True when the result has the error flag set.</returns>
    public static bool HasError(this ArgumentSearchResult thisArgumentSearchResult) => thisArgumentSearchResult.HasFlag(ArgumentSearchResult.ErrorForSearchType);
}
