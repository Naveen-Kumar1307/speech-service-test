using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// Extends the Type class.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly BindingFlags CounterFlags = 
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;

        private static readonly Type CounterClass = typeof(PerformanceCounter);
        private static readonly Type CounterType = typeof(CounterAttribute);
        private static readonly Type CategoryType = typeof(CounterCategoryAttribute);

        /// <summary>
        /// Returns a specific base counter field from this type.
        /// </summary>
        /// <param name="categoryType">a category type</param>
        /// <param name="counter">a counter attriubte</param>
        /// <returns>a base counter field</returns>
        public static FieldInfo GetBaseCounterField(this Type categoryType, string baseName)
        {
            return categoryType.GetField(baseName, CounterFlags);
        }

        /// <summary>
        /// Returns the counter gategory attribute of this type.
        /// </summary>
        /// <param name="categoryType">a category type</param>
        /// <returns>a counter category attribute</returns>
        public static CounterCategoryAttribute GetCategoryAttribute(this Type categoryType)
        {
            object[] attributes = categoryType.GetCustomAttributes(CategoryType, false);
            return (attributes.Length == 0 ? null :
                        (CounterCategoryAttribute)attributes[0]);
        }

        /// <summary>
        /// Returns the available performance counter fields from this counter category type.
        /// </summary>
        /// <param name="categoryType">a counter category type</param>
        /// <returns>the available counter fields</returns>
        public static FieldInfo[] GetCounterFields(this Type categoryType)
        {
            // filter out all non-counter fields
            return (from field in categoryType.GetFields(CounterFlags)
                    where field.FieldType == CounterClass
                    &&    field.GetCustomAttributes(CounterType, false).Length > 0
                    select field).ToArray();
        }

        /// <summary>
        /// Returns the counter attribute of this field.
        /// </summary>
        /// <param name="field">a counter field</param>
        /// <returns>a counter attribute</returns>
        public static CounterAttribute GetCounterAttribute(this FieldInfo field)
        {
            object[] attributes = field.GetCustomAttributes(CounterType, false);
            if (attributes.Length == 0) return null;

            CounterAttribute result = attributes[0] as CounterAttribute;
            if (Argument.IsAbsent(result.Name)) result.Name = field.Name;
            return result;
        }

    } // TypeExtensions


    /// <summary>
    /// Extends the PerformanceCounterType class.
    /// </summary>
    public static class CounterTypeExtensions
    {
        private static Dictionary<PerformanceCounterType, PerformanceCounterType> 
            BaseTypeMap = new Dictionary<PerformanceCounterType, PerformanceCounterType>();

        /// <summary>
        /// Initializes the base type map.
        /// </summary>
        static CounterTypeExtensions()
        {
            BaseTypeMap.Add(PerformanceCounterType.RawFraction, PerformanceCounterType.RawBase);

            BaseTypeMap.Add(PerformanceCounterType.AverageCount64, PerformanceCounterType.AverageBase);
            BaseTypeMap.Add(PerformanceCounterType.AverageTimer32, PerformanceCounterType.AverageBase);

            BaseTypeMap.Add(PerformanceCounterType.CounterMultiTimer, PerformanceCounterType.CounterMultiBase);
            BaseTypeMap.Add(PerformanceCounterType.CounterMultiTimer100Ns, PerformanceCounterType.CounterMultiBase);
            BaseTypeMap.Add(PerformanceCounterType.CounterMultiTimerInverse, PerformanceCounterType.CounterMultiBase);
            BaseTypeMap.Add(PerformanceCounterType.CounterMultiTimer100NsInverse, PerformanceCounterType.CounterMultiBase);

            BaseTypeMap.Add(PerformanceCounterType.SampleCounter, PerformanceCounterType.SampleBase);
            BaseTypeMap.Add(PerformanceCounterType.SampleFraction, PerformanceCounterType.SampleBase);
        }

        /// <summary>
        /// Indicates whether this counter type has a corresponding base type.
        /// </summary>
        /// <param name="counterType">a counter type</param>
        /// <returns>whether this counter type has a corresponding base type</returns>
        public static bool HasBaseType(this PerformanceCounterType counterType)
        {
            return BaseTypeMap.ContainsKey(counterType);
        }

        /// <summary>
        /// Returns the base type of this performance counter type.
        /// </summary>
        /// <param name="counterType">a counter type</param>
        /// <returns>a performance counter base type</returns>
        public static PerformanceCounterType GetBaseType(this PerformanceCounterType counterType)
        {
            if (HasBaseType(counterType)) return BaseTypeMap[counterType];
            else throw new ArgumentOutOfRangeException("counterType", 
                            counterType.ToString() + " has no corresponding base type");
        }

    } // CounterTypeExtensions
}
