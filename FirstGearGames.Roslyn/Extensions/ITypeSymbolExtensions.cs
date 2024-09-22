#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
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
        /// Adds generic arguments onto a string in the fashion of <type, type2, type3>.
        /// </summary>
        public static string AddGenericArguments(this string str, ISymbol symbol)
        {
            //If named then check for generic arguments.
            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
            {
                string generics = string.Empty;

                foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                {
                    //To separate each type.
                    if (generics.Length > 0)
                        generics += ", ";

                    if (typeArgument.TypeKind is TypeKind.TypeParameter)
                        generics += typeArgument.Name;
                    else
                        generics += typeArgument.GetTypeFullName();
                }

                //If any were added then add onto type symbol name.
                if (generics.Length > 0)
                    str += $"<{generics}>";
            }

            return str;
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