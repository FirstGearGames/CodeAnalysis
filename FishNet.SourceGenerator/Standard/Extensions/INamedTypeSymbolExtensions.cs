#pragma warning disable CS8602 // Dereference of a possibly null reference.
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using FishNet.SourceGenerating.Helpers;

namespace SourceGenerating.Extensions
{
    internal static class INamedTypeSymbolExtensions
    {
        /// <summary>
        /// Returns the short name of a symbol which includes the namespace.
        /// </summary>
        public static bool ImplementsInterface(this INamedTypeSymbol symbol, string interfaceFullName)
        {
            foreach (INamedTypeSymbol? interfaceNamed in symbol.Interfaces)
            {
                if (interfaceNamed == null) continue;

                if (interfaceNamed.GetSymbolFullName() == interfaceFullName)
                    return true;
            }

            return false;
        }
    }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
}