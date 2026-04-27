using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CodeAnalysis.Logging;

/// <summary>
/// Provides a static logger that buffers messages and writes them to per-suffix log files.
/// </summary>
public static class CodeAnalysisLogger
{
    /// <summary>
    /// Messages by output path suffixes.
    /// </summary>
    private static readonly Dictionary<string, List<string>> Messages = [];
    private static string _outputPathPrefix;
    private const string FileExtension = ".txt";

    private static void AddMessage(string fileSuffix, string message)
    {
        if (string.IsNullOrEmpty(fileSuffix))
            fileSuffix = "_";

        if (!Messages.TryGetValue(fileSuffix, out List<string> messages))
        {
            messages = [];
            Messages.Add(fileSuffix, messages);
        }

        messages.Add(message);
    }

    /// <summary>
    /// Sets the output path prefix used when log files are written.
    /// </summary>
    /// <remarks>
    /// The <c>.txt</c> extension should not be included in the supplied path.
    /// </remarks>
    /// <param name="outputFilePath">Path prefix to use when writing log files.</param>
    public static void SetOutputPath(string outputFilePath) => _outputPathPrefix = outputFilePath;

    /// <summary>
    /// Buffers an information-level message for later writing.
    /// </summary>
    /// <param name="message">Message to buffer.</param>
    public static void LogInformation(string message) => AddMessage("", $"Information: {message}");
    /// <summary>
    /// Buffers a warning-level message for later writing.
    /// </summary>
    /// <param name="message">Message to buffer.</param>
    public static void LogWarning(string message) => AddMessage("", $"Warning: {message}");
    /// <summary>
    /// Buffers an error-level message for later writing.
    /// </summary>
    /// <param name="message">Message to buffer.</param>
    public static void LogError(string message) => AddMessage("", $"Error: {message}");

    /// <summary>
    /// Buffers a block of source code surrounded by blank lines for later writing.
    /// </summary>
    /// <param name="message">Source text to buffer.</param>
    public static void LogCode(string message)
    {
        AddMessage("", "");
        AddMessage("", message);
        AddMessage("", "");
    }

    /// <summary>
    /// Buffers a block of source code surrounded by blank lines for later writing under the supplied file suffix.
    /// </summary>
    /// <param name="fileSuffix">Suffix appended to the output path when writing the buffer.</param>
    /// <param name="message">Source text to buffer.</param>
    public static void LogCode(string fileSuffix, string message)
    {
        AddMessage(fileSuffix, "");
        AddMessage(fileSuffix, message);
        AddMessage(fileSuffix, "");
    }

    /// <summary>
    /// Writes every buffered message to its corresponding log file and clears the buffers.
    /// </summary>
    public static void WriteToFile()
    {
        #pragma warning disable RS1035

        foreach (KeyValuePair<string, List<string>> kvp in Messages)
        {
            string fullPath = $"{_outputPathPrefix}{kvp.Key}{FileExtension}";
                
            if (!string.IsNullOrWhiteSpace(fullPath))
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    File.Delete(fullPath);

                    while (File.Exists(fullPath))
                    {
                        Thread.Sleep(100);
                        if ((DateTime.Now - startTime).TotalSeconds > 3)
                            break;
                    }
                }
                catch
                {
                    // ignored
                }

                File.WriteAllLines(fullPath, kvp.Value);
            }
                
            kvp.Value.Clear();
        }


        #pragma warning restore RS1035
    }
}