#if UNITY_EDITOR
using System;
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
        /// <summary>
        /// Tick traffic is for.
        /// </summary>
        public uint Tick;
        /// <summary>
        /// Received traffic.
        /// </summary>
        private NetworkTraffic _inboundTraffic;
        /// <summary>
        /// Sent traffic.
        /// </summary>
        private NetworkTraffic _outboundTraffic;

        /// <summary>
        /// Creates a clone of this class using cache.
        /// </summary>
        /// <returns></returns>
        public BidirectionalNetworkTraffic CloneUsingCache()
        {
            BidirectionalNetworkTraffic traffic = ResettableObjectCaches<BidirectionalNetworkTraffic>.Retrieve();
            
            traffic.Tick = Tick;
            traffic._inboundTraffic = _inboundTraffic;
            traffic._outboundTraffic = _outboundTraffic;

            return traffic;
        }
        
        public void ResetState()
        {
            Tick = TimeManager.UNSET_TICK;
            
            ResettableObjectCaches<NetworkTraffic>.StoreAndDefault(ref _inboundTraffic);
            ResettableObjectCaches<NetworkTraffic>.StoreAndDefault(ref _outboundTraffic);
        }
        
        public void InitializeState()
        {
            _inboundTraffic = ResettableObjectCaches<NetworkTraffic>.Retrieve();
            _outboundTraffic = ResettableObjectCaches<NetworkTraffic>.Retrieve();
        }
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
            public ulong Bytes { get; private set; }
            /// <summary>
            /// Percent Bytes is when compared against Bytes of other PacketMetrics.
            /// </summary>
            /// <remarks>This can only be completed after all Packet entries for each PacketId are added.</remarks>
            public float Percent { get; private set; }
            /// <summary>
            /// True if PacketId is for unspecified packets.
            /// </summary>
            public bool IsUnspecifiedPacketId => PacketId == NetworkTrafficStatistics.UNSPECIFIED_PACKETID;
            /// <summary>
            /// Currently added packets.
            /// </summary>
            private List<Packet> _packets = new();

            public void Initialize(PacketId packetId)
            {
                PacketId = packetId;
            }
            // public void Initialize(PacketId packetId, ulong bytes) => Initialize(packetId, details: string.Empty, bytes, gameObject: null);
            // public void Initialize(PacketId packetId, ulong bytes, GameObject gameObject) => Initialize(packetId, details: string.Empty, bytes, gameObject);
            // public void Initialize(PacketId packetId, string details,  ulong bytes) => Initialize(packetId, details, bytes, gameObject: null);
            // public void Initialize(PacketId packetId, string details, ulong bytes, GameObject gameObject) 
            // {
            //     PacketId = packetId;
            //     
            //     _packets.Add(new(details, bytes, gameObject));
            // }

            /// <summary>
            /// Adds traffic from a specified packetId.
            /// </summary>
            public void AddPacket(string details, ulong bytes, GameObject gameObject)
            {
                Bytes += bytes;

                _packets.Add(new(details, bytes, gameObject));
            }

            /// <summary>
            /// Sets Percent using Bytes against allPacketGroupBytes.
            /// </summary>
            public void SetPercent(ulong allPacketGroupBytes)
            {
                //Prevent divide by 0.
                if (Bytes == 0)
                    Percent = 0;
                else
                    Percent = (float)Bytes / allPacketGroupBytes;
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

        /// <summary>
        /// PacketGroup for each PacketId processed.
        /// </summary>
        private Dictionary<PacketId, PacketGroup> _packetGroups;
        /// <summary>
        /// Total bytes for all PacketGroups.
        /// </summary>
        private ulong _bytes;

        /// <summary>
        /// Adds traffic from a specified packetId.
        /// </summary>
        public void AddPacketIdData(PacketId packetId, string details, ulong bytes, GameObject gameObject) => LAddPacketId(packetId, details, bytes, gameObject);

        /// <summary>
        /// Adds traffic from a specified packetId.
        /// </summary>
        public void AddSocketData(PacketId packetId, string details, ulong bytes, GameObject gameObject) => LAddPacketId(NetworkTrafficStatistics.UNSPECIFIED_PACKETID, details, bytes, gameObject);

        /// <summary>
        /// Adds traffic to a PackerGroup.
        /// </summary>
        private void LAddPacketId(PacketId packetId, string details, ulong bytes, GameObject gameObject)
        {
            if (!_packetGroups.TryGetValue(packetId, out PacketGroup packetGroup))
            {
                packetGroup = ResettableObjectCaches<PacketGroup>.Retrieve();
                packetGroup.Initialize(packetId);

                _packetGroups[packetId] = packetGroup;
            }

            _bytes += bytes;

            packetGroup.AddPacket(details, bytes, gameObject);
        }

        /// <summary>
        /// Calculates and sets Percentage value on each PacketGroup.
        /// </summary>
        /// <remarks>This should only be called after all PacketGroup entries have been created.</remarks>
        public void SetPacketGroupPercentages()
        {
            //Field would probably get cached at runtime during iteration but let's be certain.
            ulong bytes = _bytes;

            foreach (PacketGroup pg in _packetGroups.Values)
                pg.SetPercent(bytes);
        }

        public void ResetState()
        {
            ResettableT2CollectionCaches<PacketId, PacketGroup>.StoreAndDefault(ref _packetGroups);
        }

        public void InitializeState()
        {
            _packetGroups = ResettableT2CollectionCaches<PacketId, PacketGroup>.RetrieveDictionary();
        }
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