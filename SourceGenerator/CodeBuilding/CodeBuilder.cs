using System;
using System.Linq;
using System.Text;
using SourceGenerator.Extensions;
using SourceGenerating.Constants;

namespace RoslynLearning.CodeBuilding
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
        public static string CallGetPooledWriter(out string writerVariableName, string variablePrefix = "",
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
        public static string CallStorePooledWriter(string writerVariableName, bool closeCall = true)
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
        public static string CallMethod(string methodName, string callingVariable = "", bool closeCall = true,
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
        /// Wrapps text around a single line if statement.
        /// </summary>
        public static string SingleLineIf(string conditionaltext)
        {
            _stringBuilder.Clear();
            _stringBuilder.Append($"if ({conditionaltext})");
            return _stringBuilder.ToString();
        }


        public static string CreateLocalVariable(string fullTypeName, string variableName, string defaultValue = "")
        {
            _stringBuilder.Clear();
            _stringBuilder.Append($"{fullTypeName} {variableName}");
            if (defaultValue.Length > 0)
                _stringBuilder.Append($" = {defaultValue};");
            else
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        public static string CallWriteBytes(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();
            string byteArrayVariable = $"{otherWriterA}.{FishNetConstants.Writer_GetBuffer_Name}()";

            //public void WriteBytes(byte[] value, int offset, int count)
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteBytes_Name}(" +
                                  $"{byteArrayVariable}, 0, {otherWriterA}.{FishNetConstants.Writer_Length_Name})");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }
    }
}