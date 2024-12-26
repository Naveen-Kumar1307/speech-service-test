using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// Qualifies arguments prior to use.
    /// </summary>
    public class Argument
    {
        /// <summary>
        /// Indicates a value must be more than a limit.
        /// </summary>
        public static readonly String MORE = ">";

        /// <summary>
        /// Indicates a value must not be more than a limit.
        /// </summary>
        public static readonly String NOT_MORE = "<=";

        /// <summary>
        /// Indicates a value must be less than a limit.
        /// </summary>
        public static readonly String LESS = "<";

        /// <summary>
        /// Indicates a value must not be less than a limit.
        /// </summary>
        public static readonly String NOT_LESS = ">=";

        /// <summary>
        /// Indicates a value must equal a limit.
        /// </summary>
        public static readonly String EQUAL = "==";

        /// <summary>
        /// Indicates a value must not be equal to a limit.
        /// </summary>
        public static readonly String NOT_EQUAL = "!=";

        /// <summary>
        /// No instances of this class can be constructed.
        /// </summary>
        private Argument() { }

        /// <summary>
        /// Indicates whether an argument is present.
        /// </summary>
        public static bool IsPresent(Object argument)
        {
            return !IsAbsent(argument);
        }

        /// <summary>
        /// Indicates whether an argument is present.
        /// </summary>
        public static bool IsPresent(String argument)
        {
            return !IsAbsent(argument);
        }

        /// <summary>
        /// Indicates whether an argument has elements.
        /// </summary>
        /// <typeparam name="T">element type</typeparam>
        public static bool HasElements<T>(ICollection<T> argument)
        {
            return !IsEmpty(argument);
        }

        /// <summary>
        /// Indicates whether an argument is absent.
        /// </summary>
        public static bool IsAbsent(Object argument)
        {
            return (argument == null);
        }

        /// <summary>
        /// Indicates whether an argument is absent.
        /// </summary>
        public static bool IsAbsent(String argument)
        {
            return (argument == null || argument.Length == 0);
        }

        /// <summary>
        /// Indicates whether an argument is empty (no elements).
        /// </summary>
        /// <typeparam name="T">element type</typeparam>
        public static bool IsEmpty<T>(ICollection<T> argument)
        {
            return (argument == null || argument.Count == 0);
        }

        /// <summary>
        /// Indicates whether an argument is assignable to a type.
        /// </summary>
        /// <typeparam name="ExpectedType">an expected type</typeparam>
        public static bool IsKindOf<ExpectedType>(Object argument)
        {
            return IsKindOf(typeof(ExpectedType), argument);
        }

        /// <summary>
        /// Indicates whether an argument is assignable to a type.
        /// </summary>
        public static bool IsKindOf(Type expectedType, Object argument)
        {
            if (expectedType == null || argument == null) return false;
            return expectedType.IsAssignableFrom(argument.GetType());
        }

        /// <summary>
        /// Checks whether a required argument is present.
        /// </summary>
        public static void Check(String argumentName, Object argument)
        {
            if (IsAbsent(argument)) ReportMissing(argumentName);
        }

        /// <summary>
        /// Checks whether a required argument is present.
        /// </summary>
        public static void Check(String argumentName, String argument)
        {
            if (argument == null) ReportMissing(argumentName);
            if (IsAbsent(argument)) ReportEmpty(argumentName);
        }

        /// <summary>
        /// Checks whether a required argument is present and fits available storage space.
        /// </summary>
        public static void Check(String argumentName, String argument, int limit)
        {
            if (argument == null) ReportMissing(argumentName);
            if (IsAbsent(argument)) ReportEmpty(argumentName);
            if (argument.Length > limit) ReportLengthViolation(argumentName, limit);
        }

        /// <summary>
        /// Checks whether an optional argument fits available storage space.
        /// </summary>
        public static void CheckSize(String argumentName, String argument, int limit)
        {
            if (argument == null) return; // missing text is ok
            if (argument.Length > limit) ReportLengthViolation(argumentName, limit);
        }

        /// <summary>
        /// Checks whether a required argument has any elements.
        /// </summary>
        /// <typeparam name="T">element type</typeparam>
        public static void CheckAny<T>(String argumentName, ICollection<T> argument)
        {
            Check(argumentName, argument);
            if (IsEmpty(argument)) ReportEmpty(argumentName);
        }

        /// <summary>
        /// Checks the type compatibility of a required argument.
        /// </summary>
        public static void CheckType(String argumentName, Object argument, Type expectedType)
        {
            Check(argumentName, argument);
            if (!IsKindOf(expectedType, argument))
                ReportIncompatible(argumentName, argument, expectedType);
        }

        /// <summary>
        /// Checks the type compatibility of a required argument.
        /// </summary>
        /// <typeparam name="ExpectedType">an expected type</typeparam>
        public static void CheckType<ExpectedType>(String argumentName, Object argument)
        {
            Check(argumentName, argument);
            if (!IsKindOf<ExpectedType>(argument))
                ReportIncompatible(argumentName, argument, typeof(ExpectedType));
        }

        /// <summary>
        /// Checks a value against a limit.
        /// </summary>
        public static void CheckLimit(
            String argumentName, int candidate, String comparison, int limit)
        {
            switch (comparison.Trim())
            {
                case ">":
                    if (candidate > limit) return;
                    break;

                case "<":
                    if (candidate < limit) return;
                    break;

                case ">=":
                    if (candidate >= limit) return;
                    break;

                case "<=":
                    if (candidate <= limit) return;
                    break;

                case "==":
                    if (candidate == limit) return;
                    break;

                case "!=":
                    if (candidate != limit) return;
                    break;
            }

            ReportLimitViolation(argumentName, comparison, limit);
        }

        /// <summary>
        /// Reports that a required argument is missing.
        /// </summary>
        public static void ReportMissing(String argumentName)
        {
            throw new ArgumentNullException(argumentName,
                        argumentName + " is required but missing");
        }

        /// <summary>
        /// Reports that a required argument is empty.
        /// </summary>
        public static void ReportEmpty(String argumentName)
        {
            throw new ArgumentOutOfRangeException(argumentName,
                        argumentName + " is required but empty or null");
        }

        /// <summary>
        /// Reports that an incompatible argument was supplied.
        /// </summary>
        public static void ReportIncompatible(
            String argumentName, Object argument, Type expectedType)
        {
            throw new ArgumentOutOfRangeException(argumentName,
                        argumentName + " is " + argument.GetType().Name +
                        " but should be " + expectedType.Name);
        }

        /// <summary>
        /// Reports that a value limitation was violated.
        /// </summary>
        public static void ReportLimitViolation<ValueType>(
            String argumentName, String comparison, ValueType limit)
        {
            throw new ArgumentOutOfRangeException(argumentName,
                        argumentName + " must be " + comparison + " " + limit);
        }

        /// <summary>
        /// Reports that a value length limit was violated.
        /// </summary>
        public static void ReportLengthViolation(String argumentName, int limit)
        {
            throw new ArgumentOutOfRangeException(argumentName,
                        argumentName + " length cannot exceed " + limit);
        }

    } // Argument
}
