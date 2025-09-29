#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace CodeAnalysis.Common.Extensions
{
    public static class AttributeDataExtensions
    {
        public static IReadOnlyList<AttributeData> GetAttributes(this AttributeListSyntax attributes, Compilation compilation)
        {
            // Collect pertinent syntax trees from these attributes
            HashSet<SyntaxTree> acceptedTrees = new();
            foreach (AttributeSyntax attribute in attributes.Attributes)
                acceptedTrees.Add(attribute.SyntaxTree);

            ISymbol parentSymbol = attributes.Parent!.GetDeclaredSymbol(compilation)!;
            ImmutableArray<AttributeData> parentAttributes = parentSymbol.GetAttributes();
            List<AttributeData> ret = new();
            foreach (AttributeData attribute in parentAttributes)
            {
                if (acceptedTrees.Contains(attribute.ApplicationSyntaxReference!.SyntaxTree))
                    ret.Add(attribute);
            }

            return ret;
        }

        public static bool HasAttribute(this List<AttributeListSyntax> attributeListSyntaxs,  GeneratorSyntaxContext context, string attributeFullName, bool isMetadataName)
        {
            if (attributeListSyntaxs == null)
                return false;

            // check for a specific attribute by name
            foreach (AttributeListSyntax atrList in attributeListSyntaxs)
            {
                if (atrList.HasAttribute(context, attributeFullName, isMetadataName))
                    return true;
            }

            return false;
        }

        public static bool HasAttribute(this AttributeListSyntax attributeListSyntax, GeneratorSyntaxContext context, string attributeFullName, bool isMetadataName)
        {
            if (attributeListSyntax == null)
                return false;

            foreach (AttributeSyntax atr in attributeListSyntax.Attributes)
            {
                ISymbol symbol = context.SemanticModel.GetSymbolInfo(atr).Symbol;

                if (symbol.HasAttribute(attributeFullName, isMetadataName))
                    return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetConstructorArgument<T>(this AttributeData thisAttributeData, int argumentIndex)
        {
            ImmutableArray<TypedConstant> constructorArguments = thisAttributeData.ConstructorArguments;

            if (argumentIndex > -1 && argumentIndex < constructorArguments.Length)
                return (T)constructorArguments[argumentIndex].Value;

            return default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T GetNamedArgument<T>(this AttributeData thisAttributeData, int argumentIndex)
        {
            ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = thisAttributeData.NamedArguments;

            if (argumentIndex > -1 && argumentIndex < namedArguments.Length)
                return (T)namedArguments[argumentIndex].Value.Value;

            return default;
        }

        public static T GetNamedArgument<T>(this AttributeData thisAttributeData, string argumentName) => thisAttributeData.GetNamedArgument<T>(argumentName, default);

        public static T GetNamedArgument<T>(this AttributeData thisAttributeData, string argumentName, T defaultValue)
        {
            foreach (KeyValuePair<string, TypedConstant> namedArgument in thisAttributeData.NamedArguments)
                if (namedArgument.Key == argumentName)
                    return (T)namedArgument.Value.Value;

            return defaultValue;
            ;
        }
    }
    #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
}