//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool GWrite___WriteDeltaClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0, ClientAssembly.Player.NestedStruct value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				System.UInt64 optionsFlags = (System.UInt64)options;
				writer.WriteUnsignedPackedWhole(optionsFlags);
				writer.Write<ClientAssembly.Player.NestedStruct>(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			//Delta writer could not be found for type System.Byte[]. Please report this note.
			pooledWriter.WriteUInt8ArrayAndSize(value1.Ehhhh);
			totalFlags += 4;

			System.Boolean changed = (totalFlags != 0);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool GWrite___WriteDeltaClientAssembly_Player_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructC value0, ClientAssembly.Player.MyStructC value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				System.UInt64 optionsFlags = (System.UInt64)options;
				writer.WriteUnsignedPackedWhole(optionsFlags);
				writer.Write<ClientAssembly.Player.MyStructC>(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaBoolean(value0.Works, value1.Works))
				totalFlags += 4;

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
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC, ClientAssembly.Player.MyStructC, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_MyStructC));
		}
	}
}
