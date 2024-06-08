
using System;

namespace FishNet.CodeGenerating
{
    /// <summary>
    /// Allows a SyncType to be mutable.
    /// </summary>
    public class AllowMutableSyncTypeAttribute : Attribute
    {
    }

    /// <summary>
    /// Type will be included in auto serializer creation.
    /// </summary>
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Struct), Inherited = true, AllowMultiple = false)]
    public class IncludeSerializationAttribute : Attribute
    {
    }

    /// <summary>
    /// Type will be excluded from auto serializer creation.
    /// </summary>
    public class ExcludeSerializationAttribute : Attribute
    {
    }
}