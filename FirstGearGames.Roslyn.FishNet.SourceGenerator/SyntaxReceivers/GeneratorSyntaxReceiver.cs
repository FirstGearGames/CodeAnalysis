using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Helpers.Serializing;
using FirstGearGames.Roslyn.FishNet.SyncTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.Roslyn.FishNet.Receivers
{
    public class GeneratorSyntaxReceiver : ISyntaxContextReceiver
    {
        public SerializableFinder SerializableReceiver = new();

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode syntaxNode = context.Node;


            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                LogVisit();
                SerializableReceiver.FindClassSerializables(context, classDeclaration);
            }
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
            {
                LogVisit();
                SerializableReceiver.FindStructSerializables(context, structDeclaration);
            }
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            {
                LogVisit();
                SerializableReceiver.FindRpcSerializables(context, methodDeclaration);
            }
            

            void LogVisit()
            {
                Log($"OnVisitSyntaxNode type {syntaxNode.GetType().Name}");
            }
        }

        private void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"[GeneratorSyntaxReceiver] {txt}");
        }
    }
}