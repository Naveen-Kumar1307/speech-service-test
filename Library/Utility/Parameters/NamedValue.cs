using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// A named value (of a known type).
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows some value(s)</item>
    /// <item>knows a value name and type</item>
    /// <item>encodes the value(s) uniformly as strings (for easy transport)</item>
    /// </list>
    /// <h4>Clients must:</h4>
    /// <list type="bullet">
    /// <item>supply a name and value(s) during construction</item>
    /// </list>
    /// </remarks>
    [DataContract]
    public class NamedValue
    {
        private static readonly char Separator = ',';
        private static readonly Type ObjectType = typeof(Object);
        private static readonly Type EnumerableType = typeof(IEnumerable);
        private static readonly ValueConverter Conversion = new ValueConverter();

        /// <summary>
        /// A value name.
        /// </summary>
        [DataMember]
        public String Name { get; set; }

        /// <summary>
        /// A value type name.
        /// </summary>
        [DataMember]
        public String TypeName { get; set; }

        /// <summary>
        /// Encoded value(s).
        /// </summary>
        [DataMember]
        public String Values { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a new NamedValue.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <param name="valueName">a required value name</param>
        /// <param name="value">a value</param>
        /// <returns>a new NamedValue</returns>
        public static NamedValue With<ValueType>(String valueName, ValueType value)
        {
            Type valueType = typeof(ValueType);
            Type elementType = GetElementType<ValueType>();
            if (elementType != null)
            {
                Type testType = typeof(ICollection<>).MakeGenericType(elementType);
                if (testType.IsAssignableFrom(valueType))
                {
                    return ValuesNamed(valueName, value as IEnumerable, elementType);
                }
            }

            Argument.CheckType<ValueType>("value", value);
            ValueType[] values = { value };
            return ValuesNamed(valueName, values);
        }

        /// <summary>
        /// Returns a new NamedValue.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <param name="valueName">a required value name</param>
        /// <param name="lowerBound">lower bound</param>
        /// <param name="upperBound">upper bound</param>
        /// <returns>a new NamedValue</returns>
        public static NamedValue RangeNamed<ValueType>(
            String valueName, ValueType lowerBound, ValueType upperBound)
        {
            Argument.CheckType<ValueType>("lowerBound", lowerBound);
            Argument.CheckType<ValueType>("upperBound", upperBound);
            ValueType[] values = { lowerBound, upperBound };
            return ValuesNamed(valueName, values);
        }

        /// <summary>
        /// Returns a new NamedValue.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <param name="valueName">a required value name</param>
        /// <param name="values">some values</param>
        /// <returns>a new NamedValue</returns>
        public static NamedValue ValuesNamed<ValueType>(
            String valueName, ICollection<ValueType> values)
        {
            Argument.Check("valueName", valueName);
            Argument.CheckAny("values", values);
            Type[] types = { values.GetType().GetElementType() };
            Type testType = typeof(ICollection<>).MakeGenericType(types);
            Type valueType = values.GetType();
            bool test = testType.IsAssignableFrom(valueType);
            NamedValue result = new NamedValue();
            result.Name = valueName;
            result.Include(values);
            return result;
        }

        /// <summary>
        /// Returns a new NamedValue.
        /// </summary>
        /// <param name="valueName">a required value name</param>
        /// <param name="values">some values</param>
        /// <param name="valueType">a value type</param>
        /// <returns>a new NamedValue</returns>
        private static NamedValue ValuesNamed(
            String valueName, IEnumerable values, Type valueType)
        {
            NamedValue result = new NamedValue();
            result.Name = valueName;
            result.Include(valueType, values);
            return result;
        }

        /// <summary>
        /// Returns the element type of a qualified value type.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <returns>an element type, or null</returns>
        private static Type GetElementType<ValueType>()
        {
            Type valueType = typeof(ValueType);
            Type elementType = valueType.GetElementType();
            if (elementType != null) return elementType;
            if (!valueType.Namespace.StartsWith(EnumerableType.Namespace)) return null;

            Type[] someTypes = valueType.GetGenericArguments();
            if (someTypes.Length == 0) return null;
            return someTypes[0];
        }

        /// <summary>
        /// Constructs a new NamedValue.
        /// </summary>
        public NamedValue()
        {
            Name = "Unknown";
            TypeName = typeof(String).FullName;
            Values = String.Empty;
        }
        #endregion

        #region listing values
        /// <summary>
        /// Returns a named value map given a list of named values.
        /// </summary>
        /// <param name="namedValues">named values</param>
        /// <returns>a named value map</returns>
        public static IDictionary<string, NamedValue> MapFrom(ICollection<NamedValue> namedValues)
        {
            return ListBuilder.With(namedValues).ToMap();
        }

        /// <summary>
        /// Returns a list which contains this NamedValue.
        /// </summary>
        /// <returns>a list which contains this NamedValue</returns>
        public IList<NamedValue> ToList()
        {
            return ListBuilder.With(this).ToList();
        }

        /// <summary>
        /// Returns a NamedValue.Collector (which builds a NamedValue list).
        /// </summary>
        public static Collector ListBuilder
        {
            get { return new Collector(); }
        }
        #endregion

        #region accessing values
        /// <summary>
        /// Establishes the values carried in this NamedValue.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <param name="values">some values</param>
        private void Include<ValueType>(ICollection<ValueType> values)
        {
            Type valueType = typeof(ValueType);
            TypeName = valueType.FullName;
            StringBuilder builder = new StringBuilder();
            int count = 0;
            foreach (ValueType value in values)
            {
                if (count++ > 0) builder.Append(Separator);
                builder.Append(value.ToString());
            }

            Values = builder.ToString();
        }

        /// <summary>
        /// Establishes the values carried in this NamedValue.
        /// </summary>
        /// <param name="valueType">a value type</param>
        /// <param name="values">some values</param>
        private void Include(Type valueType, IEnumerable values)
        {
            TypeName = valueType.FullName;
            StringBuilder builder = new StringBuilder();
            int count = 0;
            foreach (Object value in values)
            {
                if (count++ > 0) builder.Append(Separator);
                builder.Append(value.ToString());
            }

            Values = builder.ToString();
        }

        /// <summary>
        /// Returns the named value.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <returns>a value</returns>
        public ValueType Get<ValueType>()
        {
            return GetLowerBound<ValueType>();
        }

        /// <summary>
        /// Returns the first available value.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <returns>the first available value</returns>
        public ValueType GetLowerBound<ValueType>()
        {
            ValueType[] values = GetValues<ValueType>();
            if (values.Length == 0) return default(ValueType);
            return values[0];
        }

        /// <summary>
        /// Returns the last available value.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <returns>the last available value</returns>
        public ValueType GetUpperBound<ValueType>()
        {
            ValueType[] values = GetValues<ValueType>();
            if (values.Length == 0) return default(ValueType);
            return values[values.Length - 1];
        }

        /// <summary>
        /// Returns the encoded values.
        /// </summary>
        /// <typeparam name="ValueType">an element value type</typeparam>
        /// <returns>the encoded values</returns>
        public ValueType[] GetValues<ValueType>()
        {
            Type elementType = typeof(ValueType);
            String[] values = Values.Split(Separator);
            List<ValueType> results = new List<ValueType>();
            foreach (String value in values)
            {
                try
                {
                    results.Add(Conversion.ConvertTo<ValueType>(value));
                }
                catch (Exception)
                {
                    throw ReportConversionFailure(value, typeof(ValueType));
                }
            }

            return results.ToArray();
        }
        #endregion

        #region checking types
        /// <summary>
        /// Indicates whether a NamedValue exists and has values.
        /// </summary>
        /// <param name="candidate">a candidate</param>
        /// <returns>whether a NamedValue exists and has values</returns>
        public static bool Exists(NamedValue candidate)
        {
            return (candidate != null && candidate.Values.Length > 0);
        }

        /// <summary>
        /// Reports a value conversion failure.
        /// </summary>
        /// <param name="valueName">a value name</param>
        /// <param name="valueType">a value type</param>
        private Exception ReportConversionFailure(String valueName, Type valueType)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("NamedValue was encoded with ");
            builder.Append(TypeName);
            builder.Append(" values and could not convert ");
            builder.Append(valueName);
            builder.Append(" to ");
            builder.Append(valueType.FullName);
            return new InvalidCastException(builder.ToString());
        }
        #endregion


        /// <summary>
        /// Builds a NamedValue list.
        /// </summary>
        public class Collector
        {
            private List<NamedValue> Results = new List<NamedValue>();

            /// <summary>
            /// Constructs a new Collector.
            /// </summary>
            internal Collector()
            {
            }

            /// <summary>
            /// Adds the supplied filter value to the list.
            /// </summary>
            /// <typeparam name="ValueType">a value type</typeparam>
            /// <param name="valueName">a value name</param>
            /// <param name="value">a named value</param>
            /// <returns>this NamedValue.Collector</returns>
            public Collector With<ValueType>(string valueName, ValueType value)
            {
                return With(NamedValue.With(valueName, value));
            }

            /// <summary>
            /// Adds the supplied named values to the list.
            /// </summary>
            /// <param name="namedValues">named values</param>
            /// <returns>this NamedValue.Collector</returns>
            public Collector With(ICollection<NamedValue> namedValues)
            {
                foreach (NamedValue value in namedValues) With(value);
                return this;
            }

            /// <summary>
            /// Adds the supplied filter value to the list.
            /// </summary>
            /// <param name="namedValue">a filter value</param>
            /// <returns>this NamedValue.Collector</returns>
            public Collector With(NamedValue namedValue)
            {
                if (namedValue != null) Results.Add(namedValue);

                return this;
            }

            /// <summary>
            /// Returns the list of collected named values.
            /// </summary>
            /// <returns>a list of named values (with only this value)</returns>
            public IList<NamedValue> ToList()
            {
                return Results;
            }

            /// <summary>
            /// Returns a map of the collected named values.
            /// </summary>
            /// <returns>a map of the collected named values</returns>
            public IDictionary<string, NamedValue> ToMap()
            {
                Dictionary<string, NamedValue> results = new Dictionary<string, NamedValue>();
                if (Results.Count == 0) return results;

                foreach (NamedValue value in Results)
                {
                    results[value.Name] = value;
                }

                return results;
            }

        } // ListBuilder

    } // NamedValue
}
