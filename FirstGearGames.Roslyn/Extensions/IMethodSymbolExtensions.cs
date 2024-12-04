using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
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
        public static List<ITypeSymbol> GetBodyReturnTypes(this IMethodSymbol methodSymbol, SemanticModel semanticModel)
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
    }
}