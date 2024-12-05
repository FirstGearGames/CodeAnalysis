//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Writers
	{

		public static void GWrite___WriteClientAssembly_SimpleStructA(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructA value0)
		{
			writer.WriteInt32(value0.TheNUmber);

		}

		public static void GWrite___WriteClientAssembly_SimpleStructB(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructB value0)
		{
			writer.WriteInt32(value0.TheNUmber);

		}

		public static void GWrite___WriteClientAssembly_SimpleStructC(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructC value0)
		{
			writer.WriteInt32(value0.TheNUmber);

		}

		public static void GWrite___WriteClientAssembly_Player_BroadcastStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.BroadcastStruct value0)
		{
			writer.WriteBoolean(value0.IsBroadcast);

		}

		public static void GWrite___WriteClientAssembly_Player_EmptyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.EmptyStruct value0)
		{

		}

		public static void GWrite___WriteClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0)
		{
			writer.GWrite___WriteClientAssembly_Player_EmptyStruct(value0.Struct);
			writer.WriteUInt8ArrayAndSize(value0.ByteArr);
			writer.WriteArray(value0.StructArr);
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrMultiDimensional. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrJagged. Value will not be serialized.
			writer.WriteList(value0.StructLst);
			writer.WriteList(value0.TupleLst);
			writer.WriteDictionary(value0.StructDict);
			writer.WriteArraySegmentAndSize(value0.ArrSegment);
			writer.WriteString(value0.String);
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.ObjectType. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.GenericObjectType. Value will not be serialized.

		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericWriter<ClientAssembly.SimpleStructA>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.SimpleStructA>(GWrite___WriteClientAssembly_SimpleStructA));
			FishNet.Serializing.GenericWriter<ClientAssembly.SimpleStructB>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.SimpleStructB>(GWrite___WriteClientAssembly_SimpleStructB));
			FishNet.Serializing.GenericWriter<ClientAssembly.SimpleStructC>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.SimpleStructC>(GWrite___WriteClientAssembly_SimpleStructC));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.BroadcastStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.BroadcastStruct>(GWrite___WriteClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.EmptyStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.EmptyStruct>(GWrite___WriteClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct>(GWrite___WriteClientAssembly_Player_NestedStruct));
		}
	}
}
