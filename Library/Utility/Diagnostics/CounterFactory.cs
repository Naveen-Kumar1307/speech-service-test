using System;
using System.Text;
using System.Diagnostics;
using System.Reflection;

using Common.Logging;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// A factory for creating counters within a category.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a counter category name, type, and help</item>
    /// <item>creates a counter category and its counters (if missing)</item>
    /// </list>
    /// </remarks>
    public class CounterFactory
	{
        /// <summary>
        /// A counter category name.
        /// </summary>
		public string CategoryName { get; private set; }

        /// <summary>
        /// Help for the counter category.
        /// </summary>
        public string CategoryHelp { get; private set; }

        /// <summary>
        /// A counter category type.
        /// </summary>
        public PerformanceCounterCategoryType CategoryType { get; private set; }

        /// <summary>
        /// The data for creating the counters.
        /// </summary>
		public CounterCreationDataCollection Counters { get; private set; }

		#region creating instances
        /// <summary>
        /// Returns a new PerformanceCounterFactory.
        /// </summary>
        /// <param name="categoryType">a category type</param>
        /// <returns>a new PerformanceCounterFactory, or null</returns>
        public static CounterFactory From(Type categoryType)
        {
            Argument.Check("categoryType", categoryType);
            CounterCategoryAttribute category = 
                CounterCategoryAttribute.From(categoryType);

            if (category == null) return null;
            return category.CreateFactory().WithCounters(categoryType);
        }

        /// <summary>
        /// Returns a new PerformanceCounterFactory.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        /// <returns>a new PerformanceCounterFactory</returns>
        public static CounterFactory Named(string categoryName)
        {
            return Named(categoryName, string.Empty);
        }

		/// <summary>
        /// Returns a new PerformanceCounterFactory.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        /// <param name="categoryHelp">help text for the counter category</param>
        /// <returns>a new PerformanceCounterFactory</returns>
        public static CounterFactory Named(string categoryName, string categoryHelp)
		{
            return new CounterFactory(categoryName, categoryHelp, 
                                      PerformanceCounterCategoryType.Unknown);
		}

        /// <summary>
        /// Constructs a new PerformanceCounterFactory.
        /// </summary>
        /// <param name="categoryName">a counter category name</param>
        /// <param name="categoryHelp">help text for the counter category</param>
        /// <param name="categoryType">a counter category type</param>
        internal CounterFactory(
            string categoryName, string categoryHelp, 
            PerformanceCounterCategoryType categoryType)
        {
            CategoryName = categoryName;
            CategoryHelp = categoryHelp;
            CategoryType = categoryType;
            Counters = new CounterCreationDataCollection();
        }
		#endregion

        #region installing counters
        /// <summary>
        /// Installs the counters associated with a category.
        /// </summary>
        /// <param name="counterCategory">a counter category</param>
        public static bool InstallCounters(CounterCategory counterCategory)
        {
            Argument.Check("counterCategory", counterCategory);
            CounterFactory factory = CounterFactory.From(counterCategory.GetType());
            if (factory == null) return false;

            factory.PopulateCounters(counterCategory);
            return true;
        }

        /// <summary>
        /// Populates the counters in a counter category.
        /// </summary>
        public void PopulateCounters(CounterCategory counterCategory)
        {
            CheckMatch(counterCategory);
            Type categoryType = counterCategory.GetType();
            FieldInfo[] fields = categoryType.GetCounterFields();

            foreach (FieldInfo field in fields)
            {
                // assign the performance counter
                CounterAttribute spec = field.GetCounterAttribute();
                if (!spec.IsBase)
                {
                    PerformanceCounter counter = GetCounter(spec);
                    field.SetValue(counterCategory, counter);

                    if (spec.HasBase)
                    {
                        string baseName = spec.GetBaseName(field);
                        FieldInfo baseCounter = categoryType.GetBaseCounterField(baseName);
                        if (baseCounter != null)
                            baseCounter.SetValue(counterCategory,
                                        GetCounter(spec.BaseName, spec.Instance, false));
                    }
                }
            }
        }
        #endregion

        #region maintaining categories
        /// <summary>
        /// Recreates the counters (to ensure they match the specifications).
        /// </summary>
        public void Recreate()
        {
            if (Exists()) Delete();
            CreateIfNeeded();
        }

        /// <summary>
        /// Indicates whether the counter category for this factory exists.
        /// </summary>
        public bool Exists()
        {
            return PerformanceCounterCategory.Exists(CategoryName);
        }

        /// <summary>
        /// Creates a counter category and counters (if missing).
        /// </summary>
        public void CreateIfNeeded()
        {
            if (!Exists()) CreateCounters();
        }

        /// <summary>
        /// Deletes the counter category and its associated counters.
        /// </summary>
        public void Delete()
        {
            try
            {
                PerformanceCounterCategory.Delete(CategoryName);
            }
            catch (Exception ex)
            {
                ReportDeleteFailure(ex);
            }
        }

        /// <summary>
        /// Creates a category and its associated counters.
        /// </summary>
        private void CreateCounters()
        {
            PerformanceCounterCategory category =
            PerformanceCounterCategory.Create(CategoryName, CategoryHelp, CategoryType, Counters);
        }

        /// <summary>
        /// Checks for a matching category.
        /// </summary>
        private void CheckMatch(CounterCategory counterCategory)
        {
            Argument.Check("counterCategory", counterCategory);
            if (!Matches(counterCategory.GetType()))
                throw new ArgumentOutOfRangeException("counterCategory",
                            string.Format("Expected a counter category named {0}", CategoryName));
        }

        /// <summary>
        /// Indicates whether this factory matches a given category.
        /// </summary>
        private bool Matches(Type categoryType)
        {
            CounterCategoryAttribute test = categoryType.GetCategoryAttribute();

            if (test.Name != CategoryName) return false;
            if (test.Help != CategoryHelp) return false;
            if (test.Type != CategoryType) return false;

            return true;
        }
        #endregion

        #region accessing counters
        /// <summary>
        /// Returns a counter identified by a given name.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <returns>a counter</returns>
        public PerformanceCounter GetCounter(string counterName)
        {
            return GetCounter(counterName, false);
        }

        /// <summary>
        /// Returns a counter identified by a given name.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <param name="readOnly">indicates whether a read-only counter</param>
        /// <returns>a counter</returns>
        public PerformanceCounter GetCounter(string counterName, bool readOnly)
        {
            return GetCounter(CategoryName, counterName, string.Empty, readOnly);
        }

        /// <summary>
        /// Returns a counter identified by a given name and instance.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <param name="instanceName">an instance name</param>
        /// <returns>a counter</returns>
        public PerformanceCounter GetCounter(string counterName, string instanceName)
        {
            return GetCounter(counterName, instanceName, false);
        }

        /// <summary>
        /// Returns a counter identified by a given name and instance.
        /// </summary>
        /// <param name="counterName">a counter name</param>
        /// <param name="instanceName">an instance name</param>
        /// <param name="readOnly">indicates whether a read-only counter</param>
        /// <returns>a counter</returns>
        public PerformanceCounter GetCounter(string counterName, string instanceName, bool readOnly)
        {
            return GetCounter(CategoryName, counterName, instanceName, readOnly);
        }

        /// <summary>
        /// Returns a performance counter instance for a given name and category.
        /// </summary>
        /// <param name="categoryName">a category name</param>
        /// <param name="counterName">a counter name</param>
        /// <param name="instanceName">a counter instance name</param>
        /// <param name="readOnly">indicates whether a read-only counter</param>
        /// <returns>a performance counter</returns>
        public static PerformanceCounter GetCounter(
            string categoryName, string counterName, string instanceName, bool readOnly)
        {
            return new PerformanceCounter(categoryName, counterName, instanceName, readOnly);
        }

        /// <summary>
        /// Rerutns a performance counter instance for a given counter attribute.
        /// </summary>
        /// <param name="counter">a counter attribute</param>
        /// <returns>a performance counter</returns>
        private PerformanceCounter GetCounter(CounterAttribute counter)
        {
            return GetCounter(CategoryName, counter.Name, counter.Instance, false);
        }
        #endregion

        #region Public Methods
		/// <summary>
		/// Adds a performance counter of the given type to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a performance counter of the given type to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="type">Performance counter type.</param>
		/// <param name="help">Performance counter help text.</param>
		public void AddCounter(string name, PerformanceCounterType type, string help)
		{
			CounterCreationData counter = new CounterCreationData();
			counter.CounterName = name;
			counter.CounterType = type;
			if (help != null)
				counter.CounterHelp = help;

			Counters.Add(counter);
		}

		/// <summary>
		/// Adds a performance counter of the given type to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a performance counter of the given type to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="type">Performance counter type.</param>
		public void AddCounter(string name, PerformanceCounterType type)
		{
			AddCounter(name, type, null);
		}

		/// <summary>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public void AddNumberCounter(string name, string help)
		{
			AddCounter(name, PerformanceCounterType.NumberOfItems64, help);
		}

		/// <summary>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a NumberOfItems64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public void AddNumberCounter(string name)
		{
			AddCounter(name, PerformanceCounterType.NumberOfItems64, null);
		}

		/// <summary>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public void AddRateCounter(string name, string help)
		{
			AddCounter(name, PerformanceCounterType.RateOfCountsPerSecond64, help);
		}

		/// <summary>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Adds a RateOfCountsPerSecond64 performance counter to the category collection.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public void AddRateCounter(string name)
		{
			AddCounter(name, PerformanceCounterType.RateOfCountsPerSecond64);
		}

		/// <summary>
		/// Adds a AverageCount64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public void AddAverageCounter(string name, string help)
		{
			// add average counter
			AddCounter(name, PerformanceCounterType.AverageCount64, help);

			// add the corresponding base counter
			AddCounter(name + CounterAttribute.Base, PerformanceCounterType.AverageBase, null);
		}

		/// <summary>
		/// Adds a AverageCount64 performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public void AddAverageCounter(string name)
		{
			AddAverageCounter(name, null);
		}

		/// <summary>
		/// Adds a RawFraction performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		/// <param name="help">Performance counter help text.</param>
		public void AddFractionCounter(string name, string help)
		{
			// add average counter
			AddCounter(name, PerformanceCounterType.RawFraction, help);

			// add the corresponding base counter
			AddCounter(name + CounterAttribute.Base, PerformanceCounterType.RawBase, null);
		}

		/// <summary>
		/// Adds a RawFraction performance counter to the category collection.
		/// </summary>
		/// <remarks>
		/// Also adds the required base counter.
		/// </remarks>
		/// <param name="name">Performance counter name.</param>
		public void AddFractionCounter(string name)
		{
			AddFractionCounter(name, null);
		}

        /// <summary>
        /// Adds the counters from a given category class to this factory.
        /// </summary>
        /// <param name="categoryType">a category class</param>
        /// <returns>this factory</returns>
        public CounterFactory WithCounters(Type categoryType)
        {
            CollectCounters(categoryType);
            return this;
        }

        /// <summary>
        /// Adds the available counters from a given category type.
        /// </summary>
        private void CollectCounters(Type categoryType)
        {
            FieldInfo[] counterFields = categoryType.GetCounterFields();
            foreach (FieldInfo field in counterFields)
            {
                CounterAttribute counter = field.GetCounterAttribute();
                if (!counter.IsBase)
                {
                    // only create a counter with multiple instances once 
                    if (counter.Instance == string.Empty || !Exists(counter))
                    {
                        AddCounter(counter.Name, counter.Type, counter.Help);

                        if (counter.HasBase)
                            AddCounter(counter.BaseName, counter.BaseType);
                    }
                }
            }
        }

        /// <summary>
        /// Indicates whether the collected counters already include a given one.
        /// </summary>
        private bool Exists(CounterAttribute counter)
        {
            foreach (CounterCreationData creationData in Counters)
            {
                if (creationData.CounterName == counter.Name)
                    return true;
            }

            return false;
        }
        #endregion

        #region reporting problems
        private void ReportDeleteFailure(Exception ex)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().Name);
            builder.Append(" failed to delete counter category ");
            builder.Append(CategoryName.SinglyQuoted());
            LogManager.GetLogger(typeof(CounterFactory))
                      .Warn(builder.ToString(), ex);
        }

        /// <summary>
        /// Reports a missing counter category.
        /// </summary>
        private static Exception ReportMissingAttribute(Type categoryType)
        {
            return new ArgumentOutOfRangeException("categoryType",
                            String.Format(MissingAttributeMessage, categoryType.Name));
        }

        private static readonly string MissingAttributeMessage =
                                "{0} has no attribute [PerformanceCounterCategory]";
        #endregion

    } // CounterFactory
}
