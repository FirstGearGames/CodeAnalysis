namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.MyStructA ReadDeltaClientAssembly_MyStructA(this FishNet.Serializing.Reader reader, in ClientAssembly.MyStructA value0, bool readFull = false, bool rootCall = true)
		{
			if (readFull)
				return reader.Read<ClientAssembly.MyStructA>();

			ClientAssembly.MyStructA result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();;

			if ((totalFlags & 2) == 2)
				result.Horizontal = reader.ReadDeltaSingle(value0.Horizontal);
			else
				result.Horizontal = value0.Horizontal;

			if ((totalFlags & 4) == 4)
				result.Vertical = reader.ReadDeltaSingle(value0.Vertical);
			else
				result.Vertical = value0.Vertical;

			if ((totalFlags & 8) == 8)
				result.Running = reader.ReadDeltaBoolean(value0.Running);
			else
				result.Running = value0.Running;

			if ((totalFlags & 16) == 16)
				result.Firing = reader.ReadDeltaBoolean(value0.Firing);
			else
				result.Firing = value0.Firing;

			if ((totalFlags & 32) == 32)
				result.Jumping = reader.ReadDeltaBoolean(value0.Jumping);
			else
				result.Jumping = value0.Jumping;

			return result;
		}

	}
}
