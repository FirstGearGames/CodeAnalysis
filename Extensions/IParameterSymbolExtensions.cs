using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Common.Extensions
{
    public static class IParameterSymbolExtensions
    {
        /// <summary>
        /// Returns ParameterSymbols as a MethodParameter collection.
        /// </summary>
        /// <returns></returns>
        public static List<MethodParameter> GetMethodParameters(this IEnumerable<IParameterSymbol> parameterSymbols)
        {
            List<MethodParameter> methodParameters = new();

            foreach (IParameterSymbol parameterSymbol in parameterSymbols)
                methodParameters.Add(new(parameterSymbol));

            return methodParameters;
        }
        
        public static string OptionalValueToString(this IParameterSymbol thisValue)
        {
            if (!thisValue.HasExplicitDefaultValue)
                return string.Empty;

            object? v = thisValue.ExplicitDefaultValue;

            return v switch
            {
                null => string.Empty,
                string s => s,
                char c => $"'{c}'",
                bool b => b ? "true" : "false",
                Enum e => $"{e.GetType().Name}.{e}",
                _ => Convert.ToString(v, CultureInfo.InvariantCulture)
                     ?? v.ToString()
                     ?? "Unprintable"
            };
        }

        public static bool TypeFullNameEquals(this IParameterSymbol parameterSymbol, IParameterSymbol otherParameterSymbol) => parameterSymbol.TypeFullNameEquals(otherParameterSymbol.Type.GetTypeSymbolFullName());
        public static bool TypeFullNameEquals(this IParameterSymbol parameterSymbol, string? otherTypeFullName) => parameterSymbol.Type.GetTypeSymbolFullName().Equals(otherTypeFullName);
    }
}