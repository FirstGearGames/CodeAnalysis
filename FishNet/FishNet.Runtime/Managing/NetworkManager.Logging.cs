#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
using FishNet.Managing.Logging;
using System.Runtime.CompilerServices;

namespace FishNet.Managing
{
    public partial class NetworkManager
    {
    }

    public static class NetworkManagerExtensions
    {
        /// <summary>
        /// Performs a log using the loggingType, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(this NetworkManager networkManager, LoggingType loggingType, string value)
        {
        }

        /// <summary>
        /// Performs a common log, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(this NetworkManager networkManager, string message)
        {
        }

        /// <summary>
        /// Performs a warning log, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(this NetworkManager networkManager, string message)
        {
        }

        /// <summary>
        /// Performs an error log, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(this NetworkManager networkManager, string message)
        {
        }


        #region Backwards compatibility.

        /// <summary>
        /// Performs a common log, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Log(string msg) => NetworkManagerExtensions.Log(null, msg);

        /// <summary>
        /// Performs a warning log, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogWarning(string msg) => NetworkManagerExtensions.LogWarning(null, msg);

        /// <summary>
        /// Performs an error log, should logging settings permit it.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string msg) => NetworkManagerExtensions.LogError(null, msg);

        #endregion
    }
}