#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.CodeBuilding;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers;

namespace FirstGearGames.CodeAnalysis.Extensions
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
        /// <param name="metadataName">True to return name as metadata.</param>
        public static string GetSymbolFullName(this ISymbol? symbol, bool metadataName)
        {
            if (symbol == null) return string.Empty;
            if (symbol is ITypeSymbol typeSymbol) return typeSymbol.GetTypeSymbolFullName(metadataName);

            string containingNamespace = symbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            string joiningChar = (metadataName) ? "+" : ".";
            for (INamedTypeSymbol? currentType = symbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}{joiningChar}{fullyQualifiedName}";

            fullyQualifiedName = $"{fullyQualifiedName}{symbol.Name}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
        }


        /// <summary>
        /// Returns if a symbol has an attribute, and outputs it if so.
        /// </summary>
        /// <param name="isMetadataName">True if the attributeFullName is a metadataName.</param>
        public static bool HasAttribute(this ISymbol symbol, string attributeFullName, bool isMetadataName, out AttributeData data)
        {
            foreach (AttributeData item in symbol.GetAttributes())
            {
                INamedTypeSymbol? typeSymbol = item.AttributeClass;
                if (typeSymbol == null)
                    continue;

                if (typeSymbol.GetSymbolFullName(isMetadataName) == attributeFullName)
                {
                    data = item;
                    return true;
                }
            }

            //Fall through, not found.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            data = default;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            return false;
        }

        /// <summary>
        /// Returns if a symbol has any of the supplied attributes, and outputs it if so.
        /// </summary>
        public static bool HasAttributes(this ISymbol symbol, string[] attributeFullNames, bool isMetadataName, out List<AttributeData> datas)
        {
            datas = new List<AttributeData>();

            foreach (string fullName in attributeFullNames)
            {
                if (symbol.HasAttribute(fullName, isMetadataName, out AttributeData? d))
                    datas.Add(d!);
            }

            return datas.Count > 0;
        }

        public static bool HasAttribute(this ISymbol thisSymbol, string attributeFullName, bool isMetadataName)
        {
            foreach (AttributeData attribute in thisSymbol.GetAttributes())
            {
                if (attribute.AttributeClass is not INamedTypeSymbol namedTypeSymbol) continue;

                string symbolFullName = namedTypeSymbol.GetSymbolFullName(isMetadataName);
                if (symbolFullName == attributeFullName) return true;
            }

            return false;
        }

        public static bool HasAttributes(this ISymbol thisSymbol, bool isMetadataNames, params string[] attributeFullNames)
        {
            foreach (string fullyQualifiedAttributeName in attributeFullNames)
                if (thisSymbol.HasAttribute(fullyQualifiedAttributeName, isMetadataNames))
                    return true;

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
