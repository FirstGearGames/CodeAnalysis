using FirstGearGames.Roslyn.FishNet.Serializing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.Roslyn.FishNet.Receivers
{
    public class GeneratorSyntaxReceiver : ISyntaxContextReceiver
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