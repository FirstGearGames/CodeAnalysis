using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslyn.FishNet.Serializing;

namespace Roslyn.FishNet.Receivers
{
    internal class GeneratorSyntaxReceiver : ISyntaxContextReceiver
    {
        public SerializableReceiver SerializableReceiver = new();
        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode syntaxNode = context.Node;

            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                SerializableReceiver.FindClassSerializables(context, classDeclaration);
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
                SerializableReceiver.FindStructSerializables(context, structDeclaration);
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                SerializableReceiver.FindRpcSerializables(context, methodDeclaration);
        }
        
    }
}