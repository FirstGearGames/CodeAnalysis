using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for inspecting <see cref="ISymbol"/> instances.
/// </summary>
public static class SymbolExtensions
{
    /// <summary>
    /// Returns the source location of the identifier of a method or class declaration.
    /// </summary>
    /// <remarks>
    /// Using the identifier location keeps diagnostics focused on the member name in the IDE.
    /// </remarks>
    /// <param name="symbol">Symbol whose identifier location is being resolved.</param>
    /// <returns>The location of the symbol's identifier, or <see cref="Location.None"/> when one cannot be resolved.</returns>
    public static Location GetIdentifierLocation(this ISymbol symbol)
    {
        if (!symbol.TryGetRecordDeclaration(out SyntaxNode? syntaxNode))
            return Location.None;

        // Use pattern matching or simple casting for older Roslyn
        if (syntaxNode is MethodDeclarationSyntax method)
            return method.Identifier.GetLocation();

        if (syntaxNode is ClassDeclarationSyntax cls)
            return cls.Identifier.GetLocation();

        return syntaxNode!.GetLocation();
    }

    /// <summary>
    /// Tries to retrieve the declaring <see cref="SyntaxNode"/> for the supplied symbol when one exists in source.
    /// </summary>
    /// <param name="symbol">Symbol whose declaration should be located.</param>
    /// <param name="syntaxNode">Receives the declaring syntax node, or null when no source declaration exists.</param>
    /// <returns>True when a source declaration was found.</returns>
    public static bool TryGetRecordDeclaration(this ISymbol symbol, out SyntaxNode? syntaxNode)
    {
        SyntaxReference? reference = symbol.DeclaringSyntaxReferences.FirstOrDefault();
        syntaxNode = reference?.GetSyntax();

        return syntaxNode is not null;
    }

    /// <summary>
    /// Returns the immediate containing namespace of the supplied symbol.
    /// </summary>
    /// <param name="symbol">Symbol whose namespace is being read.</param>
    /// <returns>The name of the containing namespace, or an empty string when none exists.</returns>
    public static string GetNamespace(this ISymbol symbol) => symbol?.ContainingNamespace?.Name.EmptyIfNull()!;

    /// <summary>
    /// Returns the fully qualified name of the supplied symbol, including its containing namespace.
    /// </summary>
    /// <param name="symbol">Symbol whose full name is being generated.</param>
    /// <returns>The fully qualified name of the symbol.</returns>
    public static string GetSymbolFullName(this ISymbol symbol)
    {
        if (symbol is null)
            return string.Empty;

        if (symbol is ITypeSymbol typeSymbol)
            return typeSymbol.GetTypeSymbolFullName();

        string containingNamespace = symbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).EmptyIfNull()!;
        containingNamespace = containingNamespace.RemoveGlobalAlias();

        string fullyQualifiedName = string.Empty;
        string joiningChar = Finding.Constants.NamespaceNameJoiningCharacter;
        for (INamedTypeSymbol currentType = symbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
            fullyQualifiedName = $"{currentType.Name}{joiningChar}{fullyQualifiedName}";

        fullyQualifiedName = $"{fullyQualifiedName}{symbol.Name}";

        return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
    }

    /// <summary>
    /// Returns whether the supplied symbol declares any of the provided attribute types.
    /// </summary>
    /// <param name="symbol">Symbol whose attributes are being inspected.</param>
    /// <param name="searchScope">Scope of the search across the type hierarchy.</param>
    /// <param name="attributeTypes">Attribute types to look for.</param>
    /// <param name="data">Receives the matching attribute data instances.</param>
    /// <returns>True when at least one attribute match was found.</returns>
    public static bool HasAnyAttribute(this ISymbol symbol, SearchScope searchScope, List<Type> attributeTypes, out List<AttributeData> data)
    {
        if (attributeTypes is null)
        {
            data = null;
            return false;
        }
        List<string> fullNames = [];
        foreach (Type type in attributeTypes)
        {
            string? typeFullName = type?.FullName;
            if (typeFullName is not null)
                fullNames.Add(typeFullName);
        }

        return symbol.HasAnyAttribute(searchScope, fullNames, out data);
    }

