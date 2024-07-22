namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

			public static bool WriteDeltaClientAssembly_MyThingB(this FishNet.Serializing.Writer writer, ClientAssembly.MyThingB value0, ClientAssembly.MyThingB value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
			{
				if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				{
					System.UInt64 optionsFlags = (System.UInt64)options;
					writer.WriteUnsignedPackedWhole(optionsFlags);
					writer.Write(value1);
					return true;
				}

				System.UInt64 totalFlags = (ulong)options;
				FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

				if (pooledWriter.WriteDeltaString(value0.B, value1.B))
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

			public static bool WriteDeltaClientAssembly_MyThingA(this FishNet.Serializing.Writer writer, ClientAssembly.MyThingA value0, ClientAssembly.MyThingA value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
			{
				if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				{
					System.UInt64 optionsFlags = (System.UInt64)options;
					writer.WriteUnsignedPackedWhole(optionsFlags);
					writer.Write(value1);
					return true;
				}

				System.UInt64 totalFlags = (ulong)options;
				FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

				if (pooledWriter.WriteDeltaString(value0.A, value1.A))
				totalFlags += 4;

				if (pooledWriter.WriteDeltaClientAssembly_MyThingB(value0.C, value1.C))
				totalFlags += 8;

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

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.MyThingB>.SetWrite(new Func<FishNet.Serializing.Writer, ClientAssembly.MyThingB, ClientAssembly.MyThingB, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(WriteDeltaClientAssembly_MyThingB));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.MyThingA>.SetWrite(new Func<FishNet.Serializing.Writer, ClientAssembly.MyThingA, ClientAssembly.MyThingA, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(WriteDeltaClientAssembly_MyThingA));

		}
	}
}
