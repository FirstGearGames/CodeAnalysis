#pragma warning disable CS8603 // Possible null reference return.
#nullable enable
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for resolving symbols from a <see cref="SemanticModel"/>.
/// </summary>
public static class SemanticModelExtensions
{
    /// <summary>
    /// Returns the symbol bound to the supplied syntax node, or null when none can be resolved.
    /// </summary>
    /// <param name="semanticModel">Semantic model used to resolve the symbol.</param>
    /// <param name="node">Syntax node whose symbol is being resolved.</param>
    /// <returns>The bound symbol, or null when no symbol could be resolved.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ISymbol GetSymbol(this SemanticModel semanticModel, SyntaxNode node)
    {
        return semanticModel.GetSymbolInfo(node).Symbol;
    }

    /// <summary>
    /// Returns the type symbol bound to the supplied syntax node, or null when none can be resolved.
    /// </summary>
    /// <param name="semanticModel">Semantic model used to resolve the type.</param>
    /// <param name="node">Syntax node whose type is being resolved.</param>
    /// <returns>The bound type symbol, or null when no type could be resolved.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ITypeSymbol GetTypeSymbol(this SemanticModel semanticModel, SyntaxNode node)
    {
        return semanticModel.GetTypeInfo(node).Type;
    }

    /// <summary>
    /// Returns the <see cref="IFieldSymbol"/> for the first variable declared in the supplied <see cref="FieldDeclarationSyntax"/>.
    /// </summary>
    /// <param name="semanticModel">Semantic model used to resolve the symbol.</param>
    /// <param name="fieldDeclaration">Field declaration whose symbol is being resolved.</param>
    /// <returns>The resolved field symbol, or null when one cannot be resolved.</returns>
    public static IFieldSymbol GetFieldSymbol(this SemanticModel semanticModel, FieldDeclarationSyntax fieldDeclaration)
    {
        if (fieldDeclaration.Declaration.Variables.Count == 0)
            return null;

        VariableDeclaratorSyntax variableDeclaratorSyntax = fieldDeclaration.Declaration.Variables[0];
        ISymbol? symbol = ModelExtensions.GetDeclaredSymbol(semanticModel, variableDeclaratorSyntax);

        if (symbol is IFieldSymbol fieldSymbol)
            return fieldSymbol;

        return null;
    }
        
}
#pragma warning restore CS8603 // Possible null reference return.