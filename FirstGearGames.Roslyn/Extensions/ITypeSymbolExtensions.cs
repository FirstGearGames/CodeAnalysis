#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Helpers;

namespace FirstGearGames.Roslyn.Extensions
{
    public static class ITypeSymbolExtensions
    {
        public static string GetTypeFullName(this ITypeSymbol typeSymbol)
        {
            string containingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            for (INamedTypeSymbol? currentType = typeSymbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}.{fullyQualifiedName}";

            string typeSymbolName = typeSymbol.Name.AddGenericArguments(typeSymbol);
            fullyQualifiedName = $"{fullyQualifiedName}{typeSymbolName}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
        }

        public static string GetTypeMetadataName(this ITypeSymbol typeSymbol)
        {
            string containingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            for (INamedTypeSymbol? currentType = typeSymbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}.{fullyQualifiedName}";

            string typeSymbolName = typeSymbol.Name.AddGenericArguments(typeSymbol);
            fullyQualifiedName = $"{fullyQualifiedName}{typeSymbolName}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
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
                        results.Add(typeArgument.GetTypeFullName());
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

            return $"{str}{results.CombineGenericArguments()}";
        }

        public static bool IsUserDefinedStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol is { TypeKind: TypeKind.Struct, SpecialType: SpecialType.None };
        }

        public static bool IsUserDefinedClass(this ITypeSymbol typeSymbol)
        {
            return typeSymbol is { TypeKind: TypeKind.Class, SpecialType: SpecialType.None };
        }

        public static bool IsUserDefinedClassOrStruct(this ITypeSymbol typeSymbol)
        {
            return typeSymbol.IsUserDefinedClass() || typeSymbol.IsUserDefinedStruct();
        }

        public static IEnumerable<ITypeSymbol> EnumerateTypeHierarchy(this ITypeSymbol thisTypeSymbol)
        {
            for (ITypeSymbol typeSymbol = thisTypeSymbol; typeSymbol != null; typeSymbol = typeSymbol.BaseType) yield return typeSymbol;
        }

        public static bool IsSubtypeOf(this ITypeSymbol thisTypeSymbol, string fullyQualifiedTypeName)
        {
            foreach (ITypeSymbol typeSymbol in thisTypeSymbol.EnumerateTypeHierarchy())
            {
                if (typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedTypeName) return true;
            }

            return false;
        }

        public static bool IsSubtypeOf(this ITypeSymbol thisTypeSymbol, ImmutableHashSet<string> fullyQualifiedTypeNames, out ITypeSymbol? result)
        {
            result = null;

            foreach (ITypeSymbol typeSymbol in thisTypeSymbol.EnumerateTypeHierarchy())
            {
                if (!fullyQualifiedTypeNames.Contains(typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))) continue;

                result = typeSymbol;

                return true;
            }

            return false;
        }
    }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
}