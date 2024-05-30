#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions;

internal static class ISymbolExtensions
{
    /// <summary>
    /// Returns the full name of a symbol which includes the namespace.
    /// </summary>
    public static string GetFullName(this ISymbol symbol)
    {
        if (symbol == null) return string.Empty;

        SymbolDisplayFormat symbolDisplayFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        if (symbol.ContainingType == null)
            return symbol.ToDisplayString(symbolDisplayFormat);
        else
            return $"{symbol.ContainingType.ToDisplayString(symbolDisplayFormat)}.{symbol.Name}";
    }

    /// <summary>
    /// Returns the short name of a symbol which includes the namespace.
    /// </summary>
    public static string GetName(this ISymbol symbol)
    {
        if (symbol == null) return string.Empty;
        return symbol.Name;
    }

    /// <summary>
    /// Returns if a symbol has an attribute, and outputs it if so.
    /// </summary>
    public static bool HasAttribute<T>(this ISymbol symbol, out AttributeData data)
    {
        return symbol.HasAttribute(typeof(T).FullName, out data);
    }
    /// <summary>
    /// Returns if a symbol has an attribute, and outputs it if so.
    /// </summary>
    public static bool HasAttributes<T1, T2>(this ISymbol symbol, out List<AttributeData> datas)
    {
        datas = new();
        AttributeData ad;

        if (symbol.HasAttribute<T1>(out ad))
            datas.Add(ad);
        if (symbol.HasAttribute<T2>(out ad))
            datas.Add(ad);

        return (datas.Count > 0);
    }
    /// <summary>
    /// Returns if a symbol has an attribute, and outputs it if so.
    /// </summary>
    public static bool HasAttributes<T1, T2, T3>(this ISymbol symbol, out List<AttributeData> datas)
    {
        datas = new();
        AttributeData ad;

        if (symbol.HasAttribute<T1>(out ad))
            datas.Add(ad);
        if (symbol.HasAttribute<T2>(out ad))
            datas.Add(ad);
        if (symbol.HasAttribute<T3>(out ad))
            datas.Add(ad);

        return (datas.Count > 0);
    }
    /// <summary>
    /// Returns if a symbol has an attribute, and outputs it if so.
    /// </summary>
    public static bool HasAttribute(this ISymbol symbol, string attributeFullName, out AttributeData data)
    {
        foreach (AttributeData item in symbol.GetAttributes())
        {
            INamedTypeSymbol? typeSymbol = item.AttributeClass;
            if (typeSymbol == null) continue;

            if (typeSymbol.GetFullName() == attributeFullName)
            {
                data = item;
                return true;
            }
        }

        //Fall through, not found.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
        data = default;
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
        return false;
    }

    /// <summary>
    /// Returns if a symbol has any of the supplied attributes, and outputs it if so.
    /// </summary>
    public static bool HasAttributes(this ISymbol symbol, string[] attributeFullNames, out List<AttributeData> datas)
    {
        datas = new List<AttributeData>();

        foreach (string fullName in attributeFullNames)
        {
            if (symbol.HasAttribute(fullName, out AttributeData? d))
                datas.Add(d!);
        }

        return (datas.Count > 0);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string GetFullyQualifiedName(this ISymbol thisSymbol)
    {
        return thisSymbol.ContainingType is null ? $"global::{thisSymbol.Name}" : $"{thisSymbol.ContainingType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}.{thisSymbol.Name}";
    }

    public static bool HasAttribute(this ISymbol thisSymbol, string fullyQualifiedAttributeName)
    {
        foreach (AttributeData attribute in thisSymbol.GetAttributes())
        {
            if (attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) return true;
        }

        return false;
    }

    public static bool HasAttribute(this ISymbol thisSymbol, params string[] fullyQualifiedAttributeNames)
    {
        foreach (string fullyQualifiedAttributeName in fullyQualifiedAttributeNames)
        {
            if (thisSymbol.HasAttribute(fullyQualifiedAttributeName)) return true;
        }

        return false;
    }

    public static bool HasAttributes(this ISymbol thisSymbol, string[] fullyQualifiedAttributeNames)
    {
        foreach (string fullyQualifiedAttributeName in fullyQualifiedAttributeNames)
        {
            if (!thisSymbol.HasAttribute(fullyQualifiedAttributeName)) return false;
        }

        return true;
    }

    public static AttributeData? GetAttribute(this ISymbol thisSymbol, string fullyQualifiedAttributeName)
    {
        foreach (AttributeData attribute in thisSymbol.GetAttributes())
        {
            if (attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) return attribute;
        }

        return null;
    }

    public static ImmutableArray<AttributeData> GetAttributes(this ISymbol thisSymbol, string fullyQualifiedAttributeName)
    {
        ImmutableArray<AttributeData>.Builder attributes = ImmutableArray.CreateBuilder<AttributeData>();

        foreach (AttributeData attribute in thisSymbol.GetAttributes())
        {
            if (attribute.AttributeClass.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == fullyQualifiedAttributeName) attributes.Add(attribute);
        }

        return attributes.ToImmutable();
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
