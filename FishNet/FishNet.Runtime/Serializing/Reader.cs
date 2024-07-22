#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEVELOPMENT
#endif
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Transporting;
using System.Runtime.CompilerServices;
using FishNet.Runtime.Unity_Stuff;
using FishNet.CodeGenerating;


namespace FishNet.Serializing
{
    /// <summary>
    /// Reads data from a buffer.
    /// </summary>
    public partial class Reader
    {
        #region Types.

        public enum DataSource
        {
            Unset = 0,
            Server = 1,
            Client = 2,
        }

        #endregion

        #region Public.

        /// <summary>
        /// Which part of the network the data came from.
        /// </summary>
        public DataSource Source = DataSource.Unset;

        /// <summary>
        /// Capacity of the buffer.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        /// NetworkManager for this reader. Used to lookup objects.
        /// </summary>
        public NetworkManager NetworkManager;

        /// <summary>
        /// Offset within the buffer when the reader was created.
        /// </summary>
        public int Offset { get; private set; }

        /// <summary>
        /// Position for the next read.
        /// </summary>
        public int Position;

        /// <summary>
        /// Total number of bytes available within the buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Bytes remaining to be read. This value is Length - Position.
        /// </summary>
        public int Remaining => ((Length + Offset) - Position);

        #endregion

        #region Internal.

        /// <summary>
        /// NetworkConnection that this data came from.
        /// Value may not always be set.
        /// </summary>
        public NetworkConnection NetworkConnection { get; private set; }
#if DEVELOPMENT
        /// <summary>
        /// Last NetworkObject parsed.
        /// </summary>
        public static NetworkObject LastNetworkObject { get; private set; }
        /// <summary>
        /// Last NetworkBehaviour parsed. 
        /// </summary>
        public static NetworkBehaviour LastNetworkBehaviour { get; private set; }
#endif

        #endregion

        #region Private.

