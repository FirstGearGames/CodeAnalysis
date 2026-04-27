using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for combining argument or string lists into source-style sequences.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// Combines the supplied arguments into a natural string in the form <c>&lt;str0, str1, str2 ...&gt;</c>.
    /// </summary>
    /// <param name="argumentList">Arguments to combine.</param>
    /// <returns>The combined argument string, or an empty string when no arguments are supplied.</returns>
    public static string GetCombinedArguments(this List<Argument> argumentList)
    {
        if (argumentList is null || argumentList.Count == 0)
            return string.Empty;

        StringBuilder stringBuilder = new();

        foreach (Argument methodArgument in argumentList)
        {
            // Add separate if argument already exists.
            if (stringBuilder.Length != 0)
                stringBuilder.Append(", ");

            stringBuilder.Append(methodArgument.Name);
        }

        return $"<{stringBuilder}>";
    }

    /// <summary>
    /// Combines the supplied strings into a natural string in the form <c>&lt;str0, str1, str2 ...&gt;</c>.
    /// </summary>
    /// <param name="stringList">Strings to combine.</param>
    /// <returns>The combined string, or an empty string when no values are supplied.</returns>
    public static string GetCombinedArguments(this List<string> stringList)
    {
        if (stringList is null || stringList.Count == 0)
            return string.Empty;

        StringBuilder stringBuilder = new();

        foreach (string argumentName in stringList)
        {
            // Add separate if argument already exists.
            if (stringBuilder.Length != 0)
                stringBuilder.Append(", ");

            stringBuilder.Append(argumentName);
        }

        return $"<{stringBuilder}>";
    }
}