using System;
using System.Collections.Immutable;
using System.Linq;
using FishNet.SourceAnaylze.Constants;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Roslyn.FishNet.Helpers;
using Roslyn.FishNet.Receivers;

namespace FishNet.SourceAnaylzer
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SerializableScopeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticIds.FN0001, "Invalid scope of serializable type.",
            "Network serializable types must be declared internal or public, or the declaring class must be partial.", DiagnosticCategories.Usage,
            DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);

        private AnalyzerSyntaxReceiver _receiver = new();
        
        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            Debugg.Log("Starting");
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            //context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
            context.RegisterSyntaxNodeAction(_receiver.Anaylze, SyntaxKind.InvocationExpression);
        }
        //
        // private static void Analyze(SyntaxNodeAnalysisContext context)
        // {
        //     
        //     SyntaxNode syntaxNode = context.Node;
        //     
        //     // if (syntaxNode is ClassDeclarationSyntax classDeclaration)
        //     //     FindClassSerializables(context, classDeclaration);
        //      if (syntaxNode is StructDeclarationSyntax structDeclaration)
        //         FindStructSerializables(context, structDeclaration);
        //    
        //     
        //     //     context.ReportDiagnostic(Diagnostic.Create(Descriptor1, argumentSyntax.GetLocation(), typeSymbol?.Name));
        //     
        // }
        //
        //
        // private void FindStructSerializables(SyntaxNodeAnalysisContext context, StructDeclarationSyntax structDeclaration)
        // {
        //     
        //     ISymbol? symbol = context.SemanticModel.GetDeclaredSymbol(structDeclaration);
        //     if (symbol is not INamedTypeSymbol namedTypeSymbol) return;
        //
        //     FindNamedTypeSymbolSerializables(namedTypeSymbol);
        // }

        
        
    }
}