using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding
{
    public static class CodeBuilder
    {
        private static StringBuilder _stringBuilder = new();

    
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

        public static string CallWriteArraySegment(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegment())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteArraySegment_Name}(" +
                                  $"{otherWriterA}.{FishNetConstants.Writer_GetArraySegment_Name}())");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        public static string CallWriteArraySegmentAndSize(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegmentAndSize())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteArraySegmentAndSize_Name}(" +
                                  $"{otherWriterA}.{FishNetConstants.Writer_GetArraySegment_Name}())");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }


        public static MethodContent CreatePublicRuntimeInitializeOnLoadMethod(int indent, string methodName)
        {
            StringBuilder sb = new();
            sb.AppendLine(indent, $"[{UnityConstants.RuntimeInitializeOnLoadMethod_FullName}]");
            sb.Append(indent, $"public static void {methodName}()");

            return new MethodContent(sb);
        }

    }
}