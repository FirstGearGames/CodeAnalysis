using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CodeAnalysis.Logging
{
    public static class CodeAnalysisLogger
    {
        private static readonly List<string> _messages = new();
        private static string _outputFilePath;

        public static void SetOutputPath(string outputFilePath) => _outputFilePath = outputFilePath;
        // private string GetTypePrefix<T0>() => $"[{typeof(T0).FullName}]";
        // public void LogInformation<T0>(string message) => LogInformation($"{GetTypePrefix<T0>()}{message}");
        // public void LogWarning<T0>(string message) => LogWarning($"{GetTypePrefix<T0>()}{message}");
        // public void LogError<T0>(string message) => LogError($"{GetTypePrefix<T0>()}{message}");

        // public void LogCode<T0>(string message)
        // {
        //     message = Environment.NewLine + GetTypePrefix<T0>() + Environment.NewLine + message;
        //     LogCode(message);
        // }
        public static void LogInformation(string message) => _messages.Add($"Information: {message}");
        public static void LogWarning(string message) => _messages.Add($"Warning: {message}");
        public static void LogError(string message) => _messages.Add($"Error: {message}");

        public static void LogCode(string message)
        {
            _messages.Add($"");
            _messages.Add(message);
            _messages.Add($"");
        }

        public static void WriteToFile()
        {
            #pragma warning disable RS1035

            if (!string.IsNullOrWhiteSpace(_outputFilePath))
            {
                try
                {
                    DateTime startTime = DateTime.Now;
                    File.Delete(_outputFilePath);

                    while (File.Exists(_outputFilePath))
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

                File.WriteAllLines(_outputFilePath, _messages);
            }

            _messages.Clear();

            #pragma warning restore RS1035
        }
    }
}