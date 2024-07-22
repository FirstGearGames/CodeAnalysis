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
				result.B = reader.ReadDeltaString(value0.B);
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
				result.A = reader.ReadDeltaString(value0.A);
			else
				result.A = value0.A;

			if ((totalFlags & 8) == 8)
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
				result.C = reader.ReadDeltaString(value0.C);
			else
				result.C = value0.C;

			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingB>.SetRead(new Func<FishNet.Serializing.Reader, ClientAssembly.MyThingB, ClientAssembly.MyThingB>(GRead___ReadDeltaClientAssembly_MyThingB));

			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingA>.SetRead(new Func<FishNet.Serializing.Reader, ClientAssembly.MyThingA, ClientAssembly.MyThingA>(GRead___ReadDeltaClientAssembly_MyThingA));

			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingC>.SetRead(new Func<FishNet.Serializing.Reader, ClientAssembly.MyThingC, ClientAssembly.MyThingC>(GRead___ReadDeltaClientAssembly_MyThingC));

		}
	}
}
