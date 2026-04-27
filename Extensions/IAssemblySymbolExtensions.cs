#nullable enable
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for traversing the symbol hierarchy of an <see cref="IAssemblySymbol"/>.
/// </summary>
public static class AssemblySymbolExtensions
{
    /// <summary>
    /// Recursively returns every <see cref="INamespaceSymbol"/> contained within the assembly.
    /// </summary>
    /// <param name="assemblySymbol">Assembly symbol to traverse.</param>
    /// <returns>A list containing every namespace declared in the assembly.</returns>
    public static List<INamespaceSymbol> RecursivelyGetNamespaceSymbols(this IAssemblySymbol assemblySymbol)
    {
        if (assemblySymbol is null)
            return new();

        List<INamespaceSymbol> allNamespaces = new();

        // Get the global namespace and add it to begin iteration.
        INamespaceSymbol globalNamespace = assemblySymbol.GlobalNamespace;
        allNamespaces.Add(globalNamespace);

        for (int i = 0; i < allNamespaces.Count; i++)
        {
            INamespaceSymbol current = allNamespaces[i];
            allNamespaces.AddRange(current.GetNamespaceMembers());
        }

        return allNamespaces;
    }

    /// <summary>
    /// Recursively returns every <see cref="INamedTypeSymbol"/> contained within the assembly.
    /// </summary>
    /// <param name="assemblySymbol">Assembly symbol to traverse.</param>
    /// <returns>A list containing every named type declared in the assembly.</returns>
    public static List<INamedTypeSymbol> RecursivelyGetNamedTypeSymbols(this IAssemblySymbol assemblySymbol)
    {
        List<INamespaceSymbol> allNamespaces = assemblySymbol.RecursivelyGetNamespaceSymbols();

        List<INamedTypeSymbol> namedTypeSymbols = new();
        foreach (INamespaceSymbol namespaceSymbol in allNamespaces)
            namedTypeSymbols.AddRange(namespaceSymbol.GetTypeMembers());

        // Now recursively iterate namedTypeSymbols.
        for (int i = 0; i < namedTypeSymbols.Count; i++)
            namedTypeSymbols.AddRange(namedTypeSymbols[i].GetTypeMembers());

        return namedTypeSymbols;
    }

    /// <summary>
    /// Recursively returns every <see cref="IMethodSymbol"/> contained within the assembly.
    /// </summary>
    /// <param name="assemblySymbol">Assembly symbol to traverse.</param>
    /// <param name="requiredAccessibility">When supplied, restricts results to methods with the specified accessibility.</param>
    /// <returns>A list containing every method declared in the assembly that satisfies the supplied accessibility filter.</returns>
    public static List<IMethodSymbol> RecursivelyGetMethodSymbols(this IAssemblySymbol assemblySymbol, Accessibility? requiredAccessibility = null)
    {
        List<INamedTypeSymbol> namedTypeSymbols = assemblySymbol.RecursivelyGetNamedTypeSymbols();
        
        List<IMethodSymbol> methodSymbols = [];
        
        foreach (INamedTypeSymbol namedTypeSymbol in namedTypeSymbols)
            methodSymbols.AddRange(namedTypeSymbol.GetMethodSymbols(requiredAccessibility));
        
        return methodSymbols;
    }
}