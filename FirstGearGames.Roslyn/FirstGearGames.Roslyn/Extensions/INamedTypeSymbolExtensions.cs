#pragma warning disable CS8602 // Dereference of a possibly null reference.
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.Extensions
{
    public static class INamedTypeSymbolExtensions
    {
        private static List<IMethodSymbol> _methodSymbols = new();
        
        /// <summary>
        /// Returns field members of a named symbol.
        /// </summary>
        public static List<IFieldSymbol> GetFieldMembers(this INamedTypeSymbol symbol) 
        {
            return symbol.GetMembers().OfType<IFieldSymbol>().ToList();
        }
        
        /// <summary>
        /// Returns the short name of a symbol which includes the namespace.
        /// </summary>
        public static bool ImplementsInterface(this INamedTypeSymbol symbol, string interfaceFullName)
        {
            foreach (INamedTypeSymbol interfaceNamed in symbol.Interfaces)
            {
                if (interfaceNamed.GetTypeSymbolFullName(metadataName: false) == interfaceFullName)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// True if symbol inherits base class anywhere along hierarchy.
        /// </summary>
        public static bool InheritsClass(this INamedTypeSymbol symbol, string classFullName)
        {
            while (symbol.BaseType != null && symbol.BaseType is INamedTypeSymbol baseSymbol)
            {
                if (baseSymbol.GetTypeSymbolFullName(metadataName: false) == classFullName)
                    return true;

                symbol = symbol.BaseType;
            }

            return false;
        }

        /// <summary>
        /// Returns a method containing matching parameter names.
        /// </summary>
        public static IMethodSymbol? GetMethod(this INamedTypeSymbol symbol, string methodName, bool metadataName, params string[] parameterNames) 
        {
            IEnumerable<IMethodSymbol> methodSymbols = symbol.GetMembers(methodName).OfType<IMethodSymbol>();

            foreach (IMethodSymbol methodSymbol in methodSymbols)
            {
                if (methodSymbol.AreParametersMatching(metadataName, parameterNames))
                    return methodSymbol;
            }

            return null;
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}