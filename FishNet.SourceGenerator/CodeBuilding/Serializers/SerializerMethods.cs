using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Helpers;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding
{
    public static class SerializerMethodExtensions
    {
        public static bool IsValid(this SerializerMethod? sm)
        {
            if (sm == null) return false;
            return (sm.TypeFullName != string.Empty);
        }
    }

    public class MethodContent
    {
        public StringBuilder Header;
        public StringBuilder Body;

        public MethodContent()
        {
            Header = new();
            Body = new();
        }

        public MethodContent(StringBuilder header)
        {
            Header = header;
            Body = new();
        }

        public MethodContent(string header)
        {
            Header = new(header);
            Body = new();
        }

        public MethodContent(StringBuilder header, StringBuilder body)
        {
            Header = header;
            Body = body;
        }

        public MethodContent(string header, string body)
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

    public class SerializerMethod
    {
        /// <summary>
        /// Type the serializer is for.
        /// </summary>
        public INamedTypeSymbol? NamedTypeSymbol;
        /// <summary>
        /// Full name of the type the serializer is for.
        /// </summary>
        public readonly string TypeFullName;
        /// <summary>
        /// Name of the generated method.
        /// </summary>
        public readonly string MethodName;
        /// <summary>
        /// Number of generic arguments for the method.
        /// </summary>
        public List<string> GenericArguments;
        /// <summary>
        /// Content of the method.
        /// </summary>
        public MethodContent MethodContent;

        public SerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName)
        {
            NamedTypeSymbol = namedTypeSymbol;
            TypeFullName = namedTypeSymbol.GetTypeSymbolFullName();
            GenericArguments = namedTypeSymbol.GetGenericArgumentsString();
            MethodName = methodName;
            MethodContent = new();
        }

        public SerializerMethod(string typeFullName, string methodName, List<string> genericArguments)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            if (genericArguments == null)
                genericArguments = new();
            GenericArguments = genericArguments;
            MethodContent = new();
        }

        public SerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody)
        {
            NamedTypeSymbol = namedTypeSymbol;
            TypeFullName = namedTypeSymbol.GetTypeSymbolFullName();
            GenericArguments = namedTypeSymbol.GetGenericArgumentsString();
            MethodName = methodName;
            MethodContent = new(methodSignature, methodBody);
        }

        public SerializerMethod(string typeFullName, string methodName, List<string> genericArguments, string methodSignature, string methodBody)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            if (genericArguments == null)
                genericArguments = new();
            GenericArguments = genericArguments;
            MethodContent = new(methodSignature, methodBody);
        }

        public SerializerMethod(string typeFullName, string methodName, List<string> genericArguments,  MethodContent methodContent)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            if (genericArguments == null)
                genericArguments = new();
            MethodContent = methodContent;
        }

        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public virtual bool IsGenerated() => false;

        /// <summary>
        /// True if a delta serializer.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsDeltaSerializer() => false;
    }

    public class DeltaSerializerMethod : SerializerMethod
    {
        public DeltaSerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody) : base(namedTypeSymbol, methodName, methodSignature, methodBody) { }
        public DeltaSerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName) : base(namedTypeSymbol, methodName) { }
        public DeltaSerializerMethod(string typeFullName, string methodName, List<string> genericArguments) : base(typeFullName, methodName, genericArguments) { }
        public DeltaSerializerMethod(string typeFullName, string methodName, List<string> genericArguments, string methodSignature, string methodBody) : base(typeFullName, methodName, genericArguments, methodSignature, methodBody) { }

        public override bool IsDeltaSerializer() => true;
    }

    public class GeneratedDeltaSerializerMethod : DeltaSerializerMethod
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedDeltaSerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody) : base(namedTypeSymbol, methodName, methodSignature, methodBody) { }
    }

    public class GeneratedSerializerMethod : SerializerMethod
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedSerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody) : base(namedTypeSymbol, methodName, methodSignature, methodBody) { }
    }
}