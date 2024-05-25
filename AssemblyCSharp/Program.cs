using FishNet.Object;
using FishNet.Transporting;

namespace ClientAssembly
{

    public class Player : NetworkBehaviour
    {
        //public SyncVar<int> Count;
        //public SyncVar<Health> HealthInformation;

        [ServerRpc]
        private void MyRpc(int value, Channel c = Channel.Unreliable) { }
        //static void Main(string[] args)
        //{
        //    Console.WriteLine("Started.");
        //    //HelloFrom("Generated Code");
        //}
  //      		<PackageReference Include = "Microsoft.CodeAnalysis.CSharp" Version="4.9.2" PrivateAssets="all" />
		//<PackageReference Include = "Microsoft.CodeAnalysis.Analyzers" Version="3.3.4" PrivateAssets="all" />
       // static partial void HelloFrom(string name);

    }
     
     
    //public struct Health
    //{
    //    public int Value;
    //}


    //public class Player : NetworkBehaviour
    //{ 
    //    public SyncVar<int> Count;
    //    public SyncVar<Health> HealthInformation;

    //    [ServerRpc]
    //    private void SvrRpc(int value) { }
    //}

    //public class SyncVar<T> { }
    

}
