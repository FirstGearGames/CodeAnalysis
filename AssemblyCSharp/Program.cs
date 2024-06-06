using FishNet.Object;
using FishNet.Serializing;
using FishNet.Transporting;
using GenerateTest;

namespace ClientAssembly
{
	public class Player : NetworkBehaviour
	{
		[ServerRpc]
		private void MyRpc(int value, Channel channel = Channel.Unreliable) { }

		public void DoThing()
		{
			Writer writer = new();
		
		}
	}
}
