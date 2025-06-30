#if UNITY_EDITOR
using System.Collections.Generic;
using FishNet.Managing.Statistic;
using FishNet.Managing.Timing;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;

namespace FishNet.Editing
{

    /// <summary>
    /// Bytes collected for each packet type.
    /// </summary>
    /// <remarks>A class is used rather than a struct so values can be modified via reference.</remarks>
    internal class PacketTotalBytes : IResettable
    {
        /// <summary>
        /// PacketId bytes are for.
        /// </summary>
        public PacketId PacketId = PacketId.Unset;
        /// <summary>
        /// Total inbound bytes for PacketId.
        /// </summary>
        public ulong InboundBytes;
        /// <summary>
        /// Total outbound bytes for PacketId.
        /// </summary>
        public ulong OutboundBytes;
        /// <summary>
        /// True if has been Initialized with a PacketId.
        /// </summary>
        /// <remarks>True does not indicate all bytes have been added.</remarks>
        public bool IsInitialized => PacketId != PacketId.Unset;

        /// <summary>
        /// True if the packetId is for data which is not known or handled.
        /// </summary>
        /// <returns></returns>
        public bool IsOtherPacketId() => (ushort)PacketId == NetworkProfilerWindow.UnspecifiedPacketId;

        public PacketTotalBytes() { }

        public void Initialize(PacketId packetId)
        {
            PacketId = packetId;
        }

        /// <summary>
        /// Adds onto inbound bytes.
        /// </summary>
        public void AddInboundBytes(ulong bytes) => InboundBytes = bytes;

        /// <summary>
        /// Adds onto inbound bytes.
        /// </summary>
        public void AddOutboundBytes(ulong bytes) => OutboundBytes = bytes;

        public void ResetState()
        {
            PacketId = PacketId.Unset;
            InboundBytes = 0;
        }

        public void InitializeState() { }
    }

    /// <summary>
    /// Data for a profiled tick. 
    /// </summary>
    internal class ProfiledTickData : IResettable
    {

Read below.
// Todo -- This has to be done so we can sort details panel as well so we can draw lines on graph window.

// Todo -- This needs to set the percentiles of each packet after all packets have been 
// Todo -- There is probably no reason to have PacketTotalBytes, given it's intended to cover inbound/outbound in a single class.
// Todo -- ^ Get rid of these variables below.

// Todo -- Instead, within TrafficCollection, have a public dictionary (PacketSums), name not final) with PacketId as the key, and a class
// Todo -- (TotalDetails, name not final) which has: PacketId, TotalBytes, Percentage. 

// Todo -- In a new method, after all Entries are added to TrafficCollection, iterate the entries and storing them to PacketSums.
// Todo -- When iterating entries increase the a local entriesTotalBytes with each iteration.
// Todo -- Note: percentage on TotalDetails cannot be calculated in PacketSums until all Entries are iterated.

// Todo -- Reiterate PacketSums dictionary now setting the Percentage on TotalDetails based on it's total bytes vs the
// Todo -- entriesTotalBytes we've been increasing.

        /// <summary>
        /// Tick this is for.
        /// </summary>
        public uint Tick;
        /// <summary>
        /// Total bytes for each packet for the server.
        /// </summary>
        private Dictionary<PacketId, PacketTotalBytes> _serverPacketTotalBytes;
        /// <summary>
        /// Total bytes for each packet for the client.
        /// </summary>
        private Dictionary<PacketId, PacketTotalBytes> _clientPacketTotalBytes;
        /// <summary>
        /// Traffic collection for the server.
        /// </summary>
        private MultiwayTrafficCollection _serverTraffic;
        /// <summary>
        /// Traffic collection for the client.
        /// </summary>
        private MultiwayTrafficCollection _clientTraffic;

        public void Initialize(uint tick, MultiwayTrafficCollection serverTraffic, MultiwayTrafficCollection clientTraffic)
        {
            Tick = tick;

            _serverTraffic = serverTraffic.CloneUsingCache();
            _clientTraffic = clientTraffic.CloneUsingCache();
        }

