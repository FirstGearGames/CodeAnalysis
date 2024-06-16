namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool WriteDeltaClientAssembly_MyStructA(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructA value0, in ClientAssembly.MyStructA value1, bool writeFull = false, bool rootCall = true)
		{
			throw new Exception("Full Writer could not be found for type ClientAssembly.MyStructA. This is normal until added. Continuing...");
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaSingle(value0.Horizontal, value1.Horizontal))
				totalFlags += 2;
			if (pooledWriter.WriteDeltaSingle(value0.Vertical, value1.Vertical))
				totalFlags += 4;
			if (pooledWriter.WriteDeltaBoolean(value0.Running, value1.Running))
				totalFlags += 8;
			if (pooledWriter.WriteDeltaBoolean(value0.Firing, value1.Firing))
				totalFlags += 16;
			if (pooledWriter.WriteDeltaBoolean(value0.Jumping, value1.Jumping))
				totalFlags += 32;

			bool changed = (totalFlags != 0) || rootCall;
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
			}
			pooledWriter.Store();

			return changed;

		}

		public static bool WriteDeltaClientAssembly_MyStructB(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructB value0, in ClientAssembly.MyStructB value1, bool writeFull = false, bool rootCall = true)
		{
			throw new Exception("Full Writer could not be found for type ClientAssembly.MyStructB. This is normal until added. Continuing...");
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaSingle(value0.PositionX, value1.PositionX))
				totalFlags += 2;
			if (pooledWriter.WriteDeltaSingle(value0.PositionY, value1.PositionY))
				totalFlags += 4;
			if (pooledWriter.WriteDeltaSingle(value0.PositionZ, value1.PositionZ))
				totalFlags += 8;
			if (pooledWriter.WriteDeltaBoolean(value0.Hits, value1.Hits))
				totalFlags += 16;
			if (pooledWriter.WriteDeltaSingle(value0.Stamina, value1.Stamina))
				totalFlags += 32;
			if (pooledWriter.WriteDeltaSingle(value0.VelocityX, value1.VelocityX))
				totalFlags += 64;
			if (pooledWriter.WriteDeltaSingle(value0.VelocityY, value1.VelocityY))
				totalFlags += 128;
			if (pooledWriter.WriteDeltaSingle(value0.VelocityZ, value1.VelocityZ))
				totalFlags += 256;

			bool changed = (totalFlags != 0) || rootCall;
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteBytes(pooledWriter.GetBuffer(), 0, pooledWriter.Length);
			}
			pooledWriter.Store();

			return changed;

		}

	}
}
