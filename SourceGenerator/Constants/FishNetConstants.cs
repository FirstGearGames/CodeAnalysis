namespace SourceGenerating.Constants
{
    internal class FishNetConstants
    {
        #region Misc.

        /// <summary>
        /// FishNet.Serializing namespace.
        /// </summary>
        public const string Serializing_Namespace = "FishNet.Serializing";

        /// <summary>
        /// FishNet.Runtime assembly.
        /// </summary>
        public const string Runtime_Assembly_Name = "FishNet.Runtime";

        /// <summary>
        /// [ExcludeSerialization] class.
        /// </summary>
        public const string ExcludeSerializationAttribute_FullName = "FishNet.CodeGenerating.ExcludeSerializationAttribute";

        #endregion

        #region RPCs.

        /// <summary>
        /// [targetRpc] class.
        /// </summary>
        public const string TargetRpcAttribute_FullName = "FishNet.Object.TargetRpcAttribute";

        /// <summary>
        /// [ServerRpc] class.
        /// </summary>
        public const string ServerRpcAttribute_FullName = "FishNet.Object.ServerRpcAttribute";

        /// <summary>
        /// [ObserversRpc] class.
        /// </summary>
        public const string ObserversRpcAttribute_FullName = "FishNet.Object.ObserversRpcAttribute";

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
        public const string DefaultWriterAttribute_FullName = $"{Serializing_Namespace}.DefaultWriterAttribute";

        /// <summary>
        /// [DefaultDeltaWriter] class.
        /// </summary>
        public const string DefaultDeltaWriterAttribute_FullName = $"{Serializing_Namespace}.DefaultDeltaWriterAttribute";
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
        /// [DefaultWriter] class.
        /// </summary>
        public const string DefaultReaderAttribute_FullName = $"{Serializing_Namespace}.DefaultReaderAttribute";

        /// <summary>
        /// [DefaultDeltaWriter] class.
        /// </summary>
        public const string DefaultDeltaReaderAttribute_FullName = $"{Serializing_Namespace}.DefaultDeltaReaderAttribute";
        #endregion


    }
}