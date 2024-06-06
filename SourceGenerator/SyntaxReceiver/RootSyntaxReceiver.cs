using Microsoft.CodeAnalysis;

namespace SourceGenerating.SyntaxReceivers
{
	internal class RootSyntaxReceiver : ISyntaxContextReceiver
	{
		public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
		{
			//SerializerProcessor.OnVisitSyntaxNode(context);
		}
	}
}
