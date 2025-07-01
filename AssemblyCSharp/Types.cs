#if UNITY_EDITOR
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using FishNet.Managing.Statistic;
using FishNet.Managing.Timing;
using FishNet.Transporting;
using GameKit.Dependencies.Utilities;
using UnityEngine;

namespace FishNet.Editing
{
    /// <summary>
    /// Used to store Inbound and Outbound traffic details.
    /// </summary>
    internal class BidirectionalNetworkTraffic : IResettable
    {
        private NetworkTraffic _inboundTraffic;
        private NetworkTraffic _outboundTraffic;
        public void ResetState() { }
        public void InitializeState() { }
    }

    internal class NetworkTraffic : IResettable
    {
        #region Types.
        /// <summary>
        /// Information about a single packet.
        /// </summary>
        public struct Packet
        {
            /// <summary>
            /// Details about the packet, such as method or class name.
            /// </summary>
            /// <remarks>This may be empty.</remarks>
            public string Details;
            /// <summary>
            /// Bytes used.
            /// </summary>
            public ulong Bytes;
            /// <summary>
            /// Originating GameObject.
            /// </summary>
            /// <remarks>GameObject is used rather than a script reference because we do not want to risk unintentionally holding a script in memory. Unity will automatically clean up GameObjects, so they are safe to reference.</remarks>
            public GameObject GameObject;
            
            public Packet(ulong bytes) : this(details: string.Empty, bytes, gameObject: null) { }
            public Packet(string details, ulong bytes) : this(details, bytes, gameObject: null) { }
            public Packet(ulong bytes, GameObject gameObject) : this(details: string.Empty, bytes, gameObject) { }

            public Packet(string details, ulong bytes, GameObject gameObject)
            {
                Details = details;
                Bytes = bytes;
                GameObject = gameObject;
            }
        }

        /// <summary>
        /// Container for multiple Packets of the same type.
        /// </summary>
        public class PacketGroup : IResettable
        {
            /// <summary>
            /// PacketId of this metric.
            /// </summary>
            public PacketId PacketId { get; private set; } = PacketId.Unset;
            /// <summary>
            /// Bytes of all packets using PacketId.
            /// </summary>
            public int Bytes { get; private set; }
            /// <summary>
            /// Percent Bytes is when compared against Bytes of other PacketMetrics.
            /// </summary>
            /// <remarks>This can only be completed after all Packet entries for each PacketId are added.</remarks>
            public float Percent { get; private set; }

            public bool IsUnspecifiedPacketId => (ushort)PacketId == NetworkProfilerWindow.UNSPECIFIED_PACKETID;
            
            private List<Packet> _packets = new();

            public void Initialize(PacketId packetId) 
            {
                PacketId = packetId;
            }
            public void Initialize(PacketId packetId, ulong bytes) => Initialize(packetId, details: string.Empty, bytes, gameObject: null);
            public void Initialize(PacketId packetId, ulong bytes, GameObject gameObject) => Initialize(packetId, details: string.Empty, bytes, gameObject);
            public void Initialize(PacketId packetId, string details,  ulong bytes) => Initialize(packetId, details, bytes, gameObject: null);
            public void Initialize(PacketId packetId, string details, ulong bytes, GameObject gameObject) 
            {
                PacketId = packetId;
                
                _packets.Add(new(details, bytes, gameObject));
            }

            public void ResetState()
            {
                PacketId = PacketId.Unset;
                Bytes = 0;
                Percent = 0f;
                _packets.Clear();
            }

            public void InitializeState() { }
        }
        #endregion

        private Dictionary<PacketId, PacketGroup> _packetGroups = new();

        public void ResetState()
        {
            ResettableT2CollectionCaches<PacketId, PacketGroup>.StoreAndDefault(ref _packetGroups);
        }

        public void InitializeState() { }
    }

    Read below.
    // Todo -- This has to be done so we can sort details panel as well so we can draw lines on graph window.

    // Todo -- This needs to set the percentiles of each packet after all packets have been 
    // Todo -- There is probably no reason to have PacketTotalBytes, given it's intended to cover inbound/outbound in a single class.
    // Todo -- ^ Get rid of these variables below.

    // Todo -- Instead, within NetworkTraffic, have a public dictionary (PacketSums), name not final) with PacketId as the key, and a class
    // Todo -- (TotalDetails, name not final) which has: PacketId, TotalBytes, Percentage. 

    // Todo -- In a new method, after all Entries are added to NetworkTraffic, iterate the entries and storing them to PacketSums.
    // Todo -- When iterating entries increase the a local entriesTotalBytes with each iteration.
    // Todo -- Note: percentage on TotalDetails cannot be calculated in PacketSums until all Entries are iterated.

    // Todo -- Reiterate PacketSums dictionary now setting the Percentage on TotalDetails based on it's total bytes vs the
    // Todo -- entriesTotalBytes we've been increasing.
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
        public bool IsOtherPacketId() => (ushort)PacketId == NetworkProfilerWindow.UNSPECIFIED_PACKETID;

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
        private BidirectionalNetworkTraffic _serverTraffic;
        /// <summary>
        /// Traffic collection for the client.
        /// </summary>
        private BidirectionalNetworkTraffic _clientTraffic;

        public void Initialize(uint tick, BidirectionalNetworkTraffic serverTraffic, BidirectionalNetworkTraffic clientTraffic)
        {
            Tick = tick;

            _serverTraffic = serverTraffic.CloneUsingCache();
            _clientTraffic = clientTraffic.CloneUsingCache();
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
            static void PopulateCollectionUsingCache(ref Dictionary<PacketId, PacketTotalBytes> refPidBytes, BidirectionalNetworkTraffic trafficCollection)
            {
                /* We need to pass initial collection as ref so the field can be
                 * populated. Entries can be added using a local reference. */
                refPidBytes = ResettableT2CollectionCaches<PacketId, PacketTotalBytes>.RetrieveDictionary();

                //To access more easily throughout this method since ref cannot be used in this context.
                Dictionary<PacketId, PacketTotalBytes> collection = refPidBytes;

                AddBytesForEntries(trafficCollection.Inbound.Entries, inbound: true);
                AddBytesForEntries(trafficCollection.Outbound.Entries, inbound: false);

                //Iterates entries and adds bytes to packet total bytes.
                void AddBytesForEntries(List<NetworkTraffic.TrafficEntry> entries, bool inbound)
                {
                    foreach (NetworkTraffic.TrafficEntry entry in entries)
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

            ResettableObjectCaches<BidirectionalNetworkTraffic>.StoreAndDefault(ref _serverTraffic);
            ResettableObjectCaches<BidirectionalNetworkTraffic>.StoreAndDefault(ref _clientTraffic);

            ResettableT2CollectionCaches<PacketId, PacketTotalBytes>.StoreAndDefault(ref _serverPacketTotalBytes);
            ResettableT2CollectionCaches<PacketId, PacketTotalBytes>.StoreAndDefault(ref _clientPacketTotalBytes);
        }

        public void InitializeState() { }
    }
}
#endif