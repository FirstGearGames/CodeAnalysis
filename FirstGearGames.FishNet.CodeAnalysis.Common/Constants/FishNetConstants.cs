using FishNetTypes.Managing.Logging;
using FishNetTypes.Object;
using FishNetTypes.Transporting;

namespace FirstGearGames.FishNet.CodeAnalysis.Constants
{
    public class FishNetConstants
    {
        #region Namespaces and assemblies.
        /// <summary>
        /// FishNet.Object namespace.
        /// </summary>
        public static readonly string Object_Namespace = "FishNet.Object";
        /// <summary>
        /// FishNet.Object.Synchronizing namespace.
        /// </summary>
        public static readonly string Object_Synchronizing_Namespace = $"{Object_Namespace}.Synchronizing";
        /// <summary>
        /// FishNet.Serializing namespace.
        /// </summary>
        public static readonly string Serializing_Namespace = "FishNet.Serializing";
        /// <summary>
        /// FishNet.CodeGenerating namespace.
        /// </summary>
        public static readonly string CodeGenerating_Namespace = "FishNet.CodeGenerating";
        /// <summary>
        /// FishNet.Runtime assembly.
        /// </summary>
        public static readonly string Runtime_Assembly_Name = "FishNet.Runtime";
        /// <summary>
        /// FishNet.Object.Prediction namespace.
        /// </summary>
        public static readonly string Prediction_Namespace = "FishNet.Object.Prediction";
        /// <summary>
        /// FishNet.Broadcast namespace.
        /// </summary>
        public static readonly string Broadcast_Namespace = "FishNet.Broadcast";
        /// <summary>
        /// FishNet.Managing.Logging namespace.
        /// </summary>
        public static readonly string Logging_Namespace = $"FishNet.Managing.Logging";
        /// <summary>
        /// FishNet.Transporting namespace.
        /// </summary>
        public static readonly string Transporting_Namespace = $"FishNet.Transporting";
        #endregion

        #region Prediction.
        /// <summary>
        /// IReplicate interface.
        /// </summary>
        public static readonly string IReplicateInterface_FullName = $"{Prediction_Namespace}.IReplicateData";
        /// <summary>
        /// IReplicate interface.
        /// </summary>
        public static readonly string IReconcileInterface_FullName = $"{Prediction_Namespace}.IReconcileData";
        #endregion

        #region Codegenerating special.
        /// <summary>
        /// [IncludeSerialization] class.
        /// </summary>
        public static readonly string GenerateSerializersAttribute_FullName = $"{CodeGenerating_Namespace}.IncludeSerializationAttribute";
        /// <summary>
        /// [ExcludeSerialization] class.
        /// </summary>
        public static readonly string ExcludeSerializationAttribute_FullName = $"{CodeGenerating_Namespace}.ExcludeSerializationAttribute";
        #endregion

        #region NetworkBehaviours.
        /// <summary>
        /// NetworkBehaviour class.
        /// </summary>
        public static readonly string NetworkBehaviour_FullName = $"{Object_Namespace}.NetworkBehaviour";
        /// <summary>
        /// Field for NetworkManager.
        /// </summary>
        public static readonly string NetworkManager_Field_Name = $"NetworkManager";
        /// <summary>
        /// Field for base.NetworkManager.
        /// </summary>
        public static readonly string Base_NetworkManager_Field_Name = $"base.{NetworkManager_Field_Name}";
        /// <summary>
        /// Field for base.IsClientInitialized.
        /// </summary>
        public static readonly string Base_IsClient_Initialized_Field_Name = $"base.IsClientInitialized";
        /// <summary>
        /// Field for base.IsServerInitialized.
        /// </summary>
        public static readonly string Base_IsServer_Initialized_Field_Name = $"base.IsServerInitialized";
        #endregion

        #region RPCs.
        /// <summary>
        /// [TargetRpc] class.
        /// </summary>
        public static readonly string TargetRpcAttribute_FullName = $"{Object_Namespace}.TargetRpcAttribute";
        /// <summary>
        /// [ServerRpc] class.
        /// </summary>
        public static readonly string ServerRpcAttribute_FullName = $"{Object_Namespace}.ServerRpcAttribute";
        /// <summary>
        /// ServerRpcAttribute.RequireOwnership name.
        /// </summary>
        public static readonly string ServerRpc_RequireOwnership_FullName = $"RequireOwnership";
        /// <summary>
        /// [ObserversRpc] class.
        /// </summary>
        public static readonly string ObserversRpcAttribute_FullName = $"{Object_Namespace}.ObserversRpcAttribute";
        /// <summary>
        /// RpcAttribute.Logging name.
        /// </summary>
        public static readonly string RpcAttribute_OrderType_Name = $"OrderType";
        /// <summary>
        /// RpcAttribute.Logging name.
        /// </summary>
        public static readonly string RpcAttribute_Logging_Name = $"Logging";
        /// <summary>
        /// SendServerRpc Name.
        /// </summary>
        public static readonly string SendServerRpc_Name = $"SendServerRpc";
        /// <summary>
        /// SendObserversRpc Name.
        /// </summary>
        public static readonly string SendObserversRpc_Name = $"SendObserversRpc";
        /// <summary>
        /// SendObserversRpc Name.
        /// </summary>
        public static readonly string SendTargetRpc_Name = $"SendTargetRpc";
        /// <summary>
        /// Default channel for RPCs.
        /// </summary>
        public const Channel Default_Rpc_Channel = Channel.Reliable;
        /// <summary>
        /// DataOrderType enum.
        /// </summary>
        public static string DataOrderType_FullName = $"{Object_Namespace}.{nameof(DataOrderType)}";
        /// <summary>
        /// Default DataOrderType enum value.
        /// </summary>
        public const DataOrderType Default_DataOrderType = DataOrderType.Default;
        #endregion

