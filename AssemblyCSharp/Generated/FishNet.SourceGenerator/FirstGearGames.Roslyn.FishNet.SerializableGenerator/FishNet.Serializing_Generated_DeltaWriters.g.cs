//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool GWrite___WriteDeltaClientAssembly_MyThingB(this FishNet.Serializing.Writer writer, ClientAssembly.MyThingB value0, ClientAssembly.MyThingB value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				System.UInt64 optionsFlags = (System.UInt64)options;
				writer.WriteUnsignedPackedWhole(optionsFlags);
				writer.Write<ClientAssembly.MyThingB>(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			//Delta writer could not be found for type System.String. Please report this note.
			pooledWriter.WriteString(value1.B);
			totalFlags += 4;

			if (pooledWriter.WriteDeltaDictionary<System.Int32, System.Boolean>(value0.Dict, value1.Dict))
				totalFlags += 8;

			if (pooledWriter.WriteDeltaUInt8Array(value0.Bytey, value1.Bytey))
				totalFlags += 16;

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
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.MyThingB>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.MyThingB, ClientAssembly.MyThingB, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_MyThingB));
		}
	}
}
