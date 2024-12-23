using System;
using System.Text;
using System.Data.SqlTypes;
using System.Collections.Generic;

namespace GlobalEnglish.Utility.Parameters
{
    /// <summary>
    /// Integer extension methods.
    /// </summary>
    public static class IntegerExtensions
    {
        /// <summary>
        /// Converts this integer to an enumerated type value.
        /// </summary>
        /// <typeparam name="ResultType">an enumerated type</typeparam>
        /// <param name="value">a value</param>
        /// <returns>an enumerated value</returns>
        public static ResultType ConstrainedToEnum<ResultType>(this int value)
        {
            Type resultType = typeof(ResultType);
            Array values = Enum.GetValues(resultType);
            int index = value.BoundValue(0, values.Length);
            return (ResultType)values.GetValue(index);
        }

        /// <summary>
        /// Bounds this value to a given range.
        /// </summary>
        /// <param name="value">a value</param>
        /// <param name="lowerBound">a lower bound</param>
        /// <param name="upperBound">an upper bound</param>
        /// <returns>a value in the given range</returns>
        public static int BoundValue(this int value, int lowerBound, int upperBound)
        {
            if (value < lowerBound) return lowerBound;
            if (value > upperBound) return upperBound;
            return value;
        }

    } // IntegerExtensions

    /// <summary>
    /// DateTime extension methods.
    /// </summary>
    public static class TimeExtensions
    {
        /// <summary>
        /// Returns the candidate time or null if the given time is unacceptable.
        /// </summary>
        public static DateTime? OrNull(this DateTime candidate)
        {
            if (candidate < SqlDateTime.MinValue.Value) return null;
            return candidate;
        }

        /// <summary>
        /// Returns the candidate time or the minimum acceptable value.
        /// </summary>
        public static DateTime OrMinimum(this DateTime candidate)
        {
            return (candidate < SqlDateTime.MinValue.Value ?
                    SqlDateTime.MinValue.Value : candidate);
        }

        /// <summary>
        /// Returns the span it took to execute an action.
        /// </summary>
        /// <param name="startTime">a start time</param>
        /// <param name="action">an action</param>
        /// <returns>a time span</returns>
        public static TimeSpan TimeToRun(this DateTime startTime, Action action)
        {
            action();
            return DateTime.Now - startTime;
        }

    } // TimeExtensions

    /// <summary>
    /// String extension methods.
    /// </summary>
    public static class StringExtensions
    {
        private static readonly string Quote = "\"";
        private static readonly string SingleQuote = "'";

        /// <summary>
        /// Returns the source text wrapped in quotes.
        /// </summary>
        public static string Quoted(this string sourceText)
        {
            if (sourceText == null) return string.Empty;
            return Quote + sourceText + Quote;
        }

        /// <summary>
        /// Returns the source text wrapped in single quotes.
        /// </summary>
        public static string SinglyQuoted(this string sourceText)
        {
            if (sourceText == null) return string.Empty;
            return SingleQuote + sourceText + SingleQuote;
        }

        /// <summary>
        /// Returns the source text or an empty string if null.
        /// </summary>
        public static string OrEmpty(this string sourceText)
        {
            return (Argument.IsAbsent(sourceText) ? string.Empty : sourceText);
        }

        /// <summary>
        /// Checks that the supplied source text is not null.
        /// </summary>
        /// <exception cref="ArgumentNullException">if text is null</exception>
        public static string OrFail(this string sourceText)
        {
            Argument.Check("sourceText", (object)sourceText);
            return sourceText;
        }

        /// <summary>
        /// Checks whether text will fit in the available storage space.
        /// </summary>
        /// <param name="sourceText">the source text</param>
        /// <param name="limit">the available text storage space</param>
        /// <returns>the supplied text string</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// if the encoded text length exceeds the limit</exception>
        public static string CheckLength(this string sourceText, int limit)
        {
            if (sourceText == null) return null;
            if (sourceText.Length <= limit) return sourceText;

            throw new ArgumentOutOfRangeException("sourceText",
                "Text length exceeds available storage space " + limit);

        }

        /// <summary>
        /// Encodes Unicode characters beyond the lower ASCII range as entities.
        /// </summary>
        /// <param name="sourceText">the source text</param>
        /// <param name="limit">the available text storage space</param>
        /// <returns>a properly encoded text string</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// if the encoded text length exceeds the limit</exception>
        public static string WithXmlEntities(this string sourceText, int limit)
        {
            if (sourceText == null) return null;
            string result = sourceText.WithXmlEntities();
            if (result.Length <= limit) return result;
            throw new ArgumentOutOfRangeException("sourceText",
                "Entity encoded text length exceeds available storage space " + limit);
        }

        /// <summary>
        /// Encodes Unicode characters beyond the lower ASCII range as entities.
        /// </summary>
        /// <param name="sourceText">the source text</param>
        /// <returns>a properly encoded text string</returns>
        public static String WithXmlEntities(this String sourceText)
        {
            if (sourceText == null) return null;

            StringBuilder builder = new StringBuilder();
            foreach (char c in sourceText)
            {
                if ((int)c > 0x7F)
                {
                    builder.Append(String.Format("&#{0};", (int)c));
                }
                else
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Converts text to a result value type.
        /// </summary>
        /// <typeparam name="ResultType">a result type</typeparam>
        /// <param name="sourceText">the source text</param>
        /// <returns>a result value, or default value</returns>
        public static ResultType To<ResultType>(this string sourceText)
        {
            if (Argument.IsAbsent(sourceText)) sourceText = default(ResultType).ToString();
            return Conversion.ConvertTo<ResultType>(sourceText);
        }

        /// <summary>
        /// Converts text to a result enum value type.
        /// </summary>
        /// <typeparam name="ResultType">an enumerated type</typeparam>
        /// <param name="sourceText">the source text</param>
        /// <returns>a result value, or default value</returns>
        public static ResultType ToEnum<ResultType>(this string sourceText)
        {
            Type resultType = typeof(ResultType);
            if (Argument.IsAbsent(sourceText)) sourceText = default(ResultType).ToString();
            return (ResultType)Enum.Parse(resultType, sourceText); ;
        }

        private static readonly ValueConverter Conversion = new ValueConverter();

    } // StringExtensions
}
