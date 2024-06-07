using System;
using System.Linq;
using System.Text;
using SourceGenerator.Extensions;

namespace RoslynLearning.Helpers;

public static class CodeBuilder
{
    public const string FishNet_Serializing_Namespace = "FishNet.Serializing";
    public const string PooledWriter_FullName = $"{FishNet_Serializing_Namespace}.PooledWriter";
    private const string WriterPool_Retrieve_Name = $"{FishNet_Serializing_Namespace}.WriterPool.Retrieve()";

    private const string Writer_WriteBytes_Name = "WriteBytes";
    private const string Writer_GetBuffer_Name = "GetBuffer";
    private const string Writer_Length_Name = "Length";
    private const string PooledWriter_Store_Name = "Store";

    private static StringBuilder _stringBuilder = new();

    /// <summary>
    /// Calls WriterPool to return a pooled writer.
    /// </summary>
    /// <param name="writerVariableName">Variable name result of </param>
    public static string CallGetPooledWriter(out string writerVariableName, string variablePrefix = "", bool closeCall = true)
    {
        _stringBuilder.Clear();
        writerVariableName = $"{variablePrefix}pooledWriter";
        _stringBuilder.Append($"{_stringBuilder.ToString()}{PooledWriter_FullName} {writerVariableName} = {WriterPool_Retrieve_Name}");
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
        _stringBuilder.Append($"{writerVariableName}.{PooledWriter_Store_Name}()");
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
        string byteArrayVariable = $"{otherWriterA}.{Writer_GetBuffer_Name}()";

        //public void WriteBytes(byte[] value, int offset, int count)
        _stringBuilder.Append($"{writerName}.{Writer_WriteBytes_Name}(" +
                              $"{byteArrayVariable}, 0, {otherWriterA}.{Writer_Length_Name})");
        if (closeCall)
            _stringBuilder.Append(';');
        
        return _stringBuilder.ToString();
    }
}