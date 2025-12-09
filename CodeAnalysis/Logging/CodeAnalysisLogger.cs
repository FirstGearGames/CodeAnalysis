using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CodeBoost.Logging;

namespace CodeAnalysis.Logging
{

    public class CodeAnalysisLogger: ILogger
    {
        private readonly LoggerSetting _loggerSettings = new();
        private readonly List<string> _messages = new();
        private string _outputFilePath;
        
        public void SetOutputPath(string outputFilePath) => _outputFilePath = outputFilePath;
        public LoggerSetting GetLoggerSetting() => _loggerSettings;
        public bool DisableUnconditionalDevelopmentStacktrace() => true;
        // private string GetTypePrefix<T0>() => $"[{typeof(T0).FullName}]";
        // public void LogInformation<T0>(string message) => LogInformation($"{GetTypePrefix<T0>()}{message}");
        // public void LogWarning<T0>(string message) => LogWarning($"{GetTypePrefix<T0>()}{message}");
        // public void LogError<T0>(string message) => LogError($"{GetTypePrefix<T0>()}{message}");

        // public void LogCode<T0>(string message)
        // {
        //     message = Environment.NewLine + GetTypePrefix<T0>() + Environment.NewLine + message;
        //     LogCode(message);
        // }

        public void LogInformation(string message) => _messages.Add($"Information: {message}");
        public void LogWarning(string message) => _messages.Add($"Warning: {message}");
        public void LogError(string message) => _messages.Add($"Error: {message}");

        public void LogCode(string message)
        {
            _messages.Add($"");
            _messages.Add(message);
            _messages.Add($"");
        }

        public void WriteToFile() 
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