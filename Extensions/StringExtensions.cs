using System;
using System.IO;
using System.Text;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for transforming and inspecting strings used during code generation.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// The sentinel value indicating that an index is not found or has not been specified.
    /// </summary>
    public const int UnsetIndex = -1;

    /// <summary>
    /// Returns the supplied value, or an empty string when it is null.
    /// </summary>
    /// <param name="value">Value to inspect.</param>
    /// <returns>The supplied value, or an empty string when it is null.</returns>
    public static string EmptyIfNull(this string? value) => value ?? string.Empty;

    /// <summary>
    /// Formats a string with indents driven by opening and closing braces.
    /// </summary>
    /// <param name="thisValue">String to reformat.</param>
    /// <returns>The reformatted string with brace-driven indentation.</returns>
    public static string IndentByBrace(this string thisValue)
    {
        if (string.IsNullOrWhiteSpace(thisValue))
            return string.Empty;
            
        StringBuilder result = new();
        StringReader reader = new(thisValue);
            
        //Current indentation.
        int indentLevel = 0;
            
        while (reader.ReadLine() is { } readLine)
        {
            const string Indentation = "    ";

            // 1. Trim leading/trailing whitespace from the original line
            string trimmedLine = readLine.Trim();

            // Skip completely empty lines
            if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                result.AppendLine();
                continue;
            }

            // 2. Adjust indent level BEFORE prepending for closing braces '}'
            // If the line starts with '}', decrease the indent before applying it
            if (trimmedLine.StartsWith("}"))
                indentLevel = Math.Max(0, indentLevel - 1);

            // 3. Prepend the current indent level
            for (int i = 0; i < indentLevel; i++)
                result.Append(Indentation);

            // 4. Append the trimmed content of the line
            result.AppendLine(trimmedLine);

            // 5. Adjust indent level AFTER prepending for opening braces '{'
            // If the line contains '{', increase the indent for the NEXT line
            if (trimmedLine.Contains("{"))
                indentLevel++;
        }


        return result.ToString();
    }

    /// <summary>
    /// Removes a leading <c>global::</c> or <c>&lt;global namespace&gt;</c> prefix from the supplied value when present.
    /// </summary>
    /// <param name="value">Value to inspect.</param>
    /// <returns>The value with any global alias prefix removed.</returns>
    public static string RemoveGlobalAlias(this string value)
    {
        if (value.StartsWith("global::"))
            value = value.Substring(8);
        else if (value.StartsWith("<global namespace>"))
            value = value.Substring(18);
        return value;
    }

    /// <summary>
    /// Converts the supplied camelCase string to PascalCase, removing any non-letter and non-numeric prefix.
    /// </summary>
    /// <param name="value">String to convert.</param>
    /// <returns>The PascalCase representation of the supplied string.</returns>
    public static string CamelCaseToPascalCase(this string value)
    {
        int index = value.GetFirstLetterOrDigitIndex();

        //Index not found. String is null or has no chars/numbers.
        if (index == UnsetIndex)
            return value;

        char firstValidChar = value[index];

        //First character is not a letter, return as-is.
        if (!char.IsLetter(firstValidChar))
            return value;

        //Already capitalized.
        if (char.IsUpper(firstValidChar))
            return value;

        return $"{char.ToUpperInvariant(firstValidChar)}{value.Substring(index + 1)}";
    }

    /// <summary>
    /// Converts the supplied PascalCase string to camelCase, optionally prepending a prefix.
    /// </summary>
    /// <remarks>
    /// The prefix is only added when it is not already present at the start of the value.
    /// </remarks>
    /// <example>
    /// With a prefix of <c>_</c> the value <c>HelloWorld</c> is returned as <c>_helloWorld</c>.
    /// </example>
    /// <param name="value">String to convert.</param>
    /// <param name="prefix">Prefix to prepend when not already present.</param>
    /// <returns>The camelCase representation of the supplied string.</returns>
    public static string PascalCaseToCamelCase(this string value, string prefix)
    {
        int index = value.GetFirstLetterOrDigitIndex();

        //Index not found. String is null or has no chars/numbers.
        if (index == UnsetIndex)
            return value;

        char firstValidChar = value[index];

        /* There are marginally more efficient ways to handle these prefix operations
         * but allocations are going to occur either way - use what is easier to read. */

        StringBuilder stringBuilder = new();
        int prefixLength = prefix.Length;

        //There is a prefix.
        if (prefixLength > 0)
        {
            if (prefixLength >= value.Length)
            {
                stringBuilder.Append(prefix);

                AppendLowerFirstCharAndRemainingValue();

                return stringBuilder.ToString();
            }

            //If prefix is not yet added then do so.
            if (value.Substring(0, prefixLength) != prefix)
                stringBuilder.Append(prefix);
        }

        //Add renaming with lowercase char.
        AppendLowerFirstCharAndRemainingValue();

        return stringBuilder.ToString();

        //Appends lowercase first char, and any renaming text in value.
        void AppendLowerFirstCharAndRemainingValue()
        {
            stringBuilder.Append(char.ToLowerInvariant(firstValidChar));
            //If value has enough length remaining append it as well.
            if (value.Length >= index)
                stringBuilder.Append(value.Substring(index + 1));
        }
    }


    /// <summary>
    /// Returns the index of the first letter or digit in the supplied string.
    /// </summary>
    /// <param name="value">String to inspect.</param>
    /// <returns>The zero-based index of the first letter or digit, or <see cref="UnsetIndex"/> when none is found.</returns>
    public static int GetFirstLetterOrDigitIndex(this string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return UnsetIndex;

        for (int i = 0; i < value.Length; i++)
        {
            if (char.IsLetterOrDigit(value[i]))
                return i;
        }

        return UnsetIndex;
    }
        
    /// <summary>
    /// Returns the supplied value with all non-alphanumeric characters replaced by underscores so that it can be safely used as a file name.
    /// </summary>
    /// <param name="value">Value to make safe.</param>
    /// <returns>The supplied value with non-alphanumeric characters replaced by underscores.</returns>
    public static string MakeFileSafeName(this string value)
    {
        StringBuilder stringBuilder = new();
            
        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c))
                stringBuilder.Append(c);
            else
                stringBuilder.Append('_');
        }

        return stringBuilder.ToString();
    }

        
}