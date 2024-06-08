namespace FishNet.Serializing
{
	public static class Generated_Delta_Serializers
	{

		public static bool WriteDeltaClientAssembly_MyStructA(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructA value0, in ClientAssembly.MyStructA value1, bool writeFull = false, bool rootWrite = true)
		{
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaInt32(value0.IntValueA, value1.IntValueA))
				totalFlags += 2;

			bool changed = (totalFlags != 0) || rootWrite;
			if (changed)
			{
				writer.WritePackedWhole(totalFlags);
				writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
			}
			pooledWriter.Store();

			return changed;

		}

	}
}
