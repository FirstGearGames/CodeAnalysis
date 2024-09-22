using Microsoft.CodeAnalysis;
using System.Text;
using FirstGearGames.Roslyn.Extensions;

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
        /// Amount to indent the method.
        /// </summary>
        public readonly int Indent;
        /// <summary>
        /// Name of the generated method.
        /// </summary>
        public readonly string MethodName;
        /// <summary>
        /// Number of generic arguments for the method.
        /// </summary>
        public int GenericArgumentsCount;
        /// <summary>
        /// Content of the method.
        /// </summary>
        public MethodContent MethodContent;

        public SerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName)
        {
            TypeFullName = namedTypeSymbol.GetTypeFullName();
            GenericArgumentsCount = namedTypeSymbol.GetGenericArgumentsCount();
            MethodName = methodName;
            MethodContent = new();
        }

        public SerializerMethod(string typeFullName, string methodName, int genericArgumentsCount)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            GenericArgumentsCount = genericArgumentsCount;
            MethodContent = new();
        }

        public SerializerMethod(int indent, INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody)
        {
            Indent = indent;
            TypeFullName = namedTypeSymbol.GetTypeFullName();
            GenericArgumentsCount = namedTypeSymbol.GetGenericArgumentsCount();
            MethodName = methodName;
            MethodContent = new(methodSignature, methodBody);
        }

        public SerializerMethod(INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody)
        {
            TypeFullName = namedTypeSymbol.GetTypeFullName();
            GenericArgumentsCount = namedTypeSymbol.GetGenericArgumentsCount();
            MethodName = methodName;
            MethodContent = new(methodSignature, methodBody);
        }
        
        public SerializerMethod(int indent, string typeFullName, string methodName, int genericArgumentsCount, string methodSignature, string methodBody)
        {
            Indent = indent;
            TypeFullName = typeFullName;
            MethodName = methodName;
            GenericArgumentsCount = genericArgumentsCount;
            MethodContent = new(methodSignature, methodBody);
        }

        public SerializerMethod(string typeFullName, string methodName, int genericArgumentsCount, MethodContent methodContent)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
            GenericArgumentsCount = genericArgumentsCount;
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
        public DeltaSerializerMethod(string typeFullName, string methodName, int genericArguments) : base(typeFullName, methodName, genericArguments) { }
        public DeltaSerializerMethod(int indent, INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody) : base(indent, namedTypeSymbol, methodName, methodSignature, methodBody) { }

        public DeltaSerializerMethod(int indent, string typeFullName, string methodName, int genericArguments, string methodSignature, string methodBody) : base(indent, typeFullName, methodName, genericArguments, methodSignature, methodBody) { }

        public override bool IsDeltaSerializer() => true;
    }

    public class GeneratedDeltaSerializerMethod : DeltaSerializerMethod
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedDeltaSerializerMethod(int indent, INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody) : base(indent, namedTypeSymbol, methodName, methodSignature, methodBody) { }
    }

    public class GeneratedSerializerMethod : SerializerMethod
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedSerializerMethod(int indent, INamedTypeSymbol namedTypeSymbol, string methodName, string methodSignature, string methodBody) : base(indent, namedTypeSymbol, methodName, methodSignature, methodBody) { }
    }
}