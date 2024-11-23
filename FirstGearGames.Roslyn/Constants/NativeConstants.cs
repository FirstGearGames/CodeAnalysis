using System.Collections.Generic;

namespace FirstGearGames.Roslyn.Native.Constants
{
    public static class NativeConstants
    {
        public const string Func_FullName = "System.Func";
        public const string Action_FullName = "System.Action";
        public const string Boolean_FullName = "System.Boolean";
        public const string NonSerializedAttribute_FullName = "System.NonSerializedAttribute";
        public const string UInt64_FullName = "System.UInt64";
        public const string LineFeed = "\r\n";
        public const string GeneralParameter_Name = "T0";
        public const string GenericArray_FullName = $"{GeneralParameter_Name}[]";

        public const string List_FullName = "System.Collections.Generic.List";
        public const string GenericList_FullName = $"{List_FullName}<{GeneralParameter_Name}>";
    }
}