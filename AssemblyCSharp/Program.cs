using FishNet.Object;
using FishNet.Serializing;
using FishNet.Transporting;
using FishNet.Serializing;

namespace ClientAssembly
{
	public struct MyStructA
	{
		public int IntValueA;
		public float FloatValueA;
		public MyStructB StructB;
	}

	public struct MyStructB
	{
		public bool BoolValueA;
	}

	
	public class Player : NetworkBehaviour
	{

		[ServerRpc]
		private void MyRpcTwo(MyStructA ms)
		{
		}

		// [ServerRpc]
		// private void MyRpc(int value, Channel channel = Channel.Unreliable)
		// {
		// }

		public void DoThing()
		{
			Writer writer = new();
		
		}
	}
}
