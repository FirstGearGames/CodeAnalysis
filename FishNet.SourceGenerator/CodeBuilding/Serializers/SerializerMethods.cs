using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Text;
using FirstGearGames.Roslyn.CodeBuilding;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Helpers;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding
{
    public static class SerializerMethodExtensions
    {
        public static bool IsValid(this SerializerMethod? sm)
        {
            if (sm == null) return false;
            return !string.IsNullOrWhiteSpace(sm.TypeFullName);
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
        public readonly ITypeSymbol TypeSymbol;
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
        public readonly List<string> GenericArguments;
        /// <summary>
        /// True if generic arguments are named.
        /// </summary>
        public bool AreGenericsNamed;
        /// <summary>
        /// Content of the method.
        /// </summary>
        public readonly MethodContent MethodContent;

        /// <summary>
        /// True if TypeSymbol is IArrayType.
        /// </summary>
        private readonly bool _isArrayType;

        public SerializerMethod() { }

        public SerializerMethod(ITypeSymbol typeSymbol, string methodName)
        {
            TypeSymbol = typeSymbol;
            TypeFullName = typeSymbol.GetTypeSymbolFullNameWithGenericArguments(metadataName: false);
            AreGenericsNamed = typeSymbol.AreGenericArgumentsNamed();
            GenericArguments = typeSymbol.GetGenericArgumentsString();
            MethodName = methodName;
            MethodContent = new();

            _isArrayType = (typeSymbol is IArrayTypeSymbol);
        }

        public SerializerMethod(ITypeSymbol typeSymbol, string methodName, string methodSignature, string methodBody) : this(typeSymbol, methodName)
        {
            MethodContent = new(methodSignature, methodBody);
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

        /// <summary>
        /// Creates arguments to be passed in when returning this type.
        /// </summary>
        /// <returns></returns>
        public string ToReturnArguments(ITypeSymbol returnedTypeSymbol)
        {
            const bool metadataName = false;
            
            if (returnedTypeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                //Named arguments do not need to be specified.
                if (AreGenericsNamed)
                    return string.Empty;
                else
                    return $"<{arrayTypeSymbol.ElementType.GetSymbolFullName(metadataName)}>";
            }
            else
            {
                //Not generic.
                if (returnedTypeSymbol is not INamedTypeSymbol { IsGenericType: true } namedTypeSymbol) return string.Empty;

                if (AreGenericsNamed)
                    return string.Empty;
                else
                    return returnedTypeSymbol.GetGenericArgumentsString().GetCombinedGenericArguments(returnedTypeSymbol);
            }
        }
    }

    public class DeltaSerializerMethod : SerializerMethod
    {
        public DeltaSerializerMethod(ITypeSymbol typeSymbol, string methodName, string methodSignature, string methodBody) : base(typeSymbol, methodName, methodSignature, methodBody) { }

        public DeltaSerializerMethod(ITypeSymbol typeSymbol, string methodName) : base(typeSymbol, methodName) { }
        // public DeltaSerializerMethod(string typeFullName, string methodName, List<string> genericArguments) : base(typeFullName, methodName, genericArguments) { }
        // public DeltaSerializerMethod(string typeFullName, string methodName, List<string> genericArguments, string methodSignature, string methodBody) : base(typeFullName, methodName, genericArguments, methodSignature, methodBody) { }

        public override bool IsDeltaSerializer() => true;
    }

    public class GeneratedDeltaSerializerMethod : DeltaSerializerMethod
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedDeltaSerializerMethod(ITypeSymbol typeSymbol, string methodName, string methodSignature, string methodBody) : base(typeSymbol, methodName, methodSignature, methodBody) { }
    }

    public class GeneratedSerializerMethod : SerializerMethod
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedSerializerMethod(ITypeSymbol typeSymbol, string methodName, string methodSignature, string methodBody) : base(typeSymbol, methodName, methodSignature, methodBody) { }
    }
}