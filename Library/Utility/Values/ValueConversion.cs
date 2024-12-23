using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// Converts a byte from and to a canonical string.
    /// </summary>
    internal class ByteConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(byte);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<byte, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, byte>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a byte to a canonical string.
        /// </summary>
        /// <param name="value">a byte value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(byte value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a byte.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a byte</returns>
        public byte ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return byte.Parse(valueText);
        }

    } // ByteConversion


    /// <summary>
    /// Converts a character from and to a canonical string.
    /// </summary>
    internal class CharacterConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(char);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<char, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, char>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a character to a canonical string.
        /// </summary>
        /// <param name="value">a character value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(char value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a character.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a character</returns>
        public char ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return char.Parse(valueText);
        }

    } // CharacterConversion


    /// <summary>
    /// Converts an integer from and to a canonical string.
    /// </summary>
    internal class IntegerConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(int);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<int, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, int>(ConvertToValue); }
        }

        /// <summary>
        /// Converts an integer to a canonical string.
        /// </summary>
        /// <param name="value">an integer value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(int value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to an integer.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>an integer</returns>
        public int ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return int.Parse(valueText);
        }

    } // IntegerConversion


    /// <summary>
    /// Converts a long from and to a canonical string.
    /// </summary>
    internal class LongConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(long);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<long, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, long>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a long to a canonical string.
        /// </summary>
        /// <param name="value">a long value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(long value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a long.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a long</returns>
        public long ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return long.Parse(valueText);
        }

    } // LongConversion


    /// <summary>
    /// Converts a float from and to a canonical string.
    /// </summary>
    internal class FloatConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(float);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<float, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, float>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a float to a canonical string.
        /// </summary>
        /// <param name="value">a float value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(float value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a float.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a float</returns>
        public float ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return float.Parse(valueText);
        }

    } // FloatConversion


    /// <summary>
    /// Converts a double from and to a canonical string.
    /// </summary>
    internal class DoubleConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(double);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<double, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, double>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a double to a canonical string.
        /// </summary>
        /// <param name="value">a double value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(double value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a double.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a double</returns>
        public double ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return double.Parse(valueText);
        }

    } // DoubleConversion


    /// <summary>
    /// Converts a decimal from and to a canonical string.
    /// </summary>
    internal class DecimalConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(Decimal);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<Decimal, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, Decimal>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a decimal to a canonical string.
        /// </summary>
        /// <param name="value">a decimal value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(Decimal value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a decimal.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a decimal</returns>
        public Decimal ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return Decimal.Parse(valueText);
        }

    } // DecimalConversion


    /// <summary>
    /// Converts a boolean from and to a canonical string.
    /// </summary>
    internal class BooleanConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(bool);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<bool, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, bool>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a boolean to a canonical string.
        /// </summary>
        /// <param name="value">a boolean value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(bool value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a boolean.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a boolean</returns>
        public bool ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return bool.Parse(valueText);
        }

    } // BooleanConversion


    /// <summary>
    /// Converts a time from and to a canonical string.
    /// </summary>
    internal class TimeConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(DateTime);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<DateTime, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, DateTime>(ConvertToValue); }
        }

        /// <summary>
        /// Converts a time to a canonical string.
        /// </summary>
        /// <param name="value">a time value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(DateTime value)
        {
            return value.ToString(DateSortFormat, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts a canonical string to a time.
        /// </summary>
        /// <param name="valueText">a canonical string</param>
        /// <returns>a time</returns>
        public DateTime ConvertToValue(String valueText)
        {
            Argument.Check("valueText", valueText);
            return DateTime.Parse(valueText, CultureInfo.CurrentCulture);
        }

    } // TimeConversion


    /// <summary>
    /// Converts a text value from and to a canonical string.
    /// </summary>
    internal class TextConversion : Conversion
    {
        private static readonly Type ConversionType = typeof(String);

        /// <inheritdoc/>
        public override Type RegistrationType { get { return ConversionType; } }

        /// <inheritdoc/>
        public override Object SourceConverter
        {
            get { return new Converter<String, String>(ConvertFromValue); }
        }

        /// <inheritdoc/>
        public override Object TargetConverter
        {
            get { return new Converter<String, String>(ConvertFromValue); }
        }

        /// <summary>
        /// Converts text to a canonical string.
        /// </summary>
        /// <param name="value">a text value</param>
        /// <returns>a canonical string</returns>
        public String ConvertFromValue(String value)
        {
            return (value == null ? String.Empty : value.ToString());
        }

    } // TextConversion


    /// <summary>
    /// Converts an enum from and to a canonical string.
    /// </summary>
    internal class EnumConversion
    {
        /// <summary>
        /// Returns a new converter to a specific enum type.
        /// </summary>
        /// <typeparam name="T">an enum type</typeparam>
        /// <returns>a new converter</returns>
        public static Converter<String, T> To<T>()
        {
            Type resultType = typeof(T);
            return new Converter<String, T>(
                delegate(String value) { return (T)Enum.Parse(resultType, value); }
            );
        }

        /// <summary>
        /// Returns a new converter from a specific enum type.
        /// </summary>
        /// <typeparam name="T">an enum type</typeparam>
        /// <returns>a new converter</returns>
        public static Converter<T, String> From<T>()
        {
            return new Converter<T, String>(
                delegate(T value) { return value.ToString(); }
            );
        }

        /// <summary>
        /// Converts enumerated values to text.
        /// </summary>
        /// <typeparam name="T">a source enum type</typeparam>
        /// <param name="values">source values</param>
        /// <returns>the converted values</returns>
        public static String FromArray<T>(T[] values)
        {
            int count = 0;
            StringBuilder builder = new StringBuilder();
            foreach (T element in values)
            {
                if (count > 0) builder.Append(Conversion.Separator[0]);
                builder.Append(element.ToString());
                count++;
            }
            return builder.ToString();
        }

    } // EnumConversion


    /// <summary>
    /// Converts a value array from and to a canonical string.
    /// </summary>
    internal class ArrayConversion
    {
        internal static readonly Type BinaryConversionType = typeof(byte[]);

        /// <summary>
        /// Returns a conversion to a kind of value array.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <returns>a conversion</returns>
        public static Converter<String, T[]> To<T>()
        {
            Type valueType = typeof(T[]);
            if (valueType.FullName == BinaryConversionType.FullName)
            {
                return new Converter<String, T[]>(ConvertBinaryTo<T>);
            }
            else
            {
                return new Converter<String, T[]>(ConvertTo<T>);
            }
        }

        /// <summary>
        /// Converts a canonical string to a byte array.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="value">a canonical string</param>
        /// <returns>a byte array</returns>
        public static T[] ConvertBinaryTo<T>(String value)
        {
            if (value.StartsWith("0x"))
            {
                value = value.Substring(2, value.Length - 2);
            }

            return GetBytes(value) as T[];
        }
        private static readonly int ByteWidth = 2; // nibbles per byte

        /// <summary>
        /// Converts hex text to bytes.
        /// </summary>
        /// <param name="hexText">required hex text</param>
        /// <returns>resulting bytes</returns>
        public static byte[] GetBytes(String hexText)
        {
            Argument.Check("hexText", hexText);
            if ((hexText.Length % ByteWidth) != 0) hexText = "0" + hexText;

            char[] pair = new char[ByteWidth];
            int count = hexText.Length / ByteWidth;
            using (MemoryStream stream = new MemoryStream())
            using (StringReader reader = new StringReader(hexText))
            {
                while (count > 0)
                {
                    reader.Read(pair, 0, ByteWidth);
                    stream.WriteByte(byte.Parse(new String(pair), NumberStyles.HexNumber));
                    count--;
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Converts a canonical string to a value array.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="value">a canonical string</param>
        /// <returns>a value array</returns>
        public static T[] ConvertTo<T>(String value)
        {
            Converter<String, T> ConvertValue = Conversion.To<T>();
            String[] values = value.Split(GetSeparator(typeof(T))[0]);
            List<T> results = new List<T>();
            foreach (String element in values)
            {
                results.Add(ConvertValue(element));
            }
            return results.ToArray();
        }

        /// <summary>
        /// Returns a conversion from a kind of value array.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <returns>a conversion</returns>
        public static Converter<T[], String> From<T>()
        {
            Type valueType = typeof(T[]);
            if (valueType.FullName == BinaryConversionType.FullName)
            {
                return new Converter<T[], String>(ConvertBinaryFrom<T>);
            }
            else
            {
                return new Converter<T[], String>(ConvertFrom<T>);
            }
        }

        /// <summary>
        /// Converts a byte array to a canonical string.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="values">a byte array</param>
        /// <returns>a canonical string</returns>
        public static String ConvertBinaryFrom<T>(T[] values)
        {
            return "0x" + GetHex(values as byte[]);
        }

        /// <summary>
        /// Converts a binary buffer to hex text.
        /// </summary>
        /// <param name="buffer">binary buffer</param>
        /// <returns>resulting hex text</returns>
        public static String GetHex(byte[] buffer)
        {
            Argument.Check("buffer", buffer);
            StringBuilder builder = new StringBuilder();
            foreach (byte value in buffer)
            {
                builder.Append(value.ToString("X2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Converts a value array to a canonical string.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="values">a value array</param>
        /// <returns>a canonical string</returns>
        public static String ConvertFrom<T>(T[] values)
        {
            int count = 0;
            String elementSeparator = GetSeparator(typeof(T));
            Converter<T, String> ConvertValue = Conversion.From<T>();
            StringBuilder builder = new StringBuilder();
            foreach (T element in values)
            {
                if (count > 0) builder.Append(elementSeparator[0]);
                builder.Append(ConvertValue(element));
                count++;
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the element separator for a given element type.
        /// </summary>
        private static String GetSeparator(Type elementType)
        {
            return (CompositeTypeNames.Contains(elementType.Name) ? 
                    Conversion.Separator : Conversion.Comma);
        }

        private static readonly String[] CompositeTypeNames = 
        {
            "Size", "Point", "Rectangle", "SizeF", "PointF", "RectangleF" 
        };

    } // ArrayConversion


    /// <summary>
    /// Converts an integer pair from and to a canonical string.
    /// </summary>
    internal abstract class IntegerPairConversion : Conversion
    {
        /// <summary>
        /// Converts an integer pair to a canonical string.
        /// </summary>
        /// <param name="values">an integer pair</param>
        /// <returns>a canonical integer pair string</returns>
        protected String Convert(int[] values)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(values[0].ToString());
            buffer.Append(Comma);
            buffer.Append(values[1].ToString());
            return buffer.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a pair of integers.
        /// </summary>
        /// <param name="valuePair">a canonical string</param>
        /// <returns>an integer pair</returns>
        protected int[] Convert(String valuePair)
        {
            String[] distances = valuePair.Split(Comma[0]);
            if (valuePair.Contains("."))
            {
                float[] floats = { float.Parse(distances[0]), float.Parse(distances[1]) };
                int[] results = { (int)floats[0], (int)floats[1] };
                return results;
            }
            else
            {
                int[] results = { int.Parse(distances[0]), int.Parse(distances[1]) };
                return results;
            }
        }

    } // IntegerPairConversion


    /// <summary>
    /// Converts a float pair from and to a canonical string.
    /// </summary>
    internal abstract class FloatPairConversion : Conversion
    {
        /// <summary>
        /// Converts a float pair to a canonical string.
        /// </summary>
        /// <param name="values">a float pair</param>
        /// <returns>a canonical float pair string</returns>
        protected String Convert(float[] values)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(values[0].ToString(FloatFormat));
            buffer.Append(Comma);
            buffer.Append(values[1].ToString(FloatFormat));
            return buffer.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a pair of floats.
        /// </summary>
        /// <param name="valuePair">a canonical string</param>
        /// <returns>a float pair</returns>
        protected float[] Convert(String valuePair)
        {
            String[] distances = valuePair.Split(Comma[0]);
            float[] results = { float.Parse(distances[0]), float.Parse(distances[1]) };
            return results;
        }

    } // FloatPairConversion


    /// <summary>
    /// Converts an integer quad from and to a canonical string.
    /// </summary>
    internal abstract class IntegerQuadConversion : Conversion
    {
        /// <summary>
        /// Converts an integer quad to a canonical string.
        /// </summary>
        /// <param name="values">an integer quad</param>
        /// <returns>a canonical integer quad string</returns>
        protected String Convert(int[] values)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(values[0].ToString());
            buffer.Append(Comma);
            buffer.Append(values[1].ToString());
            buffer.Append(Comma);
            buffer.Append(values[2].ToString());
            buffer.Append(Comma);
            buffer.Append(values[3].ToString());
            return buffer.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a quad of integers.
        /// </summary>
        /// <param name="valueQuad">a canonical string</param>
        /// <returns>an integer quad</returns>
        protected int[] Convert(String valueQuad)
        {
            String[] distances = valueQuad.Split(Comma[0]);
            int[] results =
                {
                    int.Parse(distances[0]), int.Parse(distances[1]),
                    int.Parse(distances[2]), int.Parse(distances[3])
                };
            return results;
        }

    } // IntegerQuadConversion


    /// <summary>
    /// Converts a float quad from and to a canonical string.
    /// </summary>
    internal abstract class FloatQuadConversion : Conversion
    {
        /// <summary>
        /// Converts a float quad to a canonical string.
        /// </summary>
        /// <param name="values">a float quad</param>
        /// <returns>a canonical float quad string</returns>
        protected String Convert(float[] values)
        {
            StringBuilder buffer = new StringBuilder();
            buffer.Append(values[0].ToString(FloatFormat));
            buffer.Append(Comma);
            buffer.Append(values[1].ToString(FloatFormat));
            buffer.Append(Comma);
            buffer.Append(values[2].ToString(FloatFormat));
            buffer.Append(Comma);
            buffer.Append(values[3].ToString(FloatFormat));
            return buffer.ToString();
        }

        /// <summary>
        /// Converts a canonical string to a quad of floats.
        /// </summary>
        /// <param name="valueQuad">a canonical string</param>
        /// <returns>a float quad</returns>
        protected float[] Convert(String valueQuad)
        {
            String[] distances = valueQuad.Split(Comma[0]);
            float[] results =
                {
                    float.Parse(distances[0]), float.Parse(distances[1]),
                    float.Parse(distances[2]), float.Parse(distances[3])
                };
            return results;
        }

    } // FloatQuadConversion

}
