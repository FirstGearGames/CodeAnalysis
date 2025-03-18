using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using FirstGearGames.CodeAnalysis.Extensions;
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
        public static readonly DiagnosticDescriptor Descriptor1 = new(DiagnosticIds.FN0001, "Invalid scope of serializable type.", "{0}", DiagnosticCategories.Usage, DiagnosticSeverity.Error, true, customTags: WellKnownDiagnosticTags.NotConfigurable);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; } = ImmutableArray.Create(Descriptor1);

        public SerializableFinder SerializableFinder;

        private Dictionary<DiagnosticDescriptor, string> _defaultMessages;
        
        public override void Initialize(AnalysisContext context)
        {
            if (_defaultMessages == null)
            {
                _defaultMessages = new();
                _defaultMessages.Add(Descriptor1, "Network serializable types must be declared public. One or more used types are not declared public. If you do not wish to serialize the type use the ExcludeSerialization attribute on the member or type.");
            }
            
            if (SerializableFinder == null)
                SerializableFinder = new();

            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

            SerializableFinder.OnIsNotSerializableAccessible += SerializableReceiver_OnIsNotSerializableAccessible;

            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.ClassDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.StructDeclaration);
            context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.MethodDeclaration);
        }

        private void Analyze(SyntaxNodeAnalysisContext context)
        {
            SyntaxNode syntaxNode = context.Node;

            if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                SerializableFinder.AddClassSerializables(context, classDeclarationSyntax);
            else if (syntaxNode is StructDeclarationSyntax structDeclaration)
                SerializableFinder.AddStructSerializables(context, structDeclaration);
            else if (syntaxNode is MethodDeclarationSyntax methodDeclaration)
                SerializableFinder.AddRpcSerializables(context, methodDeclaration);
        }

        private void SerializableReceiver_OnIsNotSerializableAccessible(SyntaxNodeAnalysisContext context, string message, IFieldSymbol fs)
        {
            SyntaxNode syntaxNode = context.Node;

            Location location = null;
            if (fs != null)
                location = fs.Locations[0];
            else if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                location = classDeclarationSyntax.GetLocation();
            else if (syntaxNode is StructDeclarationSyntax structDeclarationSyntax)
                location = structDeclarationSyntax.GetLocation();
            else if (syntaxNode is MethodDeclarationSyntax methodDeclarationSyntax)
                location = methodDeclarationSyntax.GetLocation();
            else if (syntaxNode is FieldDeclarationSyntax fieldDeclarationSyntax)
                location = fieldDeclarationSyntax.GetLocation();

            if (location != null)
            {
                string msg = String.IsNullOrEmpty(message) ? _defaultMessages[Descriptor1] : message;
                Diagnostic diagnostic = Diagnostic.Create(Descriptor1, location, msg);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}