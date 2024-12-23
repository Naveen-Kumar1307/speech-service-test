using System;
using System.Collections;
using System.Collections.Generic;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// Makes an IEnumerator into IEnumerable.
    /// </summary>
    public class Iterator<T> : IEnumerable<T>
    {
        private IEnumerator<T> Items { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a new Iterator.
        /// </summary>
        /// <param name="items">enumerated items</param>
        /// <returns>a new Iterator</returns>
        public static Iterator<T> With(IEnumerator<T> items)
        {
            Argument.Check("items", items);
            return new Iterator<T>(items);
        }

        /// <summary>
        /// Constructs a new Iterator.
        /// </summary>
        /// <param name="items">enumerated items</param>
        private Iterator(IEnumerator<T> items)
        {
            Items = items;
        }
        #endregion

        #region IEnumerable<T> Members

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return Items;
        }

        #endregion

        #region IEnumerable Members

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items;
        }

        #endregion

    } // Iterator


    /// <summary>
    /// IEnumerator extension methods.
    /// </summary>
    public static class IEnumeratorExtensions
    {
        /// <summary>
        /// Converts an enumerator into enumerable items.
        /// </summary>
        /// <typeparam name="T">an element type</typeparam>
        /// <param name="items">enumerated items</param>
        /// <returns>an enumerator into enumerable items</returns>
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> items)
        {
            return Iterator<T>.With(items);
        }

    } // IEnumeratorExtensions
}
