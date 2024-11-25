namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.Player.NestedStruct GRead___ReadDeltaClientAssembly_Player_NestedStruct(this FishNet.Serializing.Reader reader, ClientAssembly.Player.NestedStruct value0)
		{
			ClientAssembly.Player.NestedStruct result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				
				result = reader.GRead___ReadClientAssembly_Player_NestedStruct();

				return result;
			}

			if ((totalFlags & 4) == 4)
				result.ByteArr = reader.ReadDeltaUInt8ArrayAllocated(value0.ByteArr);
			else
				result.ByteArr = value0.ByteArr;

			if ((totalFlags & 8) == 8)
				result.StructArr = reader.ReadDelta(value0.StructArr);
			else
				result.StructArr = value0.StructArr;

			if ((totalFlags & 16) == 16)
				result.StructLst = reader.ReadDeltaListAllocated<ClientAssembly.Player.NestedStruct>(value0.StructLst);
			else
				result.StructLst = value0.StructLst;

			if ((totalFlags & 32) == 32)
				result.StructDict = reader.ReadDeltaDictionaryAllocated<ClientAssembly.Player.NestedStruct, System.String>(value0.StructDict);
			else
				result.StructDict = value0.StructDict;

			if ((totalFlags & 64) == 64)
				result.ArrSegment = reader.ReadDeltaArraySegment(value0.ArrSegment);
			else
				result.ArrSegment = value0.ArrSegment;

			if ((totalFlags & 128) == 128)
			//Delta serializer not found for type System.String. Full serializer will be used.
				result.String = reader.ReadString();
			else
				result.String = value0.String;

			//Serializer not found for type System.Object. Type will not be serialized.

			//Serializer not found for type ClientAssembly.AnyType<System.Boolean>. Type will not be serialized.

			return result;
		}

		public static ClientAssembly.Player.MyStructC GRead___ReadDeltaClientAssembly_Player_MyStructC(this FishNet.Serializing.Reader reader, ClientAssembly.Player.MyStructC value0)
		{
			ClientAssembly.Player.MyStructC result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				result = reader.GRead___ReadClientAssembly_Player_MyStructC();

				return result;
			}

			if ((totalFlags & 4) == 4)
				result.Works = reader.ReadDeltaBoolean(value0.Works);
			else
				result.Works = value0.Works;

			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.NestedStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct>(GRead___ReadDeltaClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.MyStructC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.MyStructC, ClientAssembly.Player.MyStructC>(GRead___ReadDeltaClientAssembly_Player_MyStructC));
		}
	}
}
