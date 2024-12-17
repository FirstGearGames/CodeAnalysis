
using FishNet.Managing.Logging;

namespace FishNet.Object
{
    public enum DataOrderType
    {
        /// <summary>
        /// Data will buffer in the order originally intended.
        /// EG: SyncTypes will always send last, and RPCs will always send in the order they were called.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Data will be attached to the end of the packet.
        /// RPCs can be sent after all SyncTypes by using this value. Multiple RPCs with this order type will send last, in the order they were called.
        /// </summary>
        Last = 1,
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RpcAttribute : Attribute
    {
        /// <summary>
        /// Type of logging to use when the IsServer check fails.
        /// </summary>
        public LoggingType Logging = LoggingType.Warning;
        /// <summary>
        /// True to also run the RPC logic locally.
        /// </summary>
        public bool RunLocally = false;
        /// <summary>
        /// Estimated length of data being sent.
        /// When a value other than -1 the minimum length of the used serializer will be this value.
        /// This is useful for writing large packets which otherwise resize the serializer.
        /// </summary>
        public int DataLength = -1;
        /// <summary>
        /// Order in which to send data for this RPC.
        /// </summary>
        public DataOrderType OrderType = DataOrderType.Default;
    }

    /// <summary>
    /// ServerRpc methods will send messages to the server.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ServerRpcAttribute : RpcAttribute
    {
        /// <summary>
        /// True to only allow the owning client to call this RPC.
        /// </summary>
        public bool RequireOwnership = true;
    }

    /// <summary>
    /// ObserversRpc methods will send messages to all observers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ObserversRpcAttribute : RpcAttribute
    {
        /// <summary>
        /// True to exclude the owner from receiving this RPC.
        /// </summary>
        public bool ExcludeOwner = false;
        /// <summary>
        /// True to prevent the connection from receiving this Rpc if they are also server.
        /// </summary>
        public bool ExcludeServer = false;
        /// <summary>
        /// True to buffer the last value and send it to new players when the object is spawned for them.
        /// RPC will be sent on the same channel as the original RPC, and immediately before the OnSpawnServer override.
        /// </summary>
        public bool BufferLast = false;
    }

    /// <summary>
    /// TargetRpc methods will send messages to a single client.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class TargetRpcAttribute : RpcAttribute
    {
        /// <summary>
        /// True to prevent the connection from receiving this Rpc if they are also server.
        /// </summary>
        public bool ExcludeServer = false;
        /// <summary>
        /// True to validate the target is possible and output debug when not.
        /// Use this field with caution as it may create undesired results when set to false.
        /// </summary>
        public bool ValidateTarget = true;
    }

    /// <summary>
    /// Prevents a method from running if server is not active.
    /// <para>Can only be used inside a NetworkBehaviour</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ServerAttribute : Attribute
    {
        /// <summary>
        /// Type of logging to use when the IsServer check fails.
        /// </summary>
        public LoggingType Logging = LoggingType.Warning;
    }

    /// <summary>
    /// Prevents this method from running if client is not active.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ClientAttribute : Attribute
    {
        /// <summary>
        /// Type of logging to use when the IsClient check fails.
        /// </summary>
        public LoggingType Logging = LoggingType.Warning;
        /// <summary>
        /// True to only allow a client to run the method if they are owner of the object.
        /// </summary>
        public bool RequireOwnership = false;
    }
}


namespace FishNet.Object.Synchronizing
{

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SyncObjectAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SyncVarAttribute : Attribute { }


}

namespace FishNet.CodeGenerating
{

    /// <summary>
    /// Indicates a method is the default writer for a type. The first non-extension parameter indicates the type this writer is for.
    /// This attribute is primarily for internal use and may change at anytime without notice.
    /// </summary>
    public class DefaultWriterAttribute : Attribute { }
    /// <summary>
    /// Indicates a method is the default reader for a type. The return type indicates what type the reader is for.
    /// This attribute is primarily for internal use and may change at anytime without notice.
    /// </summary>
    public class DefaultReaderAttribute : Attribute { }
    
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DefaultDeltaWriterAttribute : Attribute { }
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class DefaultDeltaReaderAttribute : Attribute { }

}