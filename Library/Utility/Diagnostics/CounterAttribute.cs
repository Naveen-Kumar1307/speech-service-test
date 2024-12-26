using System;
using System.Text;
using System.Collections.Generic;
using GlobalEnglish.Utility.Parameters;
using System.Diagnostics;
using System.Reflection;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// An attribute to define a performance counter within a counter category class.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a counter name, type, and help</item>
    /// </list>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field)]
    public class CounterAttribute : Attribute
    {
        public static readonly string Base = "Base";

        /// <summary>
        /// A counter name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Help text for the counter.
        /// </summary>
        public string Help { get; private set; }

        /// <summary>
        /// A counter instance name.
        /// </summary>
        public string Instance { get; private set; }

        /// <summary>
        /// A counter type.
        /// </summary>
        public PerformanceCounterType Type { get; private set; }

        #region creating instances
        /// <summary>
        /// Constructs a new CounterAttribute.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        public CounterAttribute(string counterName)
            : this(counterName, PerformanceCounterType.NumberOfItems64)
        {
        }

        /// <summary>
        /// Constructs a new CounterAttribute.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <param name="counterType">a counter type</param>
        public CounterAttribute(
            string counterName, PerformanceCounterType counterType)
            : this(counterName, string.Empty, counterType)
        {
        }

        /// <summary>
        /// Constructs a new CounterAttribute.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <param name="counterInstance">a counter instance name</param>
        /// <param name="counterType">a counter type</param>
        public CounterAttribute(
            string counterName, string counterInstance, PerformanceCounterType counterType)
            : this(counterName, counterInstance, counterType, string.Empty)
        {
        }

        /// <summary>
        /// Constructs a new CounterAttribute.
        /// </summary>
        /// <param name="counterType">a counter type</param>
        /// <param name="counterHelp">help text for the counter</param>
        public CounterAttribute(PerformanceCounterType counterType, string counterHelp)
            : this(string.Empty, string.Empty, counterType, counterHelp)
        {
        }

        /// <summary>
        /// Constructs a new CounterAttribute.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <param name="counterInstance">a counter instance name</param>
        /// <param name="counterType">a counter type</param>
        /// <param name="counterHelp">help text for the counter</param>
        public CounterAttribute(
            string counterName, string counterInstance, 
            PerformanceCounterType counterType, string counterHelp)
        {
            if (Argument.IsAbsent(counterName)) counterName = string.Empty;
            if (Argument.IsAbsent(counterHelp)) counterHelp = string.Empty;
            if (Argument.IsAbsent(counterInstance)) counterInstance = string.Empty;

            Name = counterName;
            Help = counterHelp;
            Type = counterType;
            Instance = counterInstance;
        }
        #endregion

        #region accessing values
        /// <summary>
        /// Indicates whether this counter is a base.
        /// </summary>
        public bool IsBase
        {
            get { return Name.EndsWith(Base); }
        }

        /// <summary>
        /// Indicates whether this counter has a base.
        /// </summary>
        public bool HasBase
        {
            get { return Type.HasBaseType(); }
        }

        /// <summary>
        /// The name of the base for this counter.
        /// </summary>
        public string BaseName
        {
            get { return Name + Base; }
        }

        /// <summary>
        /// The base type for this counter.
        /// </summary>
        public PerformanceCounterType BaseType
        {
            get { return Type.GetBaseType(); }
        }

        /// <summary>
        /// Returns the name of a base field derived from a counter field.
        /// </summary>
        /// <param name="field">a counter field</param>
        /// <returns>a base field name</returns>
        public string GetBaseName(FieldInfo field)
        {
            if (field == null) return string.Empty;
            return field.Name + Base;
        }
        #endregion

    } // CounterAttribute
}
