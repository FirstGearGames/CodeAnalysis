using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for inspecting <see cref="IPropertySymbol"/> instances.
/// </summary>
public static class PropertySymbolExtensions
{
    /// <summary>
    /// Returns whether both accessors of the property exist and are declared public.
    /// </summary>
    /// <param name="symbol">Property symbol whose accessors are being inspected.</param>
    /// <returns>True when both the getter and setter are declared public.</returns>
    public static bool AreAccessorsPublic(this IPropertySymbol symbol)
    {
        bool isGetterPublic = false;
        bool isSetterPublic = false;

        if (symbol.GetMethod != null)
            isGetterPublic = symbol.GetMethod.DeclaredAccessibility == Accessibility.Public;

        if (symbol.SetMethod != null)
            isSetterPublic = symbol.SetMethod.DeclaredAccessibility == Accessibility.Public;

        return isGetterPublic && isSetterPublic;
    }
}