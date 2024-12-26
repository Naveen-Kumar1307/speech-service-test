using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using System.Diagnostics;
using System.Threading;

namespace GlobalEnglish.Utility.Diagnostics
{
    /// <summary>
    /// Verifies that the counter framework works as intended.
    /// </summary>
    [TestFixture, Ignore]
    public class CounterTestFixture
    {
        /// <summary>
        /// Verifies that performance counters work.
        /// </summary>
        [Test]
        public void CountersWork()
        {
            TestCategory test = new TestCategory();

            Random rand = new Random(42);
            int randMax = 100;
            int limit = 30000;
            int total = 0;

            test.Maximum = randMax;

            while (total < limit)
            {
                int r = rand.Next(randMax);
                test.UpdateCounters(r);

                // sleep the random amount in milliseconds
                Thread.Sleep(r);
                total += r;
            }

            test.GetCounterFactory().Delete();
        }

    } // CounterTestFixture


    /// <summary>
    /// A performance counter test.
    /// </summary>
    [CounterCategory("Random Test")]
    class TestCategory : CounterCategory
    {
        [Counter("Random Rate", PerformanceCounterType.RateOfCountsPerSecond32)]
        private static PerformanceCounter RandomRate;

        [Counter("Random Average", PerformanceCounterType.AverageCount64)]
        private PerformanceCounter RandomAverage;
        private PerformanceCounter RandomAverageBase;

        [Counter("Random Value", PerformanceCounterType.NumberOfItems64)]
        private PerformanceCounter RandomValue;

        //[Counter("Random Value", "Inverted", PerformanceCounterType.NumberOfItems64)]
        //private PerformanceCounter RandomInvertedValue;

        public int Maximum { get; set; }

        public TestCategory() : base()
        {
        }

        public void UpdateCounters(int value)
        {
            // set number counter
            RandomValue.RawValue = value;
            //RandomInvertedValue.RawValue = Maximum - value;

            // update rate counter
            RandomRate.Increment();

            // update average counter
            RandomAverage.IncrementBy(value);
            RandomAverageBase.Increment();
        }

    } // TestCategory
}
