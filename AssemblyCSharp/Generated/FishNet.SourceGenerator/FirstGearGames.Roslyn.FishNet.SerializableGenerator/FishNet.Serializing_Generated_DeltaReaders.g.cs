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

			if ((totalFlags & 8) == 8)
				result.Dict = reader.ReadDeltaDictionary<TKey, TValue>(value0.Dict);
			else
				result.Dict = value0.Dict;

			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaReader<ClientAssembly.MyThingB>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.MyThingB, ClientAssembly.MyThingB>(GRead___ReadDeltaClientAssembly_MyThingB));
		}
	}
}
