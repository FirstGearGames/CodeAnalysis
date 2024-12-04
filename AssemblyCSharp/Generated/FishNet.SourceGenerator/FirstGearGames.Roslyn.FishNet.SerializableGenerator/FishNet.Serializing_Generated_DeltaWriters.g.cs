//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

		public static bool GWrite___WriteDeltaClientAssembly_SimpleStructA(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructA value0, ClientAssembly.SimpleStructA value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_SimpleStructA(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

				//Delta serializer not found for ClientAssembly.SimpleStructA.ClientAssembly.SimpleStructA.TheNUmber; full serializer will be used.
			pooledWriter.WriteInt32(value0.TheNUmber);
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

				//Delta serializer not found for ClientAssembly.SimpleStructB.ClientAssembly.SimpleStructB.TheNUmber; full serializer will be used.
			pooledWriter.WriteInt32(value0.TheNUmber);
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

				//Delta serializer not found for ClientAssembly.Player.BroadcastStruct.ClientAssembly.Player.BroadcastStruct.IsBroadcast; full serializer will be used.
			pooledWriter.WriteBoolean(value0.IsBroadcast);
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

		public static bool GWrite___WriteDeltaClientAssembly_Player_EmptyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.EmptyStruct value0, ClientAssembly.Player.EmptyStruct value1, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			System.UInt64 totalFlags = (ulong)options;

			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				writer.WriteUnsignedPackedWhole(totalFlags);
				writer.GWrite___WriteClientAssembly_Player_EmptyStruct(value1);
				return true;
			}

			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

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

			if (pooledWriter.GWrite___WriteDeltaClientAssembly_Player_EmptyStruct(value0.Struct, value1.Struct))
				totalFlags += 4;

			if (pooledWriter.WriteDeltaUInt8Array(value0.ByteArr, value1.ByteArr))
				totalFlags += 8;

			if (pooledWriter.WriteDeltaArray(value0.StructArr, value1.StructArr))
				totalFlags += 16;

			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrMultiDimensional. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrJagged. Value will not be serialized.
			if (pooledWriter.WriteDeltaList(value0.StructLst, value1.StructLst))
				totalFlags += 32;

			if (pooledWriter.WriteDeltaList(value0.TupleLst, value1.TupleLst))
				totalFlags += 64;

			if (pooledWriter.WriteDeltaDictionary(value0.StructDict, value1.StructDict))
				totalFlags += 128;

			if (pooledWriter.WriteDeltaArraySegment(value0.ArrSegment, value1.ArrSegment))
				totalFlags += 256;

				//Delta serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.String; full serializer will be used.
			pooledWriter.WriteString(value0.String);
				totalFlags += 512;

			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.ObjectType. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.GenericObjectType. Value will not be serialized.
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
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.SimpleStructA>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.SimpleStructA, ClientAssembly.SimpleStructA, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_SimpleStructA));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.SimpleStructB>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.SimpleStructB, ClientAssembly.SimpleStructB, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_SimpleStructB));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.BroadcastStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.BroadcastStruct, ClientAssembly.Player.BroadcastStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.EmptyStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.EmptyStruct, ClientAssembly.Player.EmptyStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_NestedStruct));
		}
	}
}
