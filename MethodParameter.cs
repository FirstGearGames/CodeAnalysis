using System.Collections.Generic;
using System.Text;
using CodeAnalysis.Extensions;
using CodeAnalysis.Finding;
using Microsoft.CodeAnalysis;

namespace CodeAnalysis;

/// <summary>
/// Represents a single parameter on a method definition.
/// </summary>
public readonly struct MethodParameter
{
    /// <summary>
    /// The full type name of the parameter, including any generic arguments.
    /// </summary>
    public readonly string TypeFullName;
    /// <summary>
    /// The name of the parameter as declared on the method.
    /// </summary>
    public readonly string ParameterName;
    /// <summary>
    /// The optional default value of the parameter, formatted as a literal expression.
    /// </summary>
    public readonly string OptionalValue;
    /// <summary>
    /// The zero-based ordinal of the parameter within its method.
    /// </summary>
    public readonly int Index;
    /// <summary>
    /// Initializes a new instance of the <see cref="MethodParameter"/> struct from a parameter symbol.
    /// </summary>
    /// <param name="parameterSymbol">Parameter symbol to copy values from.</param>
    public MethodParameter(IParameterSymbol parameterSymbol) : this(parameterSymbol.Type.GetTypeSymbolFullNameWithArguments(ArgumentSearchType.PreferNamed, out _), parameterSymbol.Name, parameterSymbol.Ordinal, parameterSymbol.OptionalValueToString()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodParameter"/> struct.
    /// </summary>
    /// <param name="typeFullName">Full type name of the parameter.</param>
    /// <param name="parameterName">Name of the parameter.</param>
    /// <param name="index">Zero-based ordinal of the parameter.</param>
    /// <param name="optionalValue">Optional default value, formatted as a literal expression.</param>
    public MethodParameter(string? typeFullName, string parameterName,int index, string optionalValue)
    {
        if (typeFullName is null)
            typeFullName = string.Empty;
            
        TypeFullName = typeFullName;
        ParameterName = parameterName;
        Index = index;
        OptionalValue = optionalValue;
    }

    /// <summary>
    /// Returns the parameter type and name as they would appear in a method signature.
    /// </summary>
    /// <returns>The parameter formatted as it would appear in a method signature.</returns>
    public string GetParameterAsMethodSignature() => $"{TypeFullName} {ParameterName}";
        
}

/// <summary>
/// Extension methods for working with collections of <see cref="MethodParameter"/>.
/// </summary>
public static class MethodParameterExtensions
{
    /// <summary>
    /// Returns all entries as they would appear in a method signature.
    /// </summary>
    /// <example>bool isSafe, int healthRemaining</example>
    public static string GetAsMethodSignature(this List<MethodParameter> thisValue)
    {
        if (thisValue is null || thisValue.Count == 0)
            return string.Empty;

        StringBuilder stringBuilder = new();
        List<string> parametersAsSignatures = [];

        foreach (MethodParameter methodParameter in thisValue)
        {
            stringBuilder.Clear();

            stringBuilder.Append($"{methodParameter.TypeFullName} {methodParameter.ParameterName}");

            if (!string.IsNullOrWhiteSpace(methodParameter.OptionalValue))
            {
                stringBuilder.Append($" = {methodParameter.OptionalValue}");
                    
                /* If the type is a Single then the f to indicate
                 * float has to manually be added for compilation to complete. */
                bool typeIsSingle = methodParameter.TypeFullName.Equals(typeof(float).FullName);
                if (typeIsSingle)
                    stringBuilder.Append("f");
            }

            parametersAsSignatures.Add(stringBuilder.ToString());
        }

        return string.Join(", ", parametersAsSignatures);
    }

    /// <summary>
    /// Returns the name only of each parameter.
    /// </summary>
    public static List<string> GetParameterNames(this List<MethodParameter> methodParameters)
    {
        if (methodParameters is null || methodParameters.Count == 0)
            return [];

        List<string> names = [];

        for (int i = 0; i < methodParameters.Count; i++)
            names.Add(methodParameters[i].ParameterName);

        return names;
    }

    /// <summary>
    /// Returns the type full name of each parameter.
    /// </summary>
    public static List<string> GetParameterTypeFullNames(this List<MethodParameter> methodParameters)
    {
        if (methodParameters is null || methodParameters.Count == 0)
            return [];

        List<string> names = [];

        for (int i = 0; i < methodParameters.Count; i++)
            names.Add(methodParameters[i].TypeFullName);

        return names;
    }
}