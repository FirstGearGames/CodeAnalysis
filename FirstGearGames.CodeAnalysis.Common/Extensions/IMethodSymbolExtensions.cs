using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.CodeAnalysis.Extensions
{
    public static class IMethodSymbolExtensions
    {
        private static List<string> _strings = new();

        /// <summary>
        /// Returns true if method parameters match expected parameters.
        /// </summary>
        /// <returns></returns>
        public static bool AreParametersMatching(this IMethodSymbol methodSymbol, bool metadataName, params string[] expectedParameterNames)
        {
            _strings.Clear();

            foreach (IParameterSymbol parameterSymbol in methodSymbol.Parameters)
            {
                string parameterName = parameterSymbol.GetSymbolFullName(metadataName);
                _strings.Add(parameterName);
            }

            //Lengths do not match.
            if (expectedParameterNames.Length != _strings.Count)
                return false;

            //Compare each entry.
            for (int i = 0; i < _strings.Count; i++)
            {
                if (!string.Equals(_strings[i], expectedParameterNames[i], StringComparison.Ordinal))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Gets all types returned within the body of a method.
        /// </summary>
        public static List<ITypeSymbol> GetReturnedTypeSymbols(this IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            List<ITypeSymbol> results = new();

            List<ExpressionSyntax> expressionSyntaxes = GetReturnedExpressionSyntaxes(methodSymbol);
            foreach (ExpressionSyntax expressionSyntax in expressionSyntaxes)
            {
                if (expressionSyntax.GeTypeInfoTypeSymbol(semanticModel) is { } typeSymbol)
                    results.Add(typeSymbol);
            }

            return results;
        }

        /// <summary>
        /// Gets all ExpressionSyntax returned within the body of a method.
        /// </summary>
        public static List<ExpressionSyntax> GetReturnedExpressionSyntaxes(this IMethodSymbol methodSymbol)
        {
            List<ExpressionSyntax> results = new();

            MethodDeclarationSyntax? methodSyntax = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

            if (methodSyntax == null) return results;

            //Uses expression body, such as MethodName() => something;
            if (methodSyntax.ExpressionBody != null)
            {
                //Get the return of the arrowExpression, which is seen as => in code.
                ExpressionSyntax expressionSyntax = methodSyntax.ExpressionBody.Expression;
                results.Add(expressionSyntax);
            }
            /* If method uses return within the body.
             * EG: MethodName() { return something; } */
            else
            {
                if (methodSyntax.Body == null) return results;

                //Find all return statements.
                IEnumerable<ReturnStatementSyntax> returnStatements = methodSyntax.Body.DescendantNodes().OfType<ReturnStatementSyntax>();

                foreach (ReturnStatementSyntax? returnStatement in returnStatements)
                {
                    ExpressionSyntax? expression = returnStatement.Expression;
                    if (expression != null)
                        results.Add(expression);
                }
            }
            return results;
        }

    }
}
