using System.Text;
using FirstGearGames.Roslyn.Extensions;
using FirstGearGames.Roslyn.FishNet.Constants;
using FirstGearGames.Roslyn.FishNet.Helpers.RemoteProcedureCalls;
using Microsoft.CodeAnalysis;

namespace FirstGearGames.Roslyn.FishNet.CodeBuilding.Serializers
{
    public static class GeneralBuilder
    {
        private static StringBuilder _stringBuilder = new();

        /// <summary>
        /// Appends a line to stringBuilder indicating a serializer could not be found.
        /// </summary>
        public static string GetMissingSerializerComment(bool deltaSerializer, ITypeSymbol typeSymbol, IFieldSymbol? fieldSymbol = null)
        {
            if (deltaSerializer)
                return $"//Delta serializer not found for {typeSymbol.ToReadable(fieldSymbol)}; full serializer will be used.";
            else
                return $"//Serializer not found for {typeSymbol.ToReadable(fieldSymbol)}. Value will not be serialized.";
        }

        /// <summary>
        /// Calls WriterPool to return a pooled writer.
        /// </summary>
        /// <param name="writerVariableName">Variable name result of </param>
        public static string CallGetPooledWriter(out string writerVariableName, string variablePrefix = "", bool closeCall = true)
        {
            _stringBuilder.Clear();
            writerVariableName = $"{variablePrefix}pooledWriter";
            _stringBuilder.Append($"{_stringBuilder.ToString()}{FishNetConstants.PooledWriter_FullName} {writerVariableName} = {FishNetConstants.WriterPool_Retrieve_Name}()");
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

        public static string CallWriterPosition(string writerName, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegment())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_Position_Name}");

            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        public static string CallWriteArraySegment(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegment())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteArraySegment_Name}(" + $"{otherWriterA}.{FishNetConstants.Writer_GetArraySegment_Name}())");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        public static string CallWriteArraySegmentAndSize(string writerName, string otherWriterA, bool closeCall = true)
        {
            _stringBuilder.Clear();

            //writer.WriteArraySegment(otherWriteA.GetArraySegmentAndSize())
            _stringBuilder.Append($"{writerName}.{FishNetConstants.Writer_WriteArraySegmentAndSize_Name}(" + $"{otherWriterA}.{FishNetConstants.Writer_GetArraySegment_Name}())");
            if (closeCall)
                _stringBuilder.Append(';');

            return _stringBuilder.ToString();
        }

        public static SerializerMethodContent CreatePublicRuntimeInitializeOnLoadMethod(int indent, string methodName)
        {
            StringBuilder sb = new();
            sb.AppendLine(indent, $"[{UnityConstants.RuntimeInitializeOnLoadMethod_FullName}]");
            sb.Append(indent, $"public static void {methodName}()");

            return new SerializerMethodContent(sb);
        }
    }
}