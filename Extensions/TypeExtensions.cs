using System;

namespace CodeAnalysis.Common.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the full name of a Type.
        /// </summary>
        /// <remarks>The returned string does not include the global alias.</remarks>
        public static string GetFullName(this Type type)
        {
            string? fullName = type?.FullName;
            
            if (fullName is null)
                return string.Empty;

            return fullName;
        }
        
        /// <summary>
        /// Gets the full name of a Type while removing generic arguments and brackets.
        /// </summary>
        /// <remarks>The returned string does not include the global alias.</remarks>
        public static string GetFullNameWithoutGenerics(this Type type)
        {
            string fullName = type.GetFullName();

            int genericMarkerIndex = fullName.IndexOf("`", StringComparison.InvariantCultureIgnoreCase);
            if (genericMarkerIndex >= 0)
                return fullName.Substring(0, genericMarkerIndex);
            
            return fullName;
        }
    }
}