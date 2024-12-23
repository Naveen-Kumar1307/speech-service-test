using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Recognition.DataContracts
{
    /// <summary>
    /// Extends String to support grammar processing.
    /// </summary>
    public static class GrammarExtensions
    {
        /// <summary>
        /// A standard grammar name prefix (or simple name).
        /// </summary>
        public static readonly string FixedGrammar = ".SENTENCE";
        public static readonly string Blank = " ";
        public static readonly string Dash = "-";

        private static readonly string Answer = "answer";
        private static readonly string Punctuators = ".,!?"; // apostrophe ' is ok to keep
        private static readonly string Enclosures = "[]";
        private static readonly string Brackets = "<>";
        private static readonly string Braces = "{}";
        private static readonly string TermEnds = "()";
        private static readonly string Quotes = "'\"";
        private static readonly string HiddenCrap = "\n\r\t\f";

        private static readonly StringSplitOptions RemoveEmpties = 
                                StringSplitOptions.RemoveEmptyEntries;

        /// <summary>
        /// Indicates whether this text is a grammar name.
        /// </summary>
        public static bool IsGrammarName(this string text)
        {
            if (Argument.IsAbsent(text)) return false;
            return text.StartsWith(FixedGrammar);
        }

        /// <summary>
        /// Converts this text to lower case and replaces any punctuation with blanks.
        /// </summary>
        public static string WithPunctuationBlanked(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            string result = text;
            foreach (char punctuator in Punctuators)
            {
                result = result.Replace(punctuator.ToString(), Blank);
            }
            return result.ToLower();
        }

        /// <summary>
        /// Converts this text to a grammar phrase.
        /// </summary>
        public static string AsGrammarPhase(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            string result = text;
            foreach (char punctuator in Punctuators)
            {
                result = result.Replace(punctuator.ToString(), string.Empty);
            }
            return result.ToLower();
        }

        /// <summary>
        /// Extracts an answer index from a natural language answer specification.
        /// </summary>
        public static int ExtractAnswerIndex(this string text)
        {
            if (Argument.IsAbsent(text)) return -1;
            string[] parts = text.WithoutBraces().WithoutBrackets().SplitOnBlank();
            if (parts.Length < 2 || parts[0] != Answer) return -1;
            return int.Parse(parts[1]);
        }

        /// <summary>
        /// Splits this text into some integers.
        /// </summary>
        public static int[] SplitAsIntegers(this string text)
        {
            string[] results = text.SplitOnBlank();
            return (from result in results select int.Parse(result)).ToArray();
        }

        /// <summary>
        /// Splits this text on its blanks.
        /// </summary>
        public static string[] SplitOnBlank(this string text)
        {
            if (Argument.IsAbsent(text)) return new string[0];
            char[] separators = { Blank[0] };
            return text.Split(separators, RemoveEmpties);
        }

        /// <summary>
        /// Surrounds this text with parentheses.
        /// </summary>
        public static string AsTerm(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return TermEnds[0] + text + TermEnds[1];
        }

        /// <summary>
        /// Surrounds this text with curly braces.
        /// </summary>
        public static string Embraced(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return Braces[0] + text + Braces[1];
        }

        /// <summary>
        /// Surrounds this text with angle brackets.
        /// </summary>
        public static string Bracketed(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return Brackets[0] + text + Brackets[1];
        }
        
        /// <summary>
        /// Surrounds this text with square brackets.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Enclosed(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return Enclosures[0] + text + Enclosures[1];
        }

        /// <summary>
        /// Trims any surrounding parentheses from this text.
        /// </summary>
        public static string WithoutTermEnds(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return text.Trim().Trim(TermEnds.ToCharArray()); ;
        }

        /// <summary>
        /// Trims any surrounding curly braces from this text.
        /// </summary>
        public static string WithoutBraces(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return text.Trim().Trim(Braces.ToCharArray()); ;
        }

        /// <summary>
        /// Trims any surrounding angle brackets from this text.
        /// </summary>
        public static string WithoutBrackets(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return text.Trim().Trim(Brackets.ToCharArray()); ;
        }

        /// <summary>
        /// Trims any surrounding square brackets from this text.
        /// </summary>
        public static string WithoutClosures(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return text.Trim().Trim(Enclosures.ToCharArray()); ;
        }

        /// <summary>
        /// Trims any surrounding quotes from this text.
        /// </summary>
        public static string WithoutQuotes(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            return text.Replace(Quotes[0].ToString(), string.Empty)
                       .Replace(Quotes[1].ToString(), string.Empty);
        }

        /// <summary>
        /// Removes unacceptable characters from this grammar.
        /// </summary>
        public static string WithoutHiddenCrap(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            string result = text;
            foreach (char crap in HiddenCrap.ToCharArray())
            {
                result = result.Replace(crap.ToString(), string.Empty);
            }
            return result.Trim();
        }

        /// <summary>
        /// Converts this text to a forced alignment grammar.
        /// </summary>
        public static string AsForcedAlignmentGrammar(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            text = text.WithoutClosures();
            if (text.Contains(Braces[0].ToString()))
            {
                string[] parts = text.Split(Braces.ToCharArray(), RemoveEmpties);
                return parts[0].WithoutTermEnds();
            }
            else
            {
                return text.WithoutTermEnds();
            }
        }

        /// <summary>
        /// Converts this text to a dynamic grammar.
        /// </summary>
        public static string AsDynamicGrammar(this string text)
        {
            if (Argument.IsAbsent(text)) return string.Empty;
            text = text.Trim();
            return (text.StartsWith(Enclosures[0].ToString()) ? text : text.Enclosed());
        }

        /// <summary>
        /// Extracts grammars from this text.
        /// </summary>
        public static string[] ExtractGrammars(this string text)
        {
            string[] empty = { };
            if (Argument.IsAbsent(text)) return empty;
            return text.WithoutClosures().Split(TermEnds.ToCharArray(), RemoveEmpties);
        }

    } // GrammarExtensions
}
