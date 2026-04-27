using System.Collections.Generic;
using System.Text;
using CodeAnalysis.Constants;
using CodeAnalysis.Finding;
using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for inspecting and formatting <see cref="ITypeSymbol"/> instances.
/// </summary>
public static class TypeSymbolExtensions
{
    /// <summary>
    /// Returns the fully qualified name of the supplied type symbol.
    /// </summary>
    /// <param name="typeSymbol">Type symbol whose full name is being generated.</param>
    /// <returns>The fully qualified name of the type symbol.</returns>
    public static string GetTypeSymbolFullName(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol is null)
            return string.Empty;

        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            typeSymbol = arrayTypeSymbol.ElementType;

        // If generic then just return our const for generic.
        if (typeSymbol.TypeKind is TypeKind.TypeParameter)
            return NativeConstants.FirstGenericParameterName;


        string containingNamespace = typeSymbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).EmptyIfNull()!;
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
    /// Returns the full name of the type symbol with its generic arguments rendered as named types, such as <c>System.Collections.Generic.List&lt;System.String&gt;</c>.
    /// </summary>
    /// <param name="typeSymbol">Type symbol whose full name is being generated.</param>
    /// <param name="argumentSearchType">Search behavior to apply when resolving argument names.</param>
    /// <param name="argumentSearchResult">Receives a status describing how arguments were resolved.</param>
    /// <returns>The fully qualified name of the type symbol, including its arguments.</returns>
    public static string GetTypeSymbolFullNameWithArguments(this ITypeSymbol typeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
    {
        /* If is an array then just use the extension method
         * to return the type named as an array. */
        if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            return arrayTypeSymbol.GetArrayTypeSymbolFullNameWithArguments(argumentSearchType, out argumentSearchResult);

        if (typeSymbol is INamedTypeSymbol namedTypeSymbol)
            return namedTypeSymbol.GetNamedTypeSymbolFullNameWithArguments(argumentSearchType, out argumentSearchResult);

        argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
        return string.Empty;
    }

    /// <summary>
    /// Returns the array type formatted with its arguments, such as <c>T0[]</c> or <c>T0[][]</c>.
    /// </summary>
    /// <param name="arrayTypeSymbol">Array type symbol whose full name is being generated.</param>
    /// <param name="argumentSearchType">Search behavior to apply when resolving argument names.</param>
    /// <param name="argumentSearchResult">Receives a status describing how arguments were resolved.</param>
    /// <returns>The fully qualified name of the array type, or an empty string when it cannot be resolved.</returns>
    public static string GetArrayTypeSymbolFullNameWithArguments(this IArrayTypeSymbol arrayTypeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
    {
        StringBuilder stringBuilder = new();

        bool isSearchTypeExplicitlyNamed = argumentSearchType is ArgumentSearchType.ExplicitlyNamed;

        //Expecting named arguments, but they are not named.
        if (isSearchTypeExplicitlyNamed && !arrayTypeSymbol.ArePresentArgumentsNamed())
        {
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
            return string.Empty;
        }

        string symbolName = argumentSearchType == ArgumentSearchType.Generic ? NativeConstants.FirstGenericParameterName : arrayTypeSymbol.ElementType.GetTypeSymbolFullName();
        stringBuilder.Append($"{symbolName}[");

        //Add any additional sub-arrays.
        TryAppendMultidimensionalAndJagged(arrayTypeSymbol);

        /* Tries to append a multidimensional or jagged array.
         * True is returned if succesful or no append is required,
         * false is returned on error. */
        bool TryAppendMultidimensionalAndJagged(IArrayTypeSymbol arrSym)
        {
            for (int i = 1; i < arrSym.Rank; i++)
                stringBuilder.Append(",");

            if (arrSym.ElementType is IArrayTypeSymbol jaggedElement)
            {
                if (isSearchTypeExplicitlyNamed && !jaggedElement.ArePresentArgumentsNamed())
                    return false;

                stringBuilder.Append("][");
                if (!TryAppendMultidimensionalAndJagged(jaggedElement))
                    return false;
            }

            return true;
        }

        stringBuilder.Append("]");

        /* If here then success. Arrays themselves are arguments, so has arguments
         * is always set as the result on success. */
        argumentSearchResult = ArgumentSearchResult.HasArguments;

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Returns the named type symbol's full name with its generic arguments included.
    /// </summary>
    /// <example>
    /// <c>RootNamespace.Strings.StringBuffer&lt;int&gt;</c>.
    /// </example>
    /// <param name="namedTypeSymbol">Named type symbol whose full name is being generated.</param>
    /// <param name="argumentSearchType">Search behavior to apply when resolving argument names.</param>
    /// <param name="argumentSearchResult">Receives a status describing how arguments were resolved.</param>
    /// <returns>The fully qualified name of the named type, including its arguments.</returns>
    public static string GetNamedTypeSymbolFullNameWithArguments(this INamedTypeSymbol namedTypeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
    {
        //Default value until changed.
        string typeFullName = namedTypeSymbol.GetTypeSymbolFullName();

        List<string> results = [];

        //Type does not have arguments.
        if (!namedTypeSymbol.IsGenericType)
        {
            argumentSearchResult = ArgumentSearchResult.NoArguments;
            return typeFullName;
        }

        int typeParameterCount = 0;

        if (argumentSearchType is ArgumentSearchType.ExplicitlyNamed && !namedTypeSymbol.ArePresentArgumentsNamed())
        {
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
            return string.Empty;
        }

        argumentSearchResult = ArgumentSearchResult.HasArguments;

        foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
        {
            if (argumentSearchType is ArgumentSearchType.Generic || typeArgument.TypeKind is TypeKind.TypeParameter)
                results.Add($"T{typeParameterCount++}");
            else if (typeArgument is INamedTypeSymbol argumentNamedTypeSymbol)
                results.Add(argumentNamedTypeSymbol.GetNamedTypeSymbolFullNameWithArguments(argumentSearchType, out argumentSearchResult));
            else if (typeArgument is IArrayTypeSymbol argumentArrayTypeSymbol)
                results.Add(argumentArrayTypeSymbol.GetArrayTypeSymbolFullNameWithArguments(argumentSearchType, out argumentSearchResult));
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
    /// Returns the generic arguments of the supplied named type as a list.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose arguments are being read.</param>
    /// <param name="argumentSearchType">Search behavior to apply when resolving argument names.</param>
    /// <param name="argumentSearchResult">Receives a status describing how arguments were resolved.</param>
    /// <returns>A list containing every resolved argument.</returns>
    public static List<Argument> GetTypeSymbolArguments(this INamedTypeSymbol namedTypeSymbol, ArgumentSearchType argumentSearchType, out ArgumentSearchResult argumentSearchResult)
    {
        List<Argument> results = [];

        //Type does not have arguments.
        if (!namedTypeSymbol.IsGenericType)
        {
            argumentSearchResult = ArgumentSearchResult.NoArguments;
            return results;
        }

        int typeParameterCount = 0;

        if (argumentSearchType is ArgumentSearchType.ExplicitlyNamed && !namedTypeSymbol.ArePresentArgumentsNamed())
        {
            argumentSearchResult = ArgumentSearchResult.ErrorForSearchType;
            return results;
        }

        argumentSearchResult = ArgumentSearchResult.HasArguments;

        foreach (ITypeSymbol typeArgument in namedTypeSymbol.TypeArguments)
        {
            if (argumentSearchType is ArgumentSearchType.Generic || typeArgument.TypeKind is TypeKind.TypeParameter)
                results.Add(new(typeArgument, $"T{typeParameterCount++}", isNamed: false));
            else if (typeArgument is INamedTypeSymbol argumentNamedTypeSymbol)
                results.Add(new(typeArgument, argumentNamedTypeSymbol.GetNamedTypeSymbolFullNameWithArguments(argumentSearchType, out argumentSearchResult), isNamed: true));
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
    /// Returns whether the supplied type symbol declares generic arguments.
    /// </summary>
    /// <param name="symbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is an array or generic named type.</returns>
    public static bool HasArguments(this ITypeSymbol symbol)
    {
        if (symbol is IArrayTypeSymbol)
            return true;

        if (symbol is INamedTypeSymbol { IsGenericType: true })
            return true;

        return false;
    }

    /// <summary>
    /// Returns whether every generic argument supplied to the type is a concrete named type rather than a type parameter.
    /// </summary>
    /// <param name="symbol">Type symbol whose arguments are being inspected.</param>
    /// <returns>True when every argument resolves to a concrete named type.</returns>
    public static bool ArePresentArgumentsNamed(this ITypeSymbol symbol)
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

    /// <summary>
    /// Returns whether the supplied type symbol directly implements an interface with the specified fully qualified name.
    /// </summary>
    /// <param name="symbol">Type symbol whose implemented interfaces are being inspected.</param>
    /// <param name="interfaceFullName">Fully qualified interface name to look for.</param>
    /// <returns>True when the type symbol directly implements the specified interface.</returns>
    public static bool TypeSymbolImplementsInterface(this ITypeSymbol symbol, string? interfaceFullName)
    {
        if (interfaceFullName is null)
            return false;

        foreach (INamedTypeSymbol interfaceNamed in symbol.Interfaces)
        {
            if (interfaceNamed.GetTypeSymbolFullName() == interfaceFullName)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a user-defined enum, class, or struct.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a user-defined enum, class, or struct.</returns>
    public static bool IsUserDefinedEnumClassOrStruct(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsUserDefinedEnum() || typeSymbol.IsUserDefinedClass() || typeSymbol.IsUserDefinedStruct();
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a user-defined class or struct.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a user-defined class or struct.</returns>
    public static bool IsUserDefinedClassOrStruct(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsUserDefinedClass() || typeSymbol.IsUserDefinedStruct();
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a user-defined struct.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a user-defined struct.</returns>
    public static bool IsUserDefinedStruct(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is { TypeKind: TypeKind.Struct, SpecialType: SpecialType.None };
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a user-defined class.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a user-defined class.</returns>
    public static bool IsUserDefinedClass(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is { TypeKind: TypeKind.Class, SpecialType: SpecialType.None };
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a user-defined enum.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a user-defined enum.</returns>
    public static bool IsUserDefinedEnum(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is { TypeKind: TypeKind.Enum, SpecialType: SpecialType.None };
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a class.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a class.</returns>
    public static bool IsClass(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is { TypeKind: TypeKind.Class };
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a struct.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a struct.</returns>
    public static bool IsStruct(this ITypeSymbol typeSymbol)
    {
        return typeSymbol is { TypeKind: TypeKind.Struct };
    }

    /// <summary>
    /// Returns whether the supplied type symbol is either a class or a struct.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a class or struct.</returns>
    public static bool IsClassOrStruct(this ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsStruct() || typeSymbol.IsClass();
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a reference type or is declared as nullable.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol can hold a null reference.</returns>
    public static bool CanBeNull(this ITypeSymbol typeSymbol)
    {
        if (typeSymbol.IsReferenceType)
            return true;

        if (typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T)
            return true;

        if (typeSymbol.TypeKind is TypeKind.Pointer)
            return true;

        return typeSymbol is ITypeParameterSymbol { IsReferenceType: true };
    }

    /// <summary>
    /// Tries to retrieve the type encapsulated by a <see cref="System.Nullable{T}"/> when the encapsulated type is an <see cref="INamedTypeSymbol"/>.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <param name="encapsulatedTypeSymbol">Receives the encapsulated type symbol when successful.</param>
    /// <returns>True when the encapsulated type was resolved successfully.</returns>
    public static bool TryGetNullableEncapsulatedNamedTypeSymbol(this ITypeSymbol typeSymbol, out ITypeSymbol encapsulatedTypeSymbol)
    {
        encapsulatedTypeSymbol = null;
        
        if (!typeSymbol.IsNullable())
            return false;

        if (typeSymbol is not INamedTypeSymbol namedTypeSymbol)
            return false;

        if (namedTypeSymbol.TypeArguments.Length == 0)
            return false;

        encapsulatedTypeSymbol = namedTypeSymbol.TypeArguments[0];
        if (encapsulatedTypeSymbol is not INamedTypeSymbol)
            return false;

        return true;
    }

    /// <summary>
    /// Returns whether the supplied type symbol is a primitive type, as identified by its <see cref="SpecialType"/>.
    /// </summary>
    /// <param name="symbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a primitive type.</returns>
    public static bool IsPrimitive(this ITypeSymbol symbol)
    {
        switch (symbol.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_UInt16:
            case SpecialType.System_Int32:
            case SpecialType.System_UInt32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Char:
                return true;
            default:
                return false;
        }
    }
    /// <summary>
    /// Returns whether the supplied type symbol is encapsulated in <see cref="System.Nullable{T}"/>.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to inspect.</param>
    /// <returns>True when the type symbol is a nullable value type.</returns>
    public static bool IsNullable(this ITypeSymbol typeSymbol) => typeSymbol.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T;

    /// <summary>
    /// Returns the type symbol as a readable string, such as <c>UserStruct</c>.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to format.</param>
    /// <returns>A readable representation of the type symbol.</returns>
    public static string ToReadable(this ITypeSymbol typeSymbol) => ToReadable(typeSymbol, fieldSymbol: null);

    /// <summary>
    /// Returns the type symbol and an optional field symbol as a readable string, such as <c>UserStruct.SomeField</c>.
    /// </summary>
    /// <param name="typeSymbol">Type symbol to format.</param>
    /// <param name="fieldSymbol">Optional field symbol to append to the result.</param>
    /// <returns>A readable representation of the type symbol and optional field.</returns>
    public static string ToReadable(this ITypeSymbol typeSymbol, IFieldSymbol fieldSymbol)
    {
        StringBuilder stringBuilder = new();

        stringBuilder.Append(typeSymbol.GetTypeSymbolFullName());
        if (fieldSymbol is not null)
            stringBuilder.Append($".{fieldSymbol.GetSymbolFullName()}");

        return stringBuilder.ToString();
    }
}