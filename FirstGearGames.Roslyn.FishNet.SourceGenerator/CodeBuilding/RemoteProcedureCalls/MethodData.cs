using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.CodeBuilding.Serializers;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers.Serializing;
using FirstGearGames.Roslyn.FishNet.Receivers;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.CodeBuilding.RemoteProcedureCalls
{
    public static class MethodDataExtensions
    {
        public static bool IsValid(this MethodData? md)
        {
            if (md == null) return false;
            return !string.IsNullOrWhiteSpace(md.MethodName);
        }
    }

    public class MethodData
    {
        /// <summary>
        /// Type the serializer is for.
        /// </summary>
        public readonly IMethodSymbol MethodSymbol;
        /// <summary>
        /// Name of the generated method.
        /// </summary>
        public readonly string MethodName;
        /// <summary>
        /// Types needed to be serialized within the RPC.
        /// </summary>
        public List<SerializableType> SerializableTypes;

        public MethodData() { }

        public MethodData(IMethodSymbol methodSymbol, List<SerializableType> serializableTypes)
        {
            MethodSymbol = methodSymbol;
            MethodName = methodSymbol.Name;
            SerializableTypes = serializableTypes;
        }
    }
}