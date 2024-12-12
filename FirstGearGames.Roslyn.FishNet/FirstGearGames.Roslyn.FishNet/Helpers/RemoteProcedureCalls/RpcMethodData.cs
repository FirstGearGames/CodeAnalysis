using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.FishNet.Helpers.RemoteProcedureCalls
{
    public static class RpcMethodDataExtensions
    {
        public static bool IsValid(this RpcMethodData? md)
        {
            if (md == null) return false;
            return !string.IsNullOrWhiteSpace(md.MethodName);
        }
    }

    public class RpcMethodData
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
        public List<IParameterSymbol> SerializableParameters;

        public RpcMethodData() { }

        public RpcMethodData(IMethodSymbol methodSymbol, List<IParameterSymbol> serializableParameters)
        {
            MethodSymbol = methodSymbol;
            MethodName = methodSymbol.Name;
            SerializableParameters = serializableParameters;
        }
    }
}