using System;
using System.Text;
using System.Collections.Generic;

using NUnit.Framework;
using Spring.Core;
using GlobalEnglish.Utility.Parameters;
using Spring.Context;

namespace GlobalEnglish.Utility.Spring
{
    /// <summary>
    /// Verifies that SpringContext works as intended.
    /// </summary>
    [TestFixture]
    public class SpringContextTestFixture
    {
        /// <summary>
        /// Verifies that loading a context from a file in the file system works.
        /// </summary>
        [Test]
        public void ContextLoadedFromFile()
        {
            NamedValue test = NamedValue.With("Sample", 5);
            NamedValue sample = SpringContext.GetConfigured<NamedValue>("SampleValue");
            Assert.IsTrue(sample != null);
            Assert.IsTrue(sample.Name == test.Name);
            Assert.IsTrue(sample.Get<int>() == test.Get<int>());
        }

        /// <summary>
        /// Verifies that loading a context from an embedded resource works.
        /// </summary>
        [Test]
        public void ContextLoadedFromEmbeddedResource()
        {
            NamedValue test = NamedValue.With("Sample", 5);
            SpringContext context = SpringContext.From(GetType(), "EmbeddedContext.xml");
            NamedValue sample = context.Get<NamedValue>("SampleValue");
            Assert.IsTrue(sample != null);
            Assert.IsTrue(sample.Name == test.Name);
            Assert.IsTrue(sample.Get<int>() == test.Get<int>());
        }

        /// <summary>
        /// Verifies that embedded contexts get cached and reused.
        /// </summary>
        [Test]
        public void EmbeddedContextCaches()
        {
            SpringContext context = SpringContext.From(GetType(), "EmbeddedContext.xml");
            SpringContext cached = SpringContext.From(GetType(), "EmbeddedContext.xml");

            // context hashes are identical (same object)
            int contextHash = context.Context.GetHashCode();
            int cachedHash = cached.Context.GetHashCode();
            Assert.IsTrue(contextHash == cachedHash);
            Assert.IsTrue(context.Context == cached.Context);

            // accessed singletons are identical (same object)
            NamedValue sample = context.Get<NamedValue>("SampleValue");
            NamedValue test = cached.Get<NamedValue>("SampleValue");
            Assert.IsTrue(sample.GetHashCode() == test.GetHashCode());
            Assert.IsTrue(sample == test);
        }

        /// <summary>
        /// Verifies that file contexts are not cached.
        /// </summary>
        [Test]
        public void FileContextReloadsOnAccess()
        {
            IApplicationContext context = SpringContext.GetContext();
            IApplicationContext another = SpringContext.GetContext();

            // loaded file contexts are different objects
            int contextHash = context.GetHashCode();
            int anotherHash = another.GetHashCode();
            Assert.IsTrue(contextHash != anotherHash);

            // SO, accessed singletons are different objects!!
            NamedValue sample = SpringContext.GetConfigured<NamedValue>("SampleValue");
            NamedValue test = SpringContext.GetConfigured<NamedValue>("SampleValue");
            Assert.IsTrue(sample.GetHashCode() != test.GetHashCode());
            Assert.IsTrue(sample != test);
        }

    } // SpringContextTestFixture
}
