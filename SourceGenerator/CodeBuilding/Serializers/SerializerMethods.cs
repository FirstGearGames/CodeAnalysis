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
        public string Signature;
        public StringBuilder Body = new();
        public int Indent;

        public MethodContent(int indent = 0)
        {
            Indent = indent;
        }
        public MethodContent(int indent, string signature)
        {
            Indent = indent;
            Signature = signature;
        }

        public MethodContent(int indent, string signature, string body)
        {
            Indent = indent;
            Signature = signature;
            Body = new(body);
        }

        public void AppendBody(int extraIndent, string value)
        {
            if (Body.Length > 0)
                Body.Append(extraIndent, value);
            else
                Body.Append(Indent + 1 + extraIndent, value);
        }

        public void AppendBodyLine(int extraIndent, string value)
        {
            if (Body.Length > 0)
                Body.AppendLine(extraIndent, value);
            else
                Body.AppendLine(Indent + 1 + extraIndent, value);
        }

        public override string ToString()
        {
            return ToString(Body);
        }

        public string ToString(StringBuilder body)
        {
            StringBuilder sb = new();
            sb.Append(ToStringWithoutFooter(body));
            sb.AppendLine(Indent, "}");
            return sb.ToString();
        }
        public string ToString(string body)
        {
            StringBuilder bodySb = new();
            bodySb.Append(Indent + 1, body);
            return ToString(bodySb);
        }

        public string ToStringWithoutFooter()
        {
            return this.ToStringWithoutFooter(Body);
        }

        public string ToStringWithoutFooter(StringBuilder body)
        {
            StringBuilder sb = new();
            sb.AppendLine(Indent, Signature);
            sb.AppendLine(Indent, "{");
            sb.AppendLine(body.ToString());
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
            MethodContent = new(indent, methodSignature, methodBody);
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