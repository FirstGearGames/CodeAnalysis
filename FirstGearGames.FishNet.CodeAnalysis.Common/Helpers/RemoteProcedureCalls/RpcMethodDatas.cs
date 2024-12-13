using System.Collections.Generic;
using System.Text;
using FirstGearGames.CodeAnalysis.Extensions;
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
        /// <summary>
        /// Content of the method.
        /// </summary>
        public RpcMethodContent MethodContent;

        public RpcMethodDatas() { }

        public RpcMethodDatas(IMethodSymbol methodSymbol, List<IParameterSymbol> serializableParameters)
        {
            MethodSymbol = methodSymbol;
            MethodName = methodSymbol.Name;
            SerializableParameters = serializableParameters;
        }
    }
}