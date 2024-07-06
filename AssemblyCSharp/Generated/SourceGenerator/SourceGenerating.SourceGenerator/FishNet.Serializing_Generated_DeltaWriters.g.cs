namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool WriteDeltaClientAssembly_Player_MyStructB_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructB.MyStructC value0, ClientAssembly.Player.MyStructB.MyStructC value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaBoolean(value0.Works, value1.Works))
				totalFlags += 4;

			System.Boolean rootSerializer = options.FastContains(FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			System.Boolean changed = (totalFlags != 0);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool WriteDeltaClientAssembly_Player_MyStructB(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructB value0, ClientAssembly.Player.MyStructB value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaSingle(value0.PositionX, value1.PositionX))
				totalFlags += 4;

			if (pooledWriter.WriteDeltaSingle(value0.PositionY, value1.PositionY))
				totalFlags += 8;

			if (pooledWriter.WriteDeltaSingle(value0.PositionZ, value1.PositionZ))
				totalFlags += 16;

			if (pooledWriter.WriteDeltaBoolean(value0.Hits, value1.Hits))
				totalFlags += 32;

			if (pooledWriter.WriteDeltaSingle(value0.Stamina, value1.Stamina))
				totalFlags += 64;

			if (pooledWriter.WriteDeltaSingle(value0.VelocityX, value1.VelocityX))
				totalFlags += 128;

			if (pooledWriter.WriteDeltaSingle(value0.VelocityY, value1.VelocityY))
				totalFlags += 256;

			if (pooledWriter.WriteDeltaSingle(value0.VelocityZ, value1.VelocityZ))
				totalFlags += 512;

			if (pooledWriter.WriteDeltaClientAssembly_Player_MyStructB_MyStructC(value0.StructC, value1.StructC))
				totalFlags += 1024;

			System.Boolean rootSerializer = options.FastContains(FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			System.Boolean changed = (totalFlags != 0);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool WriteDeltaClientAssembly_MyStructA(this FishNet.Serializing.Writer writer, in ClientAssembly.MyStructA value0, in ClientAssembly.MyStructA value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaSingle(value0.Horizontal, value1.Horizontal))
				totalFlags += 4;

			if (pooledWriter.WriteDeltaSingle(value0.Vertical, value1.Vertical))
				totalFlags += 8;

			if (pooledWriter.WriteDeltaBoolean(value0.Running, value1.Running))
				totalFlags += 16;

			if (pooledWriter.WriteDeltaBoolean(value0.Firing, value1.Firing))
				totalFlags += 32;

			if (pooledWriter.WriteDeltaBoolean(value0.Jumping, value1.Jumping))
				totalFlags += 64;

			System.Boolean rootSerializer = options.FastContains(FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			System.Boolean changed = (totalFlags != 0);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

	}
}
