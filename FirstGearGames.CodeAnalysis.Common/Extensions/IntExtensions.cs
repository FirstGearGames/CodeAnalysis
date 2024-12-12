using System.Text;

namespace FirstGearGames.CodeAnalysis.Extensions
{
    public static class IntExtensions
    {
        private static StringBuilder _stringBuilder = new();

        public static string ToIndent(this int value)
        {
            _stringBuilder.Clear();
            _stringBuilder.Indent(value);
            return _stringBuilder.ToString();
        }
    }
}
