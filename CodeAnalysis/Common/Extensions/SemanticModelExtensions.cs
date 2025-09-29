#pragma warning disable CS8603 // Possible null reference return.
using Microsoft.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalysis.Common.Extensions
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

        /// <summary>
        /// Returns IFieldSymbol from a FieldDelarationSynthax.
        /// </summary>
        public static IFieldSymbol GetFieldSymbol(this SemanticModel semanticModel, FieldDeclarationSyntax fieldDeclaration)
        {
            if (fieldDeclaration.Declaration.Variables.Count == 0)
                return null;

            VariableDeclaratorSyntax variableDeclaratorSyntax = fieldDeclaration.Declaration.Variables[0];
            ISymbol symbol = ModelExtensions.GetDeclaredSymbol(semanticModel, variableDeclaratorSyntax);

            if (symbol is not null && symbol is IFieldSymbol fieldSymbol)
                return fieldSymbol;

            return null;
        }

        /// <summary>
        /// Returns the SemanticModel for context if context is a supported type.
        /// </summary>
        public static SemanticModel GetSemanticModel(this object context)
        {
            if (context is GeneratorSyntaxContext gsc)
                return gsc.SemanticModel;
            else if (context is SyntaxNodeAnalysisContext snac)
                return snac.SemanticModel;
            else
                return null;
        }
    }
    #pragma warning restore CS8603 // Possible null reference return.
}