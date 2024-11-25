#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.Native.Constants;

namespace FirstGearGames.Roslyn.Extensions
{
    public enum GenericArgumentType
    {
        /// <summary>
        /// Will return arguments as named when possible (bool, string).
        /// </summary>
        PreferNamed,
        /// <summary>
        /// Returns arguments as generic (T0, T1).
        /// </summary>
        ForceGeneric,
    }

    public static class ITypeSymbolExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullName(this ITypeSymbol typeSymbol, bool metadataName)
        {
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
                typeSymbol = arrayTypeSymbol.ElementType;

            //If generic then just return our consts for generic.
            if (typeSymbol.TypeKind is TypeKind.TypeParameter)
                return NativeConstants.GeneralParameter_Name;

            string containingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            string joiningChar = (metadataName) ? "+" : ".";
            for (INamedTypeSymbol? currentType = typeSymbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}{joiningChar}{fullyQualifiedName}";

            //fullyQualifiedName = $"{fullyQualifiedName}{typeSymbol.Name}{arraySuffix}";
            fullyQualifiedName = $"{fullyQualifiedName}{typeSymbol.Name}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
        }

        /// <summary>
        /// Returns a full name with named arguments (System.Collection.Generic.List<System.String>).
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullNameWithNamedArguments(this ITypeSymbol typeSymbol, bool metadataName) => typeSymbol.GetTypeSymbolFullNameWithArguments(metadataName, GenericArgumentType.PreferNamed);

        /// <summary>
        /// Returns a full name with generic arguments (System.Collection.Generic.List<T0>).
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullNameWithGenericArguments(this ITypeSymbol typeSymbol, bool metadataName) => typeSymbol.GetTypeSymbolFullNameWithArguments(metadataName, GenericArgumentType.ForceGeneric);

        /// <summary>
        /// Returns a full name with arguments.
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullNameWithArguments(this ITypeSymbol typeSymbol, bool metadataName, GenericArgumentType argumentType)
        {
            string fullName = typeSymbol.GetTypeSymbolFullName(metadataName);

            string arguments;
            if (typeSymbol is IArrayTypeSymbol)
                arguments = "[]";
            else
                arguments = typeSymbol.GetGenericArgumentsString(argumentType).GetCombinedGenericArguments(typeSymbol);

            return $"{fullName}{arguments}";
        }

        /// <summary>
        /// Returns if all generic arguments are named (bool, string).
        /// If there are no generic arguments, returns true.
        /// </summary>
        public static bool AreGenericArgumentsNamed(this ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
            {
                foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                {
                    if (typeArgument.TypeKind is TypeKind.TypeParameter)
                        return false;
                }
            }
            else if (symbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                return (arrayTypeSymbol.ElementType.TypeKind is not TypeKind.TypeParameter);
            }

            return true;
        }

        public static List<string> GetGenericArgumentsString(this ITypeSymbol symbol, GenericArgumentType argumentType)
        {
            bool log = (symbol.Name == "Byte");

            List<string> results = new();
            int typeParameterCount = 0;

            bool forceGeneric = (argumentType == GenericArgumentType.ForceGeneric);

            if (log)
                Log(symbol.Name);

            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
            {
                if (log)
                    Log("A");
                foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                {
                    if (forceGeneric || typeArgument.TypeKind is TypeKind.TypeParameter)
                        results.Add($"T{typeParameterCount++}");
                    else
                        results.Add(typeArgument.GetTypeSymbolFullNameWithNamedArguments(metadataName: false));
                }
            }
            else if (symbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                if (log)
                    Log("B");
                if (forceGeneric || arrayTypeSymbol.ElementType.TypeKind is TypeKind.TypeParameter)
                    results.Add($"T{typeParameterCount++}");
                else
                    results.Add(arrayTypeSymbol.ElementType.GetTypeSymbolFullNameWithNamedArguments(metadataName: false));
            }
            else
            {
                if (log)
                    Log("C");
            }

            if (log)
                Log($"results count {results.Count}");
            return results;
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

        private static void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [ITypeSymbolExtensions] {txt}");
        }
    }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
}