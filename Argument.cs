using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeAnalysis;

/// <summary>
/// Represents a single argument supplied to a method invocation.
/// </summary>
public readonly struct Argument
{
    /// <summary>
    /// The type symbol of the argument.
    /// </summary>
    public readonly ITypeSymbol TypeSymbol;
    /// <summary>
    /// The name of the argument as written in source.
    /// </summary>
    public readonly string Name;
    /// <summary>
    /// True when the argument is supplied as a named argument.
    /// </summary>
    public readonly bool IsNamed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Argument"/> struct.
    /// </summary>
    /// <param name="typeSymbol">Type symbol of the argument.</param>
    /// <param name="name">Name of the argument.</param>
    /// <param name="isNamed">True when the argument is supplied as a named argument.</param>
    public Argument(ITypeSymbol typeSymbol, string name, bool isNamed)
    {
        TypeSymbol = typeSymbol;
        Name = name;
        IsNamed = isNamed;
    }
}

/// <summary>
/// Extension methods for working with collections of <see cref="Argument"/>.
/// </summary>
public static class ArgumentExtensions
{
    /// <summary>
    /// Returns whether the provided arguments are named.
    /// </summary>
    /// <returns>True if there are no arguments present, or if all arguments are named.</returns>
    public static bool AreArgumentsEmptyOrNamed(this List<Argument> methodArguments)
    {
        if (methodArguments is null || methodArguments.Count == 0)
            return true;

        foreach (Argument argument in methodArguments)
        {
            if (!argument.IsNamed)
                return false;
        }

        return true;
    }
}