    /// <summary>
    /// Returns whether the supplied symbol declares the specified attribute.
    /// </summary>
    /// <param name="symbol">Symbol whose attributes are being inspected.</param>
    /// <param name="searchScope">Scope of the search across the type hierarchy.</param>
    /// <param name="attributeType">Attribute type to look for.</param>
    /// <param name="data">Receives the matching attribute data when one is found.</param>
    /// <returns>True when a matching attribute was found.</returns>
    public static bool HasAttribute(this ISymbol symbol, SearchScope searchScope, Type attributeType, out AttributeData data)
    {
        if (attributeType is null)
        {
            data = null;
            return false;
        }

        string? typeFullName = attributeType.FullName;
            
        return symbol.HasAttribute(searchScope, typeFullName, out data);
    }

    /// <summary>
    /// Returns whether the supplied symbol declares an attribute with the specified fully qualified name, and outputs the matching data when found.
    /// </summary>
    /// <param name="symbol">Symbol whose attributes are being inspected.</param>
    /// <param name="searchScope">Scope of the search across the type hierarchy.</param>
    /// <param name="attributeFullName">Fully qualified attribute name to look for.</param>
    /// <param name="data">Receives the matching attribute data when one is found.</param>
    /// <returns>True when a matching attribute was found.</returns>
    public static bool HasAttribute(this ISymbol symbol, SearchScope searchScope, string? attributeFullName, out AttributeData data)
    {
        if (attributeFullName is null)
        {
            data = null;
            return false;
        }

        if (symbol is not null)
        {
            foreach (AttributeData item in symbol.GetAttributes())
            {
                INamedTypeSymbol? typeSymbol = item.AttributeClass;

                while (typeSymbol is not null)
                {
                    if (typeSymbol.GetSymbolFullName() == attributeFullName)
                    {
                        data = item;
                        return true;
                    }

                    typeSymbol = searchScope == SearchScope.Hierarchy ? typeSymbol.BaseType : null;
                }
            }
        }

        data = null;

        return false;
    }

    /// <summary>
    /// Returns whether the supplied symbol declares any of the attributes named in the list, and outputs the matching data.
    /// </summary>
    /// <param name="symbol">Symbol whose attributes are being inspected.</param>
    /// <param name="searchScope">Scope of the search across the type hierarchy.</param>
    /// <param name="attributeFullNames">Fully qualified attribute names to look for.</param>
    /// <param name="data">Receives the matching attribute data instances.</param>
    /// <returns>True when at least one matching attribute was found.</returns>
    public static bool HasAnyAttribute(this ISymbol symbol, SearchScope searchScope, List<string> attributeFullNames, out List<AttributeData> data)
    {
        data = new();

        if (symbol is null)
            return false;

        foreach (string fullName in attributeFullNames)
        {
            if (symbol.HasAttribute(searchScope, fullName, out AttributeData lData))
                data.Add(lData);
        }

        return data.Count > 0;
    }

    /// <summary>
    /// Returns whether the supplied symbol inherits from the specified base class anywhere along its hierarchy.
    /// </summary>
    /// <param name="symbol">Symbol whose ancestry is being inspected.</param>
    /// <param name="classFullName">Fully qualified name of the base class to look for.</param>
    /// <returns>True when the symbol inherits from the specified base class.</returns>
    public static bool InheritsClass(this ISymbol symbol, string classFullName)
    {
        if (symbol is INamedTypeSymbol namedTypeSymbol)
            return namedTypeSymbol.InheritsClass(classFullName);

        return false;
    }

    /// <summary>
    /// Tries to retrieve the <see cref="INamedTypeSymbol"/> for the type of the supplied field or property symbol.
    /// </summary>
    /// <param name="symbol">Field or property symbol whose type is being read.</param>
    /// <param name="namedTypeSymbol">Receives the resolved named type, or null when one cannot be resolved.</param>
    /// <returns>True when a named type was resolved.</returns>
    public static bool TryGetMemberTypeINamedTypeSymbol(this ISymbol symbol, out INamedTypeSymbol? namedTypeSymbol)
    {
        namedTypeSymbol = null;

        if (symbol is IFieldSymbol fieldSymbol)
        {
            if (fieldSymbol.Type is not INamedTypeSymbol lNamedTypeSymbol)
                return false;

            namedTypeSymbol = lNamedTypeSymbol;
        }
        else if (symbol is IPropertySymbol propertySymbol)
        {
            if (propertySymbol.Type is not INamedTypeSymbol lNamedTypeSymbol)
                return false;

            namedTypeSymbol = lNamedTypeSymbol;
        }

        return namedTypeSymbol is not null;
    }
    

}