        /// <summary>
        /// Returns data for server or client.
        /// </summary>
        public void GetValues(out MultiwayTrafficCollection trafficCollection, out Dictionary<PacketId, PacketTotalBytes> packetTotalBytes, bool asServer)
        {
            if (asServer)
            {
                trafficCollection = _serverTraffic;
                packetTotalBytes = _serverPacketTotalBytes;
            }
            else
            {
                trafficCollection = _clientTraffic;
                packetTotalBytes = _clientPacketTotalBytes;
            }
        }

        /// <summary>
        /// Initializes total bytes for each packet in traffic if not already done.
        /// </summary>
        private Dictionary<PacketId, PacketTotalBytes> GetPopulatedPacketTotalBytes(bool asServer)
        {
            Dictionary<PacketId, PacketTotalBytes> collection;

            if (asServer)
            {
                if (_serverPacketTotalBytes == null)
                    PopulateCollectionUsingCache(ref _serverPacketTotalBytes, _serverTraffic);

                collection = _serverPacketTotalBytes;
            }
            else
            {
                if (_clientPacketTotalBytes == null)
                    PopulateCollectionUsingCache(ref _clientPacketTotalBytes, _clientTraffic);

                collection = _serverPacketTotalBytes;
            }

            return collection;

            //Sets value to lPidBytes using cache and populates using trafficCollection.
            static void PopulateCollectionUsingCache(ref Dictionary<PacketId, PacketTotalBytes> refPidBytes, MultiwayTrafficCollection trafficCollection)
            {
                /* We need to pass initial collection as ref so the field can be
                 * populated. Entries can be added using a local reference. */
                refPidBytes = ResettableT2CollectionCaches<PacketId, PacketTotalBytes>.RetrieveDictionary();

                //To access more easily throughout this method since ref cannot be used in this context.
                Dictionary<PacketId, PacketTotalBytes> collection = refPidBytes;

                AddBytesForEntries(trafficCollection.Inbound.Entries, inbound: true);
                AddBytesForEntries(trafficCollection.Outbound.Entries, inbound: false);

                //Iterates entries and adds bytes to packet total bytes.
                void AddBytesForEntries(List<TrafficCollection.TrafficEntry> entries, bool inbound)
                {
                    foreach (TrafficCollection.TrafficEntry entry in entries)
                    {
                        PacketId packetId = entry.PacketId;

                        if (!collection.TryGetValueIL2CPP(packetId, out PacketTotalBytes packetTotalBytes))
                        {
                            packetTotalBytes = ResettableObjectCaches<PacketTotalBytes>.Retrieve();
                            collection[packetId] = packetTotalBytes;
                        }

                        ulong bytes = (ulong)entry.Bytes;

                        if (inbound)
                            packetTotalBytes.AddOutboundBytes(bytes);
                        else
                            packetTotalBytes.AddInboundBytes(bytes);
                    }
                }
            }
        }

        /// <summary>
        /// Returns collection for total bytes for each packet.
        /// </summary>
        public Dictionary<PacketId, PacketTotalBytes> GetPacketTotalBytes(bool asServer)
        {
            return GetPopulatedPacketTotalBytes(asServer);
        }

        /// <summary>
        /// Returns PacketTotalBytes for a PacketId.
        /// </summary>
        public PacketTotalBytes GetPacketTotalBytes(PacketId packetId, bool asServer)
        {
            Dictionary<PacketId, PacketTotalBytes> collection = GetPacketTotalBytes(asServer);

            collection.TryGetValueIL2CPP(packetId, out PacketTotalBytes packetTotalBytes);

            return packetTotalBytes;
        }

        /// <summary>
        /// Resets all values and stores to caches as needed.
        /// </summary>
        public void ResetState()
        {
            Tick = TimeManager.UNSET_TICK;

            ResettableObjectCaches<MultiwayTrafficCollection>.StoreAndDefault(ref _serverTraffic);
            ResettableObjectCaches<MultiwayTrafficCollection>.StoreAndDefault(ref _clientTraffic);

            ResettableT2CollectionCaches<PacketId, PacketTotalBytes>.StoreAndDefault(ref _serverPacketTotalBytes);
            ResettableT2CollectionCaches<PacketId, PacketTotalBytes>.StoreAndDefault(ref _clientPacketTotalBytes);
        }

        public void InitializeState() { }
    }

}
#endif