using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeAnalysis.Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Formats a string with indents using opening and closing braces and the determining character.
        /// </summary>
        /// <param name="thisValue"></param>
        /// <returns></returns>
        public static string IndentByBrace(this string thisValue)
        {
            StringBuilder result = new();
            StringReader reader = new(thisValue);
            
            //Current indentation.
            int indentLevel = 0;
            
            while (reader.ReadLine() is { } readLine)
            {
                const string indentation = "    ";

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
                    result.Append(indentation);

                // 4. Append the trimmed content of the line
                result.AppendLine(trimmedLine);

                // 5. Adjust indent level AFTER prepending for opening braces '{'
                // If the line contains '{', increase the indent for the NEXT line
                if (trimmedLine.Contains("{"))
                    indentLevel++;
            }


            return result.ToString();
        }

        public static string RemoveGlobalAlias(this string value)
        {
            if (value.StartsWith("global::"))
                value = value.Substring(8);
            else if (value.StartsWith("<global namespace>"))
                value = value.Substring(18);
            return value;
        }

        /// <summary>
        /// Replaces an artifact with a replacement.
        /// </summary>
        public static string Replace(this string value, string artifact, string replacement) => value.Replace(artifact, replacement);

        /// <summary>
        /// Replaces artifacts with replacements.
        /// </summary>
        public static string Replace(this string value, List<(string Artifact, string Replacement)> replacements)
        {
            foreach ((string Artifact, string Replacement) replacement in replacements)
                value = value.Replace(replacement.Artifact, replacement.Replacement);

            return value;
        }
    }
}