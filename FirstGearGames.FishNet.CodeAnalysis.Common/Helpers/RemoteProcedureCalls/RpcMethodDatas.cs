using System.Collections.Generic;
using System.Text;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.FishNet.CodeAnalysis.Constants;
using FirstGearGames.FishNet.CodeAnalysis.RemoteProcedureCalls;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.FishNet.CodeAnalysis.Helpers.RemoteProcedureCalls
{
    public static class RpcMethodDataExtensions
    {
        public static bool IsValid(this RpcMethodDatas? md)
        {
            if (md == null) return false;
            return !string.IsNullOrWhiteSpace(md.MethodName);
        }
    }
    //
    // #region Attribute Datas.
    // public class RpcAttributeData
    // {
    //     /// <summary>
    //     /// Logging level when RPC cannot be sent over the network.
    //     /// </summary>
    //     public byte Logging = FishNetConstants.LoggingType_Warning_NumericValue;
    //     /// <summary>
    //     /// True to also run the RPC logic locally.
    //     /// </summary>
    //     public bool RunLocally = false;
    //     /// <summary>
    //     /// Estimated length of data being sent.
    //     /// When a value other than -1 the minimum length of the used serializer will be this value.
    //     /// This is useful for writing large packets which otherwise resize the serializer.
    //     /// </summary>
    //     public int DataLength = -1;
    //     /// <summary>
    //     /// Order in which to send data for this RPC.
    //     /// </summary>
    //     public byte OrderType = 0;
    // }
    //
    // public class ServerRpcAttributeData : RpcAttributeData
    // {
    //     /// <summary>
    //     /// True to only allow the owning client to call this RPC.
    //     /// </summary>
    //     public bool RequireOwnership = true;
    // }
    //
    // public class ObserverRpcAttributeData : RpcAttributeData
    // {
    //     /// <summary>
    //     /// True to exclude the owner from receiving this RPC.
    //     /// </summary>
    //     public bool ExcludeOwner = false;
    //     /// <summary>
    //     /// True to prevent the connection from receiving this Rpc if they are also server.
    //     /// </summary>
    //     public bool ExcludeServer = false;
    //     /// <summary>
    //     /// True to buffer the last value and send it to new players when the object is spawned for them.
    //     /// RPC will be sent on the same channel as the original RPC, and immediately before the OnSpawnServer override.
    //     /// </summary>
    //     public bool BufferLast = false;
    // }
    //
    // public class TargetRpcAttributeData : RpcAttributeData
    // {
    //     /// <summary>
    //     /// True to prevent the connection from receiving this Rpc if they are also server.
    //     /// </summary>
    //     public bool ExcludeServer = false;
    //     /// <summary>
    //     /// True to validate the target is possible and output debug when not.
    //     /// Use this field with caution as it may create undesired results when set to false.
    //     /// </summary>
    //     public bool ValidateTarget = true;
    // }
    // #endregion

    public class RpcMethodContent
    {
        public StringBuilder Header;
        public StringBuilder Body;

        public RpcMethodContent()
        {
            Header = new();
            Body = new();
        }

        public RpcMethodContent(StringBuilder header)
        {
            Header = header;
            Body = new();
        }

        public RpcMethodContent(string header, string body)
        {
            Header = new(header);
            Body = new(body);
        }

        public string ToString(int bracketIndent)
        {
            StringBuilder sb = new();
            sb.Append(ToStringWithoutFooter(bracketIndent));
            sb.AppendLine(bracketIndent, "}");
            return sb.ToString();
        }

        public string ToStringWithoutFooter(int bracketIndent)
        {
            StringBuilder sb = new();
            sb.AppendLine(Header.ToString());
            sb.AppendLine(bracketIndent, "{");
            sb.AppendLine(Body.ToString());
            return sb.ToString();
        }
    }

    public class RpcMethodDatas
    {
        /// <summary>
        /// Data about the attribute for this Rpc.
        /// </summary>
        public readonly RpcAttributeData RpcAttributeData;
        /// <summary>
        /// Type the serializer is for.
        /// </summary>
        public readonly IMethodSymbol MethodSymbol;
        /// <summary>
        /// Name of the generated method.
        /// </summary>
        public readonly string MethodName;
        /// <summary>
        /// Default value for the channel parameter.
        /// </summary>
        public string DefaultChannelValue;
        /// <summary>
        /// Types needed to be serialized within the RPC.
        /// </summary>
        public List<IParameterSymbol> SerializableParameters;
        /// <summary>
        /// Content of the method.
        /// </summary>
        public RpcMethodContent MethodContent;

        /// <param name="rpcAttributeData">RpcAttributeData to use for this RpcMethod. When a Rpc has multiple attributes a RpcMethodData should be made for each attribute.</param>
        public RpcMethodDatas(IMethodSymbol methodSymbol, string defaultChannelValue, List<IParameterSymbol> serializableParameters, RpcAttributeData rpcAttributeData)
        {
            RpcAttributeData = rpcAttributeData;
            MethodSymbol = methodSymbol;
            DefaultChannelValue = defaultChannelValue;
            MethodName = methodSymbol.Name;
            SerializableParameters = serializableParameters;
        }
    }
}