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
				return reader.Read<ClientAssembly.Player.NestedStruct>();

			if ((totalFlags & 4) == 4)
// System.Byte[]
				result.Ehhhh = reader.ReadDeltaUInt8Array(value0.Ehhhh);
			else
				result.Ehhhh = value0.Ehhhh;

			if ((totalFlags & 8) == 8)
// ClientAssembly.Player.NestedStruct[]
			//Delta reader could not be found for type ClientAssembly.Player.NestedStruct[]. Please report this note.
				result.NSARr = reader.Read<ClientAssembly.Player.NestedStruct[]>();
			else
				result.NSARr = value0.NSARr;

			if ((totalFlags & 16) == 16)
// System.Collections.Generic.List
				result.NSLst = reader.ReadDeltaList<ClientAssembly.Player.NestedStruct>(value0.NSLst);
			else
				result.NSLst = value0.NSLst;

			return result;
		}

		public static ClientAssembly.Player.MyStructC GRead___ReadDeltaClientAssembly_Player_MyStructC(this FishNet.Serializing.Reader reader, ClientAssembly.Player.MyStructC value0)
		{
			ClientAssembly.Player.MyStructC result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				return reader.Read<ClientAssembly.Player.MyStructC>();

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
