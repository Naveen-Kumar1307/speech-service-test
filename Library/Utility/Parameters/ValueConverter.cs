using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// Converts between supported values types and strings.
    /// </summary>
    /// <remarks>
    /// ValueConverters natively support conversion of all elementary data types.
    /// They are also extensible. Additional source and target converters may be registered.
    /// </remarks>
    public class ValueConverter
    {
        private Dictionary<Type, object> SourceMap = new Dictionary<Type, object>();
        private Dictionary<Type, object> TargetMap = new Dictionary<Type, object>();

        #region creating instances
        /// <summary>
        /// Configures the conversion type maps.
        /// </summary>
        public ValueConverter()
        {
            RegisterSource(new Converter<DateTime, string>(ConvertFromValue<DateTime>));
            RegisterSource(new Converter<string, string>(ConvertFromValue<string>));
            RegisterSource(new Converter<char, string>(ConvertFromValue<char>));
            RegisterSource(new Converter<decimal, string>(ConvertFromValue<decimal>));
            RegisterSource(new Converter<double, string>(ConvertFromValue<double>));
            RegisterSource(new Converter<float, string>(ConvertFromValue<float>));
            RegisterSource(new Converter<long, string>(ConvertFromValue<long>));
            RegisterSource(new Converter<int, string>(ConvertFromValue<int>));
            RegisterSource(new Converter<byte, string>(ConvertFromValue<byte>));
            RegisterSource(new Converter<bool, string>(ConvertFromValue<bool>));

            RegisterTarget(new Converter<string, DateTime>(
                delegate(string text) { return Convert.ToDateTime(text.OrFail()); }));

            RegisterTarget(new Converter<string, string>(
                delegate(string text) { return text.OrEmpty(); }));

            RegisterTarget(new Converter<string, char>(
                delegate(string text) { return Convert.ToChar(text.OrFail()); }));

            RegisterTarget(new Converter<string, decimal>(
                delegate(string text) { return Convert.ToDecimal(text.OrFail()); }));

            RegisterTarget(new Converter<string, double>(
                delegate(string text) { return Convert.ToDouble(text.OrFail()); }));

            RegisterTarget(new Converter<string, float>(
                delegate(string text) { return Convert.ToSingle(text.OrFail()); }));

            RegisterTarget(new Converter<string, long>(
                delegate(string text) { return Convert.ToInt64(text.OrFail()); }));

            RegisterTarget(new Converter<string, int>(
                delegate(string text) { return Convert.ToInt32(text.OrFail()); }));

            RegisterTarget(new Converter<string, byte>(
                delegate(string text) { return Convert.ToByte(text.OrFail()); }));

            RegisterTarget(new Converter<string, bool>(
                delegate(string text) { return Convert.ToBoolean(text.OrFail()); }));
        }
        #endregion

        #region registering converters
        /// <summary>
        /// Adds a target type converter to this ValueConverter.
        /// </summary>
        /// <typeparam name="TargetType">a target value type</typeparam>
        /// <param name="converter">a target converter</param>
        /// <returns>this ValueConverter</returns>
        public ValueConverter With<TargetType>(Converter<string, TargetType> converter)
        {
            RegisterTarget(converter);
            return this;
        }

        /// <summary>
        /// Adds a source type converter to this ValueConverter.
        /// </summary>
        /// <typeparam name="SourceType">a source value type</typeparam>
        /// <param name="converter">a source converter</param>
        /// <returns>this ValueConverter</returns>
        public ValueConverter With<SourceType>(Converter<SourceType, string> converter)
        {
            RegisterSource(converter);
            return this;
        }

        /// <summary>
        /// Registers a target type converter.
        /// </summary>
        public void RegisterTarget<TargetType>(Converter<string, TargetType> converter)
        {
            Type targetType = typeof(TargetType);
            TargetMap.Add(targetType, converter);
        }

        /// <summary>
        /// Registers a source type converter.
        /// </summary>
        public void RegisterSource<SourceType>(Converter<SourceType, string> converter)
        {
            Type sourceType = typeof(SourceType);
            SourceMap.Add(sourceType, converter);
        }
        #endregion

        #region converting values
        /// <summary>
        /// Converts a known kind of value to a string.
        /// </summary>
        private string ConvertFromValue<ValueType>(ValueType value)
        {
            if (value == null) return string.Empty;
            return value.ToString();
        }

        /// <summary>
        /// Converts a string to a known value type.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="text">a text string</param>
        /// <returns>a result value</returns>
        public ResultType ConvertTo<ResultType>(string text)
        {
            return GetTargetConverter<ResultType>()(text);
        }

        /// <summary>
        /// Converts a known value type to a string.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <param name="value">a value</param>
        /// <returns>a value string</returns>
        public string ConvertFrom<ValueType>(ValueType value)
        {
            return GetSourceConverter<ValueType>()(value);
        }
        #endregion

        #region accessing converters
        /// <summary>
        /// Returns a registered target converter.
        /// </summary>
        private Converter<string, ResultType> GetTargetConverter<ResultType>()
        {
            Type resultType = typeof(ResultType);
            if (!TargetMap.ContainsKey(resultType))
                throw ReportUnsupportedConversion(resultType);

            return TargetMap[resultType] as Converter<string, ResultType>;
        }

        /// <summary>
        /// Returns a registered source converter.
        /// </summary>
        private Converter<ValueType, string> GetSourceConverter<ValueType>()
        {
            Type valueType = typeof(ValueType);
            if (!SourceMap.ContainsKey(valueType))
                throw ReportUnsupportedConversion(valueType);

            return SourceMap[valueType] as Converter<ValueType, string>;
        }

        /// <summary>
        /// Reports an unsupported type conversion.
        /// </summary>
        private Exception ReportUnsupportedConversion(Type valueType)
        {
            return new ArgumentOutOfRangeException(
                        "ValueConverter does not support conversions of " + valueType.Name);
        }
        #endregion

    } // ValueConverter
}
