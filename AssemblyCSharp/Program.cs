using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Serializing;
using FishNet.Transporting;


namespace ClientAssembly
{
	public struct MyStructA
	{
		public float Horizontal;
		public float Vertical;
		public bool Running;
		public bool Firing;
		public bool Jumping;

	}

	//public struct MyStructB
	//{
	//	public float PositionX;
	//	public float PositionY;
	//	public float PositionZ;

	//	public bool Hits;
	//	public float Stamina;
	//	public float VelocityX;
	//	public float VelocityY;
	//	public float VelocityZ;
	//}

	[IncludeSerialization]
	public class MyThingA : MyThingB
	{
		public string A;
		public MyThingB C;
	}

	public class MyThingB
	{
		public string B;
	}

	public class Player : NetworkBehaviour
	{

        public struct MyStructB
        {
            public float PositionX;
            public float PositionY;
            public float PositionZ;

            public bool Hits;
            public float Stamina;
            public float VelocityX;
            public float VelocityY;
            public float VelocityZ;
        }

  //      [ServerRpc]
		//private void MyRpcOne(MyStructA ms)
		//{
		//}
		
		//[ServerRpc]
		//private void MyRpcTwo(MyStructB ms)
		//{
		//}

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
