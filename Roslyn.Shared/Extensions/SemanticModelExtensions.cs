#pragma warning disable CS8603 // Possible null reference return.

using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Roslyn.Extensions
{
    public static class SemanticModelExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ISymbol GetSymbol(this SemanticModel semanticModel, SyntaxNode node)
        {
            return semanticModel.GetSymbolInfo(node).Symbol;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ITypeSymbol GetTypeSymbol(this SemanticModel semanticModel, SyntaxNode node)
        {
            return semanticModel.GetTypeInfo(node).Type;
        }
    }
#pragma warning restore CS8603 // Possible null reference return.
}