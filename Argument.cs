using System.Collections.Generic;

namespace CodeAnalysis
{
    public readonly struct Argument
    {
        public readonly string Name;
        public readonly bool IsNamed;

        public Argument(string name, bool isNamed)
        {
            Name = name;
            IsNamed = isNamed;
        }
    }

    public static class ArgumentExtensions 
    {
        /// <summary>
        /// Returns if all arguments are named. 
        /// </summary>
        /// <returns>True if there are arguments and all are named.</returns>
        public static bool AreArgumentsNamed(this List<Argument> methodArguments) 
        {
            if (methodArguments.Count == 0)
                return false;

            foreach (Argument argument in methodArguments)
            {
                if (!argument.IsNamed)
                    return false;
            }

            return true;
        }
    }
}