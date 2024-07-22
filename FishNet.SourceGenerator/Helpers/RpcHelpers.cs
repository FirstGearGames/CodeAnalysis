using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using FishNet.SourceGenerating.Constants;
using SourceGenerating.Extensions;

namespace FishNet.SourceGenerating.Helpers
{

    internal enum RPCType
    {
        Unset,
        Target,
        Server,
        Observers
    }

    internal struct RpcAttributeData
    {
        public AttributeData AttributeData;
        public RPCType RPCType;

        public RpcAttributeData(AttributeData attributeData, RPCType rPCType)
        {
            AttributeData = attributeData;
            RPCType = rPCType;
        }
    }

    internal static class RpcHelpers
    {
        public static RPCType GetRpcType(string attributeFullName)
        {
            if (attributeFullName == FishNetConstants.TargetRpcAttribute_FullName)
                return RPCType.Target;
            else if (attributeFullName == FishNetConstants.ServerRpcAttribute_FullName)
                return RPCType.Server;
            else if (attributeFullName == FishNetConstants.ObserversRpcAttribute_FullName)
                return RPCType.Observers;
            else
                return RPCType.Unset;
        }

        /// <summary>
        /// Returns if a symbol has any RPC attributes, and outputs results.
        /// </summary>
        public static bool HasRpcAttributes(this ISymbol symbol, out List<RpcAttributeData> results)
        {
            results = new List<RpcAttributeData>();
            if (symbol.HasAttribute(FishNetConstants.TargetRpcAttribute_FullName, out AttributeData a0))
                results.Add(new RpcAttributeData(a0, RPCType.Target));
            if (symbol.HasAttribute(FishNetConstants.ServerRpcAttribute_FullName, out AttributeData a1))
                results.Add(new RpcAttributeData(a1, RPCType.Server));
            if (symbol.HasAttribute(FishNetConstants.ObserversRpcAttribute_FullName, out AttributeData a2))
                results.Add(new RpcAttributeData(a2, RPCType.Observers));

            return (results.Count > 0);
        }

    }
}