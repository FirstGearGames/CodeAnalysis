namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.MyStructA ReadDeltaClientAssembly_MyStructA(this FishNet.Serializing.Reader reader, in ClientAssembly.MyStructA value0, bool readFull = false, bool rootCall = true)
		{
			if (readFull)
				return reader.Read<ClientAssembly.MyStructA>();

			return default;
		}

		public static ClientAssembly.MyStructB ReadDeltaClientAssembly_MyStructB(this FishNet.Serializing.Reader reader, in ClientAssembly.MyStructB value0, bool readFull = false, bool rootCall = true)
		{
			if (readFull)
				return reader.Read<ClientAssembly.MyStructB>();

			return default;
		}

	}
}
