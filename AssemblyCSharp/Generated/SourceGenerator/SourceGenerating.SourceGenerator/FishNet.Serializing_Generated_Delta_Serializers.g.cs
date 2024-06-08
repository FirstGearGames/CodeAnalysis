namespace FishNet.Serializing
{
	public static class Generated_Delta_Serializers
	{

		public static bool WriteDeltaClientAssembly_MyStructB(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructB value0, in ClientAssembly.MyStructB value1, bool writeFull = false, bool rootWrite = true)
		{
			throw new Exception("Full Writer could not be found for type ClientAssembly.MyStructB. This is normal until added. Continuing...");
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			throw new Exception("Delta writer could not be found for type System.Boolean.");

			bool changed = (totalFlags != 0) || rootWrite;
			if (changed)
			{
				writer.WritePackedWhole(totalFlags);
				writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
			}
			pooledWriter.Store();

			return changed;

		}

		public static bool WriteDeltaClientAssembly_MyStructA(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructA value0, in ClientAssembly.MyStructA value1, bool writeFull = false, bool rootWrite = true)
		{
			throw new Exception("Full Writer could not be found for type ClientAssembly.MyStructA. This is normal until added. Continuing...");
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaInt32(value0.IntValueA, value1.IntValueA))
				totalFlags += 2;
			throw new Exception("Delta writer could not be found for type System.Single.");
			if (pooledWriter.WriteDeltaClientAssembly_MyStructB(in value0.StructB, in value1.StructB))
				totalFlags += 4;

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
