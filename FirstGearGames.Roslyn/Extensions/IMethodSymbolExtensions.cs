using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
using FirstGearGames.Roslyn.FishNet.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.Roslyn.Extensions
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

            MethodDeclarationSyntax? methodSyntax = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

            if (methodSyntax == null) return results;
            if (methodSyntax.Body == null) return results;
            
            //Find all return statements.
            IEnumerable<ReturnStatementSyntax> returnStatements = methodSyntax.Body.DescendantNodes().OfType<ReturnStatementSyntax>();

            foreach (ReturnStatementSyntax? returnStatement in returnStatements)
            {
                ExpressionSyntax? expression = returnStatement.Expression;
                //Make sure typeInfo can be found of return statement. If so, add to results.
                if (expression != null)
                {
                    // Get the type of the return expression
                    TypeInfo typeInfo = semanticModel.GetTypeInfo(expression);
                    if (typeInfo.Type != null)
                        results.Add(typeInfo.Type);
                }
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
            if (methodSyntax.Body == null) return results;
            
            //Find all return statements.
            IEnumerable<ReturnStatementSyntax> returnStatements = methodSyntax.Body.DescendantNodes().OfType<ReturnStatementSyntax>();

            foreach (ReturnStatementSyntax? returnStatement in returnStatements)
            {
                ExpressionSyntax? expression = returnStatement.Expression;
                if (expression != null)
                    results.Add(expression);
            }

            return results;
        }
        
        finish this to test returns on method() => returnType. 
        public static ITypeSymbol GetReturnTypeFromExpressionBody(IMethodSymbol methodSymbol, SemanticModel semanticModel)
        {
            // Check if the method is expression-bodied (i.e., uses =>)
            var methodSyntax = methodSymbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MethodDeclarationSyntax;

            if (methodSyntax == null || methodSyntax.ExpressionBody == null)
                return null; // If it's not an expression-bodied method, return null.

            // Get the arrow expression clause (i.e., the => part)
            var arrowExpression = methodSyntax.ExpressionBody;

            // The expression part of the expression-bodied method
            var returnExpression = arrowExpression.Expression;

            // Get type information from the expression
            var typeInfo = semanticModel.GetTypeInfo(returnExpression);

            // Return the type symbol of the expression
            return typeInfo.Type;
        }
        
    }
}