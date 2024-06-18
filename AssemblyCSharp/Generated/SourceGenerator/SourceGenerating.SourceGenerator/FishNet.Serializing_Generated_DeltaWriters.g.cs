namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool WriteDeltaClientAssembly_MyThingA(this FishNet.Serializing.Writer writer, ClientAssembly.MyThingA value0, ClientAssembly.MyThingA value1, bool writeFull = false, bool rootCall = true)
		{
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			//Delta writer could not be found for type System.String. Please report this note.
			pooledWriter.WriteString(value1.A);
			totalFlags += 2;

			bool changed = (totalFlags != 0) || rootCall;
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool WriteDeltaClientAssembly_MyThingB(this FishNet.Serializing.Writer writer, ClientAssembly.MyThingB value0, ClientAssembly.MyThingB value1, bool writeFull = false, bool rootCall = true)
		{
			if (writeFull)
			{
				writer.Write(value1);
				return true;
			}

			System.UInt64 totalFlags = 0;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			//Delta writer could not be found for type System.String. Please report this note.
			pooledWriter.WriteString(value1.B);
			totalFlags += 2;

			bool changed = (totalFlags != 0) || rootCall;
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
