using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Linq;
using FirstGearGames.Roslyn.FishNet.Helpers;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.Roslyn.Extensions
{
    public static class ExpressSyntaxExtensions
    {
        /// <summary>
        /// Returns the type symbol for a TypeOfExpressionSyntax.
        /// </summary>
        public static ITypeSymbol? GetTypeOfInner(this TypeOfExpressionSyntax syntax, SemanticModel semanticModel)
        {
            SymbolInfo symbolInfo = semanticModel.GetSymbolInfo(syntax.Type);
            if (symbolInfo.Symbol is not ITypeSymbol typeSymbol) return null;

            return typeSymbol;
        }
    }
}