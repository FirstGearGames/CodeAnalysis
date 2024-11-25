using System;
using FishNet.CodeGenerating;
using System.Runtime.CompilerServices;
using FishNet.Managing;
using FishNet.Runtime.Unity_Stuff;
using UnityEngine;

namespace FishNet.Serializing
{
    public partial class Reader
    {
        internal double DOUBLE_ACCURACY => Writer.DOUBLE_ACCURACY;
        internal decimal DECIMAL_ACCURACY => Writer.DECIMAL_ACCURACY;

        #region Other.

        /// <summary>
        /// Reads a boolean.
        /// </summary>
        [DefaultDeltaReader]
        public bool ReadDeltaBoolean(bool valueA) => ReadBoolean();

        #endregion

        #region Whole values.

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadDeltaInt8(sbyte valueA) => (sbyte)ReadDifference8_16_32(valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadDeltaUInt8(byte valueA) => (byte)ReadDifference8_16_32(valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadDeltaInt16(short valueA) => (short)ReadDifference8_16_32(valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadDeltaUInt16(ushort valueA) => (ushort)ReadDifference8_16_32(valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadDeltaInt32(int valueA) => (int)ReadDifference8_16_32(valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadDeltaUInt32(uint valueA) => (uint)ReadDifference8_16_32(valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadDeltaInt64(long valueA) => (long)ReadDeltaUInt64((ulong)valueA);

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadDeltaUInt64(ulong valueA)
        {
            return default;
        }

        /// <summary>
        /// Returns a new result by reading and applying a difference to a value.
        /// </summary>
        [DefaultDeltaReader]
        private long ReadDifference8_16_32(long valueA)
        {
            return default;
        }

        #endregion
       
        #region Single.
        
        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        private float ReadDeltaSingle(DeltaPrecisionType dpt, float valueA)
        {
            return default;
        }
      
        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        private float ReadUDeltaSingle(UDeltaPrecisionType dpt, float valueA)
        {
            return default;
        }

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private float ReadDeltaSingle(float valueA)
        {
            return default;
        }
        
        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float ReadUDeltaSingle(float valueA)
        {
            return default;
        }
        
        #endregion

        #region Double.
        
        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        private double ReadDeltaDouble(DeltaPrecisionType dpt, double valueA)
        {
            return default;
        }
      
        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        private double ReadUDeltaDouble(UDeltaPrecisionType dpt, double valueA)
        {
            double diff = 0;
            if (dpt.FastContains(UDeltaPrecisionType.UInt8))
                diff = (ReadUInt8Unpacked() / DOUBLE_ACCURACY);
            else if (dpt.FastContains(UDeltaPrecisionType.UInt16))
                diff = (ReadUInt16Unpacked() / DOUBLE_ACCURACY);
            else if (dpt.FastContains(UDeltaPrecisionType.UInt32))
                diff = (ReadUInt32Unpacked() / DOUBLE_ACCURACY);
            else if (dpt.FastContains(UDeltaPrecisionType.Unset))
                diff = ReadDoubleUnpacked();
            else
                NetworkManager.LogError($"Unhandled precision type of {dpt}.");

            bool bLargerThanA = dpt.FastContains(UDeltaPrecisionType.NextValueIsLarger);
            return (bLargerThanA) ? (valueA + diff) : (valueA - diff);
        }

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private double ReadDeltaDouble(double valueA)
        {
            DeltaPrecisionType dpt = (DeltaPrecisionType)ReadUInt8Unpacked();
            return ReadDeltaDouble(dpt, valueA);
        }
        
        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double ReadUDeltaDouble(double valueA)
        {
            UDeltaPrecisionType dpt = (UDeltaPrecisionType)ReadUInt8Unpacked();
            return ReadUDeltaDouble(dpt, valueA);
        }
        
        #endregion
        
        #region Decimal.

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        private decimal ReadDeltaDecimal(DeltaPrecisionType dpt, decimal valueA)
        {
            return default;
        }

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        private decimal ReadUDeltaDecimal(UDeltaPrecisionType dpt, decimal valueA)
        {
            return default;
        }

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultDeltaReader]
        public decimal ReadDeltaDecimal(decimal valueA)
        {
            return default;
        }

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DefaultDeltaReader]
        public decimal ReadUDeltaDecimal(decimal valueA)
        {
            return default;
        }

        #endregion

        #region Unity.

        /// <summary>
        /// Reads a difference, appending it onto a value.
        /// </summary>
        [DefaultDeltaReader]
        public Vector3 ReadDeltaVector3(Vector3 valueA)
        {
            return default;
        }

        #endregion

        [DefaultDeltaReader]
        public Dictionary<TKey, TValue> ReadDeltaDictionaryAllocated<TKey, TValue>(Dictionary<TKey, TValue> valueA) => default;

        [DefaultDeltaReader]
        public List<T> ReadDeltaListAllocated<T>(List<T> valueA) => default;

        [DefaultDeltaReader]
        public byte[] ReadDeltaUInt8ArrayAllocated(byte[] valueA) => default;

        [DefaultDeltaReader]
        public T[] ReadDeltaUArrayAllocated<T>(T[] valueA) => default;

        [DefaultDeltaReader]
        public ArraySegment<byte> ReadDeltaArraySegment(ArraySegment<byte> valueA) => default;


        #region Generic.

        /// <summary>
        /// Reads a delta of any time.
        /// </summary>
        public T ReadDelta<T>(T prev)
        {
            return default;
        }

        #endregion
    }
}