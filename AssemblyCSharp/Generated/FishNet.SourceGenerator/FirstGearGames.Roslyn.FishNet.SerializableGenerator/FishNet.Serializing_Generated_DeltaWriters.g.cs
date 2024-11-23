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
				writer.GWrite___WriteClientAssembly_Player_NestedStruct(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			if (pooledWriter.WriteDeltaUInt8Array(value0.ByteArr, value1.ByteArr))
				totalFlags += 4;

			if (pooledWriter.WriteDeltaArray(value0.StructArr, value1.StructArr))
				totalFlags += 8;

			if (pooledWriter.WriteDeltaList(value0.StructLst, value1.StructLst))
				totalFlags += 16;

			//Delta serializer not found for type System.String. Full serializer will be used.
//Do something with regular writer to call to full.
			if (pooledWriter.WriteDeltaList(value0.LstStruct, value1.LstStruct))
				totalFlags += 32;

			if (pooledWriter.WriteDeltaArraySegment(value0.ArrSegment, value1.ArrSegment))
				totalFlags += 64;

			//Serializer not found for type System.Object. Type will not be serialized.
			//Serializer not found for type ClientAssembly.AnyType<System.Boolean>. Type will not be serialized.
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
				writer.GWrite___WriteClientAssembly_Player_MyStructC(value1);
				return true;
			}

			System.UInt64 totalFlags = (ulong)options;
			FishNet.Serializing.PooledWriter pooledWriter = FishNet.Serializing.WriterPool.Retrieve();

			//Delta serializer not found for type System.Boolean. Full serializer will be used.
//Do something with regular writer to call to full.
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
			
			
		}
	}
}
