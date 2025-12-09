using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Text;
using CodeAnalysis.Common.Constants;

namespace CodeAnalysis.Common.Extensions
{
    public enum ArgumentSearchType
    {
        /// <summary>
        /// Will return arguments as named when possible (bool, string).
        /// </summary>
        PreferNamed,
        /// <summary>
        /// Returns arguments only if they are named.
        /// </summary>
        ExplicitlyNamed,
        /// <summary>
        /// Returns arguments as generic (T0, T1).
        /// </summary>
        Generic
    }

    [System.Flags]
    public enum ArgumentSearchResult
    {
        /// <summary>
        /// Used when the search type rules could not be met.
        /// </summary>
        ErrorForSearchType = 1 << 0,
        /// <summary>
        /// Used when the type has arguments and the search type rules could be met.
        /// </summary>
        HasArguments = 1 << 1,
        /// <summary>
        /// Used when the type does not have any arguments.
        /// </summary>
        NoArguments = 1 << 2,
    }

    public static class ArgumentSearchTypeExtensions
    {
        public static bool Is(this ArgumentSearchType thisArgumentSearchType, ArgumentSearchType otherArgumentSearchType) => thisArgumentSearchType == otherArgumentSearchType;
        public static bool IsPreferNamed(this ArgumentSearchType thisArgumentSearchType) => thisArgumentSearchType == ArgumentSearchType.PreferNamed;
        public static bool IsExplicitlyNamed(this ArgumentSearchType thisArgumentSearchType) => thisArgumentSearchType == ArgumentSearchType.ExplicitlyNamed;
        public static bool IsGeneric(this ArgumentSearchType thisArgumentSearchType) => thisArgumentSearchType == ArgumentSearchType.Generic;
    }

    public static class ArgumentSearchResultExtensions
    {
        public static bool Is(this ArgumentSearchResult thisArgumentSearchResult, ArgumentSearchResult otherArgumentSearchResult) => thisArgumentSearchResult == otherArgumentSearchResult;
        public static bool IsErrorForSearchType(this ArgumentSearchResult thisArgumentSearchResult) => thisArgumentSearchResult == ArgumentSearchResult.ErrorForSearchType;
        public static bool IsHasArguments(this ArgumentSearchResult thisArgumentSearchResult) => thisArgumentSearchResult == ArgumentSearchResult.HasArguments;
        public static bool IsNoArguments(this ArgumentSearchResult thisArgumentSearchResult) => thisArgumentSearchResult == ArgumentSearchResult.NoArguments;
        public static bool HasError(this ArgumentSearchResult thisArgumentSearchResult) => thisArgumentSearchResult.HasFlag(ArgumentSearchResult.ErrorForSearchType);
    }

    public static class ITypeSymbolExtensions
    {
        private static readonly StringBuilder _stringBuilder = new();

        /// <summary>
        /// Gets the full name of a TypeSymbol.
        /// </summary>
        public static string GetTypeSymbolFullName(this ITypeSymbol typeSymbol)
        {
            if (typeSymbol is null)
                return string.Empty;

            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
                typeSymbol = arrayTypeSymbol.ElementType;

            // If generic then just return our const for generic.
            if (typeSymbol.TypeKind is TypeKind.TypeParameter)
                return NativeConstants.FirstGenericParameter_Name;

            string containingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            string joiningChar = Finding.Constants.NamespaceNameJoiningCharacter;
            for (INamedTypeSymbol currentType = typeSymbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}{joiningChar}{fullyQualifiedName}";

            // fullyQualifiedName = $"{fullyQualifiedName}{typeSymbol.Name}{arraySuffix}";
            fullyQualifiedName = $"{fullyQualifiedName}{typeSymbol.Name}";

            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}{joiningChar}{fullyQualifiedName}";
        }

