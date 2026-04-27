
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for resolving symbols and ancestors from <see cref="SyntaxNode"/> instances.
/// </summary>
public static class SyntaxNodeExtensions
{
    /// <summary>
    /// Returns the declared symbol for the supplied syntax node.
    /// </summary>
    /// <param name="node">Syntax node whose declared symbol is being resolved.</param>
    /// <param name="compilation">Compilation used to acquire a semantic model.</param>
    /// <returns>The declared symbol, or null when one cannot be resolved.</returns>
    public static ISymbol? GetDeclaredSymbol(this SyntaxNode node, Compilation compilation)
    {
        SemanticModel model = compilation.GetSemanticModel(node.SyntaxTree);
        return model?.GetDeclaredSymbol(node);
    }

    /// <summary>
    /// Tries to walk the supplied syntax tree upwards until a parent node of the requested type is found.
    /// </summary>
    /// <typeparam name="T0">Type of parent syntax node to find.</typeparam>
    /// <param name="syntaxNode">Syntax node from which to begin walking.</param>
    /// <param name="result">Receives the matching parent, or null when one cannot be found.</param>
    /// <returns>True when a matching parent was found.</returns>
    public static bool TryGetParentSyntax<T0>(this SyntaxNode syntaxNode, out T0? result) where T0 : SyntaxNode
    {
        // set defaults
        #pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        result = null;
        #pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

        if (syntaxNode is null)
        {
            return false;
        }

        try
        {
            syntaxNode = syntaxNode.Parent;

            if (syntaxNode is null)
            {
                return false;
            }

            if (syntaxNode.GetType() == typeof(T0))
            {
                result = syntaxNode as T0;
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