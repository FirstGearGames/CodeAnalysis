#pragma warning disable CS8602 // Dereference of a possibly null reference.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Common.Extensions
{
    public static class INamedTypeSymbolExtensions
    {
        /// <summary>
        /// Gets IFieldSymbols within a symbol.
        /// </summary>
        public static List<IFieldSymbol> GetFieldSymbols(this INamedTypeSymbol namedTypeSymbol, Accessibility? requiredAccessibility = null)
        {
            List<IFieldSymbol> validSymbols = new();
            
            foreach (ISymbol symbol in namedTypeSymbol.GetMembers())
            {
                if (symbol is IFieldSymbol methodSymbol) 
                {
                    if (requiredAccessibility is null || methodSymbol.DeclaredAccessibility == requiredAccessibility.Value)
                        validSymbols.Add(methodSymbol);
                }
            }

            return validSymbols;
        }

        /// <summary>
        /// Gets IMethodSymbols within an INamedTypeSymbols.
        /// </summary>
        public static List<IMethodSymbol> GetMethodSymbols(this INamedTypeSymbol namedTypeSymbol, Accessibility? requiredAccessibility = null)
        {
            List<IMethodSymbol> validSymbols = new();
            
            foreach (ISymbol symbol in namedTypeSymbol.GetMembers())
            {
                if (symbol is IMethodSymbol methodSymbol) 
                {
                    if (requiredAccessibility is null || methodSymbol.DeclaredAccessibility == requiredAccessibility.Value)
                        validSymbols.Add(methodSymbol);
                }
            }

            return validSymbols;
        }

        /// <summary>
        /// Returns the short name of a symbol which includes the namespace.
        /// </summary>
        public static bool ImplementsInterface(this INamedTypeSymbol symbol, string interfaceFullName)
        {
            foreach (INamedTypeSymbol interfaceNamed in symbol.Interfaces)
            {
                if (interfaceNamed.GetTypeSymbolFullName() == interfaceFullName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True if symbol inherits base class anywhere along hierarchy.
        /// </summary>
        public static bool InheritsClass(this INamedTypeSymbol symbol, string classFullName)
        {
            while (symbol.BaseType is { } baseSymbol)
            {
                if (baseSymbol.GetTypeSymbolFullName() == classFullName)
                    return true;

                symbol = symbol.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Returns a method containing matching parameter names.
        /// </summary>
        public static IMethodSymbol? GetMethod(this INamedTypeSymbol symbol, string methodName, params string[] parameterNames)
        {
            IEnumerable<IMethodSymbol> methodSymbols = symbol.GetMembers(methodName).OfType<IMethodSymbol>();

            foreach (IMethodSymbol methodSymbol in methodSymbols)
            {
                if (methodSymbol.AreParametersMatching(parameterNames))
                    return methodSymbol;
            }

            return null;
        }

        /// <summary>
        /// Returns if a type has public accessibility.
        /// </summary>
        public static bool HasPublicAccessibility(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.DeclaredAccessibility is Accessibility.Public;

        /// <summary>
        /// Returns if a type has public accessibility.
        /// </summary>
        public static bool HasPartialModifier(this INamedTypeSymbol namedTypeSymbol)
        {
            if (namedTypeSymbol is null)
                return false;

            ImmutableArray<SyntaxReference> syntaxReferences = namedTypeSymbol.DeclaringSyntaxReferences;

            // If there's more than one reference then we know it's partial
            if (syntaxReferences.Length > 1)
                return true;

            SyntaxReference? firstSyntaxReference = syntaxReferences.FirstOrDefault();
            if (firstSyntaxReference is null)
                return false;

            if (firstSyntaxReference.GetSyntax() is ClassDeclarationSyntax classDeclarationSyntax)
                return classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
            if (firstSyntaxReference.GetSyntax() is StructDeclarationSyntax structDeclarationSyntax)
                return structDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

            return false;
        }

        /// <summary>
        /// Returns a types header as a string. (Eg: public partial class MyClass).
        /// </summary>
        /// <param name = "classSymbol"></param>
        /// <returns></returns>
        public static string GetClassOrStructHeader(this INamedTypeSymbol namedTypeSymbol)
        {
            string keywordText;
            if (!IsAllowedKeyword(out keywordText))
                return string.Empty;

            // Public, internal, etc. 
            string modifiersText = namedTypeSymbol.DeclaredAccessibility.ToString().ToLower();
            // Partial check.
            string partialText = HasPartialModifier(namedTypeSymbol) ? "partial " : string.Empty;

            bool IsAllowedKeyword(out string lKeyword)
            {
                lKeyword = string.Empty;

                if (namedTypeSymbol.TypeKind == TypeKind.Class)
                    lKeyword = "class";
                else if (namedTypeSymbol.TypeKind == TypeKind.Struct)
                    lKeyword = "struct";
                else
                    return false;

                return true;
            }

            return $"{modifiersText} {partialText}{keywordText} {namedTypeSymbol.Name}";
        }
    }
    #pragma warning restore CS8602 // Dereference of a possibly null reference.
}