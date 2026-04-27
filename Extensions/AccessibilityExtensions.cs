
using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for emitting source-text representations of <see cref="Accessibility"/> values.
/// </summary>
public static class AccessibilityExtensions
{
    /// <summary>
    /// Generates the source-text keyword sequence for the supplied accessibility.
    /// </summary>
    /// <param name="accessibility">Accessibility value to convert.</param>
    /// <returns>The C# accessibility keyword corresponding to the supplied value.</returns>
    public static string GenerateAccessibility(this Accessibility accessibility)
    {
        return accessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.NotApplicable => "private",
            _ => "public"
        };
    }

}