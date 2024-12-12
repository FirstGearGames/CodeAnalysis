using FirstGearGames.Roslyn.FishNet.Helpers;
using FirstGearGames.Roslyn.FishNet.Helpers.RemoteProcedureCalls;
using FirstGearGames.Roslyn.FishNet.Helpers.Serializing;
using FirstGearGames.Roslyn.FishNet.SyncTypes;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FirstGearGames.Roslyn.FishNet.Receivers
{
    public class GeneratorSyntaxReceiver : ISyntaxContextReceiver
    {
        public SerializableFinder SerializableFinder;
        public RpcFinder RpcFinder;

        public void Initialize()
        {
            SerializableFinder = new(this);
            RpcFinder = new(this);
        }

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            SyntaxNode syntaxNode = context.Node;


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
                RpcFinder.AddRpcMethod(context, methodDeclaration);
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