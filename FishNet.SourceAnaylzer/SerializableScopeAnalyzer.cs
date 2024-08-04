using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FishNet.SourceAnaylzer
{

    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SerializableScopeAnalyzer : DiagnosticAnalyzer
    {
        private static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticIds.FN0001, "Invalid scope of serializable type.",
            "Network serializable types must be declared internal or public, or the declaring class must be partial.", DiagnosticCategories.Usage,
            DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();

            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.InvocationExpression);
        }

        
        
        private static void Analyze(SyntaxNodeAnalysisContext context)
        {
            InvocationExpressionSyntax invocationExpressionSyntax = (InvocationExpressionSyntax)context.Node;

            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, invocationExpressionSyntax).Symbol is not IMethodSymbol method) return;

            string fullyQualifiedMethodName = method.GetFullyQualifiedName();

            if (fullyQualifiedMethodName != FullyQualifiedDontDestroyOnLoadMethodName) return;

            if (invocationExpressionSyntax.FirstAncestorOrSelf<BaseTypeDeclarationSyntax>()?.BaseList is not BaseListSyntax baseListSyntax) return;

            foreach (BaseTypeSyntax baseTypeSyntax in baseListSyntax.Types)
            {
                if (!context.SemanticModel.GetTypeSymbol(baseTypeSyntax.Type).IsSubtypeOf(FullyQualifiedNetworkBehaviourTypeName)) continue;

                context.ReportDiagnostic(Diagnostic.Create(Descriptor2, invocationExpressionSyntax.GetLocation()));

                return;
            }

            foreach (ArgumentSyntax argumentSyntax in invocationExpressionSyntax.ArgumentList.Arguments)
            {
                if (!context.SemanticModel.GetTypeSymbol(argumentSyntax.Expression).IsSubtypeOf(ImmutableHashSet.Create(FullyQualifiedNetworkObjectTypeName, FullyQualifiedNetworkBehaviourTypeName), out ITypeSymbol? typeSymbol)) continue;

                context.ReportDiagnostic(Diagnostic.Create(Descriptor1, argumentSyntax.GetLocation(), typeSymbol?.Name));

                return;
            }
        }
        
    }
}