using FishNet.Broadcast;
using FishNet.CodeGenerating;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Object.Synchronizing;
using FishNet.Runtime.Unity_Stuff;
using FishNet.Serializing;
using FishNet.Transporting;

namespace ClientAssembly
{
    // public struct MyStructA
    // {
    // 	public float Horizontal;
    // 	public float Vertical;
    // 	public bool Running;
    // 	public bool Firing;
    // 	public bool Jumping;
    //
    // }

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

    //[IncludeSerialization]
    //    public class MyThingA : MyThingB, IReplicateData
    // {
    // 	public Vector3 Position;
    // 	public string A;
    // 	public MyThingB C;
    //
    // 	private uint _tick;
    // }

    //    [IncludeSerialization]
    //   public class MyThingB
    // {
    // 	public string B;
    // 	public Dictionary<int, bool> Dict;
    // 	public byte[] Bytey;
    // }

    // [IncludeSerialization]
    //    public struct MyThingC : IReplicateData
    // {
    //        public string C;
    //    }

    public class AnyType<T>
    {
        public List<T> Lst;
    }

    public struct SimpleStructA
    {
        public int TheNUmber;
    }

    public struct SimpleStructB
    {
        public int TheNUmber;
    }

    public struct SimpleStructC
    {
        public int TheNUmber;
    }

    public class MyCustomSync : SyncBase, ICustomSync
    {
        public object GetSerializedType()
        {
            // if (Environment.TickCount == 100)
            //     return new SimpleStructB();

            return new SimpleStructC();
        }
    }

    public class Player : NetworkBehaviour
    {
        public SyncDictionary<SimpleStructA, SimpleStructB> _syncDictionary = new();
        public MyCustomSync _customSync = new();
        
        public struct EmptyStruct { }

        public struct BroadcastStruct : IBroadcast
        {
            public bool IsBroadcast;
        }

        public struct NestedStruct : IReplicateData
        {
            public EmptyStruct Struct;
            public byte[] ByteArr;
            public NestedStruct[] StructArr;
            public NestedStruct[,,] StructArrMultiDimensional;
            public NestedStruct[][] StructArrJagged;
            public List<NestedStruct> StructLst;
            public List<(bool, string)> TupleLst;
            public Dictionary<NestedStruct, string> StructDict;
            public ArraySegment<byte> ArrSegment;
            public string String;
            public System.Object ObjectType;
            public AnyType<bool> GenericObjectType;
        }

        //[IncludeSerialization]
        //      public class MyStructB
        //      {
        //          public float PositionX;
        //          public float PositionY;
        //          public float PositionZ;

        //          public bool Hits;
        //          public float Stamina;
        //          public float VelocityX;
        //          public float VelocityY;
        //          public float VelocityZ;
        //	public MyStructC StructC;
        [IncludeSerialization]
        public class MyStructC
        {
            public bool Works;
        }
        //      }

        //      [ServerRpc]
        //private void MyRpcOne(MyStructA ms)
        //{
        //}

        [ServerRpc]
        private void MyRpcTwo(MyStructC ms) { }

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