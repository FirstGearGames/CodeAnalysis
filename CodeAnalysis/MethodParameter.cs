using System.Collections.Generic;
using System.Text;
using CodeAnalysis.Common.Extensions;
using CodeAnalysis.Common.Finding;
using Microsoft.CodeAnalysis;

namespace CodeAnalysis
{
    public readonly struct MethodParameter
    {
        public readonly string TypeFullName;
        public readonly string ParameterName;
        public readonly int Index;

        public MethodParameter(IParameterSymbol parameterSymbol) : this(parameterSymbol.Type.GetTypeSymbolFullNameWithArguments(ArgumentSearchType.PreferNamed, out _), parameterSymbol.Name, parameterSymbol.Ordinal) 
        {
        }

        public MethodParameter(string? typeFullName, string parameterName, int index)
        {
            if (typeFullName is null)
                typeFullName = string.Empty;
            
            TypeFullName = typeFullName;
            ParameterName = parameterName;
            Index = index;
        }
        
        /// <summary>
        /// Returns the parameter type and name as it would be seen in a method signature.
        /// </summary>
        /// <returns></returns>
        public string GetParameterAsMethodSignature() => $"{TypeFullName} {ParameterName}";
    }

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

            List<string> parametersAsSignatures = [];

            foreach (MethodParameter methodParameter in thisValue) 
                parametersAsSignatures.Add($"{methodParameter.TypeFullName} {methodParameter.ParameterName}");
            
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
}