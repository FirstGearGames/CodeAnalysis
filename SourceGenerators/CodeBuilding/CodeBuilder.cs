using System.Collections.Generic;
using System.Text;
using CodeAnalysis.Constants;
using Microsoft.CodeAnalysis;
using CodeAnalysis.Extensions;

namespace CodeAnalysis.SourceGenerators.CodeBuilding;

/// <summary>
/// Provides helper methods for emitting common C# source-code fragments.
/// </summary>
public static class CodeBuilder
{
    private static StringBuilder _stringBuilder = new();

    /// <summary>
    /// Returns the declared accessibility of the supplied method as the keyword sequence used in C# source.
    /// </summary>
    /// <param name="symbol">Method symbol whose accessibility is being formatted.</param>
    /// <returns>The keyword sequence corresponding to the method's accessibility.</returns>
    public static string GetDeclaredAccessibility(this IMethodSymbol symbol)
    {
        return symbol.DeclaredAccessibility switch
        {
            Accessibility.Public => "public",
            Accessibility.Internal => "internal",
            Accessibility.Protected => "protected",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.ProtectedOrInternal => "protected internal",
            Accessibility.NotApplicable => "private",
            _ => "public"
        };
    }

    /// <summary>
    /// Creates a public static class declaration, optionally wrapped in a namespace.
    /// </summary>
    /// <param name="className">Name of the class to emit.</param>
    /// <param name="footer">Receives the closing braces required to balance the emitted opening text.</param>
    /// <param name="namespaceName">Namespace to wrap the class in, or an empty string to emit no namespace.</param>
    /// <returns>The opening text for the class declaration.</returns>
    public static string CreatePublicStaticClass(string className, out string footer, string namespaceName = "")
    {
        _stringBuilder.Clear();

        int indent = 0;
        bool hasNamespace = namespaceName.Length > 0;
        if (hasNamespace)
        {
            _stringBuilder.AppendLine($"namespace {namespaceName}");
            _stringBuilder.AppendLine("{");
            indent++;
        }

        _stringBuilder.AppendLine(indent, $"public static class {className}");
        _stringBuilder.AppendLine(indent, "{");

        StringBuilder footerSb = new();

        if (hasNamespace)
        {
            footerSb.AppendLine(indent, "}");
            footerSb.Append('}');
        }

        footer = footerSb.ToString();
        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Creates a copy of the supplied class declaration, optionally wrapped in a namespace.
    /// </summary>
    /// <param name="originalClassNamedTypeSymbol">Source class whose header is being copied.</param>
    /// <param name="footer">Receives the closing braces required to balance the emitted opening text.</param>
    /// <param name="namespaceName">Namespace to wrap the class in, or an empty string to emit no namespace.</param>
    /// <returns>The opening text for the class declaration.</returns>
    public static string CreateClassCopy(INamedTypeSymbol originalClassNamedTypeSymbol, out string footer, string namespaceName = "")
    {
        _stringBuilder.Clear();

        int indent = 0;
        bool hasNamespace = namespaceName.Length > 0;
        if (hasNamespace)
        {
            _stringBuilder.AppendLine($"namespace {namespaceName}");
            _stringBuilder.AppendLine("{");
            indent++;
        }

        _stringBuilder.AppendLine(indent, originalClassNamedTypeSymbol.GetClassOrStructHeader());
        _stringBuilder.AppendLine(indent, "{");

        StringBuilder footerSb = new();

        if (hasNamespace)
        {
            footerSb.AppendLine(indent, "}");
            footerSb.Append('}');
        }

        footer = footerSb.ToString();
        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Emits a method call with optional arguments and an optional calling variable.
    /// </summary>
    /// <param name="methodName">Name of the method to call.</param>
    /// <param name="callingVariable">Variable to call the method on, or an empty string for a static call.</param>
    /// <param name="variableNames">Argument expressions to pass to the method.</param>
    /// <returns>The emitted method call text.</returns>
    public static string CallMethod(string methodName, string callingVariable = "", List<string>? variableNames = null)
    {
        if (callingVariable.Length > 0)
            callingVariable += ".";

        _stringBuilder.Clear();
        _stringBuilder.Append($"{callingVariable}{methodName}(");

        //Add arguments.
        if (variableNames is not null)
            _stringBuilder.Append(string.Join(", ", variableNames));

        //End call.
        _stringBuilder.Append(')');

        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Creates a multi-line <c>if</c> statement containing a single body line.
    /// </summary>
    /// <param name="indent">Number of indentation units to apply to the statement.</param>
    /// <param name="conditionaltext">Condition expression to emit.</param>
    /// <param name="line">Body line to emit inside the statement.</param>
    /// <returns>The emitted multi-line if statement text.</returns>
    public static string CreateMultiLineIf(int indent, string conditionaltext, string line)
    {
        StringBuilder sb = new();
        sb.Append(indent + 1, line);
        return CreateMultiLineIf(indent, conditionaltext, sb);
    }

    /// <summary>
    /// Creates a multi-line <c>if</c> statement containing the supplied body text.
    /// </summary>
    /// <param name="indent">Number of indentation units to apply to the statement.</param>
    /// <param name="conditionaltext">Condition expression to emit.</param>
    /// <param name="lines">Body text to emit inside the statement.</param>
    /// <returns>The emitted multi-line if statement text.</returns>
    public static string CreateMultiLineIf(int indent, string conditionaltext, StringBuilder lines)
    {
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(indent, $"if ({conditionaltext})");
        _stringBuilder.AppendLine(indent, "{");
        _stringBuilder.AppendLine(lines.ToString());
        _stringBuilder.AppendLine(indent, "}");
        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Emits a local variable declaration with an optional default value.
    /// </summary>
    /// <param name="fullTypeName">Fully qualified type name of the variable.</param>
    /// <param name="variableName">Name of the variable being declared.</param>
    /// <param name="defaultValue">Optional default value expression.</param>
    /// <param name="closeLine">When true, terminates the declaration with a semicolon.</param>
    /// <returns>The emitted local variable declaration text.</returns>
    public static string CreateLocalVariable(string fullTypeName, string variableName, string defaultValue = "", bool closeLine = true)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append($"{fullTypeName} {variableName}");
        string lineCloser = closeLine ? ";" : string.Empty;
        if (defaultValue.Length > 0)
            _stringBuilder.Append($" = {defaultValue}{lineCloser}");
        else
            _stringBuilder.Append(lineCloser);

        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Emits the construction of a <see cref="System.Func{TResult}"/> with the supplied parameter types and return type.
    /// </summary>
    /// <param name="returnType">Fully qualified return type.</param>
    /// <param name="types">Fully qualified parameter types, in order.</param>
    /// <returns>The emitted Func construction text.</returns>
    public static string CreateFunction(string returnType, params string[] types)
    {
        _stringBuilder.Clear();

        _stringBuilder.Append($"new {NativeConstants.FuncFullName}<");
        foreach (string item in types)
            _stringBuilder.Append($"{item}, ");

        _stringBuilder.Append($"{returnType}>");

        return _stringBuilder.ToString();
    }

    /// <summary>
    /// Emits the construction of a <see cref="System.Action"/> with the supplied parameter types.
    /// </summary>
    /// <param name="types">Fully qualified parameter types, in order.</param>
    /// <returns>The emitted Action construction text.</returns>
    public static string CreateAction(params string[] types)
    {
        _stringBuilder.Clear();

        _stringBuilder.Append($"new {NativeConstants.ActionFullName}<");
        for (int i = 0; i < types.Length; i++)
        {
            _stringBuilder.Append($"{types[i]}");
            if (i < types.Length - 1)
                _stringBuilder.Append(", ");
        }

        _stringBuilder.Append($">");

        return _stringBuilder.ToString();
    }

}