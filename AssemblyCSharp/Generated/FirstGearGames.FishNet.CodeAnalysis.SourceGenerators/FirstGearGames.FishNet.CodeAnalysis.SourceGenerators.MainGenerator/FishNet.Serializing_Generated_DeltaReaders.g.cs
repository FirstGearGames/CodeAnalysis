namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.SimpleStructA GRead___ReadDeltaClientAssembly_SimpleStructA(this FishNet.Serializing.Reader reader, ClientAssembly.SimpleStructA value0)
		{
			ClientAssembly.SimpleStructA result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				result = reader.GRead___ReadClientAssembly_SimpleStructA();

				return result;
			}

			if ((totalFlags & 4) == 4)
				result.TheNumber = reader.ReadDeltaInt32(value0.TheNumber);
			else
				result.TheNumber = value0.TheNumber;

			return result;
		}

		public static ClientAssembly.SimpleStructB GRead___ReadDeltaClientAssembly_SimpleStructB(this FishNet.Serializing.Reader reader, ClientAssembly.SimpleStructB value0)
		{
			ClientAssembly.SimpleStructB result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				result = reader.GRead___ReadClientAssembly_SimpleStructB();

				return result;
			}

			if ((totalFlags & 4) == 4)
				result.TheNumber = reader.ReadDeltaInt32(value0.TheNumber);
			else
				result.TheNumber = value0.TheNumber;

			return result;
		}

		public static ClientAssembly.SimpleStructC GRead___ReadDeltaClientAssembly_SimpleStructC(this FishNet.Serializing.Reader reader, ClientAssembly.SimpleStructC value0)
		{
			ClientAssembly.SimpleStructC result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();

			FishNet.Serializing.DeltaSerializerOption options = (FishNet.Serializing.DeltaSerializerOption)totalFlags;
			if (options.FastContains(FishNet.Serializing.DeltaSerializerOption.FullSerialize))
			{
				result = reader.GRead___ReadClientAssembly_SimpleStructC();

				return result;
			}

			if ((totalFlags & 4) == 4)
				result.TheNumber = reader.ReadDeltaInt32(value0.TheNumber);
			else
				result.TheNumber = value0.TheNumber;

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
				result.String = reader.ReadDelta<System.String>(value0.String);
			else
				result.String = value0.String;

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
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.SimpleStructA>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.SimpleStructA, ClientAssembly.SimpleStructA>(GRead___ReadDeltaClientAssembly_SimpleStructA));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.SimpleStructB>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.SimpleStructB, ClientAssembly.SimpleStructB>(GRead___ReadDeltaClientAssembly_SimpleStructB));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.SimpleStructC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.SimpleStructC, ClientAssembly.SimpleStructC>(GRead___ReadDeltaClientAssembly_SimpleStructC));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.EmptyStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.EmptyStruct, ClientAssembly.Player.EmptyStruct>(GRead___ReadDeltaClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.BroadcastStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.BroadcastStruct, ClientAssembly.Player.BroadcastStruct>(GRead___ReadDeltaClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.NestedStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct>(GRead___ReadDeltaClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.Player.MyStructC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.MyStructC, ClientAssembly.Player.MyStructC>(GRead___ReadDeltaClientAssembly_Player_MyStructC));
		}
	}
}
