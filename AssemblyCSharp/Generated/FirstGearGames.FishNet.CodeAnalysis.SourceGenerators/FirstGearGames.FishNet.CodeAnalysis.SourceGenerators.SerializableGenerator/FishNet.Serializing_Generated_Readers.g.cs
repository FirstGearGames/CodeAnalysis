//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Readers
	{

		public static ClientAssembly.SimpleStructA GRead___ReadClientAssembly_SimpleStructA(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.SimpleStructA result = new();

			result.TheNumber = reader.ReadInt32System.Int32();
			return result;
		}

		public static ClientAssembly.SimpleStructB GRead___ReadClientAssembly_SimpleStructB(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.SimpleStructB result = new();

			result.TheNumber = reader.ReadInt32System.Int32();
			return result;
		}

		public static ClientAssembly.SimpleStructC GRead___ReadClientAssembly_SimpleStructC(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.SimpleStructC result = new();

			result.TheNumber = reader.ReadInt32System.Int32();
			return result;
		}

		public static ClientAssembly.Player.EmptyStruct GRead___ReadClientAssembly_Player_EmptyStruct(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.EmptyStruct result = new();

			return result;
		}

		public static ClientAssembly.Player.BroadcastStruct GRead___ReadClientAssembly_Player_BroadcastStruct(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.BroadcastStruct result = new();

			result.IsBroadcast = reader.ReadBooleanSystem.Boolean();
			return result;
		}

		public static ClientAssembly.Player.NestedStruct GRead___ReadClientAssembly_Player_NestedStruct(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.NestedStruct result = new();

			result.Struct = reader.GRead___ReadClientAssembly_Player_EmptyStructClientAssembly.Player.EmptyStruct();
			result.ByteArr = reader.ReadUInt8ArrayAndSizeAllocatedSystem.Byte[]();
			result.StructArr = reader.ReadArrayAllocatedClientAssembly.Player.NestedStruct[]();
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrMultiDimensional. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrJagged. Value will not be serialized.
			result.StructLst = reader.ReadListAllocatedSystem.Collections.Generic.List<ClientAssembly.Player.NestedStruct>();
			result.TupleLst = reader.ReadListAllocatedSystem.Collections.Generic.List<System.ValueTuple<System.Boolean, System.String>>();
			result.StructDict = reader.ReadDictionaryAllocatedSystem.Collections.Generic.Dictionary<ClientAssembly.Player.NestedStruct, System.String>();
			result.ArrSegment = reader.ReadArraySegmentAndSizeSystem.ArraySegment<System.Byte>();
			result.String = reader.ReadStringSystem.String();
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.ObjectType. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.GenericObjectType. Value will not be serialized.
			return result;
		}

		public static ClientAssembly.Player.MyStructC GRead___ReadClientAssembly_Player_MyStructC(this FishNet.Serializing.Reader reader)
		{
			ClientAssembly.Player.MyStructC result = new();

			result.Works = reader.ReadBooleanSystem.Boolean();
			return result;
		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericReader<ClientAssembly.SimpleStructA>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.SimpleStructA>(GRead___ReadClientAssembly_SimpleStructA));
			FishNet.Serializing.GenericReader<ClientAssembly.SimpleStructB>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.SimpleStructB>(GRead___ReadClientAssembly_SimpleStructB));
			FishNet.Serializing.GenericReader<ClientAssembly.SimpleStructC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.SimpleStructC>(GRead___ReadClientAssembly_SimpleStructC));
			FishNet.Serializing.GenericReader<ClientAssembly.Player.EmptyStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.EmptyStruct>(GRead___ReadClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericReader<ClientAssembly.Player.BroadcastStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.BroadcastStruct>(GRead___ReadClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericReader<ClientAssembly.Player.NestedStruct>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.NestedStruct>(GRead___ReadClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericReader<ClientAssembly.Player.MyStructC>.SetRead(new System.Func<FishNet.Serializing.Reader, ClientAssembly.Player.MyStructC>(GRead___ReadClientAssembly_Player_MyStructC));
		}
	}
}
