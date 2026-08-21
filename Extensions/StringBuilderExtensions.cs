using System;
using System.Text;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for emitting indented and bracketed text into a <see cref="StringBuilder"/>.
/// </summary>
public static class StringBuilderExtensions
{
    /// <summary>
    /// Appends the requested number of indentation tab characters to the supplied builder.
    /// </summary>
    /// <param name="stringBuilder">Builder to append indentation to.</param>
    /// <param name="count">Number of tab characters to append.</param>
    /// <returns>The supplied builder.</returns>
    public static StringBuilder Indent(this StringBuilder stringBuilder, int count = 1)
    {
        if (count < 0)
            throw new ArgumentOutOfRangeException(nameof(count));

        return count switch
        {
            0 => stringBuilder,
            1 => stringBuilder.Append('\t'),
            _ => stringBuilder.Append('\t', count)
        };
    }

    /// <summary>
    /// Appends the supplied text followed by an empty line to the builder.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="text">Text to append before the empty line.</param>
    public static void AppendDoubleLine(this StringBuilder stringBuilder, string text)
    {
        stringBuilder.AppendLine(text);
        stringBuilder.AppendLine();
    }

    /// <summary>
    /// Appends the supplied text followed by a line containing an opening brace.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="text">Text to append before the opening brace.</param>
    public static void AppendLineWithOpeningBracket(this StringBuilder stringBuilder, string text)
    {
        stringBuilder.AppendLine(text);
        stringBuilder.AppendLine("{");
    }

    /// <summary>
    /// Appends the supplied text followed by a line containing a closing brace, with an optional trailing blank line.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="text">Text to append before the closing brace.</param>
    /// <param name="doubleReturn">When true, appends an additional blank line after the closing brace.</param>
    public static void AppendLineWithClosingBracket(this StringBuilder stringBuilder, string text, bool doubleReturn)
    {
        stringBuilder.AppendLine(text);
        stringBuilder.AppendLine("}");
        if (doubleReturn)
            stringBuilder.AppendLine(string.Empty);
    }

    /// <summary>
    /// Appends a line containing a closing brace, with an optional trailing blank line.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="doubleReturn">When true, appends an additional blank line after the closing brace.</param>
    public static void AppendLineWithClosingBracket(this StringBuilder stringBuilder, bool doubleReturn)
    {
        stringBuilder.AppendLine("}");
        if (doubleReturn)
            stringBuilder.AppendLine(string.Empty);
    }

    /// <summary>
    /// Appends the supplied text after the requested number of indentation units.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="indentCount">Number of indentation units to prepend.</param>
    /// <param name="text">Text to append after the indentation.</param>
    public static void Append(this StringBuilder stringBuilder, int indentCount, string text) => stringBuilder.Indent(indentCount).Append(text);

    /// <summary>
    /// Appends the supplied text after the requested number of indentation units, followed by a line break.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="indentCount">Number of indentation units to prepend.</param>
    /// <param name="text">Text to append after the indentation.</param>
    public static void AppendLine(this StringBuilder stringBuilder, int indentCount, string text) => stringBuilder.Indent(indentCount).AppendLine(text);

    /// <summary>
    /// Appends a <c>throw new Exception(...)</c> line containing the supplied message after the requested number of indentation units.
    /// </summary>
    /// <param name="stringBuilder">Builder to append to.</param>
    /// <param name="indentCount">Number of indentation units to prepend.</param>
    /// <param name="text">Exception message to embed in the throw statement.</param>
    public static void AppendThrowLine(this StringBuilder stringBuilder, int indentCount, string text)
    {
        stringBuilder.Indent(indentCount).AppendLine($"throw new Exception(\"{text}\");");
    }
}