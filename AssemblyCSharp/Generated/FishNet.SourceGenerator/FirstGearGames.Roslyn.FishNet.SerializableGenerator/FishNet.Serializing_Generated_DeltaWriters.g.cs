//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_DeltaWriters
	{

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

			if (pooledWriter.WriteDeltaUInt8Array(value0.ByteArr, value1.ByteArr))
				totalFlags += 4;

			if (pooledWriter.WriteDeltaArray(value0.StructArr, value1.StructArr))
				totalFlags += 8;

			//Serializer not found for type System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>. Type will not be serialized.

			//Serializer not found for type System.Collections.Generic.Dictionary<ClientAssembly.Player.NestedStruct, System.String>. Type will not be serialized.

			if (pooledWriter.WriteDeltaArraySegment(value0.ArrSegment, value1.ArrSegment))
				totalFlags += 16;

			//Delta serializer not found for type System.String. Full serializer will be used.
			pooledWriter.WriteString(value0.String);
				totalFlags += 32;

			//Serializer not found for type System.Object. Type will not be serialized.

			//Serializer not found for type ClientAssembly.AnyType<System.Boolean>. Type will not be serialized.

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

			//Delta serializer not found for type System.Boolean. Full serializer will be used.
			pooledWriter.WriteBoolean(value0.Works);
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
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC, ClientAssembly.Player.MyStructC, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteDeltaClientAssembly_Player_MyStructC));
		}
	}
}
