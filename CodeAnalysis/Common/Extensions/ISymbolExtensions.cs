#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace CodeAnalysis.Common.Extensions
{
    public static class ISymbolExtensions
    {
        /// <summary>
        /// Returns the full name of a symbol which includes the namespace.
        /// </summary>
        public static string GetNamespace(this ISymbol symbol)
        {
            return symbol?.ContainingNamespace?.Name ?? string.Empty;
        }

        /// <summary>
        /// Returns the full name of a symbol which includes the namespace.
        /// </summary>
        /// <param name = "metadataName">True to return name as metadata.</param>
        public static string GetSymbolFullName(this ISymbol symbol, bool metadataName)
        {
            if (symbol is null)
                return string.Empty;
            
            if (symbol is ITypeSymbol typeSymbol)
                return typeSymbol.GetTypeSymbolFullName(metadataName);

            string containingNamespace = symbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            string joiningChar = metadataName ? "+" : ".";
            for (INamedTypeSymbol currentType = symbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}{joiningChar}{fullyQualifiedName}";

            fullyQualifiedName = $"{fullyQualifiedName}{symbol.Name}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
        }

        /// <summary>
        /// Returns if a symbol has an attribute, and outputs it if so.
        /// </summary>
        /// <param name = "isMetadataName">True if the attributeFullName is a metadataName.</param>
        public static bool HasAttribute(this ISymbol symbol, string attributeFullName, bool isMetadataName, out AttributeData data)
        {
            data = null;
            
            if (symbol is null)
                return false;
            
            foreach (AttributeData item in symbol.GetAttributes())
            {
                INamedTypeSymbol typeSymbol = item.AttributeClass;
                if (typeSymbol is null)
                    continue;

                if (typeSymbol.GetSymbolFullName(isMetadataName) == attributeFullName)
                {
                    data = item;
                    return true;
                }
            }
            
            return false;
        }

        /// <summary>
        /// Returns if a symbol has any of the supplied attributes, and outputs it if so.
        /// </summary>
        public static bool HasAttributes(this ISymbol symbol, string[] attributeFullNames, bool isMetadataName, out List<AttributeData> datas)
        {
            datas = new();

            if (symbol is null)
                return false;

            foreach (string fullName in attributeFullNames)
            {
                if (symbol.HasAttribute(fullName, isMetadataName, out AttributeData d))
                    datas.Add(d);
            }

            return datas.Count > 0;
        }

        public static bool HasAttribute(this ISymbol symbol, string attributeFullName, bool isMetadataName)
        {
            if (symbol is null)
                return false;
            
            foreach (AttributeData attribute in symbol.GetAttributes())
            {
                if (attribute.AttributeClass is not { } namedTypeSymbol)
                    continue;

                string symbolFullName = namedTypeSymbol.GetSymbolFullName(isMetadataName);
                if (symbolFullName == attributeFullName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True if symbol inherits base class anywhere along hierarchy.
        /// </summary>
        public static bool InheritsClass(this ISymbol symbol, string classFullName)
        {
            if (symbol is INamedTypeSymbol namedTypeSymbol)
                return namedTypeSymbol.InheritsClass(classFullName);

            return false;
        }
    }
    #pragma warning restore CS8602 // Dereference of a possibly null reference.
}