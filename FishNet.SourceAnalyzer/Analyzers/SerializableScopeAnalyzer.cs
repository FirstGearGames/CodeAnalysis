using System.Collections.Immutable;
using FishNet.SourceAnaylze.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.FishNet.Serializing;

namespace FishNet.SourceAnaylzer.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SerializableScopeAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticIds.FN0001, "Invalid scope of serializable type.",
            "Network serializable types must be declared internal or public, or the declaring class must be partial.", DiagnosticCategories.Usage,
            DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);

        private SerializableReceiver SerializableReceiver = new();

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            SerializableReceiver.OnIsNotSerializableAccessible += SerializableReceiver_OnIsNotSerializableAccessible;

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
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
            SyntaxNode syntaxNode = context.Node;

            Location location = null;
            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
                location = classDeclaration.GetLocation();
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
                location = structDeclaration.GetLocation();
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                location = methodDeclaration.GetLocation();
            
            if (location != null)
                context.ReportDiagnostic(Diagnostic.Create(Descriptor1, location));
        }
    }
}
