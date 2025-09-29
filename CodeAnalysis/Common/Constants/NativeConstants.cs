using System.Collections.Generic;

namespace CodeAnalysis.Common.Constants
{
    public static class NativeConstants
    {
        public static readonly string Func_FullName = "System.Func";
        public static readonly string Action_FullName = "System.Action";
        public static readonly string Boolean_FullName = "System.Boolean";
        public static readonly string NonSerializedAttribute_FullName = "System.NonSerializedAttribute";
        public static readonly string UInt64_FullName = "System.UInt64";
        public static readonly string Object_FullName = "System.Object";
        public static readonly string LineFeed = "\r\n";
        public static readonly string FirstGenericParameter_Name = $"{GenericParameterName_Prefix}0";
        public static readonly string GenericParameterName_Prefix = "T";
        public static readonly string GenericArray_FullName = $"{FirstGenericParameter_Name}[]";
        public static readonly string List_FullName = "System.Collections.Generic.List";
        public static readonly string GenericList_FullName = $"{List_FullName}<{FirstGenericParameter_Name}>";
    }
}