        /// <summary>
        /// Returns full name with generic arguments as named types (System.Collections.Generic.List<System.String>).
        /// </summary>
        public static string GetTypeSymbolFullNameWithArguments(this ITypeSymbol typeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
        {
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
        
            /* If is an array then just use the extension method
             * to return the type named as an array. */
            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
                return arrayTypeSymbol.GetArrayTypeSymbolFullNameWithArgumentsZ(argumentSearchType, out argumentSearchResult);
        
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
                return namedTypeSymbol.GetNamedTypeSymbolFullNameWithArgumentsZ(argumentSearchType, out argumentSearchResult);
        
            return string.Empty;
        }

        /// <summary>
        /// Returns type as a generic array, if an array (T0[], T0[][]). If not an array empty is returned.
        /// </summary>
        public static string GetArrayTypeSymbolFullNameWithArgumentsZ(this IArrayTypeSymbol arrayTypeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
        {
            _stringBuilder.Clear();

            //Default value until changed.
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;

            bool isSearchTypeExplicitlyNamed = argumentSearchType.IsExplicitlyNamed();

            //Expecting named arguments, but they are not named.
            if (isSearchTypeExplicitlyNamed && !arrayTypeSymbol.AreArgumentsPresentAndNamed())
                return string.Empty;

            string symbolName = argumentSearchType == ArgumentSearchType.Generic ? NativeConstants.FirstGenericParameter_Name : arrayTypeSymbol.ElementType.GetTypeSymbolFullName();
            _stringBuilder.Append($"{symbolName}[");

            //Add any additional sub-arrays.
            TryAppendMultidimensionalAndJagged(arrayTypeSymbol);

            /* Tries to append a multidimensional or jagged array.
             * True is returned if succesful or no append is required,
             * false is returned on error. */
            bool TryAppendMultidimensionalAndJagged(IArrayTypeSymbol arrSym)
            {
                for (int i = 1; i < arrSym.Rank; i++)
                    _stringBuilder.Append(",");

                if (arrSym.ElementType is IArrayTypeSymbol jaggedElement)
                {
                    if (isSearchTypeExplicitlyNamed && !jaggedElement.AreArgumentsPresentAndNamed())
                        return false;

                    _stringBuilder.Append("][");
                    if (!TryAppendMultidimensionalAndJagged(jaggedElement))
                        return false;
                }

                return true;
            }

            _stringBuilder.Append("]");

            /* If here then success. Arrays themselves are arguments, so has arguments
             * is always set as the result on success. */
            argumentSearchResult = ArgumentSearchResult.HasArguments;

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Returns arguments as a list.
        /// </summary>
        public static string GetNamedTypeSymbolFullNameWithArgumentsZ(this INamedTypeSymbol namedTypeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
        {
            //Default value until changed.
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
            string typeFullName = namedTypeSymbol.GetTypeSymbolFullName();

            List<string> results = new();

            //Type does not have arguments.
            if (!namedTypeSymbol.IsGenericType)
            {
                argumentSearchResult = ArgumentSearchResult.NoArguments;
                return typeFullName;
            }

            int typeParameterCount = 0;

            if (argumentSearchType.IsExplicitlyNamed() && !namedTypeSymbol.AreArgumentsPresentAndNamed())
                return string.Empty;

            foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
            {
                if (argumentSearchType.IsGeneric() || typeArgument.TypeKind is TypeKind.TypeParameter)
                    results.Add($"T{typeParameterCount++}");
                else if (typeArgument is INamedTypeSymbol argumentNamedTypeSymbol)
                    results.Add(argumentNamedTypeSymbol.GetNamedTypeSymbolFullNameWithArgumentsZ(argumentSearchType, out argumentSearchResult));
                else if (typeArgument is IArrayTypeSymbol argumentArrayTypeSymbol)
                    results.Add(argumentArrayTypeSymbol.GetArrayTypeSymbolFullNameWithArgumentsZ(argumentSearchType, out argumentSearchResult));
                else
                    argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
                
                //If search result has switched to error.
                if (argumentSearchResult.HasError())
                    return string.Empty;
            }

            /* If here there is no error. */
            argumentSearchResult = results.Count > 0 ? ArgumentSearchResult.HasArguments : ArgumentSearchResult.NoArguments;

            return $"{typeFullName}{results.GetCombinedArguments()}";
        }

        /// <summary>
        /// Returns arguments as a list.
        /// </summary>
        public static List<string> GetTypeSymbolArgumentsZ(this INamedTypeSymbol namedTypeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
        {
            //Default value until changed.
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;

            List<string> results = new();

            //Type does not have arguments.
            if (!namedTypeSymbol.IsGenericType)
            {
                argumentSearchResult = ArgumentSearchResult.NoArguments;
                return results;
            }

            int typeParameterCount = 0;

            if (argumentSearchType.IsExplicitlyNamed() && !namedTypeSymbol.AreArgumentsPresentAndNamed())
                return results;

            foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
            {
                if (argumentSearchType.IsGeneric() || typeArgument.TypeKind is TypeKind.TypeParameter)
                    results.Add($"T{typeParameterCount++}");
                else if (typeArgument is INamedTypeSymbol argumentNamedTypeSymbol)
                    results.Add(argumentNamedTypeSymbol.GetNamedTypeSymbolFullNameWithArgumentsZ(argumentSearchType, out argumentSearchResult));
                else
                    argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;

                //If search result has switched to error.
                if (argumentSearchResult.HasError())
                    return results;
            }

            /* If here there is no error. */
            argumentSearchResult = results.Count > 0 ? ArgumentSearchResult.HasArguments : ArgumentSearchResult.NoArguments;

            return results;
        }
        
        /// <summary>
        /// Returns true if a TypeSymbol has arguments.
        /// </summary>
        public static bool HasArguments(this ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol)
                return true;

            if (symbol is INamedTypeSymbol { IsGenericType: true })
                return true;

            return false;
        }

        /// <summary>
        /// Returns true if arguments are present and all are named.
        /// </summary>
        public static bool AreArgumentsPresentAndNamed(this ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol arrayTypeSymbol)
                return arrayTypeSymbol.ElementType.TypeKind is not TypeKind.TypeParameter;

            if (symbol is INamedTypeSymbol { IsGenericType: true } namedTypeSymbol)
            {
                foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
                {
                    if (typeArgument.TypeKind is TypeKind.TypeParameter)
                        return false;
                }
            }

            //No arguments, return true.
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
        public static string ToReadable(this ITypeSymbol typeSymbol, IFieldSymbol fieldSymbol)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append(typeSymbol.GetTypeSymbolFullName());
            if (fieldSymbol is not null)
                _stringBuilder.Append($".{fieldSymbol.GetSymbolFullName()}");

            return _stringBuilder.ToString();
        }
    }
}