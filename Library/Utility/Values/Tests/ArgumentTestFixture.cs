using System;
using System.Text;
using System.Collections.Generic;
using NUnit.Framework;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// Verifies argument checking works as intended.
    /// </summary>
    [TestFixture]
    public class ArgumentTestFixture
    {
        /// <summary>
        /// Verifies that object tests work as intended.
        /// </summary>
        [Test]
        public void VerifyObjectArgumentTests()
        {
            Object missing = null;
            Object[] empty = { };
            Object[] elements = { this };

            Assert.IsTrue(Argument.IsAbsent(missing));
            Assert.IsTrue(Argument.IsEmpty(empty));
            Assert.IsTrue(Argument.IsPresent(elements));
        }

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test]
        public void VerifyStringArgumentTests()
        {
            String missing = null;
            String empty = String.Empty;
            String[] elements = { "test" };

            Assert.IsTrue(Argument.IsAbsent(missing));
            Assert.IsTrue(Argument.IsAbsent(empty));
            Assert.IsTrue(Argument.IsPresent(elements));
        }

        /// <summary>
        /// Verifies that a required but missing string fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MissingStringArgument()
        {
            String missing = null;
            Argument.Check("missing", missing);
        }

        /// <summary>
        /// Verifies that a required but empty string fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EmptyStringArgument()
        {
            String test = "test";
            Argument.Check("test", test);

            String empty = String.Empty;
            Argument.Check("empty", empty);
        }

        /// <summary>
        /// Verifies that a required but missing object fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MissingObjectArgument()
        {
            Object argument = this;
            Argument.Check("argument", argument);

            Object missing = null;
            Argument.Check("missing", missing);
        }

        /// <summary>
        /// Verifies that a required but missing array fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void MissingArrayArgument()
        {
            Object[] missing = null;
            Argument.CheckAny("missing", missing);
        }

        /// <summary>
        /// Verifies that a required but empty array fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void EmptyArrayArgument()
        {
            Object[] argument = { this };
            Argument.CheckAny("argument", argument);

            Object[] empty = { };
            Argument.CheckAny("empty", empty);
        }

        /// <summary>
        /// Verifies that an improper argument type fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CheckArgumentType()
        {
            int argument = 5;
            Argument.CheckType("argument", argument, typeof(int));
            Argument.CheckType("argument", argument, typeof(double));
        }

        /// <summary>
        /// Verifies that a value limit check fails.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ValueUnderLimit()
        {
            int value = 5;
            Argument.CheckLimit("value", value, "<", 6);
            Argument.CheckLimit("value", value, ">", 6);
        }

    } // ArgumentTestFixture
}
