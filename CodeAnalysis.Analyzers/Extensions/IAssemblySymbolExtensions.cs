using FirstGearGames.FishNet.CodeAnalysis.Helpers;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.CodeAnalysis.Extensions
{
    public static class IAssemblySymbolExtensions
    {
        public static bool GetINamedTypeSymbol(this IAssemblySymbol assemblySymbol, string fullName, out INamedTypeSymbol result, bool error = true)
        {
            result = null;
            if (assemblySymbol is null)
                return false;

            if (assemblySymbol.GetTypeByMetadataName(fullName) is INamedTypeSymbol foundSymbol)
            {
                result = foundSymbol;
                return true;
            }
            else
            {
                if (error)
                    Log($"Could not find {fullName} in {assemblySymbol.GetSymbolFullName(metadataName: false)}");
                return false;
            }
        }

        private static void Log(string txt)
        {
            if (txt.Length == 0)
                Log(txt);
            else
                Log($"   [DeltaWriter_Builder] {txt}");
        }
    }
}