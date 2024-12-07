using System;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.SyncTypes;

namespace FirstGearGames.Roslyn.FishNet.RemoteProcedureCalls
{
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

        public RpcAttributeData(AttributeData attributeData, RPCType rPCType)
        {
            AttributeData = attributeData;
            RPCType = rPCType;
        }
    }

    public static class RpcHelper
    {
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
        public static bool HasRpcAttributes(this ISymbol symbol, out List<RpcAttributeData> results)
        {
            const bool isMetadataName = false;
            results = new List<RpcAttributeData>();
            if (symbol.HasAttribute(FishNetConstants.TargetRpcAttribute_FullName, isMetadataName, out AttributeData a0))
                results.Add(new RpcAttributeData(a0, RPCType.Target));
            if (symbol.HasAttribute(FishNetConstants.ServerRpcAttribute_FullName, isMetadataName, out AttributeData a1))
                results.Add(new RpcAttributeData(a1, RPCType.Server));
            if (symbol.HasAttribute(FishNetConstants.ObserversRpcAttribute_FullName, isMetadataName, out AttributeData a2))
                results.Add(new RpcAttributeData(a2, RPCType.Observers));

            return (results.Count > 0);
        }

        /// <summary>
        /// Gets the logging type specified on a RPC attribute.
        /// </summary>
        /// <param name="rpcAttributeData"></param>
        /// <returns></returns>
        public static int GetLoggingTypeNumericValue(this RpcAttributeData rpcAttributeData)
        {
            KeyValuePair<string, TypedConstant> loggingArgument = rpcAttributeData.AttributeData.NamedArguments.FirstOrDefault(arg => arg.Key == FishNetConstants.RpcAttribute_Logging_Name);

            //Argument found. Check if value matches.
            if (loggingArgument.Key != null && loggingArgument.Value.Value != null)
            {
                if (loggingArgument.Value.Value is byte loggingTypeValue)
                    return loggingTypeValue;
            }
            
            If here then we need to get the default value of the class.
            This can be done using the INamedTypeSymbol of the rpcAttribute class.
            // Get the field 'MyField' from the class symbol
            // var fieldSymbol = classSymbol.GetMembers("MyField").OfType<IFieldSymbol>().FirstOrDefault();
            //
            // if (fieldSymbol != null)
            // {
            //     // Check if the field has an initializer (default value)
            //     var initializer = fieldSymbol.Initializer?.Value;
            //
            //     if (initializer != null)
            //     {
            //         // Print the default value (the initializer value)
            //         Console.WriteLine("Default value of MyField: " + initializer.ConstantValue);
            //     }
            // }

            return -1;
        }

        /// <summary>
        /// Returns if Logging is set to off within a RPC attribute.
        /// </summary>
        public static bool IsLoggingOff(this RpcAttributeData rpcAttributeData) => rpcAttributeData.IsLoggingValue(FishNetConstants.LoggingType_Off_NumericValue);

        /// <summary>
        /// Returns if Logging is set to error within a RPC attribute.
        /// </summary>
        public static bool IsLoggingWarning(this RpcAttributeData rpcAttributeData) => rpcAttributeData.IsLoggingValue(FishNetConstants.LoggingType_Warning_NumericValue);

        /// <summary>
        /// Returns if Logging is set to error within a RPC attribute.
        /// </summary>
        public static bool IsLoggingError(this RpcAttributeData rpcAttributeData) => rpcAttributeData.IsLoggingValue(FishNetConstants.LoggingType_Error_NumericValue);

        /// <summary>
        /// Returns if Logging is set to common within a RPC attribute.
        /// </summary>
        public static bool IsLoggingCommon(this RpcAttributeData rpcAttributeData) => rpcAttributeData.IsLoggingValue(FishNetConstants.LoggingType_Common_NumericValue);

        /// <summary>
        /// Returns if Logging is a certain value.
        /// </summary>
        private static bool IsLoggingValue(this RpcAttributeData rpcAttributeData, int numericValue)
        {
            KeyValuePair<string, TypedConstant> loggingArgument = rpcAttributeData.AttributeData.NamedArguments.FirstOrDefault(arg => arg.Key == FishNetConstants.RpcAttribute_Logging_Name);

            //Argument found. Check if value matches.
            if (loggingArgument.Key != null && loggingArgument.Value.Value != null && loggingArgument.Value.Value is byte byteValue)
                return (byteValue == numericValue);

            return false;
        }
    }
}