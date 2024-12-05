namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.Player.BroadcastStruct GRead___ReadDeltaClientAssembly_Player_BroadcastStruct(this FishNet.Serializing.Reader reader, ClientAssembly.Player.BroadcastStruct value0)
		{
			ClientAssembly.Player.BroadcastStruct result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				result = reader.GRead___ReadClientAssembly_Player_BroadcastStruct();

				return result;
			}

			if ((totalFlags & 4) == 4)
				result.IsBroadcast = reader.ReadDeltaBoolean(value0.IsBroadcast);
			else
				result.IsBroadcast = value0.IsBroadcast;

			return result;
		}

		public static ClientAssembly.Player.EmptyStruct GRead___ReadDeltaClientAssembly_Player_EmptyStruct(this FishNet.Serializing.Reader reader, ClientAssembly.Player.EmptyStruct value0)
		{
			ClientAssembly.Player.EmptyStruct result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				result = reader.GRead___ReadClientAssembly_Player_EmptyStruct();

				return result;
			}

			return result;
		}

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
				result.Struct = reader.GRead___ReadDeltaClientAssembly_Player_EmptyStruct(value0.Struct);
			else
				result.Struct = value0.Struct;

			if ((totalFlags & 8) == 8)
				result.ByteArr = reader.ReadDeltaUInt8ArrayAllocated(value0.ByteArr);
			else
				result.ByteArr = value0.ByteArr;

			if ((totalFlags & 16) == 16)
				result.StructArr = reader.ReadDeltaArrayAllocated<ClientAssembly.Player.NestedStruct>(value0.StructArr);
			else
				result.StructArr = value0.StructArr;

			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrMultiDimensional. Value will not be serialized.
			if ((totalFlags & 32) == 32)
				result.StructArrJagged = reader.ReadDeltaJaggedArrayAllocated(value0.StructArrJagged);
			else
				result.StructArrJagged = value0.StructArrJagged;

			if ((totalFlags & 64) == 64)
				result.StructLst = reader.ReadDeltaListAllocated<ClientAssembly.Player.NestedStruct>(value0.StructLst);
			else
				result.StructLst = value0.StructLst;

			if ((totalFlags & 128) == 128)
				result.TupleLst = reader.ReadDeltaListAllocated<System.ValueTuple<System.Boolean, System.String>>(value0.TupleLst);
			else
				result.TupleLst = value0.TupleLst;

			if ((totalFlags & 256) == 256)
				result.StructDict = reader.ReadDeltaDictionaryAllocated<ClientAssembly.Player.NestedStruct, System.String>(value0.StructDict);
			else
				result.StructDict = value0.StructDict;

			if ((totalFlags & 512) == 512)
				result.ArrSegment = reader.ReadDeltaArraySegment(value0.ArrSegment);
			else
				result.ArrSegment = value0.ArrSegment;

			if ((totalFlags & 1024) == 1024)
				//Delta serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.String; full serializer will be used.
				result.String = reader.ReadString();
			else
				result.String = value0.String;

			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.ObjectType. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.GenericObjectType. Value will not be serialized.
			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.BroadcastStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.BroadcastStruct, ClientAssembly.Player.BroadcastStruct>(GRead___ReadDeltaClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.EmptyStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.EmptyStruct, ClientAssembly.Player.EmptyStruct>(GRead___ReadDeltaClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.NestedStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct>(GRead___ReadDeltaClientAssembly_Player_NestedStruct));
		}
	}
}
