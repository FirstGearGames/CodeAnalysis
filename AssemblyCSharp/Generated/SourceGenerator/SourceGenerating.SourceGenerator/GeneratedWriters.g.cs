namespace GenerateTest
{
	public static class GeneratedWriters
	{
// Creating DeltaWriter for ClientAssembly.MyStruct
   // Type ClientAssembly.MyStruct is supported.
   // Already created System.Int32
       //Member fullName is System.Int32, IntValueA
   // Already created System.Int32
       //Member fullName is System.Int32, IntValueB
   // Already created System.Int32
       //Member fullName is System.Int32, IntValueC
		public static void WriteDelta_ClientAssembly_MyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.MyStruct valueA,  ClientAssembly.MyStruct valueB)
		{
			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();
			if (pooledWriter.WriteDeltaInt32(valueA.IntValueA, valueB.IntValueA))
				totalFlags += 2;
			if (pooledWriter.WriteDeltaInt32(valueA.IntValueB, valueB.IntValueB))
				totalFlags += 4;
			if (pooledWriter.WriteDeltaInt32(valueA.IntValueC, valueB.IntValueC))
				totalFlags += 8;
			writer.WritePackedWhole(totalFlags);
			writer.WriteBytes((pooledWriter.GetBuffer(), 0, pooledWriter.Length))
		}

	}
}
