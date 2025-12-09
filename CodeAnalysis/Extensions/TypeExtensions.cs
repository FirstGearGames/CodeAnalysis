using System;

namespace CodeAnalysis.Common.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Gets the full name of a Type.
        /// </summary>
        /// <remarks>The returned string does not include the global alias.</remarks>
        public static string GetTypeFullName(this Type type)
        {
            if (type is null)
                return string.Empty;

            string fullName = type.FullName;
            if (fullName is null)
                return string.Empty;

            return fullName;
        }
    }
}