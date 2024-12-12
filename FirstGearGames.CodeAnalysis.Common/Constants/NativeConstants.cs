using System.Collections.Generic;

namespace FirstGearGames.CodeAnalysis.Constants
{
    public static class NativeConstants
    {
        public const string Func_FullName = "System.Func";
        public const string Action_FullName = "System.Action";
        public const string Boolean_FullName = "System.Boolean";
        public const string NonSerializedAttribute_FullName = "System.NonSerializedAttribute";
        public const string UInt64_FullName = "System.UInt64";
        public const string Object_FullName = "System.Object";
        public const string LineFeed = "\r\n";
        public const string FirstGenericParameter_Name = $"{GenericParameterName_Prefix}0";
        public const string GenericParameterName_Prefix = "T";
        public const string GenericArray_FullName = $"{FirstGenericParameter_Name}[]";
        public const string List_FullName = "System.Collections.Generic.List";
        public const string GenericList_FullName = $"{List_FullName}<{FirstGenericParameter_Name}>";
    }
}