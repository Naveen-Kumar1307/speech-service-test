using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using NHibernate;
using NHMA = NHibernate.Mapping.Attributes;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Hibernate
{
    /// <summary>
    /// A persistent object with a surrogate key.
    /// </summary>
    public interface ISurrogated
    {
        /// <summary>
        /// A surrogate key.
        /// </summary>
        int Id { get; }

    } // ISurrogated

    /// <summary>
    /// A persistent composite.
    /// </summary>
    public interface ISurrogateComposite
    {
        /// <summary>
        /// Stale components (if any).
        /// </summary>
        ISurrogated[] StaleComponents { get; }
    }


    /// <summary>
    /// A persistent object with a native surrogate key.
    /// </summary>
    public abstract class NativeSurrogate : ISurrogated, ISurrogateComposite
    {
        #region creating instances
        /// <summary>
        /// Constructs a new NativeSurrogate.
        /// </summary>
        protected NativeSurrogate()
        {
        }
        #endregion

        #region stale components
        /// <summary>
        /// Any stale components.
        /// </summary>
        public virtual ISurrogated[] StaleComponents
        {
            get { return new ISurrogated[0]; }
        }
        #endregion

        #region checking persistence
        /// <summary>
        /// Checks whether a candidate surrogate had been saved.
        /// </summary>
        /// <param name="argumentName">an argument name</param>
        /// <param name="candidate">a candidate surrogate</param>
        public static void Check(String argumentName, ISurrogated candidate)
        {
            Argument.Check(argumentName, candidate);
            Argument.CheckLimit(argumentName + ".Id", candidate.Id, Argument.MORE, 0);
        }

        /// <summary>
        /// Indicates whether all the supplied candidates have been saved (persisted).
        /// </summary>
        /// <param name="candidates">some candidate surrogates</param>
        /// <returns>whether all the surrogate candidates have been saved</returns>
        public static bool Saved<ItemType>(ICollection<ItemType> candidates)
            where ItemType : class, ISurrogated
        {
            if (candidates == null || candidates.Count == 0) return false;

            int savedItems = (from candidate in candidates 
                              where Saved(candidate) select candidate).Count();

            return (candidates.Count == savedItems);
        }

        /// <summary>
        /// Indicates whether a candidate surrogate has been saved (persisted).
        /// </summary>
        /// <param name="candidate">a candidate surrogate</param>
        /// <returns>whether a candidate has been saved</returns>
        public static bool Saved(ISurrogated candidate)
        {
            return (candidate != null && candidate.Id > 0);
        }

        /// <summary>
        /// Checks for a valid persistent ID.
        /// </summary>
        /// <param name="argumentName">an argument name</param>
        public virtual void CheckID(String argumentName)
        {
            Argument.CheckLimit(argumentName + ".Id", Id, Argument.MORE, 0);
        }

        /// <summary>
        /// Checks that a pair of surrogates are different.
        /// </summary>
        /// <param name="argumentName">an argument name</param>
        /// <param name="candidate">a candidate reference</param>
        public virtual void CheckNotID(String argumentName, NativeSurrogate candidate)
        {
            if (candidate == null) return;
            if (GetType().FullName != candidate.GetType().FullName) return;
            Argument.CheckLimit(argumentName + ".Id", Id, Argument.NOT_EQUAL, candidate.Id);
        }
        #endregion

        #region accessing values
        /// <summary>
        /// The persistent object ID.
        /// </summary>
        /// <remarks>
        /// <h4>Hibernate Mapping:</h4>
        /// <list type="bullet">
        /// <item>Id = generated automatically from a sequence</item>
        /// </list>
        /// </remarks>
        [NHMA.Id(Name = "Id")]
        [NHMA.Generator(1, Class = "native")]
        public virtual int Id { get; protected set; }
        #endregion

    } // NativeSurrogate
}
