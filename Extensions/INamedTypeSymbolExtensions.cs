#pragma warning disable CS8602 // Dereference of a possibly null reference.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeAnalysis.Extensions;

/// <summary>
/// Extension methods for inspecting and generating source from <see cref="INamedTypeSymbol"/> instances.
/// </summary>
public static class NamedTypeSymbolExtensions
{
    /// <summary>
    /// Tries to generate the namespace and class declaration text for the supplied class symbol.
    /// </summary>
    /// <example>
    /// Generated class signature example: public partial MyClass&lt;Type&gt; : BaseClass, Interface.
    /// </example>
    /// <param name="classSymbol">Class symbol whose declaration is being generated.</param>
    /// <param name="fullNamespace">Receives the fully qualified namespace, or null when one cannot be resolved.</param>
    /// <param name="classDeclaration">Receives the generated class declaration text, or null when generation fails.</param>
    /// <returns>True when both outputs were populated successfully.</returns>
    public static bool TryGenerateClassSignature(this INamedTypeSymbol classSymbol, out string? fullNamespace, out string? classDeclaration)
    {
        fullNamespace = null;
        classDeclaration = null;

        if (!classSymbol.IsReferenceType)
            return false;

        fullNamespace = classSymbol.GetNamespace();
            
        StringBuilder stringBuilder = new();
            
        /* Get modifiers. */
        SyntaxReference? classSyntaxReference = classSymbol.DeclaringSyntaxReferences.FirstOrDefault();
        if (classSyntaxReference is null)
            return false;
            
        TypeDeclarationSyntax typeDeclarationSyntax = (TypeDeclarationSyntax)classSyntaxReference.GetSyntax();
        string generatedModifiers = string.Join(" ", typeDeclarationSyntax.Modifiers.Select(x => x.Text));
            
        // public partial 
        stringBuilder.Append($"{generatedModifiers} class ");
            
        // Formatting for ToDisplayString.
        SymbolDisplayFormat thisStringFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly, genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters, kindOptions: SymbolDisplayKindOptions.IncludeMemberKeyword, memberOptions: SymbolDisplayMemberOptions.IncludeModifiers, miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
        // public partial class Name<Parameters>
        stringBuilder.Append(classSymbol.ToDisplayString(thisStringFormat));

        bool hasBaseClass = classSymbol.BaseType is not null && classSymbol.BaseType.SpecialType is not SpecialType.System_Object;

        SymbolDisplayFormat fullNameWithParameterFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters, kindOptions: SymbolDisplayKindOptions.None, memberOptions: SymbolDisplayMemberOptions.IncludeModifiers, miscellaneousOptions: SymbolDisplayMiscellaneousOptions.None);

        // public partial class Name<Parameters> : Namespace.BaseClass<Parameters>
        if (hasBaseClass)
            stringBuilder.Append($" : {classSymbol.BaseType.ToDisplayString(fullNameWithParameterFormat)}");

        for (int i = 0; i < classSymbol.Interfaces.Length; i++)
        {
            INamedTypeSymbol interfaceSymbol = classSymbol.Interfaces[i];

            /* If there is a base class or if this is the second interface
             * prefix with a comma, otherwise use a colon. */
            string prefix = hasBaseClass || i > 0 ? ", " : " : ";
            stringBuilder.Append(prefix);
                
            stringBuilder.Append(interfaceSymbol.ToDisplayString(fullNameWithParameterFormat));
        }
            
        classDeclaration = stringBuilder.ToString();
            
