using System;

namespace FirstGearGames.FishNet.CodeAnalysis.Analyzers.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Returns the highest numeric value for T.
        /// </summary>
        public static int GetHighestValue<T>() where T : Enum
        {
            Type enumType = typeof(T);
            /* Brute force enum values.
             * Linq Last/Max lookup throws for IL2CPP. */
            int highestValue = 0;
            Array pidValues = Enum.GetValues(enumType);
            foreach (T pid in pidValues)
            {
                object obj = Enum.Parse(enumType, pid.ToString());
                int value = Convert.ToInt32(obj);
                highestValue = Math.Max(highestValue, value);
            }

            return highestValue;
        }
    }
}
