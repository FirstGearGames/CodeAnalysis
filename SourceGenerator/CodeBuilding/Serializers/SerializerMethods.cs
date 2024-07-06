using Microsoft.CodeAnalysis;

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

        public SerializerMethod(string typeFullName, string methodName)
        {
            TypeFullName = typeFullName;
            MethodName = methodName;
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
        public DeltaSerializerMethod(string typeFullName, string methodName) : base(typeFullName, methodName)
        {
        }

        public override bool IsDeltaSerializer() => true;
    }

    internal class GeneratedDeltaSerializerMethod : DeltaSerializerMethod
    {
        /// <summary>
        /// NamedTypeSymbol of the method.
        /// </summary>
        public readonly INamedTypeSymbol NamedTypeSymbol;

        /// <summary>
        /// Header of the serializer. This includes the signature and opening bracket.
        /// </summary>
        public readonly string Header;

        /// <summary>
        /// Body of the serializer.
        /// </summary>
        public string Body;

        /// <summary>
        /// Footer of the serializer. This includes the closing bracket.
        /// </summary>
        public readonly string Footer;

        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedDeltaSerializerMethod(INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName, string header, string footer, string body) :
            base(typeFullName, methodName)
        {
            NamedTypeSymbol = namedTypeSymbol;
            Header = header;
            Body = body;
            Footer = footer;
        }
    }
    
    
    internal class GeneratedSerializerMethod : SerializerMethod
    {
        /// <summary>
        /// NamedTypeSymbol of the method.
        /// </summary>
        public readonly INamedTypeSymbol? NamedTypeSymbol;

        /// <summary>
        /// Header of the serializer. This includes the signature and opening bracket.
        /// </summary>
        public readonly string Header;

        /// <summary>
        /// Body of the serializer.
        /// </summary>
        public string Body;

        /// <summary>
        /// Footer of the serializer. This includes the closing bracket.
        /// </summary>
        public readonly string Footer;

        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedSerializerMethod(INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName, string header, string footer, string body) :
            base(typeFullName, methodName)
        {
            NamedTypeSymbol = namedTypeSymbol;
            Header = header;
            Body = body;
            Footer = footer;
        }
    }
}