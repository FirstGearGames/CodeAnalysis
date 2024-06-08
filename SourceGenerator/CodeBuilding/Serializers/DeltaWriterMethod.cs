using Microsoft.CodeAnalysis;

namespace SourceGenerator.CodeBuilding.Serializers
{
    internal struct DeltaWriterMethod
    {
        public INamedTypeSymbol NamedTypeSymbol;
        public string FullName;
        public string MethodName;
        public string Signature;

        public DeltaWriterMethod(INamedTypeSymbol namedTypeSymbol, string typeFullName, string methodName, string signature)
        {
            NamedTypeSymbol = namedTypeSymbol;
            FullName = typeFullName;
            MethodName = methodName;
            Signature = signature;
        }
    }
}
