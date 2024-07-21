using System.Text;
using SourceGenerator.Extensions;
using SourceGenerating.Constants;
using SourceGenerator.CodeBuilding;
using SourceGenerator.CodeBuilding.Serializers;

namespace RoslynLearning.CodeBuilding
{
    internal static class CodeBuilder
    {
        private static StringBuilder _stringBuilder = new();

        /// <summary>
        /// Creates a class optionally wrapping it in a namespace.
        /// </summary>
        internal static string CreatePublicStaticClass(string className, out string footer, string namespaceName = "")
        {
            _stringBuilder.Clear();

            int indent = 0;
            bool hasNamespace = (namespaceName.Length > 0);
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
        /// Calls WriterPool to return a pooled writer.
        /// </summary>
        /// <param name="writerVariableName">Variable name result of </param>
        internal static string CallGetPooledWriter(out string writerVariableName, string variablePrefix = "",
            bool closeCall = true)
        {
            _stringBuilder.Clear();
            writerVariableName = $"{variablePrefix}pooledWriter";
            _stringBuilder.Append(
                $"{_stringBuilder.ToString()}{FishNetConstants.PooledWriter_FullName} {writerVariableName} = {FishNetConstants.WriterPool_Retrieve_Name}()");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        /// <summary>
        /// Calls Store on a pooled writer.
        /// </summary>
        internal static string CallStorePooledWriter(string writerVariableName, bool closeCall = true)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append($"{writerVariableName}.{FishNetConstants.PooledWriter_Store_Name}()");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }


        /// <summary>
        /// Calls a method taking optional arguments.
        /// </summary>
        internal static string CallMethod(string methodName, string callingVariable = "", bool closeCall = true,
            params string[] variableNames)
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
        internal static string CreateMultiLineIf(int indent, string conditionaltext, string line)
        {
            StringBuilder sb = new();
            sb.Append(indent + 1, line);
            return CreateMultiLineIf(indent, conditionaltext, sb);
        }

        /// <summary>
        /// Creates a multiline if statement conditional.
        /// </summary>
        internal static string CreateMultiLineIf(int indent, string conditionaltext, StringBuilder lines)
        {
            _stringBuilder.Clear();
            _stringBuilder.AppendLine(indent, $"if ({conditionaltext})");
            _stringBuilder.AppendLine(indent, "{");
            _stringBuilder.AppendLine(lines.ToString());
            _stringBuilder.AppendLine(indent, "}");
            return _stringBuilder.ToString();
        }


        internal static string CreateLocalVariable(string fullTypeName, string variableName, string defaultValue = "", bool closeLine = true)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append($"{fullTypeName} {variableName}");
            string lineCloser = (closeLine) ? ";" : string.Empty;
            if (defaultValue.Length > 0)
                _stringBuilder.Append($" = {defaultValue}{lineCloser}");
            else
                _stringBuilder.Append(lineCloser);

            return _stringBuilder.ToString();
        }

        internal static string CallWriteArraySegment(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegment())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteArraySegment_Name}(" +
                                  $"{otherWriterA}.{FishNetConstants.Writer_GetArraySegment_Name}())");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        internal static string CallWriteArraySegmentAndSize(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegmentAndSize())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteArraySegmentAndSize_Name}(" +
                                  $"{otherWriterA}.{FishNetConstants.Writer_GetArraySegment_Name}())");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }


        internal static MethodContent CreatePublicRuntimeInitializeOnLoadMethod(int indent, string methodName)
        {
            StringBuilder sb = new();
            sb.AppendLine($"[{UnityConstants.RuntimeInitializeOnLoadMethod_FullName}]");
            sb.Append(indent, $"public static void {methodName}()");

            return new MethodContent(indent, sb.ToString());
        }

        internal static string CreateFunction(string returnType, params string[] types)
        {
            _stringBuilder.Clear();

            _stringBuilder.Append($"new Func<");
            foreach (string item in types)
                _stringBuilder.Append($"{item}, ");

            _stringBuilder.Append($"{returnType}>");

            return _stringBuilder.ToString();
        }
    }
}