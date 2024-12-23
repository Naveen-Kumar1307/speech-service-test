using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Threading;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Recognition.DataContracts
{
    /// <summary>
    /// A request to recognize speech.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>describes a user, a grammar, and the expected results</item>
    /// </list>
    /// </remarks>
    [Serializable]
    [DataContract]
    public class RecognitionRequest
    {
        private static readonly string Bar = "|";
        private static readonly string Dash = "-";
        private static readonly string SemiColon = ";";

        /// <summary>
        /// Indicates an audio data format.
        /// </summary>
        [XmlIgnore]
        public string AudioFormat { get; set; }


        /// <summary>
        /// Indicates a kind of recognition.
        /// </summary>
        [XmlIgnore]
        public RecognitionKind RecognitionTypeKind { get; set; }

        [DataMember]
        [XmlAttribute]
        public string RecognitionType
        {
            get { return RecognitionTypeKind.ToString(); }
            set { RecognitionTypeKind = value.ToEnum<RecognitionKind>(); }
        }

        /// <summary>
        /// Identifies a user.
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string UserId { get; set; }

        /// <summary>
        /// A grammar (or grammar name).
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string Grammar { get; set; }

        /// <summary>
        /// The expect result(s).
        /// </summary>
        [DataMember]
        [XmlElement("ExpectedResult", typeof(ExpectedResult))]
        public ExpectedResult[] ExpectedResults { get; set; }

        /// <summary>
        /// Constructs a new RecognitionRequest.
        /// </summary>
        public RecognitionRequest()
        {
            RecognitionTypeKind = RecognitionKind.ClientPreparation;

            UserId = "0";
            Grammar = "a sample grammar";
        }

        public string GetUniqueFileName()
        {
            int processID = Process.GetCurrentProcess().Id;
            int threadID = Thread.CurrentThread.ManagedThreadId;
            return processID + Dash + threadID + Dash + UserId + Dash + DateTime.Now.Ticks;
        }

        /// <summary>
        /// Returns the expected answer with all punctuation blanked.
        /// </summary>
        public string GetNormalizedAnswer()
        {
            return ExpectedResults.Length == 0 ? "" : ExpectedResults[0].FullAnswer.WithPunctuationBlanked();
        }

        /// <summary>
        /// Encodes the expected results as a single string.
        /// </summary>
        /// <returns>encoded expected results, or empty</returns>
        public string EncodeExpectedResults()
        {
            if (Argument.IsAbsent(ExpectedResults)) return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (ExpectedResult expect in ExpectedResults)
            {
                if (builder.Length > 0) builder.Append(Bar);
                builder.Append(expect.Answer.Trim());
                builder.Append(SemiColon);
                builder.Append(expect.FullAnswer.Trim());
            }

            return builder.ToString();
        }

        /// <summary>
        /// The audio file suffix.
        /// </summary>
        public string AudioFileSuffix
        {
            get
            {
                return (Argument.IsAbsent(AudioFormat) ? 
                        string.Empty : 
                        AudioFormat.TrimStart('.'));
            }
        }

        /// <summary>
        /// Returns a grammar name or a normalized grammar.
        /// </summary>
        public string GetNormalizedGrammar()
        {
            if (Argument.IsAbsent(Grammar))
            {
                StringBuilder builder = new StringBuilder();
                foreach (ExpectedResult expectation in ExpectedResults)
                {
                    builder.Append(expectation.BuildGrammar().AsTerm());
                }
                return builder.ToString().Enclosed();
            }
            else
            {
                return Grammar;

                //if (Grammar.IsGrammarName()) return Grammar;

                //if (ExpectedResults == null || ExpectedResults.Length < 2)
                //{
                //    return Grammar.AsForcedAlignmentGrammar();
                //}
                //else
                //{
                //    return Grammar.AsDynamicGrammar();
                //}
            }
        }

        /// <summary>
        /// Returns the index of the expected result that matches the supplied text.
        /// </summary>
        /// <param name="recognizedText">recognized text</param>
        /// <returns>an expected result match index</returns>
        public int FindMatchIndex(string recognizedText)
        {
            if (ExpectedResults == null)
            {
                return -1;
            }

            int result = -1;

            int resultLength = 0;
            int count = ExpectedResults.Length;
            for (int index = 0; index < count; index++)
            {
                string matchText = GetExpectedPhraseGrammar(index);
                if (recognizedText.Contains(matchText))
                {
                    if (matchText.Length > resultLength)
                    {
                        result = index;
                        resultLength = matchText.Length;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the indicated phrase grammar.
        /// </summary>
        /// <param name="index">an expected result index</param>
        /// <returns>an expected phrase grammar</returns>
        public string GetExpectedPhraseGrammar(int index)
        {
            return ExpectedResults.Length == 0 ? "" : ExpectedResults[index].Answer.AsGrammarPhase();
        }

        /// <summary>
        /// Returns the indicated sentence grammar.
        /// </summary>
        /// <param name="index">an expected result index</param>
        /// <returns>an expected sentence grammar</returns>
        public string GetExpectedSentenceGrammar(int index)
        {
            return ExpectedResults.Length == 0 ? "" : ExpectedResults[index].FullAnswer.AsGrammarPhase();
        }

        /// <summary>
        /// Indicates whether this is a pronunciation practice request.
        /// </summary>
        public bool IsPronunciationPractice()
        {
            return RecognitionTypeKind == RecognitionKind.PronunciationPractice;
        }

        /// <summary>
        /// Formats the data contained in this request.
        /// </summary>
        public string FormatData()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("user = ");
            builder.Append(UserId);
            builder.Append(", type = ");
            builder.Append(RecognitionType);
            builder.Append(", audio = ");
            builder.AppendLine(AudioFileSuffix);
            builder.Append("grammar = ");
            builder.AppendLine(Grammar.SinglyQuoted());
            builder.Append("expects = ");
            builder.AppendLine(EncodeExpectedResults().SinglyQuoted());
            return builder.ToString();
        }

    } // RecognitionRequest

    /// <summary>
    /// Describes an expected recognition result.
    /// </summary>
    [Serializable]
    [DataContract]
    public class ExpectedResult
    {
        /// <summary>
        /// An expected recognition answer.
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string Answer { get; set; }

        /// <summary>
        /// A complete answer (with context).
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string FullAnswer { get; set; }

        /// <summary>
        /// Returns a force-alignment grammar.
        /// </summary>
        public string BuildGrammar()
        {
            return FullAnswer.AsGrammarPhase();
        }

    } // ExpectedResult
}
