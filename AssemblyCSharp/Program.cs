using FishNet.Object;
using FishNet.Serializing;
using FishNet.Transporting;
using GenerateTest;

namespace ClientAssembly
{
	public struct MyStruct
	{
		public int IntValueA;
		public int IntValueB;
		public int IntValueC;
	}

	public class Player : NetworkBehaviour
	{

		[ServerRpc]
		private void MyRpcTwo(MyStruct ms)
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
