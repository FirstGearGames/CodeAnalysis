namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.MyThingB GRead___ReadDeltaClientAssembly_MyThingB(this FishNet.Serializing.Reader reader, ClientAssembly.MyThingB value0)
		{
			ClientAssembly.MyThingB result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				return reader.Read<ClientAssembly.MyThingB>();

			if ((totalFlags & 4) == 4)
			//Delta reader could not be found for type System.String. Please report this note.
				result.B = reader.ReadString();
			else
				result.B = value0.B;

			return result;
		}

		public static ClientAssembly.MyThingA GRead___ReadDeltaClientAssembly_MyThingA(this FishNet.Serializing.Reader reader, ClientAssembly.MyThingA value0)
		{
			ClientAssembly.MyThingA result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				return reader.Read<ClientAssembly.MyThingA>();

			if ((totalFlags & 4) == 4)
				result.Position = reader.ReadDeltaVector3(value0.Position);
			else
				result.Position = value0.Position;

			if ((totalFlags & 8) == 8)
			//Delta reader could not be found for type System.String. Please report this note.
				result.A = reader.ReadString();
			else
				result.A = value0.A;

			if ((totalFlags & 16) == 16)
				result.C = reader.GRead___ReadDeltaClientAssembly_MyThingB(value0.C);
			else
				result.C = value0.C;

			return result;
		}

		public static ClientAssembly.MyThingC GRead___ReadDeltaClientAssembly_MyThingC(this FishNet.Serializing.Reader reader, ClientAssembly.MyThingC value0)
		{
			ClientAssembly.MyThingC result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				return reader.Read<ClientAssembly.MyThingC>();

			if ((totalFlags & 4) == 4)
			//Delta reader could not be found for type System.String. Please report this note.
				result.C = reader.ReadString();
			else
				result.C = value0.C;

			return result;
		}

		public static ClientAssembly.Player.NestedStruct GRead___ReadDeltaClientAssembly_Player_NestedStruct(this FishNet.Serializing.Reader reader, ClientAssembly.Player.NestedStruct value0)
		{
			ClientAssembly.Player.NestedStruct result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
				return reader.Read<ClientAssembly.Player.NestedStruct>();

			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingB>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.MyThingB, ClientAssembly.MyThingB>(GRead___ReadDeltaClientAssembly_MyThingB));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingA>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.MyThingA, ClientAssembly.MyThingA>(GRead___ReadDeltaClientAssembly_MyThingA));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.MyThingC, ClientAssembly.MyThingC>(GRead___ReadDeltaClientAssembly_MyThingC));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.NestedStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct>(GRead___ReadDeltaClientAssembly_Player_NestedStruct));
		}
	}
}
