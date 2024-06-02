using Microsoft.CodeAnalysis;
using SourceGenerator.SyntaxReceiver.SyntaxProcessor;
using System.Collections.Generic;

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
