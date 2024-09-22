using System.Collections.Generic;
using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.Native.Constants;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.CodeBuilding
{
    public static class CodeBuilder
    {
        private static StringBuilder _stringBuilder = new();

        /// <summary>
        /// Creates a class optionally wrapping it in a namespace.
        /// </summary>
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
        /// Calls a method taking optional arguments.
        /// </summary>
        public static string CallMethod(string methodName, string callingVariable = "", bool closeCall = true, params string[] variableNames)
        {
            if (callingVariable.Length > 0)
                callingVariable += ".";

            _stringBuilder.Clear();
            _stringBuilder.Append($"{callingVariable}{methodName}(");

            //Add arguments.
            for (int i = 0; i < variableNames.Length; i++)
            {
                //Add comma to take another variable.
                if (i > 0)
                    _stringBuilder.Append(", ");

                _stringBuilder.Append(variableNames[i]);
            }

            //End call.
            _stringBuilder.Append(')');
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Creates a multiline if statement conditional.
        /// </summary>
        public static string CreateMultiLineIf(int indent, string conditionaltext, string line)
        {
            StringBuilder sb = new();
            sb.Append(indent + 1, line);
            return CreateMultiLineIf(indent, conditionaltext, sb);
        }

        /// <summary>
        /// Creates a multiline if statement conditional.
        /// </summary>
        public static string CreateMultiLineIf(int indent, string conditionaltext, StringBuilder lines)
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine(indent, $"if ({conditionaltext})");
            _stringBuilder.AppendLine(indent, "{");
            _stringBuilder.AppendLine(lines.ToString());
            _stringBuilder.AppendLine(indent, "}");
            return _stringBuilder.ToString();
        }

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

        public static string CreateFunction(string returnType, params string[] types)
        {
            _stringBuilder.Clear();

            _stringBuilder.Append($"new {NativeConstants.Func_FullName}<");
            foreach (string item in types)
                _stringBuilder.Append($"{item}, ");

            _stringBuilder.Append($"{returnType}>");

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Combines generic argument strings into <str0, str1, str2 ...>
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string CombineGenericArguments(this List<string> arguments)
        {
            if (arguments.Count == 0) return string.Empty;

            _stringBuilder.Clear();

            foreach (string s in arguments)
            {
                //Add separate if argument already exists.
                if (_stringBuilder.Length != 0)
                    _stringBuilder.Append(", ");

                _stringBuilder.Append(s);
            }

            return $"<{_stringBuilder.ToString()}>";
        }
        
        
        /// <summary>
        /// Combines generic argument strings into <str0, str1, str2 ...>
        /// </summary>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string CombineGenericArguments(this List<ITypeSymbol> arguments)
        {
            if (arguments.Count == 0) return string.Empty;

            List<string> results = new();

            foreach (ITypeSymbol typeSymbol in arguments)
            {
                if (typeSymbol.TypeKind is TypeKind.TypeParameter)
                    results.Add(typeSymbol.Name);
                else
                    results.Add(typeSymbol.GetTypeFullName());
            }

            return results.CombineGenericArguments();
        }
    }
}