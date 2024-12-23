using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// Extension methods for bit array operations.
    /// </summary>
    public static class BitExtensions
    {
        /// <summary>
        /// Changes a given bit in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">a bit offset</param>
        /// <param name="bitOption">indicates whether to set (1) or clear (0) the bit</param>
        /// <returns>a value with the indicated bit set or cleared</returns>
        public static int ChangeBit(this int bitsValue, int bit, bool bitOption)
        {
            return (bitOption ? bitsValue.SetBit(bit) : bitsValue.ClearBit(bit));
        }

        /// <summary>
        /// Sets the given bit in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">a bit offset</param>
        /// <returns>a value with the indicated bit set</returns>
        public static int SetBit(this int bitsValue, int bit)
        {
            int[] bits = { bit };
            return bitsValue.SetBits(bits);
        }

        /// <summary>
        /// Sets the given bits in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>a value with the indicated bits set</returns>
        public static int SetBits(this int bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return bitsValue;

            int result = bitsValue;
            foreach (int bitOffset in bits)
            {
                result |= (TrueBit << bitOffset);
            }
            return result;
        }

        /// <summary>
        /// Clears the given bit in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">a bit offset</param>
        /// <returns>a value with the indicated bit cleared</returns>
        public static int ClearBit(this int bitsValue, int bit)
        {
            int[] bits = { bit };
            return bitsValue.ClearBits(bits);
        }

        /// <summary>
        /// Clears the given bits in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>a value with the indicated bits cleared</returns>
        public static int ClearBits(this int bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return bitsValue;

            int result = bitsValue;
            foreach (int bitOffset in bits)
            {
                result &= ~(TrueBit << bitOffset);
            }
            return result;
        }

        /// <summary>
        /// Indicates whether the given bit is set in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">bit offset</param>
        /// <returns>whether the given bit is set</returns>
        public static bool HasBit(this int bitsValue, int bit)
        {
            int[] bits = { bit };
            return bitsValue.HasAnyBits(bits);
        }

        /// <summary>
        /// Indicates whether any of the given bits are set in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>whether any of the given bits are set</returns>
        public static bool HasAnyBits(this int bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return false;

            bool result = false;
            foreach (int bitOffset in bits)
            {
                result |= ((bitsValue & (TrueBit << bitOffset)) > 0);
            }
            return result;
        }

        /// <summary>
        /// Indicates whether all of the given bits are set in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>whether all of the given bits are set</returns>
        public static bool HasAllBits(this int bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return false;

            bool result = true;
            foreach (int bitOffset in bits)
            {
                result &= ((bitsValue & (TrueBit << bitOffset)) > 0);
            }
            return result;
        }

        /// <summary>
        /// Converts this bits value to appropriate enumerated values.
        /// </summary>
        /// <typeparam name="EnumType">an enumerated type</typeparam>
        /// <param name="bitsValue">a value with bits</param>
        /// <returns>the bits as enumerated values, or empty</returns>
        public static EnumType[] ToArray<EnumType>(this int bitsValue)
        {
            Type resultType = typeof(EnumType);
            Array samples = Enum.GetValues(resultType);
            if (samples.Length > MaxOffset)
                throw new ArgumentOutOfRangeException(resultType.Name,
                            "Can't convert from bits to enum type with more than 32 values");

            if (bitsValue == FalseBit) return new EnumType[0];
            List<EnumType> values = new List<EnumType>(samples.Cast<EnumType>());
            List<EnumType> results = new List<EnumType>();
            for (int index = 0; index < BitsWidth; index++)
            {
                if ((bitsValue & (TrueBit << index)) > 0)
                {
                    results.Add((EnumType)Enum.ToObject(resultType, index));
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// Converts these enumerated values to a bits value.
        /// </summary>
        /// <typeparam name="EnumType">an enumerated type</typeparam>
        /// <param name="values">enumerated values</param>
        /// <returns>a bits value derived from the enumerated values</returns>
        public static int ToBits<EnumType>(this EnumType[] values)
        {
            Type resultType = typeof(EnumType);
            Array samples = Enum.GetValues(resultType);
            if (samples.Length > MaxOffset)
                throw new ArgumentOutOfRangeException(resultType.Name,
                            "Can't convert to bits from enum type with more than 32 values");

            if (values == null || values.Length == 0) return FalseBit;

            List<int> results = new List<int>();
            foreach (EnumType value in values)
            {
                results.Add((int)(object)value);
            }
            return FalseBit.SetBits(results.ToArray());
        }


        /// <summary>
        /// Changes a given bit in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">a bit offset</param>
        /// <param name="bitOption">indicates whether to set (1) or clear (0) the bit</param>
        /// <returns>a value with the indicated bit set or cleared</returns>
        public static string ChangeBit(this string bitsValue, int bit, bool bitOption)
        {
            return (bitOption ? bitsValue.SetBit(bit) : bitsValue.ClearBit(bit));
        }

        /// <summary>
        /// Sets the given bit in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">a bit offset</param>
        /// <returns>a value with the indicated bit set</returns>
        public static string SetBit(this string bitsValue, int bit)
        {
            int[] bits = { bit };
            return bitsValue.SetBits(bits);
        }

        /// <summary>
        /// Sets the given bits in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>a value with the indicated bits set</returns>
        public static string SetBits(this string bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return bitsValue;

            StringBuilder builder = new StringBuilder(bitsValue);
            foreach (int bitOffset in bits)
            {
                builder.ExpandIfNeeded(bitOffset);
                builder[bitOffset] = TrueValue;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Clears the given bit in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">a bit offset</param>
        /// <returns>a value with the indicated bit cleared</returns>
        public static string ClearBit(this string bitsValue, int bit)
        {
            int[] bits = { bit };
            return bitsValue.ClearBits(bits);
        }

        /// <summary>
        /// Clears the given bits in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>a value with the indicated bits cleared</returns>
        public static string ClearBits(this string bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return bitsValue;

            StringBuilder builder = new StringBuilder(bitsValue);
            foreach (int bitOffset in bits)
            {
                builder.ExpandIfNeeded(bitOffset);
                builder[bitOffset] = FalseValue;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether the given bit is set in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bit">bit offset</param>
        /// <returns>whether the given bit is set</returns>
        public static bool HasBit(this string bitsValue, int bit)
        {
            int[] bits = { bit };
            return bitsValue.HasAnyBits(bits);
        }

        /// <summary>
        /// Indicates whether any of the given bits are set in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>whether any of the given bits are set</returns>
        public static bool HasAnyBits(this string bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return false;

            bool result = false;
            StringBuilder builder = new StringBuilder(bitsValue);
            foreach (int bitOffset in bits)
            {
                if (0 <= bitOffset && bitOffset < builder.Length)
                {
                    char bitValue = builder[bitOffset];
                    result |= (bitValue == TrueValue);
                }
            }
            return result;
        }

        /// <summary>
        /// Indicates whether all of the given bits are set in this bits value.
        /// </summary>
        /// <param name="bitsValue">a value with bits</param>
        /// <param name="bits">bit offsets</param>
        /// <returns>whether all of the given bits are set</returns>
        public static bool HasAllBits(this string bitsValue, int[] bits)
        {
            if (bits == null || bits.Length == 0) return false;

            bool result = true;
            StringBuilder builder = new StringBuilder(bitsValue);
            foreach (int bitOffset in bits)
            {
                if (0 <= bitOffset && bitOffset < builder.Length)
                {
                    char bitValue = builder[bitOffset];
                    result &= (bitValue == TrueValue);
                }
            }
            return result;
        }

        /// <summary>
        /// Converts this bits value to appropriate enumerated values.
        /// </summary>
        /// <typeparam name="EnumType">an enumerated type</typeparam>
        /// <param name="bitsValue">a value with bits</param>
        /// <returns>the bits as enumerated values, or empty</returns>
        public static EnumType[] ToArray<EnumType>(this string bitsValue)
        {
            Type resultType = typeof(EnumType);
            Array samples = Enum.GetValues(resultType);
            StringBuilder builder = new StringBuilder(bitsValue);
            builder.ExpandIfNeeded(samples.Length - 1);

            List<EnumType> values = new List<EnumType>(samples.Cast<EnumType>());
            List<EnumType> results = new List<EnumType>();
            for (int index = 0; index < samples.Length; index++)
            {
                char bitValue = builder[index];
                if (bitValue == TrueValue)
                {
                    results.Add((EnumType)Enum.ToObject(resultType, index));
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// Converts these enumerated values to a bits value.
        /// </summary>
        /// <typeparam name="EnumType">an enumerated type</typeparam>
        /// <param name="values">enumerated values</param>
        /// <returns>a bits value derived from the enumerated values</returns>
        public static string ToBitString<EnumType>(this EnumType[] values)
        {
            if (values == null || values.Length == 0) return string.Empty;

            Type resultType = typeof(EnumType);
            Array samples = Enum.GetValues(resultType);

            List<int> results = new List<int>();
            foreach (EnumType value in values)
            {
                results.Add((int)(object)value);
            }
            return string.Empty.SetBits(results.ToArray());
        }

        /// <summary>
        /// Expands a string to fit a bit (if needed).
        /// </summary>
        /// <param name="bitsBuilder">a bit string builder</param>
        /// <param name="bitOffset">a bit offset</param>
        public static void ExpandIfNeeded(this StringBuilder bitsBuilder, int bitOffset)
        {
            while ((bitOffset + 1) > bitsBuilder.Length)
                bitsBuilder.Append(FalseValue);
        }

        private static int FalseBit = 0;
        private static int TrueBit = 1;

        private static char FalseValue = '0';
        private static char TrueValue = '1';

        private static int BitsWidth = 32;
        private static int MaxOffset = BitsWidth - 1;

    } // BitExtensions
}
