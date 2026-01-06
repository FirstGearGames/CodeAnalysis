using System.Collections.Generic;
using System.Text;

namespace CodeAnalysis.Common.Extensions
{
    public static class ListExtensions
    {

        private static StringBuilder _stringBuilder = new();
        
        /// <summary>
        /// Combines into a natural string: <str0, str1, str2 ...>
        /// </summary>
        public static string GetCombinedArguments(this List<Argument> arguments)
        {
            if (arguments.Count == 0)
                return string.Empty;

            _stringBuilder.Clear();

            foreach (Argument methodArgument in arguments)
            {
                //Add separate if argument already exists.
                if (_stringBuilder.Length != 0)
                    _stringBuilder.Append(", ");

                _stringBuilder.Append(methodArgument.Name);
            }

            return $"<{_stringBuilder}>";
        }
        
        /// <summary>
        /// Combines into a natural string: <str0, str1, str2 ...>
        /// </summary>
        public static string GetCombinedArguments(this List<string> thisValue)
        {
            if (thisValue.Count == 0)
                return string.Empty;

            _stringBuilder.Clear();

            foreach (string argumentName in thisValue)
            {
                //Add separate if argument already exists.
                if (_stringBuilder.Length != 0)
                    _stringBuilder.Append(", ");

                _stringBuilder.Append(argumentName);
            }

            return $"<{_stringBuilder}>";
        }
    }
}