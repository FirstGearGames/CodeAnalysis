#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.Roslyn.CodeBuilding;

namespace FirstGearGames.Roslyn.Extensions
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
        /// Returns the full name of a symbol which includes the namespace.
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        public static string GetSymbolFullNameWithGenerics(this ISymbol symbol, bool metadataName)
        {
            string fullName = symbol.GetSymbolFullName(metadataName);
            string genericArguments = symbol.GetGenericArgumentsString().GetCombinedGenericArguments();

            return $"{fullName}{genericArguments}";
        }
        
        
        /// <summary>
        /// Returns generic arguments as ITypeSymbols.
        /// </summary>
        public static List<ITypeSymbol> GetGenericArgumentsTypeSymbol(this ISymbol symbol)
        {
            List<ITypeSymbol> results = new();

            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
                results.AddRange(namedTypeSymbol.TypeArguments.ToList());

            return results;
        }


        /// <summary>
        /// Returns generic arguments count.
        /// </summary>
        public static int GetGenericArgumentsCount(this ISymbol symbol)
        {
            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
                return namedTypeSymbol.TypeArguments.Length;

            return 0;
        }

        
        /// <summary>
        /// Returns generic arguments as fullName strings.
        /// </summary>
        public static List<string> GetGenericArgumentsString(this ISymbol symbol)
        {
            List<string> results = new();

            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
            {
                foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                {
                    if (typeArgument.TypeKind is TypeKind.TypeParameter)
                        results.Add(typeArgument.Name);
                    else
                        results.Add(typeArgument.GetTypeSymbolFullNameWithGenericArguments(metadataName: false));
                }
            }

            return results;
        }

        /// <summary>
        /// Adds generic arguments onto a string in the fashion of <type, type2, type3>.
        /// </summary>
        public static string AddGenericArguments(this string str, ISymbol symbol)
        {
            List<string> results = symbol.GetGenericArgumentsString();

            return $"{str}{results.GetCombinedGenericArguments()}";
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

    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}