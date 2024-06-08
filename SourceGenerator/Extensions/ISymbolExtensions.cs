#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SourceGenerator.Extensions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace FishNet.CodeAnalysis.Extensions
{
    internal static class ISymbolExtensions
    {
        /// <summary>
        /// Returns the full name of a symbol which includes the namespace.
        /// </summary>
        public static string GetNamespace(this ISymbol symbol)
        {
            return symbol?.ContainingNamespace?.Name ?? string.Empty;
        }

        /// <summary>
        /// Returns the full name of a symbol which includes the namespace.
        /// </summary>
        public static string GetSymbolFullName(this ISymbol symbol)
        {
            if (symbol == null) return string.Empty;

            string containingNamespace = symbol.ContainingNamespace?.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) ?? string.Empty;
            containingNamespace = containingNamespace.RemoveGlobalAlias();

            string fullyQualifiedName = string.Empty;
            for (INamedTypeSymbol? currentType = symbol.ContainingType; currentType is not null; currentType = currentType.ContainingType)
                fullyQualifiedName = $"{currentType.Name}.{fullyQualifiedName}";

            fullyQualifiedName = $"{fullyQualifiedName}{symbol.Name}";
            return string.IsNullOrWhiteSpace(containingNamespace) ? fullyQualifiedName : $"{containingNamespace}.{fullyQualifiedName}";
        }

        /// <summary>
        /// Returns the short name of a symbol which includes the namespace.
        /// </summary>
        public static string GetSymbolName(this ISymbol symbol)
        {
            if (symbol == null) return string.Empty;
            return symbol.Name;
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

                if (typeSymbol.GetSymbolFullName() == attributeFullName)
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
}