using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for working with <see cref="IParameterSymbol"/> instances.
/// </summary>
public static class ParameterSymbolExtensions
{
    /// <summary>
    /// Returns the supplied parameter symbols converted to a <see cref="MethodParameter"/> collection.
    /// </summary>
    /// <param name="parameterSymbols">Parameter symbols to convert.</param>
    /// <returns>A list containing one <see cref="MethodParameter"/> per supplied symbol.</returns>
    public static List<MethodParameter> GetMethodParameters(this IEnumerable<IParameterSymbol> parameterSymbols)
    {
        List<MethodParameter> methodParameters = new();

        foreach (IParameterSymbol parameterSymbol in parameterSymbols)
            methodParameters.Add(new(parameterSymbol));

        return methodParameters;
    }
        
    /// <summary>
    /// Returns the explicit default value of the parameter formatted as a literal expression.
    /// </summary>
    /// <param name="thisValue">Parameter symbol whose default value is being formatted.</param>
    /// <returns>The default value formatted as a literal expression, or an empty string when no default is declared.</returns>
    public static string OptionalValueToString(this IParameterSymbol thisValue)
    {
        if (!thisValue.HasExplicitDefaultValue)
            return string.Empty;

        object? v = thisValue.ExplicitDefaultValue;

        if (v is null)
            return string.Empty;

        if (thisValue.Type.TypeKind == TypeKind.Enum)
        {
            INamedTypeSymbol enumType = (INamedTypeSymbol)thisValue.Type;
            IFieldSymbol? match = enumType.GetMembers().OfType<IFieldSymbol>().FirstOrDefault(f => f.HasConstantValue && Equals(f.ConstantValue, v));

            if (match is not null)
                return $"{enumType.Name}.{match.Name}";
        }

        return v switch
        {
            string s => s,
            char c => $"'{c}'",
            bool b => b ? "true" : "false",
            _ => Convert.ToString(v, CultureInfo.InvariantCulture) ?? v.ToString() ?? "Unprintable"
        };
    }

    /// <summary>
    /// Returns whether the supplied parameter has the same type full name as another parameter.
    /// </summary>
    /// <param name="parameterSymbol">Parameter being compared.</param>
    /// <param name="otherParameterSymbol">Parameter to compare against.</param>
    /// <returns>True when both parameters share the same type full name.</returns>
    public static bool TypeFullNameEquals(this IParameterSymbol parameterSymbol, IParameterSymbol otherParameterSymbol) => parameterSymbol.TypeFullNameEquals(otherParameterSymbol.Type.GetTypeSymbolFullName());
    /// <summary>
    /// Returns whether the supplied parameter has the specified type full name.
    /// </summary>
    /// <param name="parameterSymbol">Parameter being compared.</param>
    /// <param name="otherTypeFullName">Type full name to compare against.</param>
    /// <returns>True when the parameter type matches the supplied name.</returns>
    public static bool TypeFullNameEquals(this IParameterSymbol parameterSymbol, string? otherTypeFullName) => parameterSymbol.Type.GetTypeSymbolFullName().Equals(otherTypeFullName);
}