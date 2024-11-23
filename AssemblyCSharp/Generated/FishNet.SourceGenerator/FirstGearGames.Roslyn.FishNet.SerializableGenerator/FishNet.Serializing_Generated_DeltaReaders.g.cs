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
				return reader.GRead___ReadClientAssembly_Player_NestedStruct();

			if ((totalFlags & 4) == 4)
// System.Byte
				result.ByteArr = reader.ReadDeltaUInt8System.Byte[](value0.ByteArr);
			else
				result.ByteArr = value0.ByteArr;

			if ((totalFlags & 8) == 8)
// ClientAssembly.Player.NestedStruct
				result.StructArr = reader.ReadDelta(value0.StructArr);
			else
				result.StructArr = value0.StructArr;

			if ((totalFlags & 16) == 16)
// System.Collections.Generic.List
			//Delta reader could not be found for type System.Collections.Generic.List. Please report this note.
				result.StructLst = reader.Read<System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>><ClientAssembly.Player.NestedStruct>();
			else
				result.StructLst = value0.StructLst;

			if ((totalFlags & 32) == 32)
// System.String
			//Delta reader could not be found for type System.String. Please report this note.
				result.String = reader.ReadString();
			else
				result.String = value0.String;

			if ((totalFlags & 64) == 64)
// System.Collections.Generic.List
			//Delta reader could not be found for type System.Collections.Generic.List. Please report this note.
				result.LstStruct = reader.Read<System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>><ClientAssembly.Player.NestedStruct>();
			else
				result.LstStruct = value0.LstStruct;

			if ((totalFlags & 128) == 128)
// System.ArraySegment
			//Delta reader could not be found for type System.ArraySegment. Please report this note.
				result.ArrSegment = reader.Read<System.ArraySegment<System.Byte>><System.Byte>();
			else
				result.ArrSegment = value0.ArrSegment;

			if ((totalFlags & 256) == 256)
// System.Object
			//Delta reader could not be found for type System.Object. Please report this note.
				result.ObjectType = reader.Read<System.Object>();
			else
				result.ObjectType = value0.ObjectType;

			if ((totalFlags & 512) == 512)
// ClientAssembly.AnyType
			//Delta reader could not be found for type ClientAssembly.AnyType. Please report this note.
				result.GenericObjectType = reader.Read<ClientAssembly.AnyType<System.Boolean>><System.Boolean>();
			else
				result.GenericObjectType = value0.GenericObjectType;

			return result;
		}

		public static ClientAssembly.Player.MyStructC GRead___ReadDeltaClientAssembly_Player_MyStructC(this FishNet.Serializing.Reader reader, ClientAssembly.Player.MyStructC value0)
		{
			ClientAssembly.Player.MyStructC result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				return reader.GRead___ReadClientAssembly_Player_MyStructC();

			if ((totalFlags & 4) == 4)
// System.Boolean
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
