//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Writers
	{

		public static void GWrite___WriteClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0)
		{
			writer.WriteUInt8ArrayAndSize(value1.ByteArr);
			writer.WriteArray<ClientAssembly.Player.NestedStruct>(value1.StructArr);
			writer.WriteString(value1.String);
			writer.WriteList<ClientAssembly.Player.NestedStruct>(value1.LstStruct);
			writer.WriteArraySegmentAndSize(value1.ArrSegment);
			//Serializer not found for System.Object. This will cause failure at runtime.
			writer.Write<System.Object>(value1.ObjectType);

		}

		public static void GWrite___WriteClientAssembly_Player_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructC value0)
		{
			writer.WriteBoolean(value1.Works);

		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct, ClientAssembly.Player.NestedStruct, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericDeltaWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Func<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC, ClientAssembly.Player.MyStructC, FishNet.Serializing.DeltaSerializerOption, System.Boolean>(GWrite___WriteClientAssembly_Player_MyStructC));
		}
	}
}
