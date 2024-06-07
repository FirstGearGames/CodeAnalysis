namespace GenerateTest
{
	public static class GeneratedWriters
	{
// Creating DeltaWriter for ClientAssembly.MyStructA
   // Type ClientAssembly.MyStructA is supported.
   // Already created System.Int32
       //Member fullName is System.Int32, IntValueA
       //Member fullName is System.Single, FloatValueA
   // Type ClientAssembly.MyStructB is supported.
       //Member fullName is System.Boolean, BoolValueA
       //Member fullName is ClientAssembly.MyStructB, StructB
		public static bool WriteDelta_ClientAssembly_MyStructB(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructB valueA, in ClientAssembly.MyStructB valueB, bool writeFull = false, bool rootWriter = true)
		{
			if (writeFull)
			{
				writer.Write(valueB);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			//WriteMethodName is empty for ClientAssembly.MyStructB
			bool changed = (totalFlags != 0) || rootWriter;
			if (changed)
				writer.WritePackedWhole(totalFlags);
			writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
			pooledWriter.Store();

			return changed;
		}

		public static bool WriteDelta_ClientAssembly_MyStructA(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructA valueA, in ClientAssembly.MyStructA valueB, bool writeFull = false, bool rootWriter = true)
		{
			if (writeFull)
			{
				writer.Write(valueB);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaInt32(valueA.IntValueA, valueB.IntValueA))
				totalFlags += 2;
			//WriteMethodName is empty for ClientAssembly.MyStructA
			if (pooledWriter.WriteDelta_ClientAssembly_MyStructB(in valueA.StructB, in valueB.StructB))
				totalFlags += 4;
			bool changed = (totalFlags != 0) || rootWriter;
			if (changed)
				writer.WritePackedWhole(totalFlags);
			writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
			pooledWriter.Store();

			return changed;
		}

	}
}
