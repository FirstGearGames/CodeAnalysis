using Microsoft.CodeAnalysis;
using FishNet.SourceGenerating.Helpers;

namespace SourceGenerating.Extensions
{
    public static class IAssemblySymbolExtensions
    {
        public static bool GetINamedTypeSymbol(this IAssemblySymbol? assemblySymbol, string fullName, out INamedTypeSymbol? result, bool error = true)
        {
            result = null;
            if (assemblySymbol == null) return false;

            if (assemblySymbol.GetTypeByMetadataName(fullName) is INamedTypeSymbol foundSymbol)
            {
                result = foundSymbol;
                return true;
            }
            else
            {
                if (error)
                    Debugg.Log($"Could not find {fullName} in {assemblySymbol.GetSymbolFullName()}");
                return false;
            }
        }
    }
}