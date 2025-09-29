using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using FirstGearGames.FishNet.CodeAnalysis.Constants;
using FirstGearGames.FishNet.CodeAnalysis.Helpers.Serializing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    internal sealed class SerializableScopeAnalyzer : DiagnosticAnalyzer
    {
        public static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticIds.FN0001, "Invalid scope", "{0}", DiagnosticCategories.Scope, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);
        private Dictionary<DiagnosticDescriptor, string> _defaultMessages;

        public override void Initialize(AnalysisContext context)
        {
            if (_defaultMessages is null)
            {
                _defaultMessages = new();
                _defaultMessages.Add(Descriptor1, "Network serializable types must be declared public or have their containing type as partial. If you do not wish to serialize the type use the ExcludeSerialization attribute on the member or type.");
            }

            if (ApiFinder is null)
                ApiFinder = new();

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            ApiFinder.OnIsNotSerializableAccessible += ApiReceiverOnIsNotApiAccessible;

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode syntaxNode = context.Node;

            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                ApiFinder.AddClassSerializables(context, classDeclarationSyntax);
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
                ApiFinder.AddStructSerializables(context, structDeclaration);
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                ApiFinder.AddRpcSerializables(context, methodDeclaration);
        }

        private void ApiReceiverOnIsNotApiAccessible(SyntaxNodeAnalysisContext context, ISymbol source, string messageOverride)
        {
            SyntaxNode syntaxNode = context.Node;

            Location location = null;
            if (source is IFieldSymbol fs)
                location = fs.Locations[0];
            else if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                location = classDeclarationSyntax.GetLocation();
            else if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax)
                location = structDeclarationSyntax.GetLocation();
            else if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax)
                location = methodDeclarationSyntax.GetLocation();
            else if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax)
                location = fieldDeclarationSyntax.GetLocation();

            if (location is not null)
            {
                string msg = String.IsNullOrEmpty(messageOverride) ? _defaultMessages[Descriptor1] : messageOverride;
                Diagnostic diagnostic = Diagnostic.Create(Descriptor1, location, msg);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}