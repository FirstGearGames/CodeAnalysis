//FishNet generated file.
namespace FishNet.Serializing
{
	public static class Generated_Writers
	{

		public static void GWrite___WriteClientAssembly_Player_EmptyStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.EmptyStruct value0)
		{

		}

		public static void GWrite___WriteClientAssembly_Player_NestedStruct(this FishNet.Serializing.Writer writer, ClientAssembly.Player.NestedStruct value0)
		{

		}

		public static void GWrite___WriteClientAssembly_Player_MyStructC(this FishNet.Serializing.Writer writer, ClientAssembly.Player.MyStructC value0)
		{

		}

		[UnityEngine.RuntimeInitializeOnLoadMethod]
		public static void InitializeSerializers()
		{
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.EmptyStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.EmptyStruct>(GWrite___WriteClientAssembly_Player_EmptyStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.NestedStruct>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.NestedStruct>(GWrite___WriteClientAssembly_Player_NestedStruct));
			FishNet.Serializing.GenericWriter<ClientAssembly.Player.MyStructC>.SetWrite(new System.Action<FishNet.Serializing.Writer, ClientAssembly.Player.MyStructC>(GWrite___WriteClientAssembly_Player_MyStructC));
		}
	}
}
