namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.MyStructA ReadDeltaClientAssembly_MyStructA(this FishNet.Serializing.Reader reader, in ClientAssembly.MyStructA value0, bool readFull = false, bool rootCall = true)
		{
			throw new Exception("Full Reader could not be found for type ClientAssembly.MyStructA. This is normal until added. Continuing...");
			if (readFull)
				return reader.Read<ClientAssembly.MyStructA>();

			return default;
		}

		public static ClientAssembly.MyStructB ReadDeltaClientAssembly_MyStructB(this FishNet.Serializing.Reader reader, in ClientAssembly.MyStructB value0, bool readFull = false, bool rootCall = true)
		{
			throw new Exception("Full Reader could not be found for type ClientAssembly.MyStructB. This is normal until added. Continuing...");
			if (readFull)
				return reader.Read<ClientAssembly.MyStructB>();

			return default;
		}

	}
}
