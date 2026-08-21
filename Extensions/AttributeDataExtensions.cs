#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for working with attribute data and attribute syntax.
/// </summary>
public static class AttributeDataExtensions
{
    // public static IReadOnlyList<AttributeData> GetAttributes(this SyntaxList<AttributeListSyntax> syntaxList, Compilation compilation)
    // {
    //     return null;
    //     // List<AttributeData> attributes = new();
    //     //
    //     // foreach (AttributeListSyntax atrList in syntaxList)
    //     // {
    //     //     if (atrList is null)
    //     //         continue;
    //     //     
    //     //     attributes.AddRange(atrList.GetAttributes(compilation));
    //     // }
    //     //
    //     // return attributes;
    // }
        
    /// <summary>
    /// Returns the attribute data values for the attributes declared in the supplied attribute list.
    /// </summary>
    /// <param name="attributes">Attribute list whose attributes should be resolved.</param>
    /// <param name="compilation">Compilation used to resolve symbols.</param>
    /// <returns>The attribute data values for the supplied attribute list.</returns>
    public static IReadOnlyList<AttributeData> GetAttributes(this AttributeListSyntax attributes, Compilation compilation)
    {
        // Collect pertinent syntax trees from these attributes
        HashSet<SyntaxTree> acceptedTrees = new();
        foreach (AttributeSyntax attribute in attributes.Attributes)
            acceptedTrees.Add(attribute.SyntaxTree);

        List<AttributeData> ret = new();

        ISymbol parentSymbol = attributes.Parent?.GetDeclaredSymbol(compilation);
        if (parentSymbol is not null)
        {
            ImmutableArray<AttributeData> parentAttributes = parentSymbol.GetAttributes();
            foreach (AttributeData attribute in parentAttributes)
            {
                if (acceptedTrees.Contains(attribute.ApplicationSyntaxReference!.SyntaxTree))
                    ret.Add(attribute);
            }
        }

        return ret;
    }

    /// <summary>
    /// Returns whether any attribute list in the supplied syntax list contains an attribute with the specified full name.
    /// </summary>
    /// <param name="syntaxList">Syntax list to search.</param>
    /// <param name="attributeFullName">Attribute name to find.</param>
    /// <returns>True when a matching attribute is found.</returns>
    public static bool HasAttribute(this SyntaxList<AttributeListSyntax> syntaxList, string attributeFullName)
    {
        foreach (AttributeListSyntax atrList in syntaxList)
        {
            if (atrList.HasAttribute(attributeFullName))
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns whether the supplied attribute list contains an attribute with the specified full name.
    /// </summary>
    /// <param name="attributeListSyntax">Attribute list to search.</param>
    /// <param name="attributeFullName">Attribute name to find.</param>
    /// <returns>True when a matching attribute is found.</returns>
    public static bool HasAttribute(this AttributeListSyntax attributeListSyntax,string attributeFullName)
    {
        if (attributeListSyntax == null)
            return false;
            
        foreach (AttributeSyntax atr in attributeListSyntax.Attributes)
        {
            if (atr.Name.ToString() == attributeFullName)
                return true;
        }
            
        return false;
    }

    /// <summary>
    /// Returns the constructor argument at the specified index, cast to the requested type.
    /// </summary>
    /// <typeparam name="T0">Type to cast the argument value to.</typeparam>
    /// <param name="thisAttributeData">Attribute data to read.</param>
    /// <param name="argumentIndex">Zero-based index of the constructor argument.</param>
    /// <returns>The argument value, or the default value when the index is out of range.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T0? GetConstructorArgument<T0>(this AttributeData thisAttributeData, int argumentIndex)
    {
        ImmutableArray<TypedConstant> constructorArguments = thisAttributeData.ConstructorArguments;

        if (argumentIndex > -1 && argumentIndex < constructorArguments.Length)
            return (T0)constructorArguments[argumentIndex].Value;

        return default;
    }


    /// <summary>
    /// Returns the named argument at the specified index, cast to the requested type.
    /// </summary>
    /// <typeparam name="T0">Type to cast the argument value to.</typeparam>
    /// <param name="thisAttributeData">Attribute data to read.</param>
    /// <param name="argumentIndex">Zero-based index of the named argument.</param>
    /// <returns>The argument value, or the default value when the index is out of range.</returns>
    public static T0? GetNamedArgument<T0>(this AttributeData thisAttributeData, int argumentIndex)
    {
        ImmutableArray<KeyValuePair<string, TypedConstant>> namedArguments = thisAttributeData.NamedArguments;

        if (argumentIndex > -1 && argumentIndex < namedArguments.Length)
            return (T0)namedArguments[argumentIndex].Value.Value;

        return default;
    }

    /// <summary>
    /// Returns the named argument with the specified name, cast to the requested type.
    /// </summary>
    /// <typeparam name="T0">Type to cast the argument value to.</typeparam>
    /// <param name="thisAttributeData">Attribute data to read.</param>
    /// <param name="argumentName">Name of the argument to look up.</param>
    /// <returns>The argument value, or the default value when no matching argument exists.</returns>
    public static T0? GetNamedArgument<T0>(this AttributeData thisAttributeData, string argumentName) => thisAttributeData.GetNamedArgument<T0>(argumentName, default);

    /// <summary>
    /// Returns the named argument with the specified name, cast to the requested type, or the supplied default when no match exists.
    /// </summary>
    /// <typeparam name="T0">Type to cast the argument value to.</typeparam>
    /// <param name="thisAttributeData">Attribute data to read.</param>
    /// <param name="argumentName">Name of the argument to look up.</param>
    /// <param name="defaultValue">Default value to return when no matching argument exists.</param>
    /// <returns>The argument value, or the supplied default value when no matching argument exists.</returns>
    public static T0? GetNamedArgument<T0>(this AttributeData thisAttributeData, string argumentName, T0? defaultValue)
    {
        foreach (KeyValuePair<string, TypedConstant> namedArgument in thisAttributeData.NamedArguments)
        {
            if (namedArgument.Key == argumentName)
                return (T0)namedArgument.Value.Value;
        }

        return defaultValue;
    }
}
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.