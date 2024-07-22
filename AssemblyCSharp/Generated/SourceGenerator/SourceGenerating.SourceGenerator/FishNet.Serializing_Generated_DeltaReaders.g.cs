namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

public static ClientAssembly.MyThingB ReadDeltaClientAssembly_MyThingB(this FishNet.Serializing.Reader reader, ClientAssembly.MyThingB value0)

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

public static ClientAssembly.MyThingA ReadDeltaClientAssembly_MyThingA(this FishNet.Serializing.Reader reader, ClientAssembly.MyThingA value0)

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
				result.C = reader.ReadDeltaClientAssembly_MyThingB(value0.C);
			else
				result.C = value0.C;

			return result;
			}

	}
}
