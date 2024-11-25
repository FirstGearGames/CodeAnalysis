//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Readers
	{

		public static ClientAssembly.Player.NestedStruct GRead___ReadClientAssembly_Player_NestedStruct(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.NestedStruct result = new();

			result.ByteArr = reader.ReadUInt8ArrayAndSizeAllocated();
			result.StructArr = reader.ReadArrayAllocated();
			result.StructLst = reader.ReadListAllocated();
			result.String = reader.ReadString();
			result.LstStruct = reader.ReadListAllocated();
			result.ArrSegment = reader.ReadArraySegmentAndSize();
			//Serializer not found for type System.Object. Type will not be serialized.

			//Serializer not found for type ClientAssembly.AnyType<System.Boolean>. Type will not be serialized.

			return result;
		}

		public static ClientAssembly.Player.MyStructC GRead___ReadClientAssembly_Player_MyStructC(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.MyStructC result = new();

			result.Works = reader.ReadBoolean();
			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericReader<ClientAssembly.Player.NestedStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.NestedStruct>(GRead___ReadClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericReader<ClientAssembly.Player.MyStructC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.MyStructC>(GRead___ReadClientAssembly_Player_MyStructC));
		}
	}
}
