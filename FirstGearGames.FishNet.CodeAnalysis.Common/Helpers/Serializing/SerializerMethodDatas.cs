using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.CodeAnalysis;
using System.Text;
using FirstGearGames.CodeAnalysis.Extensions;
using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Constants;

namespace FirstGearGames.FishNet.CodeAnalysis.CodeBuilding.Serializers
{
    public static class SerializerMethodDataExtensions
    {
        public static bool IsValid(this SerializerMethodData? smd)
        {
            if (smd == null)
                return false;
            return !string.IsNullOrWhiteSpace(smd.TypeFullName);
        }

        public static bool IsGenericReadOrWriteMethod(this SerializerMethodData? smd)
        {
            if (!smd.IsValid())
                return false;

            return smd.MethodName == FishNetConstants.Writer_Write_Name || smd.MethodName == FishNetConstants.Reader_Read_Name || smd.MethodName == FishNetConstants.Writer_WriteDelta_Name || smd.MethodName == FishNetConstants.Reader_ReadDelta_Name;
        }

        public static string GetReadOrWriteArgumentString(this SerializerMethodData? smd, ITypeSymbol typeSymbol)
        {
            if (!smd.IsValid())
                return string.Empty;

            //Uses Read/Write<T>.
            if (smd.IsGenericReadOrWriteMethod())
                return ReturnArguments();
            //Uses generated or built-in serializer.
            if (smd.HasGenericArguments && !smd.AreGenericsNamed)
                return ReturnArguments();

            return string.Empty;

            string ReturnArguments() => typeSymbol.GetTypeSymbolCombinedGenericArgumentsString(argumentType: GenericArgumentType.PreferNamed);
        }
    }

    public class SerializerMethodContent
    {
        public StringBuilder Header;
        public StringBuilder Body;

        public SerializerMethodContent()
        {
            Header = new();
            Body = new();
        }

        public SerializerMethodContent(StringBuilder header)
        {
            Header = header;
            Body = new();
        }

        public SerializerMethodContent(string header, string body)
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

    public class SerializerMethodData
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
        /// True if there are any generic arguments.
        /// </summary>
        public bool HasGenericArguments => GenericArguments.Count > 0;
        /// <summary>
        /// True if generic arguments are named.
        /// </summary>
        public bool AreGenericsNamed;
        /// <summary>
        /// Content of the method.
        /// </summary>
        public readonly SerializerMethodContent MethodContent;
        public SerializerMethodData() { }

        public SerializerMethodData(ITypeSymbol typeSymbol, string methodName)
        {
            TypeSymbol = typeSymbol;
            TypeFullName = typeSymbol.GetTypeSymbolFullNameWithNamedArguments(metadataName: false);
            AreGenericsNamed = typeSymbol.AreGenericArgumentsNamed();
            GenericArguments = typeSymbol.GetTypeSymbolGenericArgumentsString(GenericArgumentType.PreferNamed);
            MethodName = methodName;
            MethodContent = new();
        }

        public SerializerMethodData(ITypeSymbol typeSymbol, string methodName, string methodSignature, string methodBody) : this(typeSymbol, methodName)
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
    }

    public class DeltaSerializerMethod : SerializerMethodData
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

    public class GeneratedSerializerMethod : SerializerMethodData
    {
        /// <summary>
        /// True if is a generated serializer.
        /// </summary>
        public override bool IsGenerated() => true;

        public GeneratedSerializerMethod(ITypeSymbol typeSymbol, string methodName, string methodSignature, string methodBody) : base(typeSymbol, methodName, methodSignature, methodBody) { }
    }
}