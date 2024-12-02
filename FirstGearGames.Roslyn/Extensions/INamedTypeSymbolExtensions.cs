#pragma warning disable CS8602 // Dereference of a possibly null reference.
using System.Diagnostics;
using FirstGearGames.Roslyn.FishNet.Helpers;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.Extensions
{
    public static class INamedTypeSymbolExtensions
    {
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
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}