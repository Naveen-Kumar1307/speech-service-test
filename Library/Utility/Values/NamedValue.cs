using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Serialization;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// A named value (of a known type).
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows some value(s)</item>
    /// <item>knows the value name and type</item>
    /// <item>encodes the value(s) uniformly as strings (for easy transport)</item>
    /// </list>
    /// <h4>Clients must:</h4>
    /// <list type="bullet">
    /// <item>supply a field name, type, and value(s) during construction</item>
    /// </list>
    /// </remarks>
    [DataContract]
    public class NamedValue
    {
        private static readonly char Separator = ',';
        private static readonly Type ObjectType = typeof(Object);
        private static readonly Type EnumerableType = typeof(IEnumerable);

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

            CheckValue<ValueType>("value", value);
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
            CheckValue<ValueType>("lowerBound", lowerBound);
            CheckValue<ValueType>("upperBound", upperBound);
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
                builder.Append(Conversion.From<ValueType>()(value));
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
        /// Returns the encoded field values.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <returns>the encoded field values</returns>
        public ValueType[] GetValues<ValueType>()
        {
            String[] values = Values.Split(Separator);
            List<ValueType> results = new List<ValueType>();
            foreach (String value in values)
            {
                try
                {
                    results.Add(Conversion.To<ValueType>()(value));
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
        /// Verifies that an elementary value was supplied.
        /// </summary>
        /// <typeparam name="ValueType">a value type</typeparam>
        /// <param name="valueName">a value name</param>
        /// <param name="value">a value</param>
        private static void CheckValue<ValueType>(String valueName, ValueType value)
        {
            Type valueType = typeof(ValueType);
            if (ObjectType.IsAssignableFrom(valueType))
            {
                Argument.Check(valueName, value);
            }

            if (value is String) return;
            if (EnumerableType.IsAssignableFrom(valueType))
            {
                throw new ArgumentOutOfRangeException(
                            valueName, "An elementary value was expected");
            }
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

        #region converting values
        /// <summary>
        /// Returns a list which contains this NamedValue.
        /// </summary>
        /// <returns>a list which contains this NamedValue</returns>
        public IList<NamedValue> ToList()
        {
            List<NamedValue> results = new List<NamedValue>();
            results.Add(this);
            return results;
        }

        /// <summary>
        /// Returns a list which contains this as a compatible NamedValue.
        /// </summary>
        /// <typeparam name="TargetType">a compatible target type</typeparam>
        /// <returns>a list which contains this NamedValue</returns>
        public IList<TargetType> ToList<TargetType>() where TargetType : class, new()
        {
            return List<TargetType>(this.ToList());
        }

        /// <summary>
        /// Returns a compatible result value.
        /// </summary>
        /// <typeparam name="ResultType">a compatible result type</typeparam>
        /// <returns>a new result value</returns>
        public ResultType To<ResultType>() where ResultType : class, new()
        {
            return Conversion.ConvertTo<ResultType>(this);
        }

        /// <summary>
        /// Returns a new NamedValue.
        /// </summary>
        /// <typeparam name="SourceType">a source value type</typeparam>
        /// <param name="value">a required value</param>
        /// <returns>a new NamedValue</returns>
        public static NamedValue From<SourceType>(SourceType value)
        {
            return Conversion.ConvertTo<NamedValue>(value);
        }

        /// <summary>
        /// Returns a new NamedValue list.
        /// </summary>
        /// <typeparam name="SourceType">a source type compatible with NamedValue</typeparam>
        /// <param name="namedValues">some named values</param>
        /// <returns>a new NamedValue list, or empty</returns>
        public static IList<NamedValue> ListFrom<SourceType>(ICollection<SourceType> namedValues)
        {
            List<NamedValue> results = new List<NamedValue>();
            if (namedValues == null || namedValues.Count == 0) return results;
            return namedValues.Select(item => NamedValue.From<SourceType>(item)).ToList();
        }

        /// <summary>
        /// Returns a new NamedValue list.
        /// </summary>
        /// <typeparam name="TargetType">a target type compatible with NamedValue</typeparam>
        /// <param name="namedValues">some named values</param>
        /// <returns>a new NamedValue list, or empty</returns>
        public static IList<TargetType> List<TargetType>(ICollection<NamedValue> namedValues)
            where TargetType : class, new()
        {
            List<TargetType> results = new List<TargetType>();
            if (namedValues == null || namedValues.Count == 0) return results;
            return namedValues.Select(item => item.To<TargetType>()).ToList();
        }

        /// <summary>
        /// Returns a new NamedValue map.
        /// </summary>
        /// <param name="namedValues">some named values</param>
        /// <returns>a new NamedValue map, or empty</returns>
        public static IDictionary<String, NamedValue> MapFrom(ICollection<NamedValue> namedValues)
        {
            Dictionary<String, NamedValue> results = new Dictionary<string, NamedValue>();
            if (namedValues == null || namedValues.Count == 0) return results;

            foreach (NamedValue value in namedValues)
            {
                results[value.Name] = value;
            }
            return results;
        }

        /// <summary>
        /// Returns a new NamedValue map.
        /// </summary>
        /// <typeparam name="SourceType">a source type</typeparam>
        /// <param name="namedValues">some named values</param>
        /// <returns>a new NamedValue map, or empty</returns>
        public static IDictionary<String, NamedValue>
            MapFrom<SourceType>(ICollection<SourceType> namedValues)
        {
            Dictionary<string, NamedValue> results = new Dictionary<string, NamedValue>();
            if (namedValues == null || namedValues.Count == 0) return results;

            foreach (SourceType value in namedValues)
            {
                NamedValue namedValue = NamedValue.From(value);
                results[namedValue.Name] = namedValue;
            }
            return results;
        }

        /// <summary>
        /// Returns a NamedValue.Collector (which builds a list).
        /// </summary>
        public static Collector ListBuilder
        {
            get { return new Collector(); }
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
            /// Adds the supplied name-value pair to the list.
            /// </summary>
            /// <param name="namedValue">a named-value pair</param>
            /// <returns>this NamedValue.Collector</returns>
            public Collector With(NamedValue namedValue)
            {
                if (namedValue != null) Results.Add(namedValue);

                return this;
            }

            /// <summary>
            /// Adds the supplied name-value pair to the list.
            /// </summary>
            /// <typeparam name="ValueType">a value type</typeparam>
            /// <param name="valueName">a value name</param>
            /// <param name="value">a named value</param>
            /// <returns>this NamedValue.Collector</returns>
            public Collector With<ValueType>(string valueName, ValueType value)
            {
                Results.Add(NamedValue.With(valueName, value));
                return this;
            }

            /// <summary>
            /// Returns the list of collected name value pairs.
            /// </summary>
            /// <returns>a list of name value pairs</returns>
            public IList<NamedValue> ToList()
            {
                return Results;
            }

            /// <summary>
            /// Returns a list of the collected name value pairs.
            /// </summary>
            /// <typeparam name="TargetType">a target type compatible with NamedValue</typeparam>
            /// <returns>a list of name value pairs</returns>
            public IList<TargetType> ToList<TargetType>() where TargetType : class, new()
            {
                return NamedValue.List<TargetType>(Results);
            }

        } // ListBuilder

    } // NamedValue
}
