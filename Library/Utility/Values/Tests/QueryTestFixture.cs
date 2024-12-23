using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;
using System.Runtime.Serialization;

namespace GlobalEnglish.Utility.Values
{
    /// <summary>
    /// Verifies query support works as intended.
    /// </summary>
    [TestFixture]
    public class QueryTestFixture
    {
        /// <summary>
        /// Shows how client and server side code collaborate with QueryCriterion[].
        /// </summary>
        [Test]
        public void VerifyClientServerSample()
        {
            // build query client criteria
            QueryCriterion sampleString = QueryCriterion.Named("SampleString", "Welcome");
            QueryCriterion sampleInteger = QueryCriterion.Named("SampleInteger", 500);

            TypeCode[] typeCodes = { TypeCode.Boolean, TypeCode.Byte, TypeCode.Empty };
            QueryCriterion sampleEnums = QueryCriterion.Named("SampleEnum[]", typeCodes);

            DateTime now = DateTime.Now;
            TimeSpan span = TimeSpan.FromDays(10);
            DateTime lowerBound = now.Date - span;
            DateTime upperBound = now.Date + span;
            QueryCriterion dateRange =
                QueryCriterion.RangeNamed("SampleDateRange", lowerBound, upperBound)
                    .With(QueryCriterion.SortOrder.Descending);

            QueryCriterion[] criteria = { sampleString, sampleInteger, sampleEnums, dateRange };

            // digest criteria on server side and access by name
            NamedQuery sampleQuery = NamedQuery.Named("Sample").WithFilters(criteria);

            String queryText = sampleQuery["SampleString"].GetLowerBound<String>();
            int queryInteger = sampleQuery["SampleInteger"].GetLowerBound<int>();
            TypeCode[] queryEnums = sampleQuery["SampleEnum[]"].GetValues<TypeCode>();
            DateTime loBound = sampleQuery["SampleDateRange"].GetLowerBound<DateTime>();
            DateTime hiBound = sampleQuery["SampleDateRange"].GetUpperBound<DateTime>();

            Assert.IsTrue(queryText == "Welcome");
            Assert.IsTrue(queryInteger == 500);
            Assert.IsTrue(queryEnums.Length == typeCodes.Length);
            Assert.IsTrue((lowerBound - loBound) < TimeSpan.FromSeconds(1));
            Assert.IsTrue((upperBound - hiBound) < TimeSpan.FromSeconds(1));

            NamedQuery simpleQuery = 
                NamedQuery.Named("Simple")
                    .With("SampleInteger", 500)
                    .With("SampleString", "Welcome")
                    .WithRange("SampleDateRange", lowerBound, upperBound);

            queryText = simpleQuery["SampleString"].GetLowerBound<String>();
            queryInteger = simpleQuery["SampleInteger"].GetLowerBound<int>();
            loBound = simpleQuery["SampleDateRange"].GetLowerBound<DateTime>();
            hiBound = simpleQuery["SampleDateRange"].GetUpperBound<DateTime>();

            Assert.IsTrue(queryText == "Welcome");
            Assert.IsTrue(queryInteger == 500);
            Assert.IsTrue((lowerBound - loBound) < TimeSpan.FromSeconds(1));
            Assert.IsTrue((upperBound - hiBound) < TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Verifies that query criterion work.
        /// </summary>
        [Test]
        public void VerifyQueryCriterion()
        {
            // verify simple value conversions
            int[] sampleValues = { 1, 2, 3 };
            QueryCriterion sample = QueryCriterion.ValuesNamed("Sample", sampleValues);
            int[] testValues = sample.GetValues<int>();
            Assert.IsTrue(testValues.Length == sampleValues.Length);
            Assert.IsTrue(sample.FieldName == "Sample");
            Assert.IsTrue(sample.FieldType == sampleValues[0].GetType().FullName);
            Assert.IsTrue(sample.FieldValues == "1,2,3");
            Assert.IsTrue(sample.ResultsOrder == QueryCriterion.SortOrder.None);

            // verify criterion conversion across namespaces
            Test.Samples.QueryCriterion clone =
                sample.To<Test.Samples.QueryCriterion>();

            Assert.IsTrue(clone.FieldName == sample.FieldName);
            Assert.IsTrue(clone.FieldType == sample.FieldType);
            Assert.IsTrue(clone.FieldValues == sample.FieldValues);
            Assert.IsTrue(clone.ResultsOrder.ToString() == sample.ResultsOrder.ToString());

            // verify date range conversion
            DateTime now = DateTime.Now;
            TimeSpan span = TimeSpan.FromDays(10);
            DateTime lowerBound = now - span;
            DateTime upperBound = now + span;
            QueryCriterion dateRange =
                QueryCriterion.RangeNamed("DateRange", lowerBound, upperBound)
                    .With(QueryCriterion.SortOrder.Descending);

            DateTime lowerTest = dateRange.GetLowerBound<DateTime>();
            DateTime upperTest = dateRange.GetUpperBound<DateTime>();
            Assert.IsTrue(dateRange.ResultsOrder == QueryCriterion.SortOrder.Descending);
            TimeSpan lowerDiff = lowerBound - lowerTest;
            TimeSpan upperDiff = upperBound - upperTest;
            Assert.IsTrue(lowerDiff.Seconds == 0);
            Assert.IsTrue(upperDiff.Seconds == 0);

            // verify simple enum conversion
            TypeCode[] codes = { TypeCode.Boolean, TypeCode.Byte, TypeCode.Empty };
            QueryCriterion types = QueryCriterion.ValuesNamed("Types", codes);
            TypeCode[] testCodes = types.GetValues<TypeCode>();
            Assert.IsTrue(testCodes.Length == codes.Length);

            // verify enum conversion across namespaces
            QueryCriterion.SortOrder[] orders = 
            {
                QueryCriterion.SortOrder.Ascending, 
                QueryCriterion.SortOrder.Descending 
            };
            QueryCriterion orderSample = QueryCriterion.ValuesNamed("Orders", orders);
            Test.Samples.QueryCriterion.SortOrder[] testOrders =
                orderSample.GetValues<Test.Samples.QueryCriterion.SortOrder>();
        }

        /// <summary>
        /// Verifies that simple criterion construction works.
        /// </summary>
        [Test]
        public void VerifySimpleConstructions()
        {
            int[] sampleValues = { 1, 2, 3 };
            QueryCriterion sampleArray = QueryCriterion.Named("Sample", sampleValues);
            Assert.IsTrue(sampleArray.FieldValues == "1,2,3");
            QueryCriterion sampleInt = QueryCriterion.Named("Sample", 5);
            Assert.IsTrue(sampleInt.FieldValues == "5");
            QueryCriterion sampleString = QueryCriterion.Named("Sample", "Test");
            Assert.IsTrue(sampleString.FieldValues == "Test");
            QueryCriterion sampleDouble = QueryCriterion.Named("Sample", 5.1);
            Assert.IsTrue(sampleDouble.FieldValues == "5.1");
            QueryCriterion sampleDate = QueryCriterion.Named("Sample", DateTime.Now);
            Assert.IsTrue(sampleDate.FieldValues.StartsWith(DateTime.Now.Year.ToString()));
            QueryCriterion sampleFloat = QueryCriterion.Named("Sample", 5.2f);
            Assert.IsTrue(sampleFloat.FieldValues == "5.2");

            TypeCode[] codes = { TypeCode.Boolean, TypeCode.Byte, TypeCode.Empty };
            QueryCriterion types = QueryCriterion.Named("Types", codes);
            TypeCode[] testCodes = types.GetValues<TypeCode>();
            Assert.IsTrue(testCodes.Length == codes.Length);

            List<int> valueList = new List<int>(sampleValues);
            sampleArray = QueryCriterion.Named("Sample", valueList);
            Assert.IsTrue(sampleArray.FieldValues == "1,2,3");
        }

        /// <summary>
        /// Verifies that query conversions work.
        /// </summary>
        [Test]
        public void VerifyQueryConversions()
        {
            int[] sampleValues = { 1, 2, 3 };
            QueryCriterion sample = QueryCriterion.ValuesNamed("Sample", sampleValues);
            QueryCriterion test = QueryCriterion.From(sample.To<Test.Samples.QueryCriterion>());
            int[] testValues = test.GetValues<int>();
            Assert.IsTrue(testValues.Length == sampleValues.Length);
            Assert.IsTrue(test.FieldName == "Sample");
            Assert.IsTrue(test.FieldType == sampleValues[0].GetType().FullName);
            Assert.IsTrue(test.FieldValues == "1,2,3");
            Assert.IsTrue(test.ResultsOrder == QueryCriterion.SortOrder.None);
        }

        /// <summary>
        /// Verifies that invalid criterion value casting fails.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidCastException))]
        public void VerifyInvalidValueCastingFails()
        {
            QueryCriterion simple = QueryCriterion.Named("Simple", 5.0);
            DateTime[] values = simple.GetValues<DateTime>();
        }

        /// <summary>
        /// Verifies that invalid criterion sourcing fails.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidCastException))]
        public void VerifyInvalidSourcingFails()
        {
            QueryCriterion simple = QueryCriterion.From(new Object());
        }

    } // QueryTestFixture
}

namespace Test.Samples
{
    public class QueryCriterion
    {
        public String FieldName { get; set; }
        public String FieldType { get; set; }
        public String FieldValues { get; set; }
        public SortOrder ResultsOrder { get; set; }
        public Object ExtraInjection { get; set; }

        public enum SortOrder
        {
            None,
            Descending,
            Ascending

        } // SortOrder

    } // QueryCriterion

    public class NamedQuery
    {
        public String Name { get; set; }
        public QueryCriterion[] NamedFilters { get; set; }

    } // NamedQuery
}
