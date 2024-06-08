namespace SourceGenerating.Constants
{
    internal class FishNetConstants
    {
        //Namespaces.
        public const string Serializing_Namespace = "FishNet.Serializing";  
        //Assemblies.
        public const string Runtime_Assembly_Name = "FishNet.Runtime";
        //Attributes.
        public const string ExcludeSerializationAttribute_FullName = "FishNet.CodeGenerating.ExcludeSerializationAttribute";
        
        //RPCs
        public const string TargetRpcAttribute_FullName = "FishNet.Object.TargetRpcAttribute";
        public const string ServerRpcAttribute_FullName = "FishNet.Object.ServerRpcAttribute";
        public const string ObserversRpcAttribute_FullName = "FishNet.Object.ObserversRpcAttribute";        

        //Writer.
        public const string Writer_WriteBytes_Name = "WriteBytes";
        public const string Writer_GetBuffer_Name = "GetBuffer";
        public const string Writer_Length_Name = "Length";

        public const string PooledWriter_Store_Name = "Store";

        //Writer pool.
        public const string PooledWriter_FullName = $"{Serializing_Namespace}.PooledWriter";
        public const string WriterPool_Retrieve_Name = $"{Serializing_Namespace}.WriterPool.Retrieve";


        public const string WriteDelta_WriterParameter_FullName = "writer";
        public const string WriteDelta_ParameterA_Name = "valueA";
        public const string WriteDelta_ParameterB_Name = "valueB";


        public const string Writer_FullName = $"{Serializing_Namespace}.Writer";
        public const string Writer_WritePackedWhole_Name = "WritePackedWhole";
        public const string WriterAttribute_FullName = $"{Serializing_Namespace}.WriterAttribute";
        public const string DeltaWriterAttribute_FullName = $"{Serializing_Namespace}.DeltaWriterAttribute";
    }
}