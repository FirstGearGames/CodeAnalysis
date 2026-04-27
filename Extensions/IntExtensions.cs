using System.Text;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for converting integer values into source-text helpers.
/// </summary>
public static class IntExtensions
{
    private static StringBuilder _stringBuilder = new();

    /// <summary>
    /// Returns a string containing the supplied number of indentation units.
    /// </summary>
    /// <param name="value">Number of indentation units to emit.</param>
    /// <returns>A string containing the requested indentation.</returns>
    public static string ToIndent(this int value)
    {
        _stringBuilder.Clear();
        _stringBuilder.Indent(value);
        return _stringBuilder.ToString();
    }
}