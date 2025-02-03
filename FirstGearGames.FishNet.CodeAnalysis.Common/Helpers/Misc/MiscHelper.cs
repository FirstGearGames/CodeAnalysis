
using FirstGearGames.CodeAnalysis.Helpers;
using FishNetTypes.Managing.Logging;

namespace FirstGearGames.FishNet.CodeAnalysis.Misc
{

    public static class MiscHelper
    {
        
        /// <summary>
        /// Returns which Log method to use for a LoggingType.
        /// </summary>
        /// <remarks>string.empty is returned if type is not supported.</remarks>
        public static string GetLoggingMethodName(this LoggingType loggingType) 
        {
            return loggingType switch
            {
                LoggingType.Common => "Log",
                LoggingType.Warning => "LogWarning",
                LoggingType.Error => "LogError",
                _ => string.Empty
            };            
        }

        /// <summary>
        /// Calls a logging method. 
        /// </summary>
        public static string CreateLog(this LoggingType loggingType, string callSource, string text)
        {
            string loggingMethodName = loggingType.GetLoggingMethodName();
            if (loggingMethodName == string.Empty) return string.Empty;

            return $"{callSource}.{loggingMethodName}(\"{text}\");";
        }
        
        private static void Log(string txt)
        {
            if (txt.Length == 0)
                Debugg.Log(txt);
            else
                Debugg.Log($"   [MiscHelper] {txt}");
        }

    }
}