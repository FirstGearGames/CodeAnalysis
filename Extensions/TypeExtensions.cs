using System;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for inspecting <see cref="Type"/> instances during code generation.
/// </summary>
public static class TypeExtensions
{

    /// <summary>
    /// Returns the full name of the supplied type with any generic arity suffix removed.
    /// </summary>
    /// <remarks>
    /// The returned string does not include the global alias.
    /// </remarks>
    /// <param name="type">Type whose full name is being read.</param>
    /// <returns>The full name without generic arguments, or null when the type has no full name.</returns>
    public static string? GetFullNameWithoutGenerics(this Type type)
    {
        string? fullName = type.FullName;
        
        if (fullName is null)
            return null;
        
        int genericMarkerIndex = fullName.IndexOf("`", StringComparison.InvariantCultureIgnoreCase);
        if (genericMarkerIndex >= 0)
            return fullName.Substring(0, genericMarkerIndex);
            
        return fullName;
    }
        
        
    /// <summary>
    /// Returns the short name of the supplied type with any generic arity suffix removed.
    /// </summary>
    /// <remarks>
    /// The returned string does not include the global alias.
    /// </remarks>
    /// <param name="type">Type whose name is being read.</param>
    /// <returns>The short name without generic arguments.</returns>
    public static string GetNameWithoutGenerics(this Type type)
    {
        string name = type.Name;

        int genericMarkerIndex = name.IndexOf("`", StringComparison.InvariantCultureIgnoreCase);
        if (genericMarkerIndex >= 0)
            return name.Substring(0, genericMarkerIndex);
            
        return name;
    }
}