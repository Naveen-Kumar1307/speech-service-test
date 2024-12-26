using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using Iesi.Collections.Generic;

using NUnit.Framework;
using NHibernate;
using NHibernate.Mapping.Attributes;
using NHMA = NHibernate.Mapping.Attributes;

using GlobalEnglish.Utility.Xml;
using GlobalEnglish.Utility.Values;

namespace GlobalEnglish.Utility.Hibernate
{
    [TestFixture]
    public class HibernateTestFixture
    {
        private static string SeedFile =
            "assembly://GlobalEnglish.Utility.Hibernate.Tests" +
                      "/GlobalEnglish.Utility.Hibernate/SeedIDs.txt";

        [TestFixtureSetUp]
        public void PrepareDatabase()
        {
            ConfigurationBuilder builder = ConfigurationBuilder.WithConfiguredModels();
            builder.CheckConnection("SampleDatabase");
            builder.CheckDialect("SQLite");
            builder.BuildDatabase();

            SeedIDs();
        }

        private void SeedIDs()
        {
            string script = ResourceFile.Named(SeedFile).GetResourceText();
            using (ISession session = Sample.StockRepository.OpenSession())
            {
                ISQLQuery command = session.CreateSQLQuery("drop table ids;");
                int result = command.ExecuteUpdate();

                command = session.CreateSQLQuery(script);
                result = command.ExecuteUpdate();
            }
        }

        /// <summary>
        /// Verifies that assigned IDs work.
        /// </summary>
        [Test]
        public void AssignedIDs()
        {
            SampleAssigned sample = new SampleAssigned();
            SampleAssigned.StockRepository.Save(sample);
            sample = SampleAssigned.StockRepository.Get(sample.Id);
            Assert.IsTrue(sample != null);
        }

        /// <summary>
        /// Verifies that concurrent ID assignments work.
        /// </summary>
        [Test]
        public void ConcurrentAssignments()
        {
            int initialCount = SampleAssigned.StockRepository.CountAll();

            int count = 20;
            Thread[] threads = new Thread[count];
            for (int index = 0; index < count; index++)
            {
                threads[index] = GetTestThread();
                threads[index].Name = index.ToString();
                threads[index].Start();
            }

            while ((from thread in threads where thread.IsAlive select thread).Count() > 0)
            {
                Thread.Sleep(100);
            }

            IList<int> results = SampleAssigned.StockRepository.GetAllIds();
            Assert.IsTrue(results.Count == count + initialCount);
        }

        public Thread GetTestThread()
        {
            return new Thread(new ThreadStart(SaveAssignedID));
        }

        public void SaveAssignedID()
        {
            SampleAssigned sample = new SampleAssigned();
            SampleAssigned.StockRepository.Save(sample);
            //Console.WriteLine(Thread.CurrentThread.Name + " done");
        }

        /// <summary>
        /// Verify that basic CRUD works.
        /// </summary>
        [Test]
        public void SampleCRUD()
        {
            SampleAssigned assigned = new SampleAssigned();
            assigned.Name = "Simple Sample";
            SampleAssigned.StockRepository.Save(assigned);

            Child[] children = { Child.With(1), Child.With(2), Child.With(3) };
            Sample sample = Sample.With(children);
            sample.Name = "Sample";
            Sample.StockRepository.Save(sample);
            sample.AssignsManager.Add(assigned);

            sample = Sample.StockRepository.Get(sample.Id);
            Assert.IsTrue(sample != null);
            Assert.IsTrue(sample.ChildManager.CountElements() == children.Count());
            Assert.IsTrue(sample.AssignsManager.CountElements() > 0);

            sample.ChildManager.GetElements();
            sample.ChildManager.Remove(sample.Children.First());
            sample = Sample.StockRepository.Get(sample.Id);
            Assert.IsTrue(sample.ChildManager.CountElements() < children.Count());

            sample.Name = "Test Sample";
            Sample.StockRepository.Save(sample);
            sample = Sample.StockRepository.Get(sample.Id);
            Assert.IsTrue(sample != null);
            Assert.IsTrue(sample.Name == "Test Sample");

            Sample.StockRepository.Delete(sample);
            sample = Sample.StockRepository.Get(sample.Id);
            Assert.IsTrue(sample == null);
        }

    } // HibernateTestFixture


    [NHMA.Class(Table = "Sample", NameType = typeof(Sample))]
    public class Sample : NativeSurrogate, ItemRepository<Sample>.ISource
    {
        /// <inheritdoc/>
        public virtual ItemRepository<Sample> Repository
        {
            get { return StockRepository; }
        }

        /// <summary>
        /// A stock Sample repository.
        /// </summary>
        public static ItemRepository<Sample> StockRepository
        {
            get { return ItemRepository<Sample>.From(ref StandardRepository); }
        }

        private static ItemRepository<Sample> StandardRepository;


