#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.Native.Constants;

namespace FirstGearGames.Roslyn.Extensions
{
    public static class ITypeSymbolExtensions
    {
        /// <summary>
        /// Returns if a symbol is not a named symbol.
        /// </summary>
        /// <param name="checkElementType">True to check element type if an array.</param>
        public static bool IsNamedTypeSymbol(this ITypeSymbol typeSymbol, bool checkElementType)
        {
            bool result;
            if (checkElementType && typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
                result = (arrayTypeSymbol.ElementType is INamedTypeSymbol);
            else
                result = (typeSymbol is INamedTypeSymbol);

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullName(this ITypeSymbol typeSymbol, bool metadataName)
        {
            bool isGenericSymbol = !typeSymbol.IsNamedTypeSymbol(checkElementType: true);

            //Overwrite symbol is array and set array suffix.
            string arraySuffix = string.Empty;
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                typeSymbol = arrayTypeSymbol.ElementType;
                arraySuffix = "[]";
            }

            if (isGenericSymbol)
                return $"{NativeConstants.GeneralParameter_Name}{arraySuffix}";

            string containingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();


            string fullyQualifiedName = string.Empty;
            string joiningChar = (metadataName) ? "+" : ".";
            for (INamedTypeSymbol? currentType = typeSymbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}{joiningChar}{fullyQualifiedName}";

            fullyQualifiedName = $"{fullyQualifiedName}{typeSymbol.Name}{arraySuffix}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="metadataName">True to return name as metadata.</param>
            /// <returns></returns>
            public static string GetTypeSymbolFullNameWithGenericArguments(this ITypeSymbol typeSymbol, bool metadataName)
            {
                string fullName = typeSymbol.GetTypeSymbolFullName(metadataName);
                string genericArguments = typeSymbol.GetGenericArgumentsString().GetCombinedGenericArguments();

                return $"{fullName}{genericArguments}";
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