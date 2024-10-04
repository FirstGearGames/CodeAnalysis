#pragma warning disable CS8602 // Dereference of a possibly null reference.
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
                if (interfaceNamed.GetSymbolFullName(metadataName: false) == interfaceFullName)
                    return true;
            }

            return false;
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}