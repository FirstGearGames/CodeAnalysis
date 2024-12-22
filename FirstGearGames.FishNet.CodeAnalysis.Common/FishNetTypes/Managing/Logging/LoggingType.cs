using FirstGearGames.FishNet.CodeAnalysis.Constants;

namespace FishNetTypes.Managing.Logging
{
    public static class LoggingTypeExtensions 
    {
        /// <summary>
        /// Returns a LoggingType enum value while containing the enum name (eg: LoggingType.Common).
        /// </summary>
        public static string GetEnumName(this LoggingType loggingType) => $"{FishNetConstants.LoggingType_FullName}.{loggingType.ToString()}";
    }

    /// <summary>
    /// Type of logging being filtered.
    /// </summary>
    public enum LoggingType : byte
    {
        /// <summary>
        /// Disable logging.
        /// </summary>
        Off = 0,
        /// <summary>
        /// Only log errors.
        /// </summary>
        Error = 1,
        /// <summary>
        /// Log warnings and errors.
        /// </summary>
        Warning = 2,
        /// <summary>
        /// Log all common activities, warnings, and errors.
        /// </summary>
        Common = 3
    }
}