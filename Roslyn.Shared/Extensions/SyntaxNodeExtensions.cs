#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;

namespace Roslyn.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static bool TryGetParentSyntax<T>(this SyntaxNode syntaxNode, out T result) where T : SyntaxNode
        {
            // set defaults
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            result = null;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            if (syntaxNode == null)
            {
                return false;
            }

            try
            {
                syntaxNode = syntaxNode.Parent;

                if (syntaxNode == null)
                {
                    return false;
                }

                if (syntaxNode.GetType() == typeof(T))
                {
                    result = syntaxNode as T;
                    return true;
                }

                return TryGetParentSyntax(syntaxNode, out result);
            }
            catch
            {
                return false;
            }
        }
    }
}