        #region Broadcasts.
        /// <summary>
        /// IBroadcast interface.
        /// </summary>
        public static readonly string IBroadcasts_FullName = $"{Broadcast_Namespace}.IBroadcast";
        #endregion

        #region SyncTypes.
        /// <summary>
        /// SyncBase class.
        /// </summary>
        public static readonly string SyncBase_FullName = $"{Object_Synchronizing_Namespace}.SyncBase";
        /// <summary>
        /// SyncDictionary class.
        /// </summary>
        public static readonly string SyncDictionary_FullName = $"{Object_Synchronizing_Namespace}.SyncDictionary";
        /// <summary>
        /// SyncHashSet class.
        /// </summary>
        public static readonly string SyncHashSet_FullName = $"{Object_Synchronizing_Namespace}.SyncHashSet";
        /// <summary>
        /// SyncList class.
        /// </summary>
        public static readonly string SyncList_FullName = $"{Object_Synchronizing_Namespace}.SyncList";
        /// <summary>
        /// SyncVar class.
        /// </summary>
        public static readonly string SyncVar_FullName = $"{Object_Synchronizing_Namespace}.SyncVar";
        /// <summary>
        /// ICustomSync interface.
        /// </summary>
        public static readonly string ICustomSync_FullName = $"{Object_Synchronizing_Namespace}.ICustomSync";
        /// <summary>
        /// GetSerializedType() method.
        /// </summary>
        public static readonly string ICustomSync_GetSerializedType_Name = $"GetSerializedType";
        #endregion

        #region DeltaSerializing Classes and Types.
        /// <summary>
        /// Maximum flag value within DeltaSerializerOption.
        /// </summary>
        public const ulong DeltaSerializerOption_MaxValue = 2;
        /// <summary>
        /// DeltaSerializerOption enum.
        /// </summary>
        public static readonly string DeltaSerializerOption_FullName = $"{Serializing_Namespace}.{DeltaSerializerOption_Name}";
        /// <summary>
        /// DeltaSerializerOption enum.
        /// </summary>
        public static readonly string DeltaSerializerOption_Name = $"DeltaSerializerOption";
        /// <summary>
        /// DeltaSerializerOption.Unset.
        /// </summary>
        public static readonly string DeltaSerializerOption_Unset_FullName = $"{DeltaSerializerOption_FullName}.Unset";
        /// <summary>
        /// DeltaSerializerOption.FullSerialize.
        /// </summary>
        public static readonly string DeltaSerializerOption_FullSerialize_FullName = $"{DeltaSerializerOption_FullName}.FullSerialize";
        /// <summary>
        /// DeltaSerializerOption.RootSerialize.
        /// </summary>
        public static readonly string DeltaSerializeOption_RootSerialize_FullName = $"{DeltaSerializerOption_FullName}.RootSerialize";
        /// <summary>
        /// GenericDeltaWriter class.
        /// </summary>
        public static readonly string GenericDeltaWriter_FullName = $"{Serializing_Namespace}.GenericDeltaWriter";
        /// <summary>
        /// GenericWriter class.
        /// </summary>
        public static readonly string GenericWriter_FullName = $"{Serializing_Namespace}.GenericWriter";
        /// <summary>
        /// GenericDeltaWriter.SetWrite method.
        /// </summary>
        public static readonly string GenericDeltaWriter_SetWrite_Name = $"SetWrite";
        /// <summary>
        /// GenericDeltaReader class.
        /// </summary>
        public static readonly string GenericDeltaReader_FullName = $"{Serializing_Namespace}.GenericDeltaReader";
        /// <summary>
        /// GenericReader class.
        /// </summary>
        public static readonly string GenericReader_FullName = $"{Serializing_Namespace}.GenericReader";
        /// <summary>
        /// GenericDeltaReader.SetRead method.
        /// </summary>
        public static readonly string GenericDeltaReader_SetRead_Name = $"SetRead";
        /// <summary>
        /// GenericReader.SetRead method.
        /// </summary>
        public static readonly string GenericReader_SetRead_Name = $"SetRead";
        #endregion