        /// <summary>
        /// Data being read.
        /// </summary>
        private byte[] _buffer;

        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reader(byte[] bytes, NetworkManager networkManager, NetworkConnection networkConnection = null, DataSource source = DataSource.Unset)
        {
            Initialize(bytes, networkManager, networkConnection, source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Reader(ArraySegment<byte> segment, NetworkManager networkManager, NetworkConnection networkConnection = null, DataSource source = DataSource.Unset)
        {
            Initialize(segment, networkManager, networkConnection, source);
        }

        /// <summary>
        /// Outputs reader to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Position: {Position}, Length: {Length}, Buffer: {BitConverter.ToString(_buffer, Offset, Length)}.";
        }

        /// <summary>
        /// Outputs reader to string.
        /// </summary>
        /// <returns></returns>
        public string RemainingToString()
        {
            string buffer = (Remaining > 0) ? BitConverter.ToString(_buffer, Position, Remaining) : "null";
            return $"Remaining: {Remaining}, Length: {Length}, Buffer: {buffer}.";
        }

        /// <summary>
        /// Returns remaining data as an ArraySegment.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetRemainingData()
        {
            if (Remaining == 0)
                return default;
            else
                return new ArraySegment<byte>(_buffer, Position, Remaining);
        }

        /// <summary>
        /// Initializes this reader with data.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Initialize(ArraySegment<byte> segment, NetworkManager networkManager, DataSource source = DataSource.Unset)
        {
            Initialize(segment, networkManager, null, source);
        }

        /// <summary>
        /// Initializes this reader with data.
        /// </summary>
        internal void Initialize(ArraySegment<byte> segment, NetworkManager networkManager, NetworkConnection networkConnection = null, DataSource source = DataSource.Unset)
        {
            _buffer = segment.Array;
            if (_buffer == null)
                _buffer = new byte[0];

            Position = segment.Offset;
            Offset = segment.Offset;
            Length = segment.Count;

            NetworkManager = networkManager;
            NetworkConnection = networkConnection;
            Source = source;
        }

        /// <summary>
        /// Initializes this reader with data.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Initialize(byte[] bytes, NetworkManager networkManager, DataSource source = DataSource.Unset)
        {
            Initialize(new ArraySegment<byte>(bytes), networkManager, null, source);
        }

        /// <summary>
        /// Initializes this reader with data.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Initialize(byte[] bytes, NetworkManager networkManager, NetworkConnection networkConnection = null, DataSource source = DataSource.Unset)
        {
            Initialize(new ArraySegment<byte>(bytes), networkManager, networkConnection, source);
        }

        /// <summary>
        /// Reads a dictionary.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Dictionary<TKey, TValue> ReadDictionaryAllocated<TKey, TValue>()
            => default;

        /// <summary>
        /// Reads length. This method is used to make debugging easier.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int ReadLength()
            => default;

        /// <summary>
        /// Reads a packetId.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PacketId ReadPacketId()
            => default;
        /// <summary>
        /// Returns a ushort without advancing the reader.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal PacketId PeekPacketId()
            => default;

        /// <summary>
        /// Returns the next byte to be read.
        /// </summary>
        /// <returns></returns>
        internal byte PeekByte()
            => default;

        /// <summary>
        /// Skips a number of bytes in the reader.
        /// </summary>
        /// <param name="value">Number of bytes to skip.</param>
        public void Skip(int value)
        {
        }

        /// <summary>
        /// Clears remaining bytes to be read.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {}
        
        /// <summary>
        /// Returns the buffer as an ArraySegment.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetArraySegmentBuffer()
            => default;

        /// <summary>
        /// Returns the buffer as bytes. This does not trim excessive bytes.
        /// </summary>
        /// <returns></returns>
        public byte[] GetByteBuffer()
            => default;


        /// <summary>
        /// Returns the buffer as bytes and allocates into a new array.
        /// </summary>
        /// <returns></returns>
        public byte[] GetByteBufferAllocated()
            => default;
        
        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public byte ReadUInt8() => ReadUInt8Unpacked();

        /// <summary>
        /// Reads a byte.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadUInt8Unpacked()
            => default;
        
        /// <summary>
        /// Creates an ArraySegment by reading a number of bytes from position.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ArraySegment<byte> ReadArraySegment(int count)
            => default;

        /// <summary>
        /// Reads a sbyte.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public sbyte ReadInt8() => (sbyte)ReadUInt8();

        /// <summary>
        /// Reads a char.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public char ReadChar() => (char)ReadUInt16();

        /// <summary>
        /// Reads a boolean.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public bool ReadBoolean()
            => default;

        /// <summary>
        /// Reads an int16.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUInt16Unpacked()
            => default;

        /// <summary>
        /// Reads an int16.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //todo: should be using ReadPackedWhole but something relying on unpacked short/ushort is being written packed, corrupting packets.
        [DefaultReader]
        public ushort ReadUInt16() => ReadUInt16Unpacked();

        /// <summary>
        /// Reads a uint16.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //todo: should be using ReadPackedWhole but something relying on unpacked short/ushort is being written packed, corrupting packets.
        public short ReadInt16Unpacked() => (short)ReadUInt16Unpacked();

        /// <summary>
        /// Reads a uint16.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //todo: should be using ReadPackedWhole but something relying on unpacked short/ushort is being written packed, corrupting packets.
        [DefaultReader]
        public short ReadInt16() => (short)ReadUInt16Unpacked();

        /// <summary>
        /// Reads an int32.
        /// </summary>
        /// <returns></returns> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt32Unpacked()
        {
            uint result = 0;
            result |= _buffer[Position++];
            result |= (uint)_buffer[Position++] << 8;
            result |= (uint)_buffer[Position++] << 16;
            result |= (uint)_buffer[Position++] << 24;

            return result;
        }

        /// <summary>
        /// Reads an int32.
        /// </summary>
        /// <returns></returns> 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public uint ReadUInt32() => (uint)ReadUnsignedPackedWhole();

        /// <summary>
        /// Reads a uint32.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt32Unpacked() => (int)ReadUInt32Unpacked();

        /// <summary>
        /// Reads a uint32.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public int ReadInt32() => (int)ReadSignedPackedWhole();

        /// <summary>
        /// Reads a uint64.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadInt64Unpacked() => (long)ReadUInt64Unpacked();

        /// <summary>
        /// Reads a uint64.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public long ReadInt64() => (long)ReadSignedPackedWhole();

        /// <summary>
        /// Reads an int64.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUInt64Unpacked()
            => default;

        /// <summary>
        /// Reads an int64.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public ulong ReadUInt64() => ReadUnsignedPackedWhole();

        /// <summary>
        /// Reads a single.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadSingleUnpacked()
            => default;
        
        /// <summary>
        /// Reads a single.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public float ReadSingle() => ReadSingleUnpacked();

        /// <summary>
        /// Reads a double.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadDoubleUnpacked()
            => default;
        
        /// <summary>
        /// Reads a double.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public double ReadDouble() => ReadDoubleUnpacked();

        /// <summary>
        /// Reads a decimal.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public decimal ReadDecimalUnpacked()
            => default;

        /// <summary>
        /// Reads a decimal.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public decimal ReadDecimal() => ReadDecimalUnpacked();

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <returns></returns>        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public string ReadString()
            => default;

        /// <summary>
        /// Creates a byte array and reads bytes and size into it.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public byte[] ReadUInt8ArrayAndSizeAllocated()
            => default;

        /// <summary>
        /// Reads bytes and size and copies results into target. Returns UNSET if null was written.
        /// </summary>
        /// <returns>Bytes read.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadUInt8AndSize(ref byte[] target)
            => default;

        /// <summary>
        /// Reads bytes and size and returns as an ArraySegment.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public ArraySegment<byte> ReadArraySegmentAndSize()
            => default;

        /// <summary>
        /// Reads a Vector2.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2 ReadVector2Unpacked()  => default;

        /// <summary>
        /// Reads a Vector2.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Vector2 ReadVector2() => ReadVector2Unpacked();

        /// <summary>
        /// Reads a Vector3.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3 ReadVector3Unpacked() => default;

        /// <summary>
        /// Reads a Vector3.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Vector3 ReadVector3() => ReadVector3Unpacked();

        /// <summary>
        /// Reads a Vector4.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector4 ReadVector4Unpacked() => default;

        /// <summary>
        /// Reads a Vector4.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Vector4 ReadVector4() => ReadVector4Unpacked();

        /// <summary>
        /// Reads a Vector2Int.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector2Int ReadVector2IntUnpacked() => default;

        /// <summary>
        /// Reads a Vector2Int.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Vector2Int ReadVector2Int() => default;

        /// <summary>
        /// Reads a Vector3Int.
        /// </summary>
        /// <returns></returns>      
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3Int ReadVector3IntUnpacked() => default;

        /// <summary>
        /// Reads a Vector3Int.
        /// </summary>
        /// <returns></returns>      
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Vector3Int ReadVector3Int() => default;

        /// <summary>
        /// Reads a color.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Color ReadColorUnpacked()
            => default;

        /// <summary>
        /// Reads a color.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Color ReadColor()
            => default;

        /// <summary>
        /// Reads a Color32.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Color32 ReadColor32() => default;

        /// <summary>
        /// Reads a Quaternion.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion ReadQuaternionUnpacked() => default;

        /// <summary>
        /// Reads a Quaternion.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Quaternion ReadQuaternion64()
            => default;

        /// <summary>
        /// Reads a Quaternion.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Quaternion ReadQuaternion32()
            => default;

        /// <summary>
        /// Reads a Quaternion.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Quaternion ReadQuaternion(AutoPackType autoPackType)
            => default;

        /// <summary>
        /// Reads a Rect.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Rect ReadRectUnpacked() => default;

        /// <summary>
        /// Reads a Rect.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Rect ReadRect() => ReadRectUnpacked();

        /// <summary>
        /// Plane.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Plane ReadPlaneUnpacked() => default;

        /// <summary>
        /// Plane.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Plane ReadPlane() => ReadPlaneUnpacked();

        /// <summary>
        /// Reads a Ray.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Ray ReadRayUnpacked()
            => default;

        /// <summary>
        /// Reads a Ray.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Ray ReadRay() => ReadRayUnpacked();

        /// <summary>
        /// Reads a Ray.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Ray2D ReadRay2DUnpacked()
            => default;

        /// <summary>
        /// Reads a Ray.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Ray2D ReadRay2D() => ReadRay2DUnpacked();

        /// <summary>
        /// Reads a Matrix4x4.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Matrix4x4 ReadMatrix4x4Unpacked()
            => default;

        /// <summary>
        /// Reads a Matrix4x4.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Matrix4x4 ReadMatrix4x4() => ReadMatrix4x4Unpacked();

        /// <summary>
        /// Creates a new byte array and reads bytes into it.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadUInt8ArrayAllocated(int count)
            => default;

        /// <summary>
        /// Reads a Guid.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public System.Guid ReadGuid()
            => default;

        /// <summary>
        /// Reads a tick without packing.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadTickUnpacked() => ReadUInt32Unpacked();

        /// <summary>
        /// Reads a GameObject.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public GameObject ReadGameObject()
            => default;

        /// <summary>
        /// Reads a Transform.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Transform ReadTransform()
            => default;

        /// <summary>
        /// Reads a NetworkObject.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public NetworkObject ReadNetworkObject()
            => default;


        /// <summary>
        /// Reads a NetworkObjectId and nothing else.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadNetworkObjectId() => (int)ReadSignedPackedWhole();

        /// <summary>
        /// Reads a NetworkBehaviour.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public NetworkBehaviour ReadNetworkBehaviour()
            => default;

        /// <summary>
        /// Reads a DateTime.
        /// </summary>
        /// <param name="dt"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public DateTime ReadDateTime()
            => default;

        /// <summary>
        /// Reads a transport channel.
        /// </summary>
        /// <param name="channel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public Channel ReadChannel()
            => default;

        /// <summary>
        /// Reads the Id for a NetworkConnection.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadNetworkConnectionId() => (int)ReadSignedPackedWhole();

        /// <summary>
        /// Reads a LayerMask.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public LayerMask ReadLayerMask()
            => default;

        /// <summary>
        /// Reads a NetworkConnection.
        /// </summary>
        /// <param name="conn"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultReader]
        public NetworkConnection ReadNetworkConnection()
            => default;

        /// <summary>
        /// Checks if the size could possibly be an allocation attack.
        /// </summary>
        /// <param name="size"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckAllocationAttack(int size)
            => default;


        #region Packed readers.

        /// <summary>
        /// ZigZag decode an integer. Move the sign bit back to the left.
        /// </summary>
        public ulong ZigZagDecode(ulong value)
            => default;

        /// <summary>
        /// Reads a packed whole number and applies zigzag decoding.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadSignedPackedWhole() => (long)ZigZagDecode(ReadUnsignedPackedWhole());

        /// <summary>
        /// Reads a packed whole number.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadUnsignedPackedWhole()
            => default;

        #endregion

        #region Generators.

        /// <summary>
        /// Reads a list with allocations.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<T> ReadListAllocated<T>()
            => default;

        /// <summary>
        /// Reads into collection and returns item count read.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="allowNullification">True to allow the referenced collection to be nullified when receiving a null collection read.</param>
        /// <returns>Number of values read into the collection. UNSET is returned if the collection were read as null.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadList<T>(ref List<T> collection, bool allowNullification = false)
            => default;

        /// <summary>
        /// Reads an array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T[] ReadArrayAllocated<T>()
            => default;

        /// <summary>
        /// Reads into collection and returns amount read.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadArray<T>(ref T[] collection)
            => default;

        /// <summary>
        /// Reads any supported type as packed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Read<T>() => default;

        #endregion
    }
}

#pragma warning restore CS8603 // Possible null reference return.
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.