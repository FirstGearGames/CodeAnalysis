using System;
using FirstGearGames.FishNet.CodeAnalysis.Constants;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FishNetTypes.Managing.Logging;
using FishNetTypes.Object;
using FishNetTypes.Transporting;

namespace FirstGearGames.FishNet.CodeAnalysis.RemoteProcedureCalls
{
    /// <summary>
    /// Additional options for the RPC.
    /// </summary>
    /// <remarks>This may include sender connection, channel, and more.</remarks>
    public enum RPCOption
    {
        /// <summary>
        /// No options.
        /// </summary>
        Unset = 0,
        /// <summary>
        /// Sender connection parameter.
        /// </summary>
        /// <remarks>This is used by ServerRpc.</remarks>
        SenderConnection = (1 << 0),
        /// <summary>
        /// Runtime channel.
        /// </summary>
        /// <remarks>This is used by all RPCs.</remarks>
        Channel = (1 << 1),
    }

    public enum RPCType
    {
        Unset,
        Target,
        Server,
        Observers
    }

    public struct RpcAttributeData
    {
        public AttributeData AttributeData;
        public RPCType RPCType;

        public RpcAttributeData(AttributeData attributeData, RPCType rpcType)
        {
            AttributeData = attributeData;
            RPCType = rpcType;
        }
    }

    public static class RpcHelper
    {
        // public static RPCOption GetRpcOptions(IMethodSymbol methodSymbol, RPCType rpcType)
        // {
        //     RPCOption options = RPCOption.Unset;
        //
        //     List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
        //     int parametersCount = parameters.Count;
        //
        //     const bool metadataName = false;
        //
        //     //ServerRpc.
        //     if (rpcType == RPCType.Server)
        //         AddSenderNetworkConnection();
        //
        //     AddChannel();
        //     //All types support optional channel.
        //
        //     //Adds sender connection if optional and the last parameter.
        //     void AddSenderNetworkConnection()
        //     {
        //         if (parametersCount == 0) return;
        //         if (!parameters[parametersCount - 1].IsOptional) return;
        //
        //         if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.NetworkConnection_FullName)
        //         {
        //             parameters.RemoveAt(--parametersCount);
        //             options |= RPCOption.SenderConnection;
        //         }
        //     }
        //
        //     //Adds channel if the last parameter.
        //     void AddChannel()
        //     {
        //         if (parametersCount == 0) return;
        //
        //         if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.Channel_FullName)
        //             parameters.RemoveAt(--parametersCount);
        //     }
        //
        //     return options;
        // }

        public static string GetDefaultChannelValue(IMethodSymbol methodSymbol, RPCType rpcType)
        {
            List<IParameterSymbol> parameters = methodSymbol.Parameters.ToList();
            int parametersCount = parameters.Count;

            const bool metadataName = false;

            //ServerRpc. Remove connection if it exist so that channel can be grabbed from the end.
            if (rpcType == RPCType.Server)
                RemoveTrailingNetworkConnection();

            //Removes networkConnection if the last parameter.
            void RemoveTrailingNetworkConnection()
            {
                if (parametersCount == 0) return;
                //Remove channel from serializable.
                if (parameters[parametersCount - 1].Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.NetworkConnection_FullName)
                    parameters.RemoveAt(--parametersCount);
            }

            IParameterSymbol? lastParameter = (parametersCount > 0) ? parameters[parametersCount - 1] : null;
            if (lastParameter?.Type.GetTypeSymbolFullName(metadataName) == FishNetConstants.Channel_FullName) 
            {
                //Not optional, default is reliable.
                if (!lastParameter.IsOptional) return FishNetConstants.Default_Rpc_Channel.GetEnumName();
                
                //Find optional value.
                object? value = lastParameter.ExplicitDefaultValue;
                //Should never be null in this case; check for safety.
                if (value?.GetType() != Enum.GetUnderlyingType(typeof(Channel))) return FishNetConstants.Default_Rpc_Channel.GetEnumName();

                return ChannelExtensions.GetEnumName(((byte)value));
            }

            //Fall through, no parameters or last is not channel.
            return FishNetConstants.Default_Rpc_Channel.GetEnumName();
        }

        
        public static RPCType GetRpcType(this string attributeFullName)
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
        public static bool HasRpcAttributes(this IMethodSymbol methodSymbol, out List<RpcAttributeData> results)
        {
            const bool isMetadataName = false;
            results = new List<RpcAttributeData>();
            
            /* Do not use else ifs. A method can have multiple RPC attributes
             * so we need to check for them all. */
            
            if (methodSymbol.HasAttribute(FishNetConstants.TargetRpcAttribute_FullName, isMetadataName, out AttributeData a0))
                results.Add(new RpcAttributeData(a0, RPCType.Target));
            
            if (methodSymbol.HasAttribute(FishNetConstants.ServerRpcAttribute_FullName, isMetadataName, out AttributeData a1))
                results.Add(new RpcAttributeData(a1, RPCType.Server));

            if (methodSymbol.HasAttribute(FishNetConstants.ObserversRpcAttribute_FullName, isMetadataName, out AttributeData a2))
                results.Add(new RpcAttributeData(a2, RPCType.Observers));

            return (results.Count > 0);
        }

    }
}