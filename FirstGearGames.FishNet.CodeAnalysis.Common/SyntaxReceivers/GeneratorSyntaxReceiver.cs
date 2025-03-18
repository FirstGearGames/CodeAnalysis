using FirstGearGames.CodeAnalysis.Helpers;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.RemoteProcedureCalls;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.FishNet.CodeAnalysis.Receivers
{
    public class GeneratorSyntaxReceiver : ISyntaxContextReceiver
    {
        public SerializableFinder? SerializableFinder;
        public RpcFinder? RpcFinder;

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode syntaxNode = context.Node;

            if (SerializableFinder == null)
                SerializableFinder = new();
            
            if (RpcFinder == null)
                RpcFinder = new(this);

            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                LogVisit();
                SerializableFinder.AddClassSerializables(context, classDeclaration);
            }
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
            {
                LogVisit();
                SerializableFinder.AddStructSerializables(context, structDeclaration);
            }
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            {
                LogVisit();
                SerializableFinder.AddRpcSerializables(context, methodDeclaration);
                RpcFinder.CheckRpcMethod(context, methodDeclaration);
            }

            void LogVisit()
            {
                //Log($"OnVisitSyntaxNode type {syntaxNode.GetType().Name}");
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
