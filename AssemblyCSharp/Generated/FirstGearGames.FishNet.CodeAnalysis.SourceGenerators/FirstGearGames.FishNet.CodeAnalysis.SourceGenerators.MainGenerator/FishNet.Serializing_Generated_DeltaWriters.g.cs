//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool GWrite___WriteDeltaClientAssembly_Player_SimpleStructA(this FishNet.Serializing.Writer writer, ClientAssembly.Player.SimpleStructA value0, ClientAssembly.Player.SimpleStructA value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_Player_SimpleStructA(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaInt32(value0.TheNumber, value1.TheNumber))
				totalFlags += 4;

			System.Boolean changed = (totalFlags != 0 || options == FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool GWrite___WriteDeltaClientAssembly_SimpleStructB(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructB value0, ClientAssembly.SimpleStructB value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_SimpleStructB(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaInt32(value0.TheNumber, value1.TheNumber))
				totalFlags += 4;

			System.Boolean changed = (totalFlags != 0 || options == FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool GWrite___WriteDeltaClientAssembly_SimpleStructC(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructC value0, ClientAssembly.SimpleStructC value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_SimpleStructC(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaInt32(value0.TheNumber, value1.TheNumber))
				totalFlags += 4;

			System.Boolean changed = (totalFlags != 0 || options == FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool GWrite___WriteDeltaClientAssembly_Player_BroadcastStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.BroadcastStruct value0, ClientAssembly.Player.BroadcastStruct value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_Player_BroadcastStruct(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaBoolean(value0.IsBroadcast, value1.IsBroadcast))
				totalFlags += 4;

			System.Boolean changed = (totalFlags != 0 || options == FishNet.Serializing.DeltaSerializerOption.RootSerialize);
			if (changed)
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.WriteArraySegment(pooledWriter.GetArraySegment());
			}
			pooledWriter.Store();

			return changed;
		}

		public static bool GWrite___WriteDeltaClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0, ClientAssembly.Player.NestedStruct value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_Player_NestedStruct(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDelta<System.String>(value0.String, value1.String))
				totalFlags += 4;

			System.Boolean changed = (totalFlags != 0 || options == FishNet.Serializing.DeltaSerializerOption.RootSerialize);
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
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_Player_MyStructC(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaBoolean(value0.Works, value1.Works))
				totalFlags += 4;

			System.Boolean changed = (totalFlags != 0 || options == FishNet.Serializing.DeltaSerializerOption.RootSerialize);
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
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.SimpleStructA>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.SimpleStructA, ClientAssembly.Player.SimpleStructA, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_SimpleStructA));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.SimpleStructB>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.SimpleStructB, ClientAssembly.SimpleStructB, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_SimpleStructB));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.SimpleStructC>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.SimpleStructC, ClientAssembly.SimpleStructC, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_SimpleStructC));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.BroadcastStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.BroadcastStruct, ClientAssembly.Player.BroadcastStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC, ClientAssembly.Player.MyStructC, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_MyStructC));
		}
	}
}
