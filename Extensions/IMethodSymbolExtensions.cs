using System;
using System.Collections.Generic;
using System.Linq;
using CodeAnalysis.Finding;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for inspecting <see cref="IMethodSymbol"/> instances.
/// </summary>
public static class MethodSymbolExtensions
{
    /// <summary>
    /// Returns whether the parameters of the method match the supplied expected parameter type names.
    /// </summary>
    /// <param name="methodSymbol">Method whose parameters are being compared.</param>
    /// <param name="expectedParameterNames">Expected parameter type names, in order.</param>
    /// <returns>True when each parameter type name matches the expected sequence in order.</returns>
    public static bool AreParametersMatching(this IMethodSymbol methodSymbol, params string[] expectedParameterNames)
    {
        List<string> parameters = new();

        foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
        {
            string parameterName = parameterSymbol.GetSymbolFullName();
            parameters.Add(parameterName);
        }

        // Lengths do not match.
        if (expectedParameterNames.Length != parameters.Count)
            return false;

        // Compare each entry.
        for (int i = 0; i < parameters.Count; i++)
        {
            if (!string.Equals(parameters[i], expectedParameterNames[i], StringComparison.Ordinal))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns every type returned by an expression within the body of the method.
    /// </summary>
    /// <param name="methodSymbol">Method whose returned expressions should be inspected.</param>
    /// <param name="semanticModel">Semantic model used to resolve types.</param>
    /// <returns>A list containing the type symbol of every returned expression.</returns>
    public static List<ITypeSymbol> GetReturnedTypeSymbols(this IMethodSymbol methodSymbol, SemanticModel semanticModel)
    {
        List<ITypeSymbol> results = new();

        List<ExpressionSyntax> expressionSyntaxes = GetReturnedExpressionSyntaxes(methodSymbol);
        foreach (ExpressionSyntax expressionSyntax in expressionSyntaxes)
        {
            if (expressionSyntax.GetTypeInfoTypeSymbol(semanticModel) is { } typeSymbol)
                results.Add(typeSymbol);
        }

        return results;
    }

    /// <summary>
    /// Returns the type arguments declared by the method, formatted according to the requested search behavior.
    /// </summary>
    /// <param name="thisValue">Method whose type arguments are being read.</param>
    /// <param name="argumentSearchType">Search behavior to apply when resolving argument names.</param>
    /// <param name="argumentSearchResult">Receives a status describing whether arguments were found, missing, or could not be resolved.</param>
    /// <returns>A list of arguments resolved from the method's type arguments.</returns>
    public static List<Argument> GetMethodSymbolArguments(this IMethodSymbol thisValue, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
    {
        List<Argument> arguments = [];

        if (thisValue is null)
        {
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
            return arguments;
        }

        if (argumentSearchType is ArgumentSearchType.ExplicitlyNamed && !thisValue.ArePresentArgumentsNamed())
        {
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
            return arguments;
        }

        if (thisValue.TypeArguments.Length == 0)
        {
            argumentSearchResult = ArgumentSearchResult.NoArguments;
            return arguments;
        }

        argumentSearchResult = ArgumentSearchResult.HasArguments;

        bool useNamed = argumentSearchType is ArgumentSearchType.ExplicitlyNamed || argumentSearchType is ArgumentSearchType.PreferNamed;
        int iteration = 0;

        foreach (ITypeSymbol typeSymbol in thisValue.TypeArguments)
        {
            if (useNamed && typeSymbol is INamedTypeSymbol namedTypeSymbol)
                arguments.Add(new(namedTypeSymbol, namedTypeSymbol.GetTypeSymbolFullNameWithArguments(argumentSearchType, out argumentSearchResult), isNamed: true));
            else
                arguments.Add(new(typeSymbol, $"T{iteration}", isNamed: false));

            iteration++;

            if (argumentSearchResult.HasError())
                return arguments;
        }

        argumentSearchResult = ArgumentSearchResult.HasArguments;
        return arguments;
    }

    /// <summary>
    /// Returns whether every type argument supplied to the method is a concrete named type rather than a type parameter.
    /// </summary>
    /// <param name="thisValue">Method whose type arguments are being inspected.</param>
    /// <returns>True when every type argument resolves to a concrete named type.</returns>
    public static bool ArePresentArgumentsNamed(this IMethodSymbol thisValue)
    {
        foreach (ITypeSymbol typeSymbol in thisValue.TypeArguments)
        {
            if (typeSymbol.TypeKind is TypeKind.TypeParameter)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Returns every <see cref="ExpressionSyntax"/> returned from the body of the method.
    /// </summary>
    /// <param name="methodSymbol">Method whose returned expressions should be inspected.</param>
    /// <returns>A list containing every returned expression syntax found in the method body.</returns>
    public static List<ExpressionSyntax> GetReturnedExpressionSyntaxes(this IMethodSymbol methodSymbol)
    {
        List<ExpressionSyntax> results = new();

        MethodDeclarationSyntax? methodSyntax = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

        if (methodSyntax is null)
            return results;

        // Uses expression body, such as MethodName() => something;
        if (methodSyntax.ExpressionBody is not null)
        {
            // Get the return of the arrowExpression, which is seen as => in code.
            ExpressionSyntax expressionSyntax = methodSyntax.ExpressionBody.Expression;
            results.Add(expressionSyntax);
        }
        /* If method uses return within the body.
         * EG: MethodName() { return something; } */
        else
        {
            if (methodSyntax.Body is null)
                return results;

            // Find all return statements.
            IEnumerable<ReturnStatementSyntax> returnStatements = methodSyntax.Body.DescendantNodes().OfType<ReturnStatementSyntax>();

            foreach (ReturnStatementSyntax returnStatement in returnStatements)
            {
                ExpressionSyntax? expression = returnStatement.Expression;
                if (expression is not null)
                    results.Add(expression);
            }
        }
        return results;
    }

    /// <summary>
    /// Tries to collect every <see cref="ISymbol"/> referenced from within the body of the method.
    /// </summary>
    /// <remarks>
    /// True may be returned even when no symbols were found.
    /// </remarks>
    /// <param name="methodSymbol">Method whose referenced symbols should be collected.</param>
    /// <param name="semanticModel">Semantic model used to resolve identifiers.</param>
    /// <param name="referencedSymbols">Receives the set of symbols referenced by the method, or null when the operation could not proceed.</param>
    /// <returns>True when the operation completed without error.</returns>
    public static bool TryGetReferencedSymbols(this IMethodSymbol methodSymbol, SemanticModel semanticModel, out HashSet<ISymbol>? referencedSymbols)
    {
        if (semanticModel is null)
        {
            referencedSymbols = null;
            return false;
        }

        referencedSymbols = [];

        foreach (SyntaxReference syntaxReference in methodSymbol.DeclaringSyntaxReferences)
        {
            SyntaxNode methodSyntaxNode = syntaxReference.GetSyntax();
            IEnumerable<IdentifierNameSyntax> identifierNameSyntaxes = methodSyntaxNode.DescendantNodes().OfType<IdentifierNameSyntax>();

            /* There is a chance the SyntaxTree is in a different file, such as a partial file.
             * When this is the case fetch the correct tree. */
            SemanticModel treeSemanticModel = semanticModel.SyntaxTree == syntaxReference.SyntaxTree ? semanticModel : semanticModel.Compilation.GetSemanticModel(syntaxReference.SyntaxTree);

            foreach (IdentifierNameSyntax identifier in identifierNameSyntaxes)
            {
                SymbolInfo symbolInfo = treeSemanticModel.GetSymbolInfo(identifier);
                ISymbol? symbol = symbolInfo.Symbol;

                if (symbol is not null)
                    referencedSymbols.Add(symbol);
            }
        }

        return true;
    }
}