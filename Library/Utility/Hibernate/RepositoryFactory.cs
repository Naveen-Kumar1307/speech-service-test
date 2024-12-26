using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;

using NHibernate;
using Environment = NHibernate.Cfg.Environment;

using Common.Logging;
using Spring.Transaction;
using Spring.Data.Common;
using Spring.Data.NHibernate;
using Spring.Data.NHibernate.Support;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Hibernate
{
    /// <summary>
    /// Creates specific kinds of repository.
    /// </summary>
    public class RepositoryFactory
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(RepositoryFactory));
        private static RepositoryFactory SoleInstance = null;

        private ConfigurationBuilder builder;
        private ISessionFactory sessionFactory;

        #region creating instances
        /// <summary>
        /// The sole instance of this class.
        /// </summary>
        public static RepositoryFactory Instance
        {
            get { return GetInstance(); }
        }

        /// <summary>
        /// Returns the sole instance of this class.
        /// </summary>
        /// <returns>the sole instance of this class</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static RepositoryFactory GetInstance()
        {
            if (SoleInstance == null)
                SoleInstance = new RepositoryFactory();

            return SoleInstance;
        }

        /// <summary>
        /// Constructs a new RepositoryFactory.
        /// </summary>
        private RepositoryFactory()
        {
            this.builder = ConfigurationBuilder.With(
                            ConfigurationBuilder.GetConfiguredModelNames(String.Empty));

            Logger.Info(GetType().Name + " was created!!");
        }
        #endregion

        #region creating repositories
        /// <summary>
        /// Builds a database with a schema defined by the supplied configuration.
        /// </summary>
        public void BuildDatabase()
        {
            ConfigurationBuilder.BuildDatabase();
        }

        /// <summary>
        /// Builds a database with a schema defined by the supplied configuration.
        /// </summary>
        /// <param name="connectionString">a required database connection</param>
        public void BuildDatabase(String connectionString)
        {
            Argument.Check("connectionString", connectionString);
            DatabaseConnection = connectionString;
            ConfigurationBuilder.BuildDatabase();
        }

        /// <summary>
        /// Returns a new NativeRepository.
        /// </summary>
        /// <typeparam name="ItemType">an item type</typeparam>
        /// <returns>a new NativeRepository</returns>
        public ItemRepository<ItemType> CreateRepository<ItemType>()
            where ItemType : class, ISurrogated
        {
            return ItemRepository<ItemType>.With(GetSessionFactory());
        }

        /// <summary>
        /// Returns a new NativeRepository.
        /// </summary>
        /// <typeparam name="ItemType">an item type</typeparam>
        /// <param name="valuePrefix">a configured value prefix</param>
        /// <returns>a new NativeRepository</returns>
        public ItemRepository<ItemType> CreateRepository<ItemType>(String valuePrefix)
            where ItemType : class, ISurrogated
        {
            if (valuePrefix.Length == 0) return CreateRepository<ItemType>();
            return ItemRepository<ItemType>.With(GetSessionFactory(valuePrefix));
        }
        #endregion

        #region configuring resources
        /// <summary>
        /// A session factory.
        /// </summary>
        private ISessionFactory GetSessionFactory(String valuePrefix)
        {
            String[] modelNames = ConfigurationBuilder.GetConfiguredModelNames(valuePrefix);
            return ConfigurationBuilder.With(modelNames).BuildSessionFactory(valuePrefix);
        }

        /// <summary>
        /// The (standard) session factory.
        /// </summary>
        private ISessionFactory GetSessionFactory()
        {
            if (this.sessionFactory == null)
                this.sessionFactory = ConfigurationBuilder.ConfiguredSessionFactory;

            return this.sessionFactory;
        }

        /// <summary>
        /// The database connection.
        /// </summary>
        private String DatabaseConnection
        {
            set
            {
                ConfigurationBuilder.SessionConfiguration
                    .SetProperty(Environment.ConnectionString, value);
            }
        }

        /// <summary>
        /// Returns the configured transaction manager.
        /// </summary>
        public IPlatformTransactionManager GetTransactionManager()
        {
            IDbProvider provider = ConfigurationBuilder.DatabaseProvider;
            if (provider == null) return null;

            HibernateTransactionManager result = new HibernateTransactionManager();
            result.SessionFactory = GetSessionFactory();
            result.DbProvider = provider;
            return result;
        }

        /// <summary>
        /// The configuration builder.
        /// </summary>
        public ConfigurationBuilder ConfigurationBuilder
        {
            get
            {
                if (this.builder == null)
                    throw new InvalidOperationException(
                        "Missing NHibernate configuration - see ConfigurationBuilder");

                return this.builder;
            }
        }
        #endregion

    } // RepositoryFactory
}
