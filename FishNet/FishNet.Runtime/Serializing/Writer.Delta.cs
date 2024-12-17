using FishNet.CodeGenerating;
using FishNet.Managing;
using FishNet.Object;
using FishNet.Object.Prediction;
using FishNet.Runtime.Unity_Stuff;

namespace FishNet.Serializing
{
    public struct TransformProperties { }
    

    public partial class Writer
    {
        #region Types.
        [System.Flags]
        internal enum UnsignedVector3DeltaFlag : int
        {
            Unset = 0,
            More = (1 << 0),
            X1 = (1 << 1),
            NextXIsLarger = (1 << 2),
            Y1 = (1 << 3),
            NextYIsLarger = (1 << 4),
            Z1 = (1 << 5),
            NextZIsLarger = (1 << 6),
            X2 = (1 << 8),
            X4 = (1 << 9),
            Y2 = (1 << 10),
            Y4 = (1 << 11),
            Z2 = (1 << 12),
            Z4 = (1 << 13),
        }
        #endregion


        #region Other.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaBoolean(bool valueA, bool valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            bool valuesMatch = (valueA == valueB);
            if (valuesMatch && option == DeltaSerializerOption.Unset)
                return false;

            WriteBoolean(valueB);

            return true;
        }
        #endregion

        #region Whole values.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt8(sbyte valueA, sbyte valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDifference8_16_32(valueA, valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        public bool WriteDeltaUInt8(byte valueA, byte valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDifference8_16_32(valueA, valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt16(short valueA, short valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDifference8_16_32(valueA, valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaUInt16(ushort valueA, ushort valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDifference8_16_32(valueA, valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt32(int valueA, int valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDifference8_16_32(valueA, valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaUInt32(uint valueA, uint valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDifference8_16_32(valueA, valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaInt64(long valueA, long valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDeltaUInt64((ulong)valueA, (ulong)valueB, option);

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaUInt64(ulong valueA, ulong valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            bool unchangedValue = (valueA == valueB);
            if (unchangedValue && option == DeltaSerializerOption.Unset) return false;

            bool bLargerThanA = (valueB > valueA);
            ulong next = (bLargerThanA) ? (valueB - valueA) : (valueA - valueB);

            WriteBoolean(bLargerThanA);
            WriteUnsignedPackedWhole(next);

            return true;
        }

        /// <summary>
        /// Writes the difference between two values for signed and unsigned shorts and ints.
        /// </summary>
        private bool WriteDifference8_16_32(long valueA, long valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            bool unchangedValue = (valueA == valueB);
            if (unchangedValue && option == DeltaSerializerOption.Unset) return false;

            long next = (valueB - valueA);
            WriteSignedPackedWhole(next);

            return true;
        }
        #endregion

        #region Single.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaSingle(float valueA, float valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            UDeltaPrecisionType dpt = GetUDeltaPrecisionType(valueA, valueB, out float unsignedDifference);

            if (dpt == UDeltaPrecisionType.Unset && option == DeltaSerializerOption.Unset)
                return false;

            WriteUInt8Unpacked((byte)dpt);
            WriteDeltaSingle(dpt, unsignedDifference, unsigned: true);

