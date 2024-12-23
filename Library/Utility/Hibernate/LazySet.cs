using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Iesi.Collections.Generic;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Hibernate
{
    /// <summary>
    /// Maintains the contents of a lazy persistent set.
    /// </summary>
    /// <typeparam name="OwnerType">a kind of set owner</typeparam>
    /// <typeparam name="ElementType">a kind of set element</typeparam>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a set and its owner</item>
    /// <item>maintains the set elements</item>
    /// </list>
    /// </remarks>
    public class LazySet<OwnerType, ElementType> 
        where ElementType : class, ISurrogated
        where OwnerType : class, ISurrogated, ItemRepository<OwnerType>.ISource
    {
        private ISet<ElementType> Contents { get; set; }
        private OwnerType Owner { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a LazySet, caching a new instance if needed.
        /// </summary>
        /// <param name="cache">a cache reference</param>
        /// <param name="contents">the set contents</param>
        /// <param name="owner">the set owner</param>
        /// <returns>a LazySet</returns>
        public static LazySet<OwnerType, ElementType> From(
            ref LazySet<OwnerType, ElementType> cache,
            ISet<ElementType> contents, OwnerType owner)
        {
            lock (owner)
            {
                if (cache == null)
                    cache = With(contents, owner);

                return cache;
            }
        }

        /// <summary>
        /// Returns a new LazySet.
        /// </summary>
        /// <param name="contents">the set contents</param>
        /// <param name="owner">the set owner</param>
        /// <returns>a new LazySet</returns>
        public static LazySet<OwnerType, ElementType> With(
            ISet<ElementType> contents, OwnerType owner)
        {
            LazySet<OwnerType, ElementType> result = new LazySet<OwnerType, ElementType>();
            result.Contents = contents;
            result.Owner = owner;
            return result;
        }

        /// <summary>
        /// Constructs a new LazySet.
        /// </summary>
        private LazySet()
        {
        }
        #endregion

        #region maintaining elements
        /// <summary>
        /// Counts the elements in the set.
        /// </summary>
        public int CountElements()
        {
            return Owner.Repository.GetSize(Contents, Owner);
        }

        /// <summary>
        /// Returns a list of the available elements.
        /// </summary>
        /// <returns>an element list</returns>
        public IList<ElementType> GetElements()
        {
            FillSet();
            return new List<ElementType>(Contents);
        }

        /// <summary>
        /// Adds an element to a lazy set.
        /// </summary>
        /// <param name="element">an element</param>
        public void Add(ElementType element)
        {
            Owner.Repository.AddElement(element, Contents, Owner);
        }

        /// <summary>
        /// Removes an element from a lazy set.
        /// </summary>
        /// <param name="element">an element</param>
        public void Remove(ElementType element)
        {
            Owner.Repository.RemoveElement(element, Contents, Owner);
        }

        /// <summary>
        /// Finds an element if already contained in a managed set.
        /// </summary>
        /// <param name="sample">a sample element</param>
        /// <returns>a found element, or null</returns>
        public ElementType FindElement(ElementType sample)
        {
            if (!NativeSurrogate.Saved(sample)) return null;

            FillSet();
            return (from element in Contents
                    where element.Id == sample.Id
                    select element).FirstOrDefault();
        }

        /// <summary>
        /// Fills the managed set.
        /// </summary>
        public void FillSet()
        {
            Owner.Repository.FillSet<ElementType>(Contents, Owner);
        }
        #endregion

    } // LazySet
}