        #region Writer.
        ///// <summary>
        ///// Writer.WriteBytes method.
        ///// </summary>
        //public static readonly string Writer_WriteUInt8Array_Name = "WriteUInt8Array";
        /// <summary>
        /// Writer.GetBuffer() property.
        /// </summary>
        public static readonly string Writer_GetBuffer_Name = "GetBuffer";
        /// <summary>
        /// Writer.Write() method.
        /// </summary>
        public static readonly string Writer_Write_Name = "Write";
        /// <summary>
        /// Writer.WriteDelta() method.
        /// </summary>
        public static readonly string Writer_WriteDelta_Name = "WriteDelta";
        /// <summary>
        /// Writer.WriteArraySegmentAndSize() method.
        /// </summary>
        public static readonly string Writer_WriteArraySegmentAndSize_Name = "WriteArraySegmentAndSize";
        /// <summary>
        /// Writer.WriteArraySegment() method.
        /// </summary>
        public static readonly string Writer_WriteArraySegment_Name = "WriteArraySegment";
        /// <summary>
        /// Writer.GetArraySegment() method.
        /// </summary>
        public static readonly string Writer_GetArraySegment_Name = "GetArraySegment";
        /// <summary>
        /// Writer.Length property.
        /// </summary>
        public static readonly string Writer_Length_Name = "Length";
        /// <summary>
        /// Writer.Position property.
        /// </summary>
        public static readonly string Writer_Position_Name = "Position";
        /// <summary>
        /// PooledWriter.Store method.
        /// </summary>
        public static readonly string PooledWriter_Store_Name = "Store";
        /// <summary>
        /// PooledWriter class.
        /// </summary>
        public static readonly string PooledWriter_FullName = $"{Serializing_Namespace}.PooledWriter";
        /// <summary>
        /// WriterPool.Retrieve method.
        /// </summary>
        public static readonly string WriterPool_Retrieve_Name = $"{Serializing_Namespace}.WriterPool.Retrieve";
        /// <summary>
        /// Writer class.
        /// </summary>
        public static readonly string Writer_FullName = $"{Serializing_Namespace}.Writer";
        /// <summary>
        /// Writer.WriteUnsignedPackedWhole method.
        /// </summary>
        public static readonly string Writer_WriteUnsignedPackedWhole_Name = "WriteUnsignedPackedWhole";
        /// <summary>
        /// [DefaultWriter] class.
        /// </summary>
        public static readonly string DefaultWriterAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultWriterAttribute";
        /// <summary>
        /// [DefaultDeltaWriter] class.
        /// </summary>
        public static readonly string DefaultDeltaWriterAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultDeltaWriterAttribute";
        /// <summary>
        /// Prefix to use for all generated writers.
        /// </summary>
        public static readonly string GeneratedWriterPrefix = "GWrite___";
        #endregion

        #region Reader.
        /// <summary>
        /// Reader.Read() method.
        /// </summary>
        public static readonly string Reader_Read_Name = "Read";
        /// <summary>
        /// Reader.Read() method.
        /// </summary>
        public static readonly string Reader_ReadDelta_Name = "ReadDelta";
        /// <summary>
        /// Writer class.
        /// </summary>
        public static readonly string Reader_FullName = $"{Serializing_Namespace}.Reader";
        /// <summary>
        /// Writer.WriteUnsignedPackedWhole method.
        /// </summary>
        public static readonly string Reader_ReadUnsignedPackedWhole_Name = "ReadUnsignedPackedWhole";
        /// <summary>
        /// Reader.ReadArrayAllocated method.
        /// </summary>
        public static readonly string Reader_ReadArrayAllocated_Name = "ReadArrayAllocated";
        /// <summary>
        /// [DefaultWriter] class.
        /// </summary>
        public static readonly string DefaultReaderAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultReaderAttribute";
        /// <summary>
        /// [DefaultDeltaWriter] class.
        /// </summary>
        public static readonly string DefaultDeltaReaderAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultDeltaReaderAttribute";
        /// <summary>
        /// Prefix to use for all generated readers.
        /// </summary>
        public static readonly string GeneratedReaderPrefix = "GRead___";
        #endregion

        #region Logging.
        /// <summary>
        /// LoggingType enum.
        /// </summary>
        public static readonly string LoggingType_FullName = $"{Logging_Namespace}.{nameof(LoggingType)}";
        /// <summary>
        /// LoggingType value to use when none is specified.
        /// </summary>
        public const LoggingType Default_LoggingType = LoggingType.Warning;
        #endregion

        #region General.
        /// <summary>
        /// FastContains method.
        /// </summary>
        public static readonly string FastContains_Name = "FastContains";
        /// <summary>
        /// NetworkConnection class.
        /// </summary>
        public static readonly string NetworkConnection_FullName = "FishNet.Connection.NetworkConnection";
        /// <summary>
        /// Channel enum.
        /// </summary>
        public static readonly string Channel_FullName = $"{Transporting_Namespace}.{nameof(Channel)}";
        /// <summary>
        /// NetworkManagerExtensions class.
        /// </summary>
        public static readonly string NetworkManagerExtensions_Name = "NetworkManagerExtensions";
        #endregion
    }
}