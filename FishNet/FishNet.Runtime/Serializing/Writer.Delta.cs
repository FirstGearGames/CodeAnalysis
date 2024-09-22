using FishNet.CodeGenerating;
using System.Runtime.CompilerServices;
using FishNet.Managing;
using FishNet.Runtime.Unity_Stuff;

namespace FishNet.Serializing
{
    public partial class Writer
    {
        private const double LARGEST_DELTA_PRECISION_INT8 = (sbyte.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_INT16 = (short.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_INT32 = (int.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_INT64 = (long.MaxValue / DOUBLE_ACCURACY);

        private const double LARGEST_DELTA_PRECISION_UINT8 = (byte.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT16 = (ushort.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT32 = (uint.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT64 = (ulong.MaxValue / DOUBLE_ACCURACY);
        internal const double DOUBLE_ACCURACY = 1000d;
        internal const decimal DECIMAL_ACCURACY = 1000m;

        #region Other.

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaBoolean(bool valueA, bool valueB)
        {
            return default;
        }

        #endregion

        #region Whole values.

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt8(sbyte valueA, sbyte valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        public bool WriteDeltaUInt8(byte valueA, byte valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt16(short valueA, short valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt16(ushort valueA, ushort valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt32(int valueA, int valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt32(uint valueA, uint valueB) => WriteDifference8_16_32(valueA, valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt64(long valueA, long valueB) => WriteDeltaUInt64((ulong)valueA, (ulong)valueB);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt64(ulong valueA, ulong valueB)
        {
            return default;
        }

        /// <summary>
        /// Writes the difference between two values for signed and unsigned shorts and ints.
        /// </summary>
        private bool WriteDifference8_16_32(long valueA, long valueB)
        {
            return default;
        }

        #endregion

        #region Single.
        /// <summary>
        /// Writes a single using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaSingle(DeltaPrecisionType dpt, float value)
        {

        }

        /// <summary>
        /// Writes a single using DeltaPrecisionType.
        /// </summary>
        private void WriteUDeltaSingle(UDeltaPrecisionType dpt, float positiveValue)
        {

        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaSingle(float valueA, float valueB)
        {
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// Value returned should be written as signed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DeltaPrecisionType GetDeltaPrecisionType(float valueA, float valueB, out float difference)
        {
            difference = 0;
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UDeltaPrecisionType GetUDeltaPrecisionType(float valueA, float valueB, out float difference)
        {
            difference = 0;
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private DeltaPrecisionType GetDeltaPrecisionType(float positiveValue)
        {
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private UDeltaPrecisionType GetUDeltaPrecisionType(float positiveValue)
        {
            return default;
        }
        
        #endregion

        #region Double.

        /// <summary>
        /// Writes a decimal using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaDouble(DeltaPrecisionType dpt, double value)
        {

        }

        /// <summary>
        /// Writes a double using DeltaPrecisionType.
        /// </summary>
        private void WriteUDeltaDouble(UDeltaPrecisionType dpt, double positiveValue)
        {

        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        public bool WriteDeltaDouble(double valueA, double valueB)
        {
            return default;
        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaDouble(double valueA, double valueB)
        {
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// Value returned should be written as signed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DeltaPrecisionType GetDeltaPrecisionType(double valueA, double valueB, out double difference)
        {
            difference = (valueB - valueA);
            double posValue = (difference < 0d) ? (difference * -1d) : difference;
            return GetDeltaPrecisionType(posValue);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UDeltaPrecisionType GetUDeltaPrecisionType(double valueA, double valueB, out double difference)
        {
            difference = 0;
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private DeltaPrecisionType GetDeltaPrecisionType(double positiveValue)
        {
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private UDeltaPrecisionType GetUDeltaPrecisionType(double positiveValue)
        {
            return default;
        }

        #endregion

        #region Decimal
        
        /// <summary>
        /// Writes a decimal using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaDecimal(DeltaPrecisionType dpt, decimal value)
        {

        }

        /// <summary>
        /// Writes a decimal using DeltaPrecisionType.
        /// </summary>
        private void WriteUDeltaDecimal(UDeltaPrecisionType dpt, decimal positiveValue)
        {
            
        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        public bool WriteDeltaDecimal(decimal valueA, decimal valueB)
        {
            return default;
        }

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaDecimal(decimal valueA, decimal valueB)
        {
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// Value returned should be written as signed.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DeltaPrecisionType GetDeltaPrecisionType(decimal valueA, decimal valueB, out decimal difference)
        {
            difference = 0;
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private UDeltaPrecisionType GetUDeltaPrecisionType(decimal valueA, decimal valueB, out decimal difference)
        {
            difference = 0;
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private DeltaPrecisionType GetDeltaPrecisionType(decimal positiveValue)
        {
            return default;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        private UDeltaPrecisionType GetUDeltaPrecisionType(decimal positiveValue)
        {
            return default;
        }

        #endregion

        #region Unity.

        /// <summary>
        /// Writes a delta Vector3.
        /// </summary>
        [DefaultDeltaWriter]
        public bool WriteDeltaVector3(Vector3 valueA, Vector3 valueB)
        {
            return default;
        }

        #endregion

        //public void WriteDeltaDictionary<TKey, TValue>(Dictionary<TKey, TValue> valueA, Dictionary<TKey, TValue> valueB, DeltaSerializerOption option) { }

        #region Generic.

        public bool WriteDelta<T>(T prev, T next, DeltaSerializerOption option)
        {
            return default;
        }

        #endregion
    }
    
}