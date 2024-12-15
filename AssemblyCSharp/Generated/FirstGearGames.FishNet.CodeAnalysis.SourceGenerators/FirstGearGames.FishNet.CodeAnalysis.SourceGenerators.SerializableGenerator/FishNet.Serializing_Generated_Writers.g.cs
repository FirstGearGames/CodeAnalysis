//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Writers
	{

		public static void GWrite___WriteClientAssembly_SimpleStructA(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructA value0)
		{
			writer.Write<System.Int32>(value0.TheNumber);

		}

		public static void GWrite___WriteClientAssembly_SimpleStructB(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructB value0)
		{
			writer.Write<System.Int32>(value0.TheNumber);

		}

		public static void GWrite___WriteClientAssembly_SimpleStructC(this FishNet.Serializing.Writer writer, ClientAssembly.SimpleStructC value0)
		{
			writer.Write<System.Int32>(value0.TheNumber);

		}

		public static void GWrite___WriteClientAssembly_Player_EmptyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.EmptyStruct value0)
		{

		}

		public static void GWrite___WriteClientAssembly_Player_BroadcastStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.BroadcastStruct value0)
		{
			writer.Write<System.Boolean>(value0.IsBroadcast);

		}

		public static void GWrite___WriteClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0)
		{
			writer.Write<ClientAssembly.Player.EmptyStruct>(value0.Struct);
			writer.Write<System.Byte[]>(value0.ByteArr);
			writer.Write<ClientAssembly.Player.NestedStruct[]>(value0.StructArr);
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrMultiDimensional. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.StructArrJagged. Value will not be serialized.
			writer.Write<System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>>(value0.StructLst);
			writer.Write<System.Collections.Generic.List<System.ValueTuple<System.Boolean, System.String>>>(value0.TupleLst);
			writer.Write<System.Collections.Generic.Dictionary<ClientAssembly.Player.NestedStruct, System.String>>(value0.StructDict);
			writer.Write<System.ArraySegment<System.Byte>>(value0.ArrSegment);
			writer.Write<System.String>(value0.String);
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.ObjectType. Value will not be serialized.
			//Serializer not found for ClientAssembly.Player.NestedStruct.ClientAssembly.Player.NestedStruct.GenericObjectType. Value will not be serialized.

		}

		public static void GWrite___WriteClientAssembly_Player_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructC value0)
		{
			writer.Write<System.Boolean>(value0.Works);

		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericWriter<ClientAssembly.SimpleStructA>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.SimpleStructA>(GWrite___WriteClientAssembly_SimpleStructA));
			FishNet.Serializing.GenericWriter<ClientAssembly.SimpleStructB>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.SimpleStructB>(GWrite___WriteClientAssembly_SimpleStructB));
			FishNet.Serializing.GenericWriter<ClientAssembly.SimpleStructC>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.SimpleStructC>(GWrite___WriteClientAssembly_SimpleStructC));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.EmptyStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.EmptyStruct>(GWrite___WriteClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.BroadcastStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.BroadcastStruct>(GWrite___WriteClientAssembly_Player_BroadcastStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct>(GWrite___WriteClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC>(GWrite___WriteClientAssembly_Player_MyStructC));
		}
	}
}
