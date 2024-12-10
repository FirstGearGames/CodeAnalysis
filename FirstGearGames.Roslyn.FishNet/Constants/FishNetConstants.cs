namespace FirstGearGames.Roslyn.FishNet.Constants
{
    public class FishNetConstants
    {
        #region Namespaces and assemblies.
        /// <summary>
        /// FishNet.Object namespace.
        /// </summary>
        public const string Object_Namespace = "FishNet.Object";
        /// <summary>
        /// FishNet.Object.Synchronizing namespace.
        /// </summary>
        public const string Object_Synchronizing_Namespace = $"{Object_Namespace}.Synchronizing";
        /// <summary>
        /// FishNet.Serializing namespace.
        /// </summary>
        public const string Serializing_Namespace = "FishNet.Serializing";
        /// <summary>
        /// FishNet.CodeGenerating namespace.
        /// </summary>
        public const string CodeGenerating_Namespace = "FishNet.CodeGenerating";
        /// <summary>
        /// FishNet.Runtime assembly.
        /// </summary>
        public const string Runtime_Assembly_Name = "FishNet.Runtime";
        /// <summary>
        /// FishNet.Object.Prediction namespace.
        /// </summary>
        public const string Prediction_Namespace = "FishNet.Object.Prediction";
        /// <summary>
        /// FishNet.Broadcast namespace.
        /// </summary>
        public const string Broadcast_Namespace = "FishNet.Broadcast";
        /// <summary>
        /// FishNet.Managing.Logging namespace.
        /// </summary>
        public const string Logging_Namespace = $"FishNet.Managing.Logging";
        #endregion

        #region Prediction.
        /// <summary>
        /// IReplicate interface.
        /// </summary>
        public const string IReplicateInterface_FullName = $"{Prediction_Namespace}.IReplicateData";
        /// <summary>
        /// IReplicate interface.
        /// </summary>
        public const string IReconcileInterface_FullName = $"{Prediction_Namespace}.IReconcileData";
        #endregion

        #region Codegenerating special.

        /// <summary>
        /// [IncludeSerialization] class.
        /// </summary>
        public const string GenerateSerializersAttribute_FullName = $"{CodeGenerating_Namespace}.IncludeSerializationAttribute";
        /// <summary>
        /// [ExcludeSerialization] class.
        /// </summary>
        public const string ExcludeSerializationAttribute_FullName = $"{CodeGenerating_Namespace}.ExcludeSerializationAttribute";

        #endregion
        
        #region NetworkBehaviours.
        /// <summary>
        /// NetworkBehaviour class.
        /// </summary>
        public const string NetworkBehaviour_FullName = $"{Object_Namespace}.NetworkBehaviour";
        #endregion

        #region RPCs.

        /// <summary>
        /// [TargetRpc] class.
        /// </summary>
        public const string TargetRpcAttribute_FullName = $"{Object_Namespace}.TargetRpcAttribute";

        /// <summary>
        /// [ServerRpc] class.
        /// </summary>
        public const string ServerRpcAttribute_FullName = $"{Object_Namespace}.ServerRpcAttribute";
        /// <summary>
        /// ServerRpcAttribute.RequireOwnership name.
        /// </summary>
        public const string ServerRpc_RequireOwnership_FullName = $"RequireOwnership";

        /// <summary>
        /// [ObserversRpc] class.
        /// </summary>
        public const string ObserversRpcAttribute_FullName = $"{Object_Namespace}.ObserversRpcAttribute";

        /// <summary>
        /// RpcAttribute.Logging name.
        /// </summary>
        public const string RpcAttribute_Logging_Name = $"Logging";
        #endregion
        
        #region Broadcasts.
        /// <summary>
        /// IBroadcast interface.
        /// </summary>
        public const string IBroadcasts_FullName = $"{Broadcast_Namespace}.IBroadcast";
        #endregion

        #region SyncTypes.
        /// <summary>
        /// SyncBase class.
        /// </summary>
        public const string SyncBase_FullName = $"{Object_Synchronizing_Namespace}.SyncBase";
        /// <summary>
        /// SyncDictionary class.
        /// </summary>
        public const string SyncDictionary_FullName =$"{Object_Synchronizing_Namespace}.SyncDictionary";
        /// <summary>
        /// SyncHashSet class.
        /// </summary>
        public const string SyncHashSet_FullName =$"{Object_Synchronizing_Namespace}.SyncHashSet";
        /// <summary>
        /// SyncList class.
        /// </summary>
        public const string SyncList_FullName =$"{Object_Synchronizing_Namespace}.SyncList";
        /// <summary>
        /// SyncVar class.
        /// </summary>
        public const string SyncVar_FullName =$"{Object_Synchronizing_Namespace}.SyncVar";
        /// <summary>
        /// ICustomSync interface.
        /// </summary>
        public const string ICustomSync_FullName = $"{Object_Synchronizing_Namespace}.ICustomSync";
        /// <summary>
        /// GetSerializedType() method.
        /// </summary>
        public const string ICustomSync_GetSerializedType_Name = $"GetSerializedType";
        #endregion
        
        #region DeltaSerializing.

        /// <summary>
        /// Maximum flag value within DeltaSerializerOption.
        /// </summary>
        public const ulong DeltaSerializerOption_MaxValue = 2;
        /// <summary>
        /// DeltaSerializerOption enum.
        /// </summary>
        public const string DeltaSerializerOption_FullName = $"{Serializing_Namespace}.{DeltaSerializerOption_Name}";
        /// <summary>
        /// DeltaSerializerOption enum.
        /// </summary>
        public const string DeltaSerializerOption_Name = $"DeltaSerializerOption";
        /// <summary>
        /// DeltaSerializerOption.Unset.
        /// </summary>
        public const string DeltaSerializerOption_Unset_FullName = $"{DeltaSerializerOption_FullName}.Unset";
        /// <summary>
        /// DeltaSerializerOption.FullSerialize.
        /// </summary>
        public const string DeltaSerializerOption_FullSerialize_FullName = $"{DeltaSerializerOption_FullName}.FullSerialize";
        /// <summary>
        /// DeltaSerializerOption.RootSerialize.
        /// </summary>
        public const string DeltaSerializeOption_RootSerialize_FullName = $"{DeltaSerializerOption_FullName}.RootSerialize";
        /// <summary>
        /// GenericDeltaWriter class.
        /// </summary>
        public const string GenericDeltaWriter_FullName = $"{Serializing_Namespace}.GenericDeltaWriter";
        /// <summary>
        /// GenericWriter class.
        /// </summary>
        public const string GenericWriter_FullName = $"{Serializing_Namespace}.GenericWriter";
        /// <summary>
        /// GenericDeltaWriter.SetWrite method.
        /// </summary>
        public const string GenericDeltaWriter_SetWrite_Name = $"SetWrite";
        /// <summary>
        /// GenericDeltaReader class.
        /// </summary>
        public const string GenericDeltaReader_FullName = $"{Serializing_Namespace}.GenericDeltaReader";
        /// <summary>
        /// GenericReader class.
        /// </summary>
        public const string GenericReader_FullName = $"{Serializing_Namespace}.GenericReader";
        /// <summary>
        /// GenericDeltaReader.SetRead method.
        /// </summary>
        public const string GenericDeltaReader_SetRead_Name = $"SetRead";
        /// <summary>
        /// GenericReader.SetRead method.
        /// </summary>
        public const string GenericReader_SetRead_Name = $"SetRead";

        #endregion

        #region Writer.

        ///// <summary>
        ///// Writer.WriteBytes method.
        ///// </summary>
        //public const string Writer_WriteUInt8Array_Name = "WriteUInt8Array";

        /// <summary>
        /// Writer.GetBuffer() property.
        /// </summary>
        public const string Writer_GetBuffer_Name = "GetBuffer";

        /// <summary>
        /// Writer.Write() method.
        /// </summary>
        public const string Writer_Write_Name = "Write";
        /// <summary>
        /// Writer.WriteArraySegmentAndSize() method.
        /// </summary>
        public const string Writer_WriteArraySegmentAndSize_Name = "WriteArraySegmentAndSize";
        /// <summary>
        /// Writer.WriteArraySegment() method.
        /// </summary>
        public const string Writer_WriteArraySegment_Name = "WriteArraySegment";
        /// <summary>
        /// Writer.GetArraySegment() method.
        /// </summary>
        public const string Writer_GetArraySegment_Name = "GetArraySegment";


        /// <summary>
        /// Writer.Length property.
        /// </summary>
        public const string Writer_Length_Name = "Length";

        /// <summary>
        /// Writer.Position property.
        /// </summary>
        public const string Writer_Position_Name = "Position";
        
        /// <summary>
        /// PooledWriter.Store method.
        /// </summary>
        public const string PooledWriter_Store_Name = "Store";

        /// <summary>
        /// PooledWriter class.
        /// </summary>
        public const string PooledWriter_FullName = $"{Serializing_Namespace}.PooledWriter";

        /// <summary>
        /// WriterPool.Retrieve method.
        /// </summary>
        public const string WriterPool_Retrieve_Name = $"{Serializing_Namespace}.WriterPool.Retrieve";

        /// <summary>
        /// Writer class.
        /// </summary>
        public const string Writer_FullName = $"{Serializing_Namespace}.Writer";

        /// <summary>
        /// Writer.WriteUnsignedPackedWhole method.
        /// </summary>
        public const string Writer_WriteUnsignedPackedWhole_Name = "WriteUnsignedPackedWhole";

        /// <summary>
        /// [DefaultWriter] class.
        /// </summary>
        public const string DefaultWriterAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultWriterAttribute";

        /// <summary>
        /// [DefaultDeltaWriter] class.
        /// </summary>
        public const string DefaultDeltaWriterAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultDeltaWriterAttribute";
        /// <summary>
        /// Prefix to use for all generated writers.
        /// </summary>
        public const string GeneratedWriterPrefix = "GWrite___";

        #endregion

        #region Reader.

        /// <summary>
        /// Writer.Write() method.
        /// </summary>
        public const string Reader_Read_Name = "Read";
        /// <summary>
        /// Writer class.
        /// </summary>
        public const string Reader_FullName = $"{Serializing_Namespace}.Reader";
        /// <summary>
        /// Writer.WriteUnsignedPackedWhole method.
        /// </summary>
        public const string Reader_ReadUnsignedPackedWhole_Name = "ReadUnsignedPackedWhole";
        /// <summary>
        /// Reader.ReadArrayAllocated method.
        /// </summary>
        public const string Reader_ReadArrayAllocated_Name = "ReadArrayAllocated";
        /// <summary>
        /// [DefaultWriter] class.
        /// </summary>
        public const string DefaultReaderAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultReaderAttribute";
        /// <summary>
        /// [DefaultDeltaWriter] class.
        /// </summary>
        public const string DefaultDeltaReaderAttribute_FullName = $"{CodeGenerating_Namespace}.DefaultDeltaReaderAttribute";
        /// <summary>
        /// Prefix to use for all generated readers.
        /// </summary>
        public const string GeneratedReaderPrefix = "GRead___";

        #endregion

        #region Logging.
        /// <summary>
        /// LoggingType enum.
        /// </summary>
        public const string LoggingType_FullName = $"{Logging_Namespace}.LoggingType";
        /// <summary>
        /// LoggingType.Off enum value.
        /// </summary>
        public const string LoggingType_Off_FullName = $"{Logging_Namespace}.LoggingType.Off";
        /// <summary>
        /// LoggingType.Error enum value.
        /// </summary>
        public const string LoggingType_Error_FullName = $"{Logging_Namespace}.LoggingType.Error";
        /// <summary>
        /// LoggingType.Warning enum value.
        /// </summary>
        public const string LoggingType_Warning_FullName = $"{Logging_Namespace}.LoggingType.Warning";
        /// <summary>
        /// LoggingType.Common enum value.
        /// </summary>
        public const string LoggingType_Common_FullName = $"{Logging_Namespace}.LoggingType.Common";
        /// <summary>
        /// LoggingType.Off enum value.
        /// </summary>
        public const string LoggingType_Off_Name = $"LoggingType.Off";
        /// <summary>
        /// LoggingType.Error enum value.
        /// </summary>
        public const string LoggingType_Error_Name = $"LoggingType.Error";
        /// <summary>
        /// LoggingType.Warning enum value.
        /// </summary>
        public const string LoggingType_Warning_Name = $"LoggingType.Warning";
        /// <summary>
        /// LoggingType.Common enum value.
        /// </summary>
        public const string LoggingType_Common_Name = $"LoggingType.Common";
        /// <summary>
        /// LoggingType.Off numeric value.
        /// </summary>
        public const int LoggingType_Off_NumericValue = 0;
        /// <summary>
        /// LoggingType.Error numeric value.
        /// </summary>
        public const int LoggingType_Error_NumericValue = 1;
        /// <summary>
        /// LoggingType.Warning numeric value.
        /// </summary>
        public const int LoggingType_Warning_NumericValue = 2;
        /// <summary>
        /// LoggingType.Common numeric value.
        /// </summary>
        public const int LoggingType_Common_NumericValue = 3;
        /// <summary>
        /// LoggingType value to use when none is specified.
        /// </summary>
        public const int Default_LoggingType_NumericValue = LoggingType_Warning_NumericValue;
        #endregion
        
        #region General.

        /// <summary>
        /// FastContains method.
        /// </summary>
        public const string FastContains_Name = "FastContains";

        #endregion
    }
}