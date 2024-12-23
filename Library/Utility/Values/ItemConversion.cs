using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Runtime.Serialization;
using Common.Logging;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// Copies values between (potentially) compatible items.
    /// </summary>
    internal class ItemConversion
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemConversion));

        #region converting compatible types
        /// <summary>
        /// Checks the copy compatibility of two types.
        /// </summary>
        /// <param name="sourceType">a source type</param>
        /// <param name="resultType">a result type</param>
        public static void CheckCompatible(Type sourceType, Type resultType)
        {
            if (resultType.IsAssignableFrom(sourceType)) return;

            Type sourceElement = sourceType.GetElementType();
            Type resultElement = resultType.GetElementType();
            if (resultElement != null)
            {
                if (resultType.IsAssignableFrom(sourceType)) return;

                throw ReportIncompatible(sourceType, resultType);
            }

            Type baseType = FindCompatibleBase(sourceType, resultType);
            if (baseType == null)
                throw ReportIncompatible(sourceType, resultType);
        }

        private static Type FindCompatibleBase(Type sourceType, Type resultType)
        {
            Type testType = sourceType;
            while (testType != null)
            {
                if (testType.Name == resultType.Name) return testType;

                testType = testType.BaseType;
            }

            return testType;
        }

        private static Exception ReportIncompatible(Type sourceType, Type resultType)
        {
            return new InvalidCastException(
                        sourceType.FullName + " is not compatible with " + resultType.FullName);
        }
        #endregion

        #region creating instances
        /// <summary>
        /// Returns a new ItemConversion.
        /// </summary>
        /// <param name="source">a source item</param>
        /// <returns>a new ItemConversion</returns>
        public static ItemConversion With(Object source)
        {
            ItemConversion result = new ItemConversion();
            result.Source = source;
            return result;
        }
        #endregion

        #region copying values
        /// <summary>
        /// Returns a new compatible result copied from a source.
        /// </summary>
        /// <param name="resultType">a result type</param>
        /// <returns>a new compatible result copied from a source</returns>
        public Object ConvertTo(Type resultType)
        {
            if (resultType.IsAssignableFrom(Source.GetType())) return Source;
            Target = resultType.GetConstructor(Type.EmptyTypes).Invoke(null);
            CopyPropertyValues();
            return Target;
        }

        /// <summary>
        /// Copies values from a source to a compatible target.
        /// </summary>
        private void CopyPropertyValues()
        {
            Type sourceType = Source.GetType();
            Type targetType = Target.GetType();
            PropertyInfo[] sourceValues = sourceType.GetProperties();
            PropertyInfo[] targetValues = targetType.GetProperties();
            var sourceNames = (from item in sourceValues select item.Name).ToList();
            var targetNames = (from item in targetValues select item.Name).ToList();
            var commonNames = sourceNames.Intersect(targetNames);
            var copyValues = (from item in targetValues
                              where commonNames.Contains(item.Name)
                              select item).ToList();

            foreach (PropertyInfo targetValue in copyValues) CopyValue(targetValue);
            Logger.Debug("copied " + Source.GetType().FullName + " to " + Target.GetType().FullName);
        }

        /// <summary>
        /// Copies a value from a source to a compatible target.
        /// </summary>
        /// <param name="targetValue">a target value</param>
        private void CopyValue(PropertyInfo targetValue)
        {
            Type sourceType = Source.GetType();
            PropertyInfo sourceValue = sourceType.GetProperty(targetValue.Name);
            object[] typeAttributes = sourceType.GetCustomAttributes(true);
            object[] valueAttributes = sourceValue.GetCustomAttributes(true);

            // don't copy non-contract members
            var contract = typeAttributes.Any(item => item is DataContractAttribute);
            var member = valueAttributes.Any(item => item is DataMemberAttribute);
            if (contract && !member)
            {
                Logger.Debug(targetValue.Name + " was not copied, not [DataMember]");
                return;
            }

            Object value = sourceValue.GetValue(Source, null);
            if (value == null)
            {
                Logger.Debug(targetValue.Name + " was null, not copied");
                return;
            }

            if (value.GetType().FullName == targetValue.PropertyType.FullName)
            {
                targetValue.SetValue(Target, value, null);
                Logger.Debug(targetValue.Name + " was copied");
                return;
            }

            Type valueType = targetValue.PropertyType;
            if (Conversion.EnumType.IsAssignableFrom(valueType))
            {
                targetValue.SetValue(Target, Enum.Parse(valueType, value.ToString()), null);
                Logger.Debug(targetValue.Name + " was copied");
                return;
            }

            if (valueType.Name.EndsWith("]"))
            {
                Object array = CopyElements(value as IEnumerable, valueType.GetElementType());
                targetValue.SetValue(Target, array, null);
                Logger.Debug(targetValue.Name + " was copied");
            }
            else
            {
                targetValue.SetValue(Target, Conversion.ConvertTo(valueType, value), null);
                Logger.Debug(targetValue.Name + " was copied");
            }
        }

        /// <summary>
        /// Copies the elements of an array.
        /// </summary>
        /// <param name="elements">array elements</param>
        /// <param name="elementType">an array element type</param>
        /// <returns>a source array copy</returns>
        private Array CopyElements(IEnumerable elements, Type elementType)
        {
            int count = 0;
            foreach (Object element in elements) count++;
            Array result = Array.CreateInstance(elementType, count);

            int index = 0;
            if (Conversion.EnumType.IsAssignableFrom(elementType))
            {
                foreach (Object element in elements)
                {
                    result.SetValue(Enum.Parse(elementType, element.ToString()), index++);
                }
            }
            else
            {
                foreach (Object element in elements)
                {
                    result.SetValue(Conversion.ConvertTo(elementType, element), index++);
                }
            }

            return result;
        }
        #endregion

        #region accessing items
        /// <summary>
        /// A source item.
        /// </summary>
        public Object Source { get; set; }

        /// <summary>
        /// A target item.
        /// </summary>
        public Object Target { get; set; }
        #endregion

    } // ItemConversion
}
