namespace FishNet.Serializing
{
	public static class Generated_DeltaReaders
	{

		public static ClientAssembly.Player.MyStructB.MyStructC ReadDeltaClientAssembly_Player_MyStructB_MyStructC(this FishNet.Serializing.Reader reader, ClientAssembly.Player.MyStructB.MyStructC value0, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			ClientAssembly.Player.MyStructB.MyStructC result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();;

			if ((totalFlags & 2) == 2)
				result.Works = reader.ReadDeltaBoolean(value0.Works);
			else
				result.Works = value0.Works;

			return result;
		}

		public static ClientAssembly.Player.MyStructB ReadDeltaClientAssembly_Player_MyStructB(this FishNet.Serializing.Reader reader, ClientAssembly.Player.MyStructB value0, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
			ClientAssembly.Player.MyStructB result = new();
			System.UInt64 totalFlags = reader.ReadUnsignedPackedWhole();;

			if ((totalFlags & 2) == 2)
				result.PositionX = reader.ReadDeltaSingle(value0.PositionX);
			else
				result.PositionX = value0.PositionX;

			if ((totalFlags & 4) == 4)
				result.PositionY = reader.ReadDeltaSingle(value0.PositionY);
			else
				result.PositionY = value0.PositionY;

			if ((totalFlags & 8) == 8)
				result.PositionZ = reader.ReadDeltaSingle(value0.PositionZ);
			else
				result.PositionZ = value0.PositionZ;

			if ((totalFlags & 16) == 16)
				result.Hits = reader.ReadDeltaBoolean(value0.Hits);
			else
				result.Hits = value0.Hits;

			if ((totalFlags & 32) == 32)
				result.Stamina = reader.ReadDeltaSingle(value0.Stamina);
			else
				result.Stamina = value0.Stamina;

			if ((totalFlags & 64) == 64)
				result.VelocityX = reader.ReadDeltaSingle(value0.VelocityX);
			else
				result.VelocityX = value0.VelocityX;

			if ((totalFlags & 128) == 128)
				result.VelocityY = reader.ReadDeltaSingle(value0.VelocityY);
			else
				result.VelocityY = value0.VelocityY;

			if ((totalFlags & 256) == 256)
				result.VelocityZ = reader.ReadDeltaSingle(value0.VelocityZ);
			else
				result.VelocityZ = value0.VelocityZ;

			if ((totalFlags & 512) == 512)
				result.StructC = reader.ReadDeltaClientAssembly_Player_MyStructB_MyStructC(value0.StructC);
			else
				result.StructC = value0.StructC;

			return result;
		}

		public static ClientAssembly.MyStructA ReadDeltaClientAssembly_MyStructA(this FishNet.Serializing.Reader reader, in ClientAssembly.MyStructA value0, FishNet.Serializing.DeltaSerializerOption options = FishNet.Serializing.DeltaSerializerOption.Unset)
		{
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