            return true;
        }

        /// <summary>
        /// Writes a delta value using a compression type.
        /// </summary>
        private void WriteDeltaSingle(UDeltaPrecisionType dpt, float value, bool unsigned) { }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// Value returned should be written as signed.
        /// </summary>
        public UDeltaPrecisionType GetSDeltaPrecisionType(float valueA, float valueB, out float signedDifference)
        {
            signedDifference = (valueB - valueA);
            float posValue = (signedDifference < 0f) ? (signedDifference * -1f) : signedDifference;

            return GetDeltaPrecisionType(posValue, unsigned: false);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        public UDeltaPrecisionType GetUDeltaPrecisionType(float valueA, float valueB, out float unsignedDifference)
        {
            bool bIsLarger = (valueB > valueA);
            if (bIsLarger)
                unsignedDifference = (valueB - valueA);
            else
                unsignedDifference = (valueA - valueB);

            UDeltaPrecisionType result = GetDeltaPrecisionType(unsignedDifference, unsigned: true);
            //If result is set then set if bIsLarger.
            if (bIsLarger && result != UDeltaPrecisionType.Unset)
                result |= UDeltaPrecisionType.NextValueIsLarger;

            return result;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        public UDeltaPrecisionType GetDeltaPrecisionType(float positiveValue, bool unsigned) => default;
        #endregion

        #region Double.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaDouble(double valueA, double valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            UDeltaPrecisionType dpt = GetUDeltaPrecisionType(valueA, valueB, out double positiveDifference);

            if (dpt == UDeltaPrecisionType.Unset && option == DeltaSerializerOption.Unset) return false;

            WriteUInt8Unpacked((byte)dpt);
            WriteDeltaDouble(dpt, positiveDifference, unsigned: true);

            return true;
        }

        /// <summary>
        /// Writes a double using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaDouble(UDeltaPrecisionType dpt, double value, bool unsigned) { }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        public UDeltaPrecisionType GetSDeltaPrecisionType(double valueA, double valueB, out double signedDifference)
        {
            signedDifference = (valueB - valueA);
            double posValue = (signedDifference < 0d) ? (signedDifference * -1d) : signedDifference;

            return GetDeltaPrecisionType(posValue, unsigned: false);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        public UDeltaPrecisionType GetUDeltaPrecisionType(double valueA, double valueB, out double unsignedDifference)
        {
            bool bIsLarger = (valueB > valueA);
            if (bIsLarger)
                unsignedDifference = (valueB - valueA);
            else
                unsignedDifference = (valueA - valueB);

            UDeltaPrecisionType result = GetDeltaPrecisionType(unsignedDifference, unsigned: true);
            if (bIsLarger && result != UDeltaPrecisionType.Unset)
                result |= UDeltaPrecisionType.NextValueIsLarger;

            return result;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        public UDeltaPrecisionType GetDeltaPrecisionType(double positiveValue, bool unsigned) => default;
        #endregion

        #region Decimal
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteUDeltaDecimal(decimal valueA, decimal valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            UDeltaPrecisionType dpt = GetUDeltaPrecisionType(valueA, valueB, out decimal positiveDifference);

            if (dpt == UDeltaPrecisionType.Unset && option == DeltaSerializerOption.Unset) return false;

            WriteUInt8Unpacked((byte)dpt);
            WriteDeltaDecimal(dpt, positiveDifference, unsigned: true);

            return true;
        }

        /// <summary>
        /// Writes a double using DeltaPrecisionType.
        /// </summary>
        private void WriteDeltaDecimal(UDeltaPrecisionType dpt, decimal value, bool unsigned) { }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        public UDeltaPrecisionType GetSDeltaPrecisionType(decimal valueA, decimal valueB, out decimal signedDifference)
        {
            signedDifference = (valueB - valueA);
            decimal posValue = (signedDifference < 0m) ? (signedDifference * -1m) : signedDifference;

            return GetDeltaPrecisionType(posValue, unsigned: false);
        }

        /// <summary>
        /// Returns DeltaPrecisionType for the difference of two values.
        /// </summary>
        public UDeltaPrecisionType GetUDeltaPrecisionType(decimal valueA, decimal valueB, out decimal unsignedDifference)
        {
            bool bIsLarger = (valueB > valueA);
            if (bIsLarger)
                unsignedDifference = (valueB - valueA);
            else
                unsignedDifference = (valueA - valueB);

            UDeltaPrecisionType result = GetDeltaPrecisionType(unsignedDifference, unsigned: true);
            if (bIsLarger && result != UDeltaPrecisionType.Unset)
                result |= UDeltaPrecisionType.NextValueIsLarger;

            return result;
        }

        /// <summary>
        /// Returns DeltaPrecisionType for a value.
        /// </summary>
        public UDeltaPrecisionType GetDeltaPrecisionType(decimal positiveValue, bool unsigned) => default;
        #endregion

        #region FishNet Types.
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        /// <returns>True if written.</returns>
        [DefaultDeltaWriter]
        public bool WriteDeltaNetworkBehaviour(NetworkBehaviour valueA, NetworkBehaviour valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            bool unchangedValue = (valueA == valueB);
            if (unchangedValue && option == DeltaSerializerOption.Unset) return false;

            WriteNetworkBehaviour(valueB);
            return true;
        }
        #endregion

        public struct TransformProperties { }

        #region Unity.
        /// <summary>
        /// Writes delta position, rotation, and scale of a transform.
        /// </summary>
        public bool WriteDeltaTransformProperties(TransformProperties valueA, TransformProperties valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)  => default;

        
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        [DefaultDeltaWriter]
        public bool WriteDeltaQuaternion(Quaternion valueA, Quaternion valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => default;

        /// <summary>
        /// Writes a delta value.
        /// </summary>
        [DefaultDeltaWriter]
        public bool WriteDeltaVector2(Vector2 valueA, Vector2 valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset)  => default;

        [DefaultDeltaWriter]
        public bool WriteDeltaVector3(Vector3 valueA, Vector3 valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => default;
        
        /// <summary>
        /// Writes a delta value.
        /// </summary>
        //[DefaultDeltaWriter]
        public bool WriteDeltaVector3_New(Vector3 valueA, Vector3 valueB, DeltaSerializerOption option = DeltaSerializerOption.Unset) => default;
        #endregion

        #region Prediction.
        /// <summary>
        /// Writes a delta reconcile.
        /// </summary>
        internal void WriteDeltaReconcile<T>(T lastReconcile, T value, DeltaSerializerOption option = DeltaSerializerOption.Unset) => WriteDelta(lastReconcile, value, option);

        /// <summary>
        /// Writes a delta replicate using a list.
        /// </summary>
        internal void WriteDeltaReplicate<T>(List<T> values, int offset, DeltaSerializerOption option = DeltaSerializerOption.Unset) where T : IReplicateData
        {
            int collectionCount = values.Count;
            //Replicate list will never be null, no need to write null check.
            //Number of entries being written.
            byte count = (byte)(collectionCount - offset);
            WriteUInt8Unpacked(count);

            T prev;
            //Set previous if not full and if enough room in the collection to go back.
            if (option != DeltaSerializerOption.FullSerialize && collectionCount > count)
                prev = values[offset - 1];
            else
                prev = default;

            for (int i = offset; i < collectionCount; i++)
            {
                T v = values[i];
                WriteDelta(prev, v, option);

                prev = v;
                //After the first loop the deltaOption can be set to root, if not already.
                option = DeltaSerializerOption.RootSerialize;
            }
        }
        #endregion

        #region Generic.
        public bool WriteDelta<T>(T prev, T next, DeltaSerializerOption option = DeltaSerializerOption.Unset)
        {
            Func<Writer, T, T, DeltaSerializerOption, bool> del = GenericDeltaWriter<T>.Write;

            if (del == null)
            {
                NetworkManager.LogError($"Write delta method not found for {typeof(T).FullName}. Use a supported type or create a custom serializer.");

                return false;
            }
            else
            {
                return del.Invoke(this, prev, next, option);
            }
        }
        #endregion
    }
}