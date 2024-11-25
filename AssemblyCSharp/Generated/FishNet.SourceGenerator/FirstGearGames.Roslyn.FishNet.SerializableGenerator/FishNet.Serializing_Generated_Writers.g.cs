//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Writers
	{

		public static void GWrite___WriteClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0)
		{
			writer.WriteUInt8ArrayAndSize(value0.ByteArr);
			writer.WriteArray(value0.StructArr);
			//Serializer not found for type System.Collections.Generic.List<ClientAssembly.Player.NestedStruct>. Type will not be serialized.

			//Serializer not found for type System.Collections.Generic.Dictionary<ClientAssembly.Player.NestedStruct, System.String>. Type will not be serialized.

			writer.WriteArraySegmentAndSize(value0.ArrSegment);
			writer.WriteString(value0.String);
			//Serializer not found for type System.Object. Type will not be serialized.

			//Serializer not found for type ClientAssembly.AnyType<System.Boolean>. Type will not be serialized.


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
