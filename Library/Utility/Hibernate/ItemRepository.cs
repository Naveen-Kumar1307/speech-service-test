using System;
using System.Text;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Common.Logging;
using Iesi.Collections.Generic;

using NHibernate;
using NHibernate.Linq;
using Spring.Data.NHibernate;
using SDNG = Spring.Data.NHibernate.Generic;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Hibernate
{
    /// <summary>
    /// Defines protocol for counting items using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>a count, or zero</returns>
    public delegate int
        CountItems<ItemType>(IQueryable<ItemType> query) where ItemType : ISurrogated;

    /// <summary>
    /// Defines protocol for selecting items using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>some items, or empty</returns>
    public delegate IList<ItemType> 
        FindItems<ItemType>(IQueryable<ItemType> query) where ItemType : ISurrogated;

    /// <summary>
    /// Defines protocol for selecting item IDs using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>some items, or empty</returns>
    public delegate IList<int>
        FindItemIds<ItemType>(IQueryable<ItemType> query) where ItemType : ISurrogated;

    /// <summary>
    /// Defines protocol for selecting item parts using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <typeparam name="ResultType">a result type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>some items, or empty</returns>
    public delegate IList<ResultType>
        FindItemParts<ItemType, ResultType>(IQueryable<ItemType> query) where ItemType : ISurrogated;

    /// <summary>
    /// Defines protocol for selecting results using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>some results, or empty</returns>
    public delegate IList
        FindResults<ItemType>(IQueryable<ItemType> query) where ItemType : ISurrogated;

    /// <summary>
    /// Defines protocol for selecting a single ID using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>a single ID, or zero</returns>
    public delegate int
        FindSingleId<ItemType>(IQueryable<ItemType> query) where ItemType : class, ISurrogated;

    /// <summary>
    /// Defines protocol for selecting a single result using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>a single result, or null</returns>
    public delegate ItemType
        FindSingleItem<ItemType>(IQueryable<ItemType> query) where ItemType : class, ISurrogated;

    /// <summary>
    /// Defines protocol for selecting a single item part using a base query.
    /// </summary>
    /// <typeparam name="ItemType">a surrogate item type</typeparam>
    /// <typeparam name="ResultType">a result type</typeparam>
    /// <param name="query">a surrogate item query</param>
    /// <returns>a single result, or null</returns>
    public delegate ResultType
        FindSinglePart<ItemType, ResultType>(IQueryable<ItemType> query) 
                 where ItemType : ISurrogated;

    /// <summary>
    /// A repository of (surrogated) items.
    /// </summary>
    /// <typeparam name="ItemType">a kind of (surrogated) item</typeparam>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>maintains persistent items in a backing store</item>
    /// <item>provides access to items obtained from a backing store</item>
    /// <item>creates a cached instance as needed</item>
    /// </list>
    /// </remarks>
    public class ItemRepository<ItemType> where ItemType : class, ISurrogated
    {
        /// <summary>
        /// Defines a repository source.
        /// </summary>
        public interface ISource
        {
            /// <summary>
            /// An item repository.
            /// </summary>
            ItemRepository<ItemType> Repository { get; }

        } // ISource

        /// <summary>
        /// A repository logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ItemRepository<ItemType>));
        private static readonly List<ItemType> EmptyList = new List<ItemType>();
        private static readonly List<int> EmptyIDs = new List<int>();

        /// <summary>
        /// A repository session factory.
        /// </summary>
        public ISessionFactory SessionFactory { get; private set; }

        /// <summary>
        /// The logging level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a cached repository.
        /// </summary>
        /// <param name="cache">a cache reference</param>
        /// <returns>a cached repository</returns>
        /// <remarks>Initializes the cache if needed.</remarks>
        public static ItemRepository<ItemType> From(ref ItemRepository<ItemType> cache)
        {
            return From(ref cache, String.Empty);
        }

        /// <summary> 
        /// Returns a cached repository.
        /// </summary>
        /// <param name="cache">a cache reference</param>
        /// <param name="valuePrefix">a configured value name prefix</param>
        /// <returns>a cached repository</returns>
        /// <remarks>Initializes the cache if needed.</remarks>
        public static ItemRepository<ItemType> From(
            ref ItemRepository<ItemType> cache, String valuePrefix)
        {
            Type itemType = typeof(ItemType);
            lock (itemType)
            {
                if (cache == null)
                    cache = RepositoryFactory.Instance
                                .CreateRepository<ItemType>(valuePrefix);

                return cache;
            }
        }

        /// <summary>
        /// Returns a new NativeRepository.
        /// </summary>
        /// <param name="sessionFactory">a session factory</param>
        /// <returns>a new NativeRepository</returns>
        internal static ItemRepository<ItemType> With(ISessionFactory sessionFactory)
        {
            ItemRepository<ItemType> result = new ItemRepository<ItemType>();
            result.SessionFactory = sessionFactory;
            return result;
        }

        /// <summary>
        /// Constructs a new NativeRepository.
        /// </summary>
        private ItemRepository()
        {
            //Query.Registry.Register(typeof(ItemType));
            LogLevel = LogLevel.Warn;
        }
        #endregion

        #region logging messages
        /// <summary>
        /// Returns a logger.
        /// </summary>
        public ILog GetLogger()
        {
            return Logger;
        }

        /// <summary>
        /// Establishes the logging level.
        /// </summary>
        /// <param name="logLevel">a log level</param>
        /// <returns></returns>
        public ItemRepository<ItemType> With(LogLevel logLevel)
        {
            LogLevel = logLevel;
            return this;
        }
        #endregion

        #region counting items
        /// <summary>
        /// Counts the number of items in the backing store.
        /// </summary>
        /// <returns>a count of all the stored items</returns>
        public int CountAll()
        {
            return Execute<int>((ISession session) => GetItemQuery(session).Count());
        }

        /// <summary>
        /// Counts the number of items that match the selection criteria of a given query. 
        /// </summary>
        /// <param name="Count">a count query</param>
        /// <returns>a count of the selected items, or zero</returns>
        public int CountWith(CountItems<ItemType> Count)
        {
            return CountWith<ItemType>(Count);
        }

        /// <summary>
        /// Counts the number of items that match the selection criteria of a given query. 
        /// </summary>
        /// <param name="Count">a count query</param>
        /// <returns>a count of the selected items, or zero</returns>
        public int CountWith<EntityType>(CountItems<EntityType> Count)
            where EntityType : ItemType
        {
            if (Count == null) return 0;
            return Execute<int>((ISession session) => Count(GetItemQuery<EntityType>(session)));
        }

        /// <summary>
        /// Counts the number of items that would be returned by Find.
        /// </summary>
        /// <param name="query">a required query</param>
        /// <returns>a result count</returns>
        //public int Count(Query query)
        //{
        //    if (query == null) return 0;
        //    return query.CountResults<ItemType>(GetGenericTemplate());
        //}
        #endregion

        #region loading item IDs
        /// <summary>
        /// Returns the Ids of all the stored items.
        /// </summary>
        /// <returns>the Ids of all the stored items</returns>
        public IList<int> GetAllIds()
        {
            return FindIdsWith(
                    (IQueryable<ItemType> items) => (from item in items select item.Id).ToList());
        }

        /// <summary>
        /// Finds the ID of a single item that matches the selection criteria of a given query.
        /// </summary>
        /// <param name="Find">a query delegate</param>
        /// <returns>the selected item ID, or zero</returns>
        public int FindOneIdWith(FindSingleId<ItemType> Find)
        {
            if (Find == null) return 0;
            return Execute<int>((ISession session) => Find(GetItemQuery<ItemType>(session)));
        }

        /// <summary>
        /// Finds the IDs of all items that match the selection criteria of a given query.
        /// </summary>
        /// <typeparam name="EntityType">a derived entity type</typeparam>
        /// <param name="Find">a query delegate</param>
        /// <returns>the selected items, or empty</returns>
        public IList<int> 
            FindIdsWith<EntityType>(FindItemIds<EntityType> Find) where EntityType : ItemType
        {
            if (Find == null) return EmptyIDs;
            return ExecuteFind<int>((ISession session) => Find(GetItemQuery<EntityType>(session)));
        }

        /// <summary>
        /// Finds a single ID that matches the given selection criteria.
        /// </summary>
        /// <param name="query">a required query</param>
        /// <returns>a single ID, or default value</returns>
        //public int FindOneId(Query query)
        //{
        //    IList<int> results = FindIds(query);
        //    return (results.Count == 0 ? 0 : results[0]);
        //}

        /// <summary>
        /// Finds the Ids of all the items that match the given selection criteria.
        /// </summary>
        /// <param name="query">a required query</param>
        /// <returns>the selected item Ids</returns>
        //public IList<int> FindIds(Query query)
        //{
        //    if (query == null) return new List<int>();
        //    return query.FindIds<ItemType, int>(GetGenericTemplate());
        //}
        #endregion

        #region loading items
        /// <summary>
        /// Returns a single item with a given ID.
        /// </summary>
        /// <param name="itemID">an item ID</param>
        /// <returns>an item, or null</returns>
        public ItemType Get(int itemID)
        {
            if (itemID < 1) return null;
            return FindOneWith((IQueryable<ItemType> items) =>
                        (from item in items where item.Id == itemID select item).FirstOrDefault());
        }

        /// <summary>
        /// Returns all the stored items.
        /// </summary>
        /// <returns>all the stored items</returns>
        public IList<ItemType> GetAll()
        {
            return GetGenericTemplate().LoadAll<ItemType>();
        }

        /// <summary>
        /// Returns the identified items.
        /// </summary>
        /// <param name="itemIDs">the item IDs</param>
        /// <returns>the identified items, or empty</returns>
        public IList<ItemType> GetAll(ICollection<int> itemIDs)
        {
            if (Argument.IsEmpty(itemIDs)) return EmptyList;

            IList<int> ids = itemIDs.Distinct().ToList();
            return FindWith<ItemType>((IQueryable<ItemType> items) =>
                        (from item in items where ids.Contains(item.Id) select item).ToList());
        }

        /// <summary>
        /// Finds a single item that matches the selection criteria of a given query.
        /// </summary>
        /// <param name="Find">a query delegate</param>
        /// <returns>an item, or null</returns>
        public ItemType FindOneWith(FindSingleItem<ItemType> Find)
        {
            if (Find == null) return null;
            return Execute<ItemType>((ISession session) => Find(GetItemQuery<ItemType>(session)));
        }

        /// <summary>
        /// Finds all items that match the selection criteria of a given query.
        /// </summary>
        /// <param name="Find">a query delegate</param>
        /// <returns>the selected items, or empty</returns>
        public IList<ItemType> FindWith(FindItems<ItemType> Find)
        {
            return FindWith<ItemType>(Find);
        }

        /// <summary>
        /// Finds all items that match the selection criteria of a given query.
        /// </summary>
        /// <typeparam name="EntityType">a derived entity type</typeparam>
        /// <param name="Find">a query delegate</param>
        /// <returns>the selected items, or empty</returns>
        public IList<EntityType> FindWith<EntityType>(FindItems<EntityType> Find)
            where EntityType : ItemType
        {
            if (Find == null) return new List<EntityType>();
            return ExecuteFind<EntityType>(
                        (ISession session) => Find(GetItemQuery<EntityType>(session)));
        }

        /// <summary>
        /// Finds a part of all items that match the selection criteria of a given query.
        /// </summary>
        /// <typeparam name="EntityType">a derived entity type</typeparam>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="Find">a query delegate</param>
        /// <returns>the selected items, or empty</returns>
        public IList<ResultType> FindPartsWith<EntityType, ResultType>(
            FindItemParts<EntityType, ResultType> Find)
            where EntityType : ItemType
        {
            if (Find == null) return new List<ResultType>();
            return ExecuteFind<ResultType>(
                        (ISession session) => Find(GetItemQuery<EntityType>(session)));
        }

        /// <summary>
        /// Finds a single item that matches the supplied selection criteria.
        /// </summary>
        /// <param name="query">a required query</param>
        /// <returns>a single item, or null</returns>
        //public ItemType FindOne(Query query)
        //{
        //    return FindOne<ItemType>(query);
        //}

        /// <summary>
        /// Finds a single item that matches the supplied selection criteria.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="query">a required query</param>
        /// <returns>a single item, or null</returns>
        //public ResultType FindOne<ResultType>(Query query) where ResultType : class
        //{
        //    IList<ResultType> results = Find<ResultType>(query);
        //    return (results.Count == 0 ? null : results[0]);
        //}

        /// <summary>
        /// Finds all the items that match the supplied selection criteria.
        /// </summary>
        /// <param name="query">a required query</param>
        /// <returns>the selected items</returns>
        //public IList<ItemType> Find(Query query)
        //{
        //    return Find<ItemType>(query);
        //}

        /// <summary>
        /// Finds all the results that match the supplied selection criteria.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="query">a required query</param>
        /// <returns>the selected results</returns>
        //public IList<ResultType> Find<ResultType>(Query query) where ResultType : class
        //{
        //    if (query == null) return new List<ResultType>();
        //    return query.Find<ResultType>(GetGenericTemplate());
        //}

        /// <summary>
        /// Finds all the results that match the selection criteria of a given query.
        /// </summary>
        /// <param name="Find">a query</param>
        /// <returns>the selected results</returns>
        public IList FindResultsWith(FindResults<ItemType> Find)
        {
            if (Find == null) return new ArrayList();
            return Execute<IList>((ISession session) => Find(GetItemQuery<ItemType>(session)));
        }

        /// <summary>
        /// Finds a single result that matches the selection criteria of a given query.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="Find">a single item part query</param>
        /// <returns>the selected result, or null</returns>
        public ResultType FindOnePartWith<ResultType>(
            FindSinglePart<ItemType, ResultType> Find)
        {
            if (Find == null) return default(ResultType);
            return Execute<ResultType>(
                        (ISession session) => Find(GetItemQuery<ItemType>(session)));
        }

        /// <summary>
        /// Finds all the results that match the selection criteria of a given query.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="Find">a query</param>
        /// <returns>the selected results</returns>
        public IList<ResultType> FindPartsWith<ResultType>(
            FindItemParts<ItemType, ResultType> Find)
        {
            if (Find == null) return new List<ResultType>();
            return ExecuteFind<ResultType>(
                        (ISession session) => Find(GetItemQuery<ItemType>(session)));
        }
        #endregion

        #region loading parts
        /// <summary>
        /// Refreshes a stale item.
        /// </summary>
        /// <param name="item">a stale item</param>
        public void Refresh(ISurrogated item)
        {
            ISurrogated[] items = { item };
            RefreshAll(items);
        }

        /// <summary>
        /// Refreshes stale items.
        /// </summary>
        /// <param name="items">stale items</param>
        public void RefreshAll(IList<ISurrogated> items)
        {
            if (items == null || items.Count == 0) return;

            Execute((ISession session) =>
            {
                foreach (ISurrogated item in items)
                    if (NativeSurrogate.Saved(item)) session.Refresh(item);

                return true;
            });
        }
        #endregion

        #region maintaining lazy sets
        /// <summary>
        /// Fills a set that was lazy loaded.
        /// </summary>
        /// <typeparam name="ElementType">a set element type</typeparam>
        /// <param name="set">a lazy set</param>
        /// <param name="owner">a lazy set owner</param>
        /// <returns>a loaded set</returns>
        public ISet<ElementType> FillSet<ElementType>(ISet<ElementType> set, ItemType owner)
        {
            if (set == null || owner == null) return set;
            if (!NHibernateUtil.IsInitialized(set))
            {
                Execute((ISession session) =>
                {
                    session.Lock(owner, LockMode.None);
                    NHibernateUtil.Initialize(set);
                    return true;
                });
            }
            return set;
        }

        /// <summary>
        /// Returns the size of a lazy set.
        /// </summary>
        /// <typeparam name="ElementType">an element type</typeparam>
        /// <param name="set">a lazy set</param>
        /// <param name="owner">a lazy set owner</param>
        /// <returns>a count of the persisted set elements</returns>
        public int GetSize<ElementType>(ISet<ElementType> set, ItemType owner)
        {
            if (set == null || owner == null) return 0;
            return Execute<int>((ISession session) =>
            {
                session.Lock(owner, LockMode.None);
                object result = session.CreateFilter(set, "select count(*)").List()[0];
                return int.Parse(result.ToString());
            });
        }

        /// <summary>
        /// Adds an element to a lazy set.
        /// </summary>
        /// <typeparam name="ElementType">an element type</typeparam>
        /// <param name="element">an element</param>
        /// <param name="set">a lazy set</param>
        /// <param name="owner">a lazy set owner</param>
        public void AddElement<ElementType>(
            ElementType element, ISet<ElementType> set, ItemType owner)
            where ElementType : ISurrogated
        {
            if (set == null || owner == null) return;
            if (!NativeSurrogate.Saved(element)) return;
            Execute((ISession session) =>
            {
                session.Lock(owner, LockMode.None);
                if (!NHibernateUtil.IsInitialized(set))
                {
                    NHibernateUtil.Initialize(set);
                }

                ElementType found = Find(element, set);
                if (found != null) return owner;

                set.Add(element);
                return owner;
            });
        }

        /// <summary>
        /// Removes an element from a lazy set.
        /// </summary>
        /// <typeparam name="ElementType">an element type</typeparam>
        /// <param name="element">an element</param>
        /// <param name="set">a lazy set</param>
        /// <param name="owner">a lazy set owner</param>
        public void RemoveElement<ElementType>(
            ElementType element, ISet<ElementType> set, ItemType owner)
            where ElementType : ISurrogated
        {
            if (set == null || owner == null) return;
            if (!NativeSurrogate.Saved(element)) return;
            Execute((ISession session) =>
            {
                session.Lock(owner, LockMode.None);
                if (!NHibernateUtil.IsInitialized(set))
                {
                    NHibernateUtil.Initialize(set);
                }

                ElementType found = Find(element, set);
                if (found == null) return owner;

                set.Remove(found);
                return owner;
            });
        }

        /// <summary>
        /// Finds a set element.
        /// </summary>
        private ElementType Find<ElementType>(ElementType element, ISet<ElementType> set)
            where ElementType : ISurrogated
        {
            return (from item in set where item.Id == element.Id select item).FirstOrDefault();
        }
        #endregion

        #region maintaining lazy lists
        /// <summary>
        /// Fills a list that was lazy loaded.
        /// </summary>
        /// <typeparam name="ElementType">a list element type</typeparam>
        /// <param name="list">a lazy list</param>
        /// <param name="owner">a lazy list owner</param>
        /// <returns>a loaded list</returns>
        public IList<ElementType> FillList<ElementType>(IList<ElementType> list, ItemType owner)
        {
            if (list == null || owner == null) return list;
            if (!NHibernateUtil.IsInitialized(list))
            {
                Execute((ISession session) =>
                {
                    session.Lock(owner, LockMode.None);
                    NHibernateUtil.Initialize(list);
                    return true;
                });
            }
            return list;
        }

        /// <summary>
        /// Returns the size of a lazy list.
        /// </summary>
        /// <typeparam name="ElementType">an element type</typeparam>
        /// <param name="list">a lazy list</param>
        /// <param name="owner">a lazy list owner</param>
        /// <returns>a count of the persisted list elements</returns>
        public int GetSize<ElementType>(IList<ElementType> list, ItemType owner)
        {
            if (list == null || owner == null) return 0;
            return Execute<int>((ISession session) =>
            {
                session.Lock(owner, LockMode.None);
                object result = session.CreateFilter(list, "select count(*)").List()[0];
                return int.Parse(result.ToString());
            });
        }

        /// <summary>
        /// Adds an element to a lazy list.
        /// </summary>
        /// <typeparam name="ElementType">an element type</typeparam>
        /// <param name="element">an element</param>
        /// <param name="list">a lazy list</param>
        /// <param name="owner">a lazy list owner</param>
        public void AddElement<ElementType>(
            ElementType element, IList<ElementType> list, ItemType owner)
            where ElementType : ISurrogated
        {
            if (list == null || owner == null) return;
            if (!NativeSurrogate.Saved(element)) return;
            Execute((ISession session) =>
            {
                session.Lock(owner, LockMode.None);
                if (!NHibernateUtil.IsInitialized(list))
                {
                    NHibernateUtil.Initialize(list);
                }

                ElementType found = Find(element, list);
                if (found != null) return owner;

                list.Add(element);
                return owner;
            });
        }

        /// <summary>
        /// Removes an element from a lazy list.
        /// </summary>
        /// <typeparam name="ElementType">an element type</typeparam>
        /// <param name="element">an element</param>
        /// <param name="list">a lazy list</param>
        /// <param name="owner">a lazy list owner</param>
        public void RemoveElement<ElementType>(
            ElementType element, IList<ElementType> list, ItemType owner)
            where ElementType : ISurrogated
        {
            if (list == null || owner == null) return;
            if (!NativeSurrogate.Saved(element)) return;
            Execute((ISession session) =>
            {
                session.Lock(owner, LockMode.None);
                if (!NHibernateUtil.IsInitialized(list))
                {
                    NHibernateUtil.Initialize(list);
                }

                ElementType found = Find(element, list);
                if (found == null) return owner;

                list.Remove(found);
                return owner;
            });
        }

        /// <summary>
        /// Finds a list element.
        /// </summary>
        private ElementType Find<ElementType>(ElementType element, IList<ElementType> set)
            where ElementType : ISurrogated
        {
            return (from item in set where item.Id == element.Id select item).FirstOrDefault();
        }
        #endregion

        #region saving items
        /// <summary>
        /// Saves the supplied item.
        /// </summary>
        /// <param name="item">a required item</param>
        public void Save(ItemType item)
        {
            if (item == null) return;
            ItemType[] items = { item };
            Save(items);
        }

        /// <summary>
        /// Saves the supplied items.
        /// </summary>
        /// <param name="items">some required items</param>
        public void Save(ICollection<ItemType> items)
        {
            if (items == null || items.Count == 0) return;
            ISurrogated[] staleComponents = GetStaleComponents(items);

            try
            {
                GetTemplate().SaveOrUpdateAll(items.ToArray());
            }
            catch (Exception ex)
            {
                Type itemType = typeof(ItemType);
                if (LogLevel > LogLevel.Info)
                {
                    Logger.Warn(itemType.Name + ".Save() failed!", ex);
                }
                else
                {
                    Logger.Debug(itemType.Name + ".Save() failed!", ex);
                }
                throw ex;
            }

            RefreshAll(staleComponents);
        }

        /// <summary>
        /// Obtains any stale components from the supplied items.
        /// </summary>
        /// <param name="items">some potential composites</param>
        /// <returns>any stale item components</returns>
        private ISurrogated[] GetStaleComponents(ICollection<ItemType> items)
        {
            return (from item in items
                    let composite = item as ISurrogateComposite
                    where composite != null
                    from component in composite.StaleComponents
                    select component).ToArray();
        }
        #endregion

        #region deleting items
        /// <summary>
        /// Deletes the supplied item from the backing store.
        /// </summary>
        /// <param name="item">an item</param>
        public void Delete(ItemType item)
        {
            if (!NativeSurrogate.Saved(item)) return;
            ItemType[] items = { item };
            Delete(items);
        }

        /// <summary>
        /// Deletes the supplied items from the backing store.
        /// </summary>
        /// <param name="items">some items</param>
        public void Delete(ICollection<ItemType> items)
        {
            if (!NativeSurrogate.Saved(items)) return;
            List<ItemType> list = new List<ItemType>(items);
            GetTemplate().DeleteAll(list);
        }

        /// <summary>
        /// Deletes the identified items from the backing store.
        /// </summary>
        /// <param name="itemIDs">some item IDs</param>
        public void Delete(ICollection<int> itemIDs)
        {
            if (itemIDs == null || itemIDs.Count == 0) return;
            foreach (int itemID in itemIDs.Distinct().ToList()) Delete(Get(itemID));
        }
        #endregion

        #region building queries
        /// <summary>
        /// Builds an item query for a session.
        /// </summary>
        /// <param name="session">a session</param>
        /// <returns>an item query</returns>
        private IQueryable<ItemType> GetItemQuery(ISession session)
        {
            return GetItemQuery<ItemType>(session);
        }

        /// <summary>
        /// Builds an entity query for a session.
        /// </summary>
        /// <typeparam name="EntityType">a derived entity type</typeparam>
        /// <param name="session">a session</param>
        /// <returns>an entity query</returns>
        private IQueryable<EntityType> 
            GetItemQuery<EntityType>(ISession session) where EntityType : ItemType
        {
            return (from item in session.Query<EntityType>() select item);
        }
        #endregion

        #region executing queries
        /// <summary>
        /// Returns the result of executing a query.
        /// </summary>
        /// <param name="query">a query</param>
        /// <returns>a query result</returns>
        private Object Execute(HibernateDelegate query)
        {
            return GetTemplate().Execute(query);
        }

        /// <summary>
        /// Returns the result of executing a query.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="query">a query</param>
        /// <returns>a query result</returns>
        private ResultType Execute<ResultType>(SDNG.HibernateDelegate<ResultType> query)
        {
            return GetGenericTemplate().Execute<ResultType>(query);
        }

        /// <summary>
        /// Returns the results from executing a query.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="query">a query</param>
        /// <returns>query results</returns>
        private IList<ResultType>
            ExecuteFind<ResultType>(SDNG.FindHibernateDelegate<ResultType> query)
        {
            return GetGenericTemplate().ExecuteFind<ResultType>(query);
        }
        #endregion

        #region creating sessions
        /// <summary>
        /// Opens a new transactional session.
        /// </summary>
        /// <returns>a session</returns>
        /// <remarks>The client must close the returned session.</remarks>
        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        /// <summary>
        /// Returns a new HibernateTemplate.
        /// </summary>
        /// <returns>a new HibernateTemplate</returns>
        protected HibernateTemplate GetTemplate()
        {
            return new HibernateTemplate(SessionFactory);
        }

        /// <summary>
        /// Returns a new HibernateTemplate.
        /// </summary>
        /// <returns>a new HibernateTemplate</returns>
        protected SDNG.HibernateTemplate GetGenericTemplate()
        {
            return new SDNG.HibernateTemplate(SessionFactory);
        }
        #endregion

    } // ItemRepository
}
