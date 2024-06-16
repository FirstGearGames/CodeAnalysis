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
        /// <summary>
        /// Writer.WriteBytes method.
        /// </summary>
        public const string Writer_WriteBytes_Name = "WriteBytes";

        /// <summary>
        /// Writer.GetBuffer() property.
        /// </summary>
        public const string Writer_GetBuffer_Name = "GetBuffer";

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
        /// [Writer] class.
        /// </summary>
        public const string WriterAttribute_FullName = $"{Serializing_Namespace}.WriterAttribute";

        /// <summary>
        /// [DeltaWriter] class.
        /// </summary>
        public const string DeltaWriterAttribute_FullName = $"{Serializing_Namespace}.DeltaWriterAttribute";
        #endregion
        
        
        #region Reader.
        // /// <summary>
        // /// Writer.WriteBytes method.
        // /// </summary>
        // public const string Writer_WriteBytes_Name = "WriteBytes";
        //
        // /// <summary>
        // /// Writer.GetBuffer() property.
        // /// </summary>
        // public const string Writer_GetBuffer_Name = "GetBuffer";
        //
        // /// <summary>
        // /// Writer.Length property.
        // /// </summary>
        // public const string Writer_Length_Name = "Length";
        //
        // /// <summary>
        // /// PooledWriter.Store method.
        // /// </summary>
        // public const string PooledWriter_Store_Name = "Store";
        //
        // /// <summary>
        // /// PooledWriter class.
        // /// </summary>
        // public const string PooledWriter_FullName = $"{Serializing_Namespace}.PooledWriter";
        //
        // /// <summary>
        // /// WriterPool.Retrieve method.
        // /// </summary>
        // public const string WriterPool_Retrieve_Name = $"{Serializing_Namespace}.WriterPool.Retrieve";
        //
        /// <summary>
        /// Writer class.
        /// </summary>
        public const string Reader_FullName = $"{Serializing_Namespace}.Reader";
        //
        // /// <summary>
        // /// Writer.WriteUnsignedPackedWhole method.
        // /// </summary>
        // public const string Writer_WriteUnsignedPackedWhole_Name = "WriteUnsignedPackedWhole";
        //
        // /// <summary>
        // /// [Writer] class.
        // /// </summary>
        // public const string WriterAttribute_FullName = $"{Serializing_Namespace}.WriterAttribute";
        //
        // /// <summary>
        // /// [DeltaWriter] class.
        // /// </summary>
        // public const string DeltaWriterAttribute_FullName = $"{Serializing_Namespace}.DeltaWriterAttribute";
        #endregion

        
    }
}