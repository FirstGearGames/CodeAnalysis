#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using FirstGearGames.CodeAnalysis.CodeBuilding;
using FirstGearGames.CodeAnalysis.Constants;
using FirstGearGames.CodeAnalysis.Helpers;

namespace FirstGearGames.CodeAnalysis.Extensions
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
        private static StringBuilder _stringBuilder = new();

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
                return NativeConstants.FirstGenericParameter_Name;

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
        /// Returns full name with generic arguments as named types (System.Collections.Generic.List<System.String>).
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullNameWithNamedArguments(this ITypeSymbol typeSymbol, bool metadataName) => typeSymbol.GetTypeSymbolFullNameWithArguments(metadataName, GenericArgumentType.PreferNamed);

        /// <summary>
        /// Returns full name with generic arguments as generic types (System.Collections.Generic.List<T0>).
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullNameWithGenericArguments(this ITypeSymbol typeSymbol, bool metadataName) => typeSymbol.GetTypeSymbolFullNameWithArguments(metadataName, GenericArgumentType.ForceGeneric);

        /// <summary>
        /// Returns full name with generic arguments as named types (System.Collections.Generic.List<System.String>).
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolFullNameWithArguments(this ITypeSymbol typeSymbol, bool metadataName, GenericArgumentType argumentType)
        {
            string fullName = typeSymbol.GetTypeSymbolFullName(metadataName);

            string genericArguments = (typeSymbol is IArrayTypeSymbol) ? "[]" : typeSymbol.GetGenericArgumentsString(argumentType).GetCombinedGenericArguments(typeSymbol);

            return $"{fullName}{genericArguments}";
        }



        /// <summary>
        /// Returns type as a generic array, if an array (T0[], T0[][]). If not an array empty is returned.
        /// </summary>
        /// <param name="metadataName">True to return name as metadata.</param>
        /// <returns></returns>
        public static string GetTypeSymbolAsGenericArray(this ITypeSymbol typeSymbol, bool metadataName)
        {
            if (typeSymbol is not IArrayTypeSymbol arrayTypeSymbol)
                return string.Empty;

            _stringBuilder.Clear();
            _stringBuilder.Append($"{NativeConstants.FirstGenericParameter_Name}[");

            AppendMultidimensionalAndJagged(arrayTypeSymbol);

            void AppendMultidimensionalAndJagged(IArrayTypeSymbol arrSym)
            {
                for (int i = 1; i < arrSym.Rank; i++)
                    _stringBuilder.Append(",");

                if (arrSym.ElementType is IArrayTypeSymbol jaggedElement)
                {
                    _stringBuilder.Append("][");
                    AppendMultidimensionalAndJagged(jaggedElement);
                }
            }

            _stringBuilder.Append("]");

            Debugg.Log($"Tuple? {arrayTypeSymbol.IsTupleType}. Element type? {arrayTypeSymbol.ElementType.TypeKind}");

            return _stringBuilder.ToString();
        }


        public static List<string> GetGenericArgumentsString(this ITypeSymbol symbol, GenericArgumentType argumentType)
        {
            List<string> results = new();
            int typeParameterCount = 0;
            bool forceGeneric = (argumentType == GenericArgumentType.ForceGeneric);

            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
            {
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
                if (forceGeneric || arrayTypeSymbol.ElementType.TypeKind is TypeKind.TypeParameter)
                    results.Add($"T{typeParameterCount++}");
                else
                    results.Add(arrayTypeSymbol.ElementType.GetTypeSymbolFullNameWithNamedArguments(metadataName: false));
            }

            return results;
        }

        /// <summary>
        /// Returns true if all generic arguments are named.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
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

            //No generic arguemnts; return true.
            return true;
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

        /// <summary>
        /// Returns a string as readable context (UserStruct.SomeField).
        /// </summary>
        public static string ToReadable(this ITypeSymbol typeSymbol) => ToReadable(typeSymbol, fieldSymbol: null);

        /// <summary>
        /// Returns a string as readable context (UserStruct.SomeField).
        /// </summary>
        public static string ToReadable(this ITypeSymbol typeSymbol, IFieldSymbol? fieldSymbol)
        {
            bool metadataName = false;
            _stringBuilder.Clear();
            _stringBuilder.Append(typeSymbol.GetTypeSymbolFullName(metadataName));
            if (fieldSymbol != null)
                _stringBuilder.Append($".{fieldSymbol.GetSymbolFullName(metadataName)}");

            return _stringBuilder.ToString();
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