        return true;
    }

    /// <summary>
    /// Returns the field symbols declared on the supplied named type, optionally filtered by accessibility.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose fields are being read.</param>
    /// <param name="requiredAccessibility">When supplied, restricts results to fields with the specified accessibility.</param>
    /// <returns>A list containing every matching field symbol.</returns>
    public static List<IFieldSymbol> GetFieldSymbols(this INamedTypeSymbol namedTypeSymbol, Accessibility? requiredAccessibility)
    {
        List<IFieldSymbol> validSymbols = new();

        foreach (ISymbol symbol in namedTypeSymbol.GetMembers())
        {
            if (symbol is IFieldSymbol methodSymbol)
            {
                if (requiredAccessibility is null || methodSymbol.DeclaredAccessibility == requiredAccessibility.Value)
                    validSymbols.Add(methodSymbol);
            }
        }

        return validSymbols;
    }

    /// <summary>
    /// Returns the method symbols declared on the supplied named type, optionally filtered by accessibility.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose methods are being read.</param>
    /// <param name="requiredAccessibility">When supplied, restricts results to methods with the specified accessibility.</param>
    /// <returns>A list containing every matching method symbol.</returns>
    public static List<IMethodSymbol> GetMethodSymbols(this INamedTypeSymbol namedTypeSymbol, Accessibility? requiredAccessibility)
    {
        List<IMethodSymbol> validSymbols = new();

        foreach (ISymbol symbol in namedTypeSymbol.GetMembers())
        {
            if (symbol is IMethodSymbol methodSymbol)
            {
                if (requiredAccessibility is null || methodSymbol.DeclaredAccessibility == requiredAccessibility.Value)
                    validSymbols.Add(methodSymbol);
            }
        }

        return validSymbols;
    }

    /// <summary>
    /// Returns whether the supplied named type implements an interface with the specified fully qualified name.
    /// </summary>
    /// <param name="symbol">Named type whose implemented interfaces are being inspected.</param>
    /// <param name="interfaceFullName">Fully qualified interface name to look for.</param>
    /// <returns>True when the named type implements the specified interface.</returns>
    public static bool NamedTypeSymbolImplementsInterface(this INamedTypeSymbol symbol, string? interfaceFullName)
    {
        if (interfaceFullName is null)
            return false;

        foreach (INamedTypeSymbol interfaceNamed in symbol.Interfaces)
        {
            if (interfaceNamed.GetTypeSymbolFullName() == interfaceFullName)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Returns whether the supplied symbol inherits from the specified base class anywhere along its hierarchy.
    /// </summary>
    /// <param name="symbol">Named type whose ancestry is being inspected.</param>
    /// <param name="classFullName">Fully qualified name of the base class to look for.</param>
    /// <returns>True when the symbol inherits from the specified base class.</returns>
    public static bool InheritsClass(this INamedTypeSymbol symbol, string classFullName)
    {
        while (symbol.BaseType is { } baseSymbol)
        {
            if (baseSymbol.GetTypeSymbolFullName() == classFullName)
                return true;

            symbol = symbol.BaseType;
        }

        return false;
    }

    /// <summary>
    /// Returns the method matching the specified name and parameter type names.
    /// </summary>
    /// <param name="symbol">Named type whose methods are being searched.</param>
    /// <param name="methodName">Name of the method to find.</param>
    /// <param name="parameterNames">Expected parameter type names, in order.</param>
    /// <returns>The matching method symbol, or null when no match exists.</returns>
    public static IMethodSymbol? GetMethod(this INamedTypeSymbol symbol, string methodName, params string[] parameterNames)
    {
        IEnumerable<IMethodSymbol> methodSymbols = symbol.GetMembers(methodName).OfType<IMethodSymbol>();

        foreach (IMethodSymbol methodSymbol in methodSymbols)
        {
            if (methodSymbol.AreParametersMatching(parameterNames))
                return methodSymbol;
        }

        return null;
    }

    /// <summary>
    /// Returns whether the supplied type is declared with public accessibility.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose accessibility is being inspected.</param>
    /// <returns>True when the type is declared public.</returns>
    public static bool HasPublicAccessibility(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.DeclaredAccessibility is Accessibility.Public;

    /// <summary>
    /// Returns whether the supplied type is declared with the partial modifier.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose modifiers are being inspected.</param>
    /// <returns>True when the type is declared partial.</returns>
    public static bool HasPartialModifier(this INamedTypeSymbol namedTypeSymbol)
    {
        if (namedTypeSymbol is null)
            return false;

        ImmutableArray<SyntaxReference> syntaxReferences = namedTypeSymbol.DeclaringSyntaxReferences;

        // If there's more than one reference then we know it's partial
        if (syntaxReferences.Length > 1)
            return true;

        SyntaxReference? firstSyntaxReference = syntaxReferences.FirstOrDefault();
        if (firstSyntaxReference is null)
            return false;

        if (firstSyntaxReference.GetSyntax() is ClassDeclarationSyntax classDeclarationSyntax)
            return classDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);
        if (firstSyntaxReference.GetSyntax() is StructDeclarationSyntax structDeclarationSyntax)
            return structDeclarationSyntax.Modifiers.Any(SyntaxKind.PartialKeyword);

        return false;
    }

    /// <summary>
    /// Returns the type header as a string, such as <c>public partial class MyClass</c>.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose header is being generated.</param>
    /// <returns>The generated type header text, or an empty string when the type kind is unsupported.</returns>
    public static string GetClassOrStructHeader(this INamedTypeSymbol namedTypeSymbol)
    {
        string keywordText;
        if (!IsAllowedKeyword(out keywordText))
            return string.Empty;

        // Public, internal, etc. 
        string modifiersText = namedTypeSymbol.DeclaredAccessibility.ToString().ToLower();
        // Partial check.
        string partialText = HasPartialModifier(namedTypeSymbol) ? "partial " : string.Empty;

        bool IsAllowedKeyword(out string lKeyword)
        {
            lKeyword = string.Empty;

            if (namedTypeSymbol.TypeKind == TypeKind.Class)
                lKeyword = "class";
            else if (namedTypeSymbol.TypeKind == TypeKind.Struct)
                lKeyword = "struct";
            else
                return false;

            return true;
        }

        return $"{modifiersText} {partialText}{keywordText} {namedTypeSymbol.Name}";
    }
    
    /// <summary>
    /// Returns whether the supplied symbol implements <see cref="IEquatable{T}"/>.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose interfaces are being inspected.</param>
    /// <returns>True when the symbol implements <see cref="IEquatable{T}"/>.</returns>
    public static bool ImplementsIEquatable(this INamedTypeSymbol namedTypeSymbol)
    {
        string? iEquatableFullName = typeof(IEquatable<>).FullName;
        if (iEquatableFullName is null)
            return false;
        
        return namedTypeSymbol.NamedTypeSymbolImplementsInterface(iEquatableFullName);
    }
    
    /// <summary>
    /// Returns whether the supplied symbol declares a user-defined equality operator.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose members are being inspected.</param>
    /// <returns>True when the symbol declares an equality operator.</returns>
    public static bool ImplementsOpEquality(this INamedTypeSymbol namedTypeSymbol)
    {
        foreach (ISymbol member in namedTypeSymbol.GetMembers(WellKnownMemberNames.EqualityOperatorName))
        {
            // Cast to IMethodSymbol to check the MethodKind
            if (member is IMethodSymbol { MethodKind: MethodKind.UserDefinedOperator } method)
            {
                //Two parameters are expected for the operator== method.
                if (method.Parameters.Length == 2)
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns whether the supplied named type can be compared using the <c>==</c> and <c>!=</c> operators.
    /// </summary>
    /// <param name="namedTypeSymbol">Named type whose semantics are being inspected.</param>
    /// <returns>True when the type can be compared with built-in equality operators.</returns>
    public static bool IsEqualityComparable(this INamedTypeSymbol namedTypeSymbol) => namedTypeSymbol.IsValueType && !namedTypeSymbol.IsClassOrStruct();

}
#pragma warning restore CS8602 // Dereference of a possibly null reference.