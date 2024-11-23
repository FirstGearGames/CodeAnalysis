//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Readers
	{

		public static ClientAssembly.Player.NestedStruct GRead___ReadClientAssembly_Player_NestedStruct(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.NestedStruct result = new();

			result.ByteArr = reader.ReadUInt8ArrayAndSizeAllocated();
			result.StructArr = reader.ReadArrayAllocated<ClientAssembly.Player.NestedStruct>();
			result.StructLst = reader.ReadListAllocated<ClientAssembly.Player.NestedStruct>();
			result.String = reader.ReadString();
			result.LstStruct = reader.ReadListAllocated<ClientAssembly.Player.NestedStruct>();
			result.ArrSegment = reader.ReadArraySegmentAndSize();
			//Serializer not found for System.Object. This will cause failure at runtime.
			result.ObjectType = reader.Read<System.Object>();
			//Serializer not found for ClientAssembly.AnyType<System.Boolean>. This will cause failure at runtime.
			result.GenericObjectType = reader.Read<ClientAssembly.AnyType<System.Boolean>>();

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
