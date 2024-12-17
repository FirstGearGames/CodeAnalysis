using FishNet.Connection;
using FishNet.Serializing;
using FishNet.Transporting;

namespace FishNet.Object
{
    public partial class NetworkBehaviour
    {
        protected internal void SendServerRpc(uint hash, PooledWriter methodWriter, Channel channel, DataOrderType orderType) { }
        protected internal void SendObserversRpc(uint hash, PooledWriter methodWriter, Channel channel, DataOrderType orderType, bool bufferLast, bool excludeServer, bool excludeOwner) { }
        protected internal void SendTargetRpc(uint hash, PooledWriter methodWriter, Channel channel, DataOrderType orderType, NetworkConnection target, bool excludeServer, bool validateTarget = true) { }
    }
}
