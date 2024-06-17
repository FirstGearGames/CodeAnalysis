using System.Runtime.CompilerServices;
using FishNet.Managing;

namespace FishNet.Serializing
{
    ///* THIS IS IN DRAFTING / WIP. Do not attempt to use or modify this file. */
    ///* THIS IS IN DRAFTING / WIP. Do not attempt to use or modify this file. */
    ///* THIS IS IN DRAFTING / WIP. Do not attempt to use or modify this file. */
    public partial class Writer
    {
        private const double LARGEST_DELTA_PRECISION_UINT8 = ((double)byte.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT16 = ((double)ushort.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT32 = ((double)uint.MaxValue / DOUBLE_ACCURACY);
        private const double LARGEST_DELTA_PRECISION_UINT64 = ((double)ulong.MaxValue / DOUBLE_ACCURACY);
        internal const double DOUBLE_ACCURACY = 1000d;
        internal const decimal DECIMAL_ACCURACY = 1000m;

       // [DefaultDeltaWriter]
        public bool WriteDeltaBoolean(bool valueA, bool valueB)
        {
            if (valueA == valueB) return false;

            WriteBoolean(valueB);

            return true;
        }


        #region Whole values.
        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt8(sbyte valueA, sbyte valueB) => WriteDifference8_16_32((long)valueA, (long)valueB);

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt8(byte valueA, byte valueB) => WriteDifference8_16_32((long)valueA, (long)valueB);

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt16(short valueA, short valueB) => WriteDifference8_16_32((long)valueA, (long)valueB);

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt16(ushort valueA, ushort valueB) => WriteDifference8_16_32((long)valueA, (long)valueB);

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt32(int valueA, int valueB) => WriteDifference8_16_32((long)valueA, (long)valueB);

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt32(uint valueA, uint valueB) => WriteDifference8_16_32((long)valueA, (long)valueB);

        /// <summary>
        /// Writes the difference between two values for signed and unsigned shorts and ints.
        /// </summary>
        private bool WriteDifference8_16_32(long valueA, long valueB)
        {
            if (valueA == valueB) return false;

            long next = ((long)valueB - (long)valueA);
            WriteSignedPackedWhole(next);

            return true;
        }

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaInt64(long valueA, long valueB) => WriteDeltaUInt64((ulong)valueA, (ulong)valueB);

        [DefaultDeltaWriter]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool WriteDeltaUInt64(ulong valueA, ulong valueB)
        {
            if (valueA == valueB) return false;

            bool bLargerThanA = (valueB > valueA);
            ulong next = (bLargerThanA) ? (valueB - valueA) : (valueA - valueB);

            WriteBoolean(bLargerThanA);
            WriteUnsignedPackedWhole(next);

            return true;
        }
        #endregion


        #region Precision values.
        private DeltaPrecisionType GetDeltaPrecisionPack(decimal positiveValue)
        {
            return positiveValue switch
            {
                < (decimal)LARGEST_DELTA_PRECISION_UINT8 => DeltaPrecisionType.UInt8,
                < (decimal)LARGEST_DELTA_PRECISION_UINT16 => DeltaPrecisionType.UInt16,
                < (decimal)LARGEST_DELTA_PRECISION_UINT32 => DeltaPrecisionType.UInt32,
                < (decimal)LARGEST_DELTA_PRECISION_UINT64 => DeltaPrecisionType.UInt64,
                _ => DeltaPrecisionType.Unpacked,
            };
        }

        private DeltaPrecisionType GetDeltaPrecisionPack(double positiveValue)
        {
            return positiveValue switch
            {
                < LARGEST_DELTA_PRECISION_UINT8 => DeltaPrecisionType.UInt8,
                < LARGEST_DELTA_PRECISION_UINT16 => DeltaPrecisionType.UInt16,
                < LARGEST_DELTA_PRECISION_UINT32 => DeltaPrecisionType.UInt32,
                _ => DeltaPrecisionType.Unpacked,
            };
        }

        private DeltaPrecisionType GetDeltaPrecisionPack(float positiveValue)
        {
            return positiveValue switch
            {
                < (float)LARGEST_DELTA_PRECISION_UINT8 => DeltaPrecisionType.UInt8,
                < (float)LARGEST_DELTA_PRECISION_UINT16 => DeltaPrecisionType.UInt16,
                _ => DeltaPrecisionType.Unpacked,
            };
        }

        [DefaultDeltaWriter]
        public bool WriteDeltaSingle(float valueA, float valueB)
        {
            if (valueA == valueB) return false;

            bool bLargerThanA = (valueB > valueA);
            float difference = (bLargerThanA) ? (valueB - valueA) : (valueA - valueB);
            DeltaPrecisionType dpt = GetDeltaPrecisionPack(difference);

            if (bLargerThanA)
                dpt |= DeltaPrecisionType.NextValueIsLarger;

            WriteSingleDeltaPrecision(dpt, difference);
            return true;
        }

        [DefaultDeltaWriter]
        public bool WriteDeltaDouble(double valueA, double valueB)
        {
            if (valueA == valueB) return false;

            bool bLargerThanA = (valueB > valueA);
            double difference = (bLargerThanA) ? (valueB - valueA) : (valueA - valueB);
            DeltaPrecisionType dpt = GetDeltaPrecisionPack(difference);

            if (bLargerThanA)
                dpt |= DeltaPrecisionType.NextValueIsLarger;

            WriteDoubleDeltaPrecision(dpt, difference);
            return true;
        }

        [DefaultDeltaWriter]
        public bool WriteDeltaDecimal(decimal valueA, decimal valueB)
        {
            if (valueA == valueB) return false;

            bool bLargerThanA = (valueB > valueA);
            decimal difference = (bLargerThanA) ? (valueB - valueA) : (valueA - valueB);
            DeltaPrecisionType dpt = GetDeltaPrecisionPack(difference);

            if (bLargerThanA)
                dpt |= DeltaPrecisionType.NextValueIsLarger;

            WriteDecimalDeltaPrecision(dpt, difference);
            return true;
        }

        private void WriteSingleDeltaPrecision(DeltaPrecisionType dpt, float positiveValue)
        {
            WriteUInt8Unpacked((byte)dpt);

            if (dpt.FastContains(DeltaPrecisionType.UInt8))
                WriteUInt8Unpacked((byte)Math.Floor(positiveValue * DOUBLE_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.UInt16))
                WriteUInt16Unpacked((ushort)Math.Floor(positiveValue * DOUBLE_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.Unpacked))
                WriteSingleUnpacked(positiveValue);
            else
                NetworkManagerExtensions.LogError($"Unhandled precision type of {dpt}.");
        }

        private void WriteDoubleDeltaPrecision(DeltaPrecisionType dpt, double positiveValue)
        {
            WriteUInt8Unpacked((byte)dpt);

            if (dpt.FastContains(DeltaPrecisionType.UInt8))
                WriteUInt8Unpacked((byte)Math.Floor(positiveValue * DOUBLE_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.UInt16))
                WriteUInt16Unpacked((ushort)Math.Floor(positiveValue * DOUBLE_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.UInt32))
                WriteUInt32Unpacked((uint)Math.Floor(positiveValue * DOUBLE_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.Unpacked))
                WriteDoubleUnpacked(positiveValue);
            else
                NetworkManagerExtensions.LogError($"Unhandled precision type of {dpt}.");
        }

        private void WriteDecimalDeltaPrecision(DeltaPrecisionType dpt, decimal positiveValue)
        {
            WriteUInt8Unpacked((byte)dpt);

            if (dpt.FastContains(DeltaPrecisionType.UInt8))
                WriteUInt8Unpacked((byte)Math.Floor(positiveValue * DECIMAL_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.UInt16))
                WriteUInt16Unpacked((ushort)Math.Floor(positiveValue * DECIMAL_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.UInt32))
                WriteUInt32Unpacked((uint)Math.Floor(positiveValue * DECIMAL_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.UInt64))
                WriteUInt64Unpacked((ulong)Math.Floor(positiveValue * DECIMAL_ACCURACY));
            else if (dpt.FastContains(DeltaPrecisionType.Unpacked))
                WriteDecimalUnpacked(positiveValue);
            else
                NetworkManagerExtensions.LogError($"Unhandled precision type of {dpt}.");
        }
        #endregion

    }
    
}