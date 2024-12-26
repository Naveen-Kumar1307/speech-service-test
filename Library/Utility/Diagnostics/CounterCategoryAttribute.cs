using System;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// An attribute to group the performance counters from a given class in a counter category.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a counter category name, type, and help</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class)]
    public class CounterCategoryAttribute : Attribute
    {
        private static readonly Type ClassType = typeof(CounterCategoryAttribute);

        /// <summary>
        /// A counter category name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Help text for the counter category.
        /// </summary>
        public string Help { get; private set; }

        /// <summary>
        /// A counter category type.
        /// </summary>
        public PerformanceCounterCategoryType Type { get; private set; }

        #region creating instances
        /// <summary>
        /// Returns a category attribute for the supplied type (if available).
        /// </summary>
        /// <param name="type">a counter category type</param>
        /// <returns>a category attribute, or null</returns>
        public static CounterCategoryAttribute From(Type type)
        {
            object[] attributes = type.GetCustomAttributes(ClassType, false);
            return (attributes.Length < 1 ? null : 
                    (CounterCategoryAttribute)attributes[0]);
        }

        /// <summary>
        /// Constructs a new CounterCategoryAttribute.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        public CounterCategoryAttribute(string categoryName)
            : this(categoryName, string.Empty)
        {
        }

        /// <summary>
        /// Constructs a new CounterCategoryAttribute.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        /// <param name="categoryHelp">help text for the counter category</param>
        public CounterCategoryAttribute(string categoryName, string categoryHelp)
            : this(categoryName, PerformanceCounterCategoryType.Unknown, categoryHelp)
        {
        }

        /// <summary>
        /// Constructs a new CounterCategoryAttribute.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        /// <param name="categoryType">a counter category type</param>
        public CounterCategoryAttribute(
            string categoryName, PerformanceCounterCategoryType categoryType)
            : this(categoryName, categoryType, string.Empty)
        {
        }

        /// <summary>
        /// Constructs a new CounterCategoryAttribute.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        /// <param name="categoryType">a counter category type</param>
        /// <param name="categoryHelp">help text for the counter category</param>
        public CounterCategoryAttribute(
            string categoryName, PerformanceCounterCategoryType categoryType, string categoryHelp)
        {
            Argument.Check("categoryName", categoryName);
            if (Argument.IsAbsent(categoryHelp)) categoryHelp = string.Empty;

            Name = categoryName;
            Help = categoryHelp;
            Type = categoryType;
        }

        /// <summary>
        /// Returns a new PerformanceCounterFactory.
        /// </summary>
        public CounterFactory CreateFactory()
        {
            return new CounterFactory(Name, Help, Type);
        }
        #endregion

    } // CounterCategoryAttribute
}
