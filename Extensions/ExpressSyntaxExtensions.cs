using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for resolving type symbols from expression syntax nodes.
/// </summary>
public static class ExpressSyntaxExtensions
{
    /// <summary>
    /// Returns the <see cref="ITypeSymbol"/> referenced by a <see cref="TypeOfExpressionSyntax"/>.
    /// </summary>
    /// <param name="syntax">Type-of expression to resolve.</param>
    /// <param name="semanticModel">Semantic model used to resolve symbols.</param>
    /// <returns>The referenced type symbol, or null when it cannot be resolved.</returns>
    public static ITypeSymbol? GetTypeIdentifier(this TypeOfExpressionSyntax syntax, SemanticModel semanticModel) => semanticModel?.GetSymbolInfo(syntax.Type).Symbol as ITypeSymbol;

    /// <summary>
    /// Returns the <see cref="ITypeSymbol"/> for an expression syntax using <see cref="SemanticModel.GetTypeInfo(SyntaxNode, System.Threading.CancellationToken)"/>.
    /// </summary>
    /// <param name="syntax">Expression syntax to resolve.</param>
    /// <param name="semanticModel">Semantic model used to resolve types.</param>
    /// <returns>The resolved type symbol, or null when it cannot be resolved.</returns>
    public static ITypeSymbol? GetTypeInfoTypeSymbol(this ExpressionSyntax syntax, SemanticModel semanticModel) => semanticModel?.GetTypeInfo(syntax).Type;
}