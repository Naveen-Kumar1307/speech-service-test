using System;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Globalization;
using System.Data.SqlTypes;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Common.Logging;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// Converts a value from and to a canonical string.
    /// </summary>
    public abstract class Conversion
    {
        /// <summary>
        /// Separates elements of a multi-valued item (e.g., an array).
        /// </summary>
        public static readonly String Separator = ";";

        /// <summary>
        /// Separates components of a composite item (e.g., a Color).
        /// </summary>
        public static readonly String Comma = ",";

        internal static readonly String ArraySuffix = "[]";
        internal static readonly Type EnumType = typeof(Enum);

        private static Dictionary<Type, Object> SourceConversions = new Dictionary<Type, Object>();
        private static Dictionary<Type, Object> TargetConversions = new Dictionary<Type, Object>();

        /// <summary>
        /// A short date format.
        /// </summary>
        protected static readonly String DateSortFormat = "o";

        /// <summary>
        /// A four digit float format.
        /// </summary>
        protected static readonly String FloatFormat = "F04";

        #region registering conversions
        /// <summary>
        /// Initializes the converter registries.
        /// </summary>
        static Conversion()
        {
            Register<bool>(new BooleanConversion());
            Register<byte>(new ByteConversion());
            Register<char>(new CharacterConversion());
            Register<int>(new IntegerConversion());
            Register<long>(new LongConversion());
            Register<float>(new FloatConversion());
            Register<double>(new DoubleConversion());
            Register<decimal>(new DecimalConversion());
            Register<String>(new TextConversion());
            Register<DateTime>(new TimeConversion());

            Register<Color>(new ColorConversion());
            Register<Size>(new SizeConversion());
            Register<SizeF>(new SizeFConversion());
            Register<Point>(new PointConversion());
            Register<PointF>(new PointFConversion());
            Register<Rectangle>(new RectangleConversion());
            Register<RectangleF>(new RectangleFConversion());
            return;
        }

        /// <summary>
        /// Registers the delegates from a conversion.
        /// </summary>
        /// <typeparam name="T">a value type</typeparam>
        /// <param name="conversion">a required conversion</param>
        public static void Register<T>(Conversion conversion)
        {
            Argument.Check("conversion", conversion);
            Type valueType = typeof(T);
            conversion.Check<T>();
            conversion.RegisterConversions();

            Type arrayType = typeof(T[]);
            SourceConversions.Add(arrayType, ArrayConversion.From<T>());
            TargetConversions.Add(arrayType, ArrayConversion.To<T>());
        }

        /// <summary>
        /// Registers the delegates of this conversion.
        /// </summary>
        private void RegisterConversions()
        {
            SourceConversions.Add(RegistrationType, SourceConverter);
            TargetConversions.Add(RegistrationType, TargetConverter);
        }

        /// <summary>
        /// A concrete type handled by this conversion.
        /// </summary>
        public abstract Type RegistrationType { get; }

        /// <summary>
        /// Converts the registration type to a canonical string.
        /// </summary>
        public abstract Object SourceConverter { get; }

        /// <summary>
        /// Converts a canonical string to the registration type.
        /// </summary>
        public abstract Object TargetConverter { get; }

        /// <summary>
        /// Checks whether a conversion handles an expected type.
        /// </summary>
        /// <typeparam name="ExpectedType">an expected type</typeparam>
        public void Check<ExpectedType>()
        {
            Type expectedType = typeof(ExpectedType);
            if (!expectedType.IsAssignableFrom(RegistrationType))
            {
                throw new ArgumentOutOfRangeException("conversion",
                            "Conversion was expected to handle " + expectedType.Name);
            }
        }
        #endregion

        #region accessing conversions
        /// <summary>
        /// Converts text to enumerated values.
        /// </summary>
        /// <typeparam name="T">a result enum type</typeparam>
        /// <param name="enumValues">enum text</param>
        /// <returns>the enumerated values</returns>
        public static T[] ToValues<T>(String enumValues)
        {
            return ArrayConversion.ConvertTo<T>(enumValues);
        }

        /// <summary>
        /// Converts enumerated values to text.
        /// </summary>
        /// <typeparam name="T">a source enum type</typeparam>
        /// <param name="values">source values</param>
        /// <returns>the converted values</returns>
        public static String FromValues<T>(T[] values)
        {
            return ArrayConversion.ConvertFrom<T>(values);
        }

        /// <summary>
        /// Converts a compatible enumerated value to a result type.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="value">a value</param>
        /// <returns>a result derived from a compatible enumerated value</returns>
        public static ResultType To<ResultType>(Enum value)
        {
            return Conversion.To<ResultType>()(value.ToString());
        }

        /// <summary>
        /// A conversion from string to a specific type.
        /// </summary>
        /// <typeparam name="T">a specific type</typeparam>
        /// <returns>a converter</returns>
        /// <exception cref="ArgumentNullException">
        /// if no such conversion exists</exception>
        public static Converter<String, T> To<T>()
        {
            Type targetType = typeof(T);
            if (EnumType.IsAssignableFrom(targetType))
                return EnumConversion.To<T>();

            return GetConversionTo<T>();
        }

        /// <summary>
        /// A conversion to string from a specific type.
        /// </summary>
        /// <typeparam name="T">a specific type</typeparam>
        /// <returns>a converter</returns>
        /// <exception cref="ArgumentNullException">
        /// if no such conversion exists</exception>
        public static Converter<T, String> From<T>()
        {
            Type sourceType = typeof(T);
            if (EnumType.IsAssignableFrom(sourceType))
                return EnumConversion.From<T>();

            return GetConversionFrom<T>();
        }

        /// <summary>
        /// Returns a conversion from a source type.
        /// </summary>
        /// <typeparam name="T">a source type</typeparam>
        /// <returns>a source conversion</returns>
        private static Converter<T, String> GetConversionFrom<T>()
        {
            return SourceConversions[typeof(T)] as Converter<T, String>;
        }

        /// <summary>
        /// Returns a conversion to a target type.
        /// </summary>
        /// <typeparam name="T">a target type</typeparam>
        /// <returns>a target conversion</returns>
        private static Converter<String, T> GetConversionTo<T>()
        {
            return TargetConversions[typeof(T)] as Converter<String, T>;
        }
        #endregion

        #region translating payloads
        /// <summary>
        /// Returns a new compatible result copied from a source.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="source">a required source</param>
        /// <returns>a new compatible result copied from a source</returns>
        public static ResultType ConvertTo<ResultType>(Object source) where ResultType : class, new()
        {
            Argument.Check("source", source);
            Type resultType = typeof(ResultType);
            return ConvertTo(resultType, source) as ResultType;
        }

        /// <summary>
        /// Retruns a new compatible result copied from a source.
        /// </summary>
        /// <param name="resultType">a result type</param>
        /// <param name="source">a source item</param>
        /// <returns>a new compatible result copied from a source</returns>
        public static Object ConvertTo(Type resultType, Object source)
        {
            Argument.Check("resultType", resultType);
            Argument.Check("source", source);
            Type sourceType = source.GetType();
            ItemConversion.CheckCompatible(sourceType, resultType);
            return ItemConversion.With(source).ConvertTo(resultType);
        }

        /// <summary>
        /// Converts items to copies of a compatible type.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="items">compatible items</param>
        /// <returns>copies of the supplied items</returns>
        public static ResultType[] ConvertTo<ResultType>(ICollection<Object> items)
            where ResultType : class, new()
        {
            if (Argument.IsEmpty(items)) return new ResultType[0];
            List<ResultType> results = new List<ResultType>(items.Count);
            foreach (Object item in items)
            {
                results.Add(ConvertTo<ResultType>(item));
            }
            return results.ToArray();
        }
        #endregion

    } // Conversion
}
