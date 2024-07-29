using System.Runtime.CompilerServices;

namespace FishNet.Serializing
{
    [System.Flags]
    internal enum DeltaVector3Type : byte
    {
        /// <summary>
        /// This is unused.
        /// </summary>
        Unset = 0,
        /// <summary>
        /// Contains X as 1 byte.
        /// </summary>
        XInt8 = 1,
        /// <summary>
        /// Contains X as 2 bytes.
        /// </summary>
        XInt16 = 2,
        /// <summary>
        /// Contains X as 4 bytes.
        /// </summary>
        XInt32 = 4,
        /// <summary>
        /// Contains Z as 1 byte.
        /// </summary>
        ZInt8 = 8,
        /// <summary>
        /// Contains Z as 2 bytes.
        /// </summary>
        ZInt16 = 16,
        /// <summary>
        /// Contains Z as 4 bytes.
        /// </summary>
        ZInt32 = 32,
        /// <summary>
        /// Contains Y as 2 bytes.
        /// </summary>
        YInt16 = 64,
        /// <summary>
        /// Contains Y as 4 bytes.
        /// </summary>
        YInt32 = 128,
    }

    [System.Flags]
    internal enum UDeltaPrecisionType : byte
    {
        /// <summary>
        /// Indicates there is no compression. This can also be used to initialize the enum.
        /// </summary>
        Unset = 0,
        /// <summary>
        /// When set this indicates the new value is larger than the previous.
        /// When not set, indicates new value is smaller than the previous.
        /// </summary>
        NextValueIsLarger = 1,
        /// <summary>
        /// Data is written as a byte.
        /// </summary>
        UInt8 = 2,
        /// <summary>
        /// Data is written as a ushort.
        /// </summary>
        UInt16 = 4,
        /// <summary>
        /// Data is written as a uint.
        /// </summary>
        UInt32 = 8,
        /// <summary>
        /// Data is written as a ulong.
        /// </summary>
        UInt64 = 16,
        /// <summary>
        /// data is written as two ulong.
        /// </summary>
        UInt128 = 32,
    }
    [System.Flags]
    internal enum DeltaPrecisionType : byte
    {
        /// <summary>
        /// Indicates there is no compression. This can also be used to initialize the enum.
        /// </summary>
        Unset = 0,
        /// <summary>
        /// Data is written as a sbyte.
        /// </summary>
        Int8 = 2,
        /// <summary>
        /// Data is written as a short.
        /// </summary>
        Int16 = 4,
        /// <summary>
        /// Data is written as a int.
        /// </summary>
        Int32 = 8,
        /// <summary>
        /// Data is written as a long.
        /// </summary>
        Int64 = 16,
        /// <summary>
        /// data is written as two long.
        /// </summary>
        Int128 = 32,
    }

    internal static class DeltaTypeExtensions
    {
        public static bool FastContains(this UDeltaPrecisionType whole, UDeltaPrecisionType part) => (whole & part) == part;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastContains(this UDeltaPrecisionType whole, UDeltaPrecisionType part, int shift) => FastContains((int)whole, (int)part, shift);
        
        public static bool FastContains(this DeltaPrecisionType whole, DeltaPrecisionType part) => (whole & part) == part;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastContains(this DeltaPrecisionType whole, DeltaPrecisionType part, int shift) => FastContains((int)whole, (int)part, shift);

        public static bool FastContains(this DeltaVector3Type whole, DeltaVector3Type part) => (whole & part) == part;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool FastContains(this DeltaVector3Type whole, DeltaVector3Type part, int shift) => FastContains((int)whole, (int)part, shift);
        
        private static bool FastContains(int whole, int part, int shift)
        {
            int intPart = part >> shift;
            return (whole & intPart) == intPart;
        }
    }

}