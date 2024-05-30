#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions;

internal static class AttributeDataExtensions
{
    public static IReadOnlyList<AttributeData> GetAttributes(this AttributeListSyntax attributes, Compilation compilation)
    {
        // Collect pertinent syntax trees from these attributes
        var acceptedTrees = new HashSet<SyntaxTree>();
        foreach (var attribute in attributes.Attributes)
            acceptedTrees.Add(attribute.SyntaxTree);

        var parentSymbol = attributes.Parent!.GetDeclaredSymbol(compilation)!;
        var parentAttributes = parentSymbol.GetAttributes();
        var ret = new List<AttributeData>();
        foreach (var attribute in parentAttributes)
        {
            if (acceptedTrees.Contains(attribute.ApplicationSyntaxReference!.SyntaxTree))
                ret.Add(attribute);
        }

        return ret;
    }

    public static ISymbol? GetDeclaredSymbol(this SyntaxNode node, Compilation compilation)
    {
        var model = compilation.GetSemanticModel(node.SyntaxTree);
        return model.GetDeclaredSymbol(node);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? GetConstructorArgument<T>(this AttributeData thisAttributeData, int argumentIndex)
	{
		ImmutableArray<TypedConstant> constructorArguments = thisAttributeData.ConstructorArguments;

		if (argumentIndex > -1 && argumentIndex < constructorArguments.Length) return (T)constructorArguments[argumentIndex].Value;

		return default;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static T? GetNamedArgument<T>(this AttributeData thisAttributeData, int argumentIndex)
	{
		ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = thisAttributeData.NamedArguments;

		if (argumentIndex > -1 && argumentIndex < namedArguments.Length) return (T)namedArguments[argumentIndex].Value.Value;

		return default;
	}

	public static T? GetNamedArgument<T>(this AttributeData thisAttributeData, string argumentName)
	{
		foreach (KeyValuePair<string, TypedConstant> namedArgument in thisAttributeData.NamedArguments)
		{
			if (namedArgument.Key == argumentName) return (T)namedArgument.Value.Value;
		}

		return default;
	}
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.