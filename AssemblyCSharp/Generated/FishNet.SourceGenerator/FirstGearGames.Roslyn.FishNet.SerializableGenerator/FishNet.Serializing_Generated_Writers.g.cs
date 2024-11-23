//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Writers
	{

		public static void GWrite___WriteClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0)
		{
			writer.WriteUInt8ArrayAndSize(value0.ByteArr);
			writer.WriteArray<ClientAssembly.Player.NestedStruct>(value0.StructArr);
			writer.WriteList<ClientAssembly.Player.NestedStruct>(value0.StructLst);
			writer.WriteString(value0.String);
			writer.WriteList<ClientAssembly.Player.NestedStruct>(value0.LstStruct);
			writer.WriteArraySegmentAndSize(value0.ArrSegment);
			//Serializer not found for System.Object. This will cause failure at runtime.
			writer.Write<System.Object>(value0.ObjectType);
			//Serializer not found for ClientAssembly.AnyType<System.Boolean>. This will cause failure at runtime.
			writer.Write<ClientAssembly.AnyType<System.Boolean>>(value0.GenericObjectType);

		}

		public static void GWrite___WriteClientAssembly_Player_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructC value0)
		{
			writer.WriteBoolean(value0.Works);

		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct>(GWrite___WriteClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC>(GWrite___WriteClientAssembly_Player_MyStructC));
		}
	}
}
