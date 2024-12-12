using System;
using System.Text;

namespace FirstGearGames.CodeAnalysis.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder Indent(this StringBuilder stringBuilder, int count = 1)
        {
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            return count switch
            {
                0 => stringBuilder,
                1 => stringBuilder.Append('\t'),
                _ => stringBuilder.Append('\t', count)
            };
        }

        public static void Append(this StringBuilder sb, int indentCount, string text)
        {
            sb.Indent(indentCount).Append(text);
        }

        public static void AppendLine(this StringBuilder sb, int indentCount, string text)
        {
            sb.Indent(indentCount).AppendLine(text);
        }

        public static void AppendThrowLine(this StringBuilder sb, int indentCount, string text)
        {
            sb.Indent(indentCount).AppendLine($"throw new Exception(\"{text}\");");
        }



    }
}
