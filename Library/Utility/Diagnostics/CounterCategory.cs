using System;
using System.Text;
using System.Collections.Generic;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// Provides a base for counter category classes.
    /// </summary>
    /// <remarks>This base class automates the installation of counters defined by derived ckasses.
    /// </remarks>
    public class CounterCategory
    {
        #region creating instances
        /// <summary>
        /// Constructs a new PerformanceCounter Category.
        /// </summary>
        protected CounterCategory() : this(false) { }

        /// <summary>
        /// Constructs a new CounterCategory.
        /// </summary>
        /// <param name="reset">indicates whether to recreate the counters</param>
        protected CounterCategory(bool recreate)
        {
            CounterFactory factory = GetCounterFactory();

            if (recreate)
            {
                factory.Recreate();
            }
            else
            {
                factory.CreateIfNeeded();
            }

            factory.PopulateCounters(this);
        }

        /// <summary>
        /// Returns a counter factory for this category.
        /// </summary>
        /// <returns>a counter factory for this category</returns>
        public CounterFactory GetCounterFactory()
        {
            return CounterFactory.From(GetType());
        }
        #endregion

    } // CounterCategoryBase
}
