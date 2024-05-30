using Microsoft.CodeAnalysis;
using SourceGenerator.SyntaxReceiver.SyntaxProcessor;

namespace SourceGenerating.SyntaxReceivers
{
    internal class RootSyntaxReceiver : ISyntaxContextReceiver
    {

        public SerializerProcessor SerializerProcessor = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            //SerializerProcessor.OnVisitSyntaxNode(context);
        }
    }
}
