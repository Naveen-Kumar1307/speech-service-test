using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using Iesi.Collections.Generic;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Hibernate
{
    /// <summary>
    /// Maintains the contents of a lazy persistent list.
    /// </summary>
    /// <typeparam name="OwnerType">a kind of list owner</typeparam>
    /// <typeparam name="ElementType">a kind of list element</typeparam>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a list and its owner</item>
    /// <item>maintains the list elements</item>
    /// </list>
    /// </remarks>
    public class LazyList<OwnerType, ElementType>
        where ElementType : class, ISurrogated
        where OwnerType : class, ISurrogated, ItemRepository<OwnerType>.ISource
    {
        private IList<ElementType> Contents { get; set; }
        private OwnerType Owner { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a LazyList, caching a new instance if needed.
        /// </summary>
        /// <param name="cache">a cache reference</param>
        /// <param name="contents">the set contents</param>
        /// <param name="owner">the set owner</param>
        /// <returns>a LazyList</returns>
        public static LazyList<OwnerType, ElementType> From(
            ref LazyList<OwnerType, ElementType> cache,
            IList<ElementType> contents, OwnerType owner)
        {
            lock (owner)
            {
                if (cache == null)
                    cache = With(contents, owner);

                return cache;
            }
        }

        /// <summary>
        /// Returns a new LazyList.
        /// </summary>
        /// <param name="contents">the set contents</param>
        /// <param name="owner">the set owner</param>
        /// <returns>a new LazyList</returns>
        public static LazyList<OwnerType, ElementType> With(
            IList<ElementType> contents, OwnerType owner)
        {
            LazyList<OwnerType, ElementType> result = new LazyList<OwnerType, ElementType>();
            result.Contents = contents;
            result.Owner = owner;
            return result;
        }

        /// <summary>
        /// Constructs a new LazyList.
        /// </summary>
        private LazyList()
        {
        }
        #endregion

        #region maintaining elements
        /// <summary>
        /// Counts the elements in the list.
        /// </summary>
        public int CountElements()
        {
            return Owner.Repository.GetSize(Contents, Owner);
        }

        /// <summary>
        /// Adds an element to a lazy list.
        /// </summary>
        /// <param name="element">an element</param>
        public void Add(ElementType element)
        {
            Owner.Repository.AddElement(element, Contents, Owner);
        }

        /// <summary>
        /// Removes an element from a lazy list.
        /// </summary>
        /// <param name="element">an element</param>
        public void Remove(ElementType element)
        {
            Owner.Repository.RemoveElement(element, Contents, Owner);
        }

        /// <summary>
        /// Returns a list of the available elements.
        /// </summary>
        /// <returns>an element list</returns>
        public IList<ElementType> GetElements()
        {
            FillList();
            return new List<ElementType>(Contents);
        }

        /// <summary>
        /// Finds an element if already contained in a managed list.
        /// </summary>
        /// <param name="sample">a sample element</param>
        /// <returns>a found element, or null</returns>
        public ElementType FindElement(ElementType sample)
        {
            if (!NativeSurrogate.Saved(sample)) return null;

            FillList();
            return (from element in Contents
                    where element.Id == sample.Id
                    select element).FirstOrDefault();
        }

        /// <summary>
        /// Fills the managed list.
        /// </summary>
        public void FillList()
        {
            Owner.Repository.FillList<ElementType>(Contents, Owner);
        }
        #endregion

    } // LazyList
}
