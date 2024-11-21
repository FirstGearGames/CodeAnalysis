#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
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
    /// Writes data to a buffer.
    /// </summary>
    public partial class Writer
    {
        #region Public.

        /// <summary>
        /// Capacity of the buffer.
        /// </summary>
        public int Capacity => _buffer.Length;

        /// <summary>
        /// Current write position.
        /// </summary>
        public int Position;

        /// <summary>
        /// Number of bytes writen to the buffer.
        /// </summary>
        public int Length;

        /// <summary>
        /// NetworkManager associated with this writer. May be null.
        /// </summary>
        public NetworkManager NetworkManager;

        #endregion

        #region Private.

        /// <summary>
        /// Buffer to prevent new allocations. This will grow as needed.
        /// </summary>
        private byte[] _buffer = new byte[64];

        #endregion

        #region Const.

        /// <summary>
        /// Replicate data is default of T.
        /// </summary>
        internal const byte REPLICATE_DEFAULT_BYTE = 0;

        /// <summary>
        /// Replicate data is the same as the previous.
        /// </summary>
        internal const byte REPLICATE_DUPLICATE_BYTE = 1;

        /// <summary>
        /// Replicate data is different from the previous.
        /// </summary>
        internal const byte REPLICATE_UNIQUE_BYTE = 2;

        /// <summary>
        /// Replicate data is repeating for every entry.
        /// </summary>
        internal const byte REPLICATE_REPEATING_BYTE = 3;

        /// <summary>
        /// All datas in the replicate are default.
        /// </summary>
        internal const byte REPLICATE_ALL_DEFAULT_BYTE = 4;

        /// <summary>
        /// Value used when a collection is unset, as in null.
        /// </summary>
        public const int UNSET_COLLECTION_SIZE_VALUE = -1;

        #endregion

        /// <summary>
        /// Outputs reader to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Position: {Position}, Length: {Length}, Buffer: {BitConverter.ToString(_buffer, 0, Length)}.";
        }

        /// <summary>
        /// Resets the writer as though it was unused. Does not reset buffers.
        /// </summary>
        public void Reset(NetworkManager manager = null)
        {
            Length = 0;
            Position = 0;
            NetworkManager = manager;
        }

        public void Skip(int count)
        {
        }

        /// <summary>
        /// Writes a dictionary.
        /// </summary>
        [DefaultWriter]
        public void WriteDictionary<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
        }
        
        /// <summary>
        /// Ensures the buffer Capacity is of minimum count.
        /// </summary>
        /// <param name="count"></param>
        public void EnsureBufferCapacity(int count)
        {
        }

        /// <summary>
        /// Ensure a number of bytes to be available in the buffer from current position.
        /// </summary>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnsureBufferLength(int count)
        {
        }
        
        /// <summary>
        /// Returns the buffer. The returned value will be the full buffer, even if not all of it is used.
        /// </summary>
        /// <returns></returns>
        public byte[] GetBuffer()
        {
            return _buffer;
        }

        /// <summary>
        /// Returns the used portion of the buffer as an ArraySegment.
        /// </summary>
        /// <returns></returns>
        public ArraySegment<byte> GetArraySegment()
        {
            return new ArraySegment<byte>(_buffer, 0, Length);
        }

        /// <summary>
        /// Reserves a number of bytes from current position.
        /// </summary>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reserve(int count)
        {
        }
        /// <summary>
        /// Writes length. This method is used to make debugging easier.
        /// </summary>
        /// <param name="length"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteLength(int length)
        {
        }
        /// <summary>
        /// Sends a packetId.
        /// </summary>
        /// <param name="pid"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WritePacketIdUnpacked(PacketId pid)
        {
        }
        
        /// <summary>
        /// Inserts value at index within the buffer.
        /// This method does not perform error checks.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public void FastInsertUInt8Unpacked(byte value, int index)
        {
        }

        /// <summary>
        /// Writes a byte.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteUInt8Unpacked(byte value)
        {
        }

        /// <summary>
        /// Writes bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt8Array(byte[] value, int offset, int count)
        {
        }

        /// <summary>
        /// Writes bytes and length of bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt8ArrayAndSize(byte[] value, int offset, int count)
        {
        }

        /// <summary>
        /// Writes all bytes in value and length of bytes.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteUInt8ArrayAndSize(byte[] value)
        {
        }


        /// <summary>
        /// Writes a sbyte.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteInt8(sbyte value)
        {
        }


        /// <summary>
        /// Writes a char.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteChar(char value)
        {
        }


        /// <summary>
        /// Writes a boolean.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteBoolean(bool value)
        {
        }


        /// <summary>
        /// Writes a uint16 unpacked.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt16Unpacked(ushort value)
        {
        }


        public void WriteInt8Unpacked(byte value)
        {
        }

        /// <summary>
        /// Writes a uint16.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //todo: should be using WritePackedWhole but something relying on unpacked short/ushort is being written packed, corrupting packets.
        [DefaultWriter]
        public void WriteUInt16(ushort value) => WriteUInt16Unpacked(value);

        /// <summary>
        /// Writes a int16 unpacked.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //todo: should be WritePackedWhole but something relying on unpacked short/ushort is being written packed, corrupting packets.
        public void WriteInt16Unpacked(short value) => WriteUInt16Unpacked((ushort)value);

        /// <summary>
        /// Writes a int16.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)] //todo: should be WritePackedWhole but something relying on unpacked short/ushort is being written packed, corrupting packets.
        [DefaultWriter]
        public void WriteInt16(short value) => WriteUInt16Unpacked((ushort)value);

        /// <summary>
        /// Writes a int32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt32Unpacked(int value) => WriteUInt32Unpacked((uint)value);

        /// <summary>
        /// Writes an int32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteInt32(int value) => WriteSignedPackedWhole(value);

        /// <summary>
        /// Writes value to dst without error checking.
        /// </summary>
        public static void WriteUInt32Unpacked(byte[] dst, uint value, ref int position)
        {
        }

        /// <summary>
        /// Writes a uint32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt32Unpacked(uint value)
        {
        }


        /// <summary>
        /// Writes a uint32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteUInt32(uint value) => WriteSignedPackedWhole(value);

        /// <summary>
        /// Writes a uint64.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt64Unpacked(ulong value)
        {
        }

        /// <summary>
        /// Writes a uint64.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteUInt64(ulong value) => WriteUnsignedPackedWhole(value);

        /// <summary>
        /// Writes a int64.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt64Unpacked(long value) => WriteUInt64((ulong)value);

        /// <summary>
        /// Writes an int64.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteInt64(long value) => WriteSignedPackedWhole(value);

        /// <summary>
        /// Writes a single (float).
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSingleUnpacked(float value)
        {
        }


        /// <summary>
        /// Writes a single (float).
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteSingle(float value) => WriteSingleUnpacked(value);

        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDoubleUnpacked(double value)
        {
        }

        /// <summary>
        /// Writes a double.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteDouble(double value) => WriteDoubleUnpacked(value);

        /// <summary>
        /// Writes a decimal.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteDecimalUnpacked(decimal value)
        {
        }

        /// <summary>
        /// Writes a decimal.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteDecimal(decimal value) => WriteDecimalUnpacked(value);

        /// <summary>
        /// Writes a string.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteString(string value)
        {
        }

        /// <summary>
        /// Writes a byte ArraySegment and it's size.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteArraySegmentAndSize(ArraySegment<byte> value) => WriteUInt8ArrayAndSize(value.Array, value.Offset, value.Count);

        /// <summary>
        /// Writes an ArraySegment without size.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArraySegment(ArraySegment<byte> value) => WriteUInt8Array(value.Array, value.Offset, value.Count);

        /// <summary>
        /// Writes a Vector2.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector2Unpacked(Vector2 value)
        {
        }


        /// <summary>
        /// Writes a Vector2.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteVector2(Vector2 value) => WriteVector2Unpacked(value);

        /// <summary>
        /// Writes a Vector3
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector3Unpacked(Vector3 value)
        {
        }

        /// <summary>
        /// Writes a Vector3
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteVector3(Vector3 value) => WriteVector3Unpacked(value);

        /// <summary>
        /// Writes a Vector4.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector4Unpacked(Vector4 value)
        {
        }


        /// <summary>
        /// Writes a Vector4.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteVector4(Vector4 value) => WriteVector4Unpacked(value);

        /// <summary>
        /// Writes a Vector2Int.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector2IntUnpacked(Vector2Int value)
        {
        }


        /// <summary>
        /// Writes a Vector2Int.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteVector2Int(Vector2Int value)
        {
        }


        /// <summary>
        /// Writes a Vector3Int.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVector3IntUnpacked(Vector3Int value)
        {
        }


        /// <summary>
        /// Writes a Vector3Int.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteVector3Int(Vector3Int value)
        {
        }


        /// <summary>
        /// Writes a Color.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteColorUnpacked(Color value)
        {
        }

        /// <summary>
        /// Writes a Color.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteColor(Color value)
        {
        }

        /// <summary>
        /// Writes a Color32.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteColor32(Color32 value)
        {
        }


        /// <summary>
        /// Writes a Quaternion.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteQuaternionUnpacked(Quaternion value)
        {
        }

        /// <summary>
        /// Writes a Quaternion.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteQuaternion32(Quaternion value)
        {
        }


        /// <summary>
        /// Reads a Quaternion.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void WriteQuaternion(Quaternion value, AutoPackType autoPackType)
        {
        }


        /// <summary>
        /// Writes a rect.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRectUnpacked(Rect value)
        {
        }


        /// <summary>
        /// Writes a rect.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteRect(Rect value) => WriteRectUnpacked(value);

        /// <summary>
        /// Writes a plane.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WritePlaneUnpacked(Plane value)
        {
        }


        /// <summary>
        /// Writes a plane.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WritePlane(Plane value) => WritePlaneUnpacked(value);

        /// <summary>
        /// Writes a Ray.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRayUnpacked(Ray value)
        {
        }


        /// <summary>
        /// Writes a Ray.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteRay(Ray value) => WriteRayUnpacked(value);

        /// <summary>
        /// Writes a Ray2D.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRay2DUnpacked(Ray2D value)
        {
        }


        /// <summary>
        /// Writes a Ray2D.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteRay2D(Ray2D value) => WriteRay2DUnpacked(value);


        /// <summary>
        /// Writes a Matrix4x4.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteMatrix4x4Unpacked(Matrix4x4 value)
        {
        }


        /// <summary>
        /// Writes a Matrix4x4.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteMatrix4x4(Matrix4x4 value) => WriteMatrix4x4Unpacked(value);

        /// <summary>
        /// Writes a Guid.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteGuidAllocated(System.Guid value)
        {
        }

        /// <summary>
        /// Writes a GameObject. GameObject must be spawned over the network already or be a prefab with a NetworkObject attached.
        /// </summary>
        /// <param name="go"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteGameObject(GameObject go)
        {
        }


        /// <summary>
        /// Writes a Transform. Transform must be spawned over the network already or be a prefab with a NetworkObject attached.
        /// </summary>
        /// <param name="t"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteTransform(Transform t)
        {
        }



        /// <summary>
        /// Writes a NetworkObject.
        /// </summary>
        /// <param name="nob"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteNetworkObject(NetworkObject nob)
        {
        }

        /// <summary>
        /// Writes a NetworkBehaviour.
        /// </summary>
        /// <param name="nb"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteNetworkBehaviour(NetworkBehaviour nb)
        {
        }


        /// <summary>
        /// Writes a DateTime.
        /// </summary>
        /// <param name="dt"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteDateTime(DateTime dt) => WriteSignedPackedWhole(dt.ToBinary());

        /// <summary>
        /// Writes a transport channel.
        /// </summary>
        /// <param name="channel"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteChannel(Channel channel) => WriteUInt8Unpacked((byte)channel);

        /// <summary>
        /// Writers a LayerMask.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteLayerMask(LayerMask value)
        {
        }


        /// <summary>
        /// Writes a NetworkConnection.
        /// </summary>
        /// <param name="connection"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteNetworkConnection(NetworkConnection connection)
        {
        }


        #region Packed writers.
        /// <summary>
        /// Writes a packed whole number.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSignedPackedWhole(long value)
        {
        }


        /// <summary>
        /// Writes a packed whole number.
        /// </summary>
        /// <param name="value"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUnsignedPackedWhole(ulong value)
        {
        }

        #endregion
        
        #region Generators
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteList<T>(List<T> value) { }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultWriter]
        public void WriteArray<T>(T[] value) { }

        public void Write<T>(T value) { }

        #endregion
    }
}