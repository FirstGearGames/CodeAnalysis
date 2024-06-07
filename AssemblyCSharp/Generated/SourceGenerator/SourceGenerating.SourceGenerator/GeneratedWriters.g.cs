namespace GenerateTest
{
	public static class GeneratedWriters
	{
// Creating DeltaWriter for ClientAssembly.MyStruct
   // Type ClientAssembly.MyStruct is supported.
   // Already created System.Int32
       //Member fullName is System.Int32, IntValue
		public static void WriteDelta_ClientAssembly_MyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.MyStruct valueA,  ClientAssembly.MyStruct valueB)
		{

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();
			if (pooledWriter.WriteDeltaInt32(valueA.IntValue, valueB.IntValue))

		}

	}
}
