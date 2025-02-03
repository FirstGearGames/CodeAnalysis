using System.Collections.Generic;
using System.Text;

namespace FirstGearGames.FishNet.CodeAnalysis.Misc
{
    public static class PerformanceHelper
    {
        private static Stack<StringBuilder> _stringBuilders = new();

        public static StringBuilder RetrieveStringBuilder()
        {
            if (_stringBuilders.Count == 0)
            {
                return new();
            }
            else
            {
                StringBuilder sb = _stringBuilders.Pop();
                sb.Clear();
                return sb;
            }
        }

        public static void StoreStringBuilder(StringBuilder sb)
        {
            _stringBuilders.Push(sb);
        }
    }
}