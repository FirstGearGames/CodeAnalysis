using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.FishNet.Helpers;
using Roslyn.FishNet.Serializing;

namespace Roslyn.FishNet.Receivers
{
    public class AnalyzerSyntaxReceiver
    {
        public SerializableReceiver SerializableReceiver = new();

        public void Initialize()
        {
            SerializableReceiver.OnIsNotSerializableAccessible += SerializableReceiver_OnIsNotSerializableAccessible;
        }
        public void Anaylze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode syntaxNode = context.Node;

            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                SerializableReceiver.FindClassSerializables(context, classDeclaration);
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
                SerializableReceiver.FindStructSerializables(context, structDeclaration);
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                SerializableReceiver.FindRpcSerializables(context, methodDeclaration);
        }

        private void SerializableReceiver_OnIsNotSerializableAccessible(SyntaxNodeAnalysisContext context)
        {
            Debugg.Log("DERPPPPPP");
        }
    }
}