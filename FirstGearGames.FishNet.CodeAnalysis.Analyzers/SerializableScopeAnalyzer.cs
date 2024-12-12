using System.Collections.Immutable;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Helpers.Serializing;
using FirstGearGames.FishNet.CodeAnalysis.Analyzers.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SerializableScopeAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticIds.FN0001, "Invalid scope of serializable type.",
            "Network serializable types must be declared public.", DiagnosticCategories.Usage,
            DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

           // SerializableFinder.OnIsNotSerializableAccessible += SerializableReceiver_OnIsNotSerializableAccessible;

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode syntaxNode = context.Node;
            //
            // if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            //     SerializableFinder.AddClassSerializables(context, classDeclaration);
            // else if (syntaxNode is StructDeclarationSyntax structDeclaration)
            //     SerializableFinder.AddStructSerializables(context, structDeclaration);
            // else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
            //     SerializableFinder.AddRpcSerializables(context, methodDeclaration);
        }
        //
        // private void SerializableReceiver_OnIsNotSerializableAccessible(SyntaxNodeAnalysisContext context)
        // {
        //     SyntaxNode syntaxNode = context.Node;
        //
        //     Location location = null;
        //     if (syntaxNode is ClassDeclarationSyntax classDeclaration)
        //         location = classDeclaration.GetLocation();
        //     else if (syntaxNode is StructDeclarationSyntax structDeclaration)
        //         location = structDeclaration.GetLocation();
        //     else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
        //         location = methodDeclaration.GetLocation();
        //
        //     if (location != null)
        //         context.ReportDiagnostic(Diagnostic.Create(Descriptor1, location));
        // }
    }
}
