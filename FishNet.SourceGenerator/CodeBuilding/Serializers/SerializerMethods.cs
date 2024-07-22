using Microsoft.CodeAnalysis;
using SourceGenerator.Extensions;
using System.Text;

namespace SourceGenerator.CodeBuilding.Serializers
{
    internal static class SerializerMethodExtensions
    {
        public static bool IsValid(this SerializerMethod? sm)
        {
            if (sm == null) return false;
            return (sm.TypeFullName != string.Empty);
        }
    }

    internal class MethodContent
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

    internal class SerializerMethod
    {
        /// <summary>
        /// Full name of the type the serializer is for.
        /// </summary>
        public readonly string TypeFullName;

        /// <summary>
        /// Name of the generated method.
        /// </summary>
        public readonly string MethodName;
        /// <summary>
        /// Content of the method.
        /// </summary>
        public MethodContent MethodContent;

        public SerializerMethod(string typeFullName, string methodName)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            MethodContent = new();
        }


        public SerializerMethod(int indent, string typeFullName, string methodName, string methodSignature, string methodBody)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            MethodContent = new(methodSignature, methodBody);
        }

        public SerializerMethod(string typeFullName, string methodName, MethodContent methodContent)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
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

    internal class DeltaSerializerMethod : SerializerMethod
    {
        public DeltaSerializerMethod(string typeFullName, string methodName) : base(typeFullName, methodName) { }

        public DeltaSerializerMethod(int indent, string typeFullName, string methodName, string methodSignature, string methodBody) :
            base(indent, typeFullName, methodName, methodSignature, methodBody)
        { }

        public override bool IsDeltaSerializer() => true;
    }

    internal class GeneratedDeltaSerializerMethod : DeltaSerializerMethod
    {
        /// <summary>
        /// NamedTypeSymbol of the method.
        /// </summary>
        public readonly INamedTypeSymbol NamedTypeSymbol;

        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedDeltaSerializerMethod(int indent, INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName, string methodSignature, string methodBody) :
            base(indent, typeFullName, methodName, methodSignature, methodBody)
        {
            NamedTypeSymbol = namedTypeSymbol;
        }
    }


    internal class GeneratedSerializerMethod : SerializerMethod
    {
        /// <summary>
        /// NamedTypeSymbol of the method.
        /// </summary>
        public readonly INamedTypeSymbol? NamedTypeSymbol;

        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedSerializerMethod(int indent, INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName, string methodSignature, string methodBody) :
            base(indent, typeFullName, methodName, methodSignature, methodBody)
        {
            NamedTypeSymbol = namedTypeSymbol;
        }
    }
}