        /// <summary>
        /// A sample name.
        /// </summary>
        /// <remarks>
        /// <h4>Hibernate Mapping:</h4>
        /// <list type="bullet">
        /// <item>NotNull = true, Name must be present</item>
        /// </list>
        /// </remarks>
        [NHMA.Property(Type = "AnsiString", Length = 50, NotNull = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// The sample children.
        /// </summary>
        /// <remarks>
        /// <h4>Hibernate Mapping:</h4>
        /// <list type="bullet">
        /// <item>Set = ManyToMany managed collection of <see cref="Child"/></item>
        /// <item>ForeignKey = FK_Sample_Child, ChildId = <see cref="NativeSurrogate.Id">Child.Id</see></item>
        /// <item>Key = FK_Child_Sample, SampleId = <see cref="NativeSurrogate.Id">Sample.Id</see></item>
        /// <item>Lazy = true, related children will be loaded as needed with each Sample</item>
        /// <item>Cascade = all-delete-orphan, all related children will be deleted with each Sample</item>
        /// </list>
        /// </remarks>
        [NHMA.Set(Table = "SampleChild", BatchSize = 10, Generic = true, 
            Lazy = NHMA.CollectionLazy.True, Cascade = "all-delete-orphan")]
        [NHMA.Key(1, Column = "SampleId", ForeignKey = "FK_Child_Sample")]
        [NHMA.ManyToMany(2, Column = "ChildId", 
            ClassType = typeof(Child), ForeignKey = "FK_Sample_Child")]
        public virtual ISet<Child> Children { get; protected set; }
        private LazySet<Sample, Child> SampleCache;
        public virtual LazySet<Sample, Child> ChildManager
        {
            get { return LazySet<Sample, Child>.From(ref SampleCache, Children, this); }
        }

        [NHMA.List(Table = "AssignedChild", BatchSize = 10, Generic = true,
            Lazy = NHMA.CollectionLazy.True, Cascade = "all-delete-orphan")]
        [NHMA.Key(1, Column = "SampleId", ForeignKey = "FK_Assigned_Sample")]
        [NHMA.Index(2, Column = "AssignedOrder")]
        [NHMA.ManyToMany(3, Column = "AssignedId",
            ClassType = typeof(SampleAssigned), ForeignKey = "FK_Sample_Assigned")]
        public virtual IList<SampleAssigned> Assigns { get; protected set; }
        private LazyList<Sample, SampleAssigned> AssignedCache;
        public virtual LazyList<Sample, SampleAssigned> AssignsManager
        {
            get { return LazyList<Sample, SampleAssigned>.From(ref AssignedCache, Assigns, this); }
        }


        /// <summary>
        /// Returns a new Sample with children.
        /// </summary>
        /// <param name="children">some children</param>
        /// <returns>a new Sample</returns>
        public static Sample With(ICollection<Child> children)
        {
            Sample result = new Sample();
            result.Children.AddAll(children);
            return result;
        }

        /// <summary>
        /// Constructs a new Sample.
        /// </summary>
        public Sample()
        {
            Name = "";
            Children = new HashedSet<Child>();
            Assigns = new List<SampleAssigned>();
        }

        public virtual int AssignedHash
        {
            get
            {
                return Assigns == null ? 0 : Assigns.GetHashCode();
            }
        }

    } // Sample

    [NHMA.Class(Table = "Child", NameType = typeof(Child))]
    public class Child : NativeSurrogate
    {
        /// <summary>
        /// A count (of something).
        /// </summary>
        /// <remarks>
        /// <h4>Hibernate Mapping:</h4>
        /// <list type="bullet">
        /// <item>NotNull = true, Count must be present</item>
        /// </list>
        /// </remarks>
        [NHMA.Property(NotNull = true)]
        public virtual int Count { get; set; }


        /// <summary>
        /// Returns a new Child.
        /// </summary>
        /// <param name="count">a count</param>
        /// <returns>a new Child</returns>
        public static Child With(int count)
        {
            Child result = new Child();
            result.Count = count;
            return result;
        }

        /// <summary>
        /// Constructs a new Child.
        /// </summary>
        public Child()
        {
            Count = 0;
        }

    } // Child


    [NHMA.Class(Table = "SampleAssigned", NameType = typeof(SampleAssigned))]
    public class SampleAssigned : ISurrogated
    {
        /// <summary>
        /// A stock Sample repository.
        /// </summary>
        public static ItemRepository<SampleAssigned> StockRepository
        {
            get { return ItemRepository<SampleAssigned>.From(ref StandardRepository); }
        }

        private static ItemRepository<SampleAssigned> StandardRepository;

        /// <summary>
        /// The persistent object ID.
        /// </summary>
        /// <remarks>
        /// <h4>Hibernate Mapping:</h4>
        /// <list type="bullet">
        /// <item>Id = generated automatically from a value in a database table</item>
        /// </list>
        /// </remarks>
        [NHMA.Id(Name = "Id", Column = "Id")]
        [NHMA.Generator(1, Class = "hilo")]
        [NHMA.Param(2, Name = "table", Content = "ids")]
        [NHMA.Param(2, Name = "column", Content = "table_id")]
        [NHMA.Param(2, Name = "max_lo", Content = "0")]
        [NHMA.Param(2, Name = "where", Content = "table_name = 'SampleAssigned'")]
        public virtual int Id { get; protected set; }

        /// <summary>
        /// A sample name.
        /// </summary>
        /// <remarks>
        /// <h4>Hibernate Mapping:</h4>
        /// <list type="bullet">
        /// <item>NotNull = true, Name must be present</item>
        /// </list>
        /// </remarks>
        [NHMA.Property(Type = "AnsiString", Length = 50, NotNull = true)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Constructs a new SampleAssigned.
        /// </summary>
        public SampleAssigned()
        {
            Name = "SampleAssigned";
        }

    } // SampleAssigned
}
