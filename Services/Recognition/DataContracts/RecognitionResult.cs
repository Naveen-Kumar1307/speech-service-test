using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Recognition.DataContracts
{
    /// <summary>
    /// Indicates which kind of recognition activity will be performed.
    /// </summary>
    [Serializable]
    public enum RecognitionKind
    {
        /// <summary>
        /// Indicates the client is preparing equipment for recognition.
        /// </summary>
        ClientPreparation,

        /// <summary>
        /// Indicates the client is conducting a communication exercise.
        /// </summary>
        CommunicationPractice,

        /// <summary>
        /// Indicates the client is conducting a pronunciation exercise.
        /// </summary>
        PronunciationPractice

    } // RecognitionKind

    /// <summary>
    /// Indicates a kind of recognition result.
    /// </summary>
    [Serializable]
    public enum ResultKind
    {
        /// <summary>
        /// Indicates a server was unavailable.
        /// </summary>
        RecognitionUnavailable,

        /// <summary>
        /// Indicates a server was available.
        /// </summary>
        RecognitionAvailable,

        /// <summary>
        /// Indicates a recognition problem occurred.
        /// </summary>
        RecognitionError,

        /// <summary>
        /// Indicates a recognition failed.
        /// </summary>
        RecognitionFailed,

        /// <summary>
        /// Indicates a recognition succeeded.
        /// </summary>
        RecognitionSucceeded,

    } // ResultKind

    /// <summary>
    /// Indicates a kind of problem detected.
    /// </summary>
    public enum ResultDetailKind
    {
        /// <summary>
        /// Indicates an audio quality problem may exist.
        /// </summary>
        AudioQualityWarning,

        /// <summary>
        /// Indicates the audio file was missing.
        /// </summary>
        AudioFileMissing,

        /// <summary>
        /// Indicates the supplied grammar was rejected.
        /// </summary>
        GrammarWasRejected,

        /// <summary>
        /// Indicates the confidence was too low.
        /// </summary>
        ConfidenceWasTooLow,

        /// <summary>
        /// Indicates that the audio did not match the supplied grammar.
        /// </summary>
        NoRecognition,

        /// <summary>
        /// Indicates that the request was cancelled (interrupted) by the client.
        /// </summary>
        RecognitionCancelled,

    } // ResultDetailKind

    /// <summary>
    /// Indicates a kind of pronunciation problem.
    /// </summary>
    public enum PronunciationProblemKind
    {
        /// <summary>
        /// No problem was detected.
        /// </summary>
        None,

        /// <summary>
        /// A grapheme / phoneme pronunciation problem was detected.
        /// </summary>
        Grapheme,

        /// <summary>
        /// A word pronunciation problem was detected.
        /// </summary>
        Word

    } // PronunciationProblemKind

    /// <summary>
    /// Indicates the quality of the audio level.
    /// </summary>
    public enum AudioLevel
    {
        /// <summary>
        /// Indicates audio level is normal.
        /// </summary>
        Normal,

        /// <summary>
        /// Indicates audio level is too low.
        /// </summary>
        TooLow,

        /// <summary>
        /// Indicates audio level is too high.
        /// </summary>
        TooHigh

    } // AudioLevel

    /// <summary>
    /// Indicates whether audio truncation occurred.
    /// </summary>
    public enum AudioTruncation
    {
        /// <summary>
        /// Indicates no truncation occurred.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the audio was truncated at the start.
        /// </summary>
        Start,

        /// <summary>
        /// Indicates that the audio was truncated at the end.
        /// </summary>
        End,

        /// <summary>
        /// Indicates that the audio was truncated at both start and end.
        /// </summary>
        Both

    } // AudioTruncation

    /// <summary>
    /// Indicates whether noise was detected in the audio.
    /// </summary>
    public enum AudioNoise
    {
        /// <summary>
        /// Indicates no noise was detected.
        /// </summary>
        None,

        /// <summary>
        /// Indicates the audio had a low noise level.
        /// </summary>
        Low,

        /// <summary>
        /// Indicates the audio had a moderate noise level.
        /// </summary>
        Moderate,

        /// <summary>
        /// Indicates the audio had a high noise level.
        /// </summary>
        High

    } // 

    /// <summary>
    /// Describes the result of a speech recognition.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>indicates whether a recognition succeeded</item>
    /// <item>knows the results of a successful recognition</item>
    /// </list>
    /// ASR Phoneme Tracking (Wiki)
    /// GetFloatParameter(ep.EndSeconds)??
    /// </remarks>
    [Serializable]
    [DataContract]
    public class RecognitionResult
    {
        /// <summary>
        /// Indicates the kind of result.
        /// </summary>
        public ResultKind TypeKind { get; set; }

        [DataMember]
        [XmlAttribute]
        public string Type
        {
            get { return TypeKind.ToString(); }
            set { TypeKind = value.ToEnum<ResultKind>(); }
        }

        /// <summary>
        /// Indicates a result detail.
        /// </summary>
        public ResultDetailKind[] ResultDetailKinds { get; set; }

        [DataMember]
        [XmlElement("ResultDetail", typeof(string))]
        public string[] ResultDetails
        {
            get
            {
                if (Argument.IsEmpty(ResultDetailKinds))
                {
                    string[] empty = { };
                    return empty;
                }
                else
                {
                    return ResultDetailKinds.Select(item => item.ToString()).ToArray();
                }
            }
            set
            {
                if (Argument.IsEmpty(value))
                {
                    ResultDetailKind[] empty = { };
                    ResultDetailKinds = empty;
                }
                else
                {
                    ResultDetailKinds = (from detail in value
                                         let kind = detail.ToEnum<ResultDetailKind>()
                                         select kind).ToArray();
                }
            }
        }

        /// <summary>
        /// A recognition message.
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// The amount of time (msecs) spent during recognition.
        /// </summary>
        [DataMember]
        public int RecognitionTime { get; set; }

        /// <summary>
        /// The amount of time (msecs) spent during recognition, 
        /// including time spent waiting in queue.
        /// </summary>
        [DataMember]
        public int QueuedRecognitionTime { get; set; }

        /// <summary>
        /// The saved audio file name.
        /// </summary>
        [DataMember]
        public string RecordedFileName { get; set; }

        /// <summary>
        /// The saved audio file type.
        /// </summary>
        [DataMember]
        public string RecordedFileType { get; set; }

        /// <summary>
        /// The audio quality measures.
        /// </summary>
        [DataMember]
        public AudioQuality AudioMeasure { get; set; }

        /// <summary>
        /// A matched sentence result (if recognized).
        /// </summary>
        [DataMember]
        [XmlElement]
        public SentenceMatch Sentence { get; set; }

        /// <summary>
        /// Constructs a new RecognitionResult.
        /// </summary>
        public RecognitionResult()
        {
            TypeKind = ResultKind.RecognitionUnavailable;
            ResultDetailKinds = new ResultDetailKind[0];
            AudioMeasure = new AudioQuality();
            Message = string.Empty;
        }

        /// <summary>
        /// Returns a new RecognitionResult for a missing audio file.
        /// </summary>
        /// <param name="filePath">a file path</param>
        /// <returns>a missing file RecognitionResult</returns>
        public static RecognitionResult WithMissingFile(string filePath)
        {
            RecognitionResult result = new RecognitionResult();
            result.AddDetail(ResultDetailKind.AudioFileMissing);
            result.TypeKind = ResultKind.RecognitionError;
            result.Message = filePath;
            return result;
        }

        /// <summary>
        /// Returns a new RecognitionResult for an unavailable recognizer.
        /// </summary>
        /// <returns>an unavailable RecognitionResult</returns>
        public static RecognitionResult WithUnavailable()
        {
            return new RecognitionResult();
        }

        /// <summary>
        /// Returns a new RecognitionResult for an invalid grammar.
        /// </summary>
        /// <returns>an invalid grammar RecognitionResult</returns>
        public static RecognitionResult WithInvalidGrammar()
        {
            RecognitionResult result = new RecognitionResult();
            result.AddDetail(ResultDetailKind.GrammarWasRejected);
            result.TypeKind = ResultKind.RecognitionError;
            return result;
        }

        /// <summary>
        /// Returns a new RecognitionResult for a failure.
        /// </summary>
        /// <returns>a failed RecognitionResult</returns>
        public static RecognitionResult WithFailure(bool cancelled)
        {
            RecognitionResult result = new RecognitionResult();
            result.TypeKind = ResultKind.RecognitionFailed;
            result.AddDetail(cancelled ? 
                             ResultDetailKind.RecognitionCancelled : 
                             ResultDetailKind.NoRecognition);
            return result;
        }

        /// <summary>
        /// Returns a new RecognitionResult for an available recognizer.
        /// </summary>
        /// <returns>a ping result</returns>
        public static RecognitionResult WithAvailable()
        {
            RecognitionResult result = new RecognitionResult();
            result.TypeKind = ResultKind.RecognitionAvailable;
            return result;
        }

        /// <summary>
        /// Returns a new RecognitionResult with an error.
        /// </summary>
        /// <param name="ex">an exception</param>
        /// <returns>an error RecognitionResult</returns>
        public static RecognitionResult WithError(Exception ex)
        {
            return WithError(ex.Message);
        }

        /// <summary>
        /// Returns a new RecognitionResult with an error.
        /// </summary>
        /// <param name="message">an error message</param>
        /// <returns>an error RecognitionResult</returns>
        public static RecognitionResult WithError(string message)
        {
            RecognitionResult result = new RecognitionResult();
            result.TypeKind = ResultKind.RecognitionError;
            result.Message = message;
            return result;
        }

        /// <summary>
        /// Updates the result type base on overall match confidence.
        /// </summary>
        /// <param name="matchedText">a matched phrase</param>
        public void UpdateConfidence(string matchedText)
        {
            EvaluateConfidence(matchedText);
            if (!Sentence.RecognitionAccepted)
            {
                TypeKind = ResultKind.RecognitionFailed;
                AddDetail(ResultDetailKind.ConfidenceWasTooLow);
            }
        }

        /// <summary>
        /// Evaluates the result confidence.
        /// </summary>
        public void EvaluateConfidence(string matchedText)
        {
            WordQuality[] matchedWords = Sentence.FindMatchedWords(matchedText);
            Sentence.EvaluateConfidence(matchedWords);
        }

        /// <summary>
        /// Returns a formatted report of all results.
        /// </summary>
        public string FormatAllResults()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(TypeKind.ToString());
            if (WasSuccess())
            {
                builder.Append(" ");
                builder.Append(Sentence.FormatConfidence());
                if (ResultDetailKinds.Length > 0)
                {
                    builder.Append(", ");
                    builder.Append(FormatDetails());
                }

                builder.AppendLine();
                builder.Append("sentence = ");
                builder.Append(Sentence.RecognizedText.SinglyQuoted());
                if (Sentence.Quality != null)
                {
                    builder.Append(", words = ");
                    builder.AppendLine(Sentence.FormatWords());
                    builder.Append("phonemes = ");
                    builder.AppendLine(Sentence.FormatPhonemes());
                    if (Sentence.Problems.Length > 0)
                    {
                        builder.Append("problems = ");
                        builder.AppendLine(Sentence.FormatProblems());
                    }
                }
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns a formatted report of the details.
        /// </summary>
        public string FormatDetails()
        {
            StringBuilder builder = new StringBuilder();
            foreach (ResultDetailKind detail in ResultDetailKinds)
            {
                if (builder.Length > 0) builder.Append(", ");
                builder.Append(detail.ToString());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns a formatted report for the recognized sentence.
        /// </summary>
        public string FormatReport()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(TypeKind.ToString());
            builder.Append(" ");
            builder.Append(Sentence.FormatConfidence());
            if (ResultDetailKinds.Length > 0)
            {
                builder.Append(", ");
                builder.Append(FormatDetails());
            }
            return builder.ToString();
        }

        /// <summary>
        /// Establishes the file name and suffix.
        /// </summary>
        /// <param name="fileName">a file name</param>
        /// <param name="fileSuffix">a file suffix</param>
        /// <returns>this result</returns>
        public RecognitionResult With(string fileName, string fileSuffix)
        {
            RecordedFileName = fileName;
            RecordedFileType = fileSuffix;
            return this;
        }

        /// <summary>
        /// Indicates whether a recognition needs recording in history.
        /// </summary>
        public bool NeedsSave()
        {
            if (TypeKind == ResultKind.RecognitionSucceeded) return true;
            if (TypeKind == ResultKind.RecognitionFailed) return true;
            return false;
        }

        /// <summary>
        /// Indicates whether a recognition was successful.
        /// </summary>
        /// <returns></returns>
        public bool WasSuccess()
        {
            return TypeKind == ResultKind.RecognitionSucceeded;
        }

        /// <summary>
        /// Indicates whether a recognizer was unavailable.
        /// </summary>
        public bool WasUnavailable()
        {
            return TypeKind == ResultKind.RecognitionUnavailable;
        }

        /// <summary>
        /// Indicates whether a recognition error occurred.
        /// </summary>
        public bool WasError()
        {
            return TypeKind == ResultKind.RecognitionError;
        }

        /// <summary>
        /// Indicates whether a recognition was cancelled.
        /// </summary>
        public bool WasCancelled()
        {
            return TypeKind == ResultKind.RecognitionFailed &&
                   ResultDetailKinds.Length > 0 && 
                   ResultDetailKinds[0] == ResultDetailKind.RecognitionCancelled;
        }

        /// <summary>
        /// Adds a result detail to this result.
        /// </summary>
        /// <param name="detail">a detail</param>
        /// <returns>this result</returns>
        public RecognitionResult With(ResultDetailKind detail)
        {
            AddDetail(detail);
            return this;
        }

        /// <summary>
        /// Adds a detail to this result.
        /// </summary>
        /// <param name="detail">a detail</param>
        public void AddDetail(ResultDetailKind detail)
        {
            List<ResultDetailKind> details = new List<ResultDetailKind>(ResultDetailKinds);
            details.Add(detail);
            ResultDetailKinds = details.ToArray();
        }

    } // RecognitionResult

    /// <summary>
    /// Contains audio quality measures.
    /// </summary>
    [Serializable]
    [DataContract]
    public class AudioQuality
    {
        /// <summary>
        /// Indicates the level of the captured audio.
        /// </summary>
        public AudioLevel LevelKind { get; set; }

        [DataMember]
        public string Level
        {
            get { return LevelKind.ToString(); }
            set { LevelKind = value.ToEnum<AudioLevel>(); }
        }

        /// <summary>
        /// Indicates whether the audio was truncated.
        /// </summary>
        public AudioTruncation TruncationKind { get; set; }

        [DataMember]
        public string Truncation
        {
            get { return TruncationKind.ToString(); }
            set { TruncationKind = value.ToEnum<AudioTruncation>(); }
        }

        /// <summary>
        /// The signal to noise ratio in the recognized audio.
        /// </summary>
        [DataMember]
        public float SignalNoiseRatio { get; set; }

        /// <summary>
        /// Indicates the audio noise level.
        /// </summary>
        public AudioNoise NoiseLevelKind { get; set; }

        [DataMember]
        public string NoiseLevel
        {
            get { return NoiseLevelKind.ToString(); }
            set { NoiseLevelKind = value.ToEnum<AudioNoise>(); }
        }

        /// <summary>
        /// The contents of RecClient result.AudioCheckString.
        /// </summary>
        [DataMember]
        public string Miscellany { get; set; }

        [IgnoreDataMember]
        public float NoiseAcceptanceThreshold { get; set; }

        /// <summary>
        /// Constructs a new AudioQuality.
        /// </summary>
        public AudioQuality()
        {
            LevelKind = AudioLevel.Normal;
            NoiseLevelKind = AudioNoise.None;
            TruncationKind = AudioTruncation.None;
            NoiseAcceptanceThreshold = 30;
            SignalNoiseRatio = 0;
            Miscellany = string.Empty;
        }

        /// <summary>
        /// Indicates whether the audio quality warrants a warning.
        /// </summary>
        public bool WarrantsWarning()
        {
            return LevelKind != AudioLevel.Normal ||
                   //NoiseLevelKind > AudioNoise.None ||
                   SignalNoiseRatio < NoiseAcceptanceThreshold ||
                   TruncationKind != AudioTruncation.None;
        }

        /// <summary>
        /// Returns a formatted report of the measures.
        /// </summary>
        public string FormatResults()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Audio Level = ");
            builder.Append(LevelKind.ToString());
            builder.Append(", Audio Noise = ");
            builder.Append(NoiseLevelKind.ToString());
            builder.Append(", Truncation = ");
            builder.Append(TruncationKind.ToString());
            return builder.ToString();
        }

    } // AudioQuality

    /// <summary>
    /// Contains matched sentence information.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SentenceMatch
    {
        /// <summary>
        /// The recognized sentence text.
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public string RecognizedText { get; set; }

        /// <summary>
        /// The recognized sentence interpretation (if available).
        /// </summary>
        [DataMember]
        public string Interpretation { get; set; }

        /// <summary>
        /// The matched answer index.
        /// </summary>
        [DataMember]
        [XmlAttribute]
        public int MatchedIndex { get; set; }

        /// <summary>
        /// The sentence quality measures.
        /// </summary>
        [DataMember]
        [XmlElement]
        public SentenceQuality Quality { get; set; }

        /// <summary>
        /// Any pronunciation problems that were detected.
        /// </summary>
        [DataMember]
        [XmlElement("PronunciationProblem", typeof(PronunciationProblem))]
        public PronunciationProblem[] Problems { get; set; }

        /// <summary>
        /// Constructs a new SentenceMatch.
        /// </summary>
        public SentenceMatch()
        {
            MatchedIndex = -1;
            RecognizedText = string.Empty;
            Interpretation = string.Empty;
            Problems = new PronunciationProblem[0];
            Quality = new SentenceQuality();
        }

        /// <summary>
        /// Returns a formatted report for the recognized words.
        /// </summary>
        public string FormatWords()
        {
            if (Quality == null) return string.Empty;
            string[] words = RecognizedText.SplitOnBlank();
            if (words.Length == 0) return string.Empty;

            int index = 0;
            StringBuilder builder = new StringBuilder();
            foreach (WordQuality word in Quality.Words)
            {
                if (index > 0) builder.Append(" ");
                builder.Append(word.FormatMeasures(words[index]));
                index++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns a formatted report for the recognized phonemes.
        /// </summary>
        public string FormatPhonemes()
        {
            return PhonemeQuality.FormatPhonemes(Quality.Phonemes);
        }
        
        /// <summary>
        /// Returns a formatted report of any pronunciation problems.
        /// </summary>
        public string FormatProblems()
        {
            if (Quality == null) return string.Empty;

            int index = 0;
            StringBuilder builder = new StringBuilder();
            foreach (PronunciationProblem problem in Problems)
            {
                if (index > 0) builder.Append(" ");
                builder.Append(problem.FormatReport());
                index++;
            }

            return builder.ToString();
        }
        
        /// <summary>
        /// Returns a formatted report of the sentence and phrase confidence.
        /// </summary>
        public string FormatConfidence()
        {
            if (Quality == null) return string.Empty;
            StringBuilder builder = new StringBuilder();
            builder.Append("sentence confidence = ");
            builder.Append(Quality.Confidence);

            if (Quality.PhraseConfidence > 0)
            {
                builder.Append(", phrase confidence = ");
                builder.Append(Quality.PhraseConfidence);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Indicates whether a recognition has sufficient confidence.
        /// </summary>
        public bool RecognitionAccepted
        {
            get { return (RecognizedText.Length > 0 && Quality.RecognitionAccepted); }
        }

        /// <summary>
        /// Finds the recognized words that match the supplied text.
        /// </summary>
        /// <param name="matchedText">a text match</param>
        /// <returns>the matched words</returns>
        public WordQuality[] FindMatchedWords(string matchedText)
        {
            int matchedWordCount = matchedText.SplitOnBlank().Length;

            int lowerBound = 0;
            int upperBound = matchedWordCount;

            int matchPosition = RecognizedText.IndexOf(matchedText);
            while (matchPosition > 0 && 
                   RecognizedText[matchPosition - 1] != GrammarExtensions.Blank[0])
            {
                matchPosition--; // find start of contraction if needed
            }

            if (matchPosition > 0)
            {
                string leadingText = RecognizedText.Substring(0, matchPosition - 1);
                string[] leadingWords = leadingText.SplitOnBlank();
                lowerBound += leadingWords.Length;
                upperBound += leadingWords.Length;
            }

            List<WordQuality> results = new List<WordQuality>();
            for (int index = lowerBound; index < upperBound; index++)
            {
                results.Add(Quality.Words[index]);
            }
            return results.ToArray();
        }

        /// <summary>
        /// Evaluates the confidence of a matched phrase.
        /// </summary>
        /// <param name="words">the matched phrase words</param>
        public void EvaluateConfidence(WordQuality[] words)
        {
            float beta = 20480.0f;
            int phoneTotal = 0;
            double confidenceTotal = 0.0;
            foreach (WordQuality word in words)
            {
                double x = InverseSigmoid(word.Confidence, beta);
                confidenceTotal += (x * word.PhoneCount);
                phoneTotal += word.PhoneCount;
            }

            confidenceTotal /= phoneTotal;

            Quality.PhraseConfidence = (int)Sigmoid(confidenceTotal, beta);
        }

        private double Sigmoid(double x, float beta)
        {
            return 100.0 * (1.0 / (1.0 + Math.Exp(-x / beta)));
        }

        private double InverseSigmoid(float x, float beta)
        {
            return -beta * Math.Log((100.0 / x) - 1.0);
        }

        /// <summary>
        /// Updates this with any problems discovered.
        /// </summary>
        /// <param name="sentence">an expected sentence</param>
        /// <param name="history">a past phoneme history</param>
        public void UpdateProblems(string sentence, PhonemeQuality[] history)
        {
            Problems = FindPronunciationProblems(sentence, history);
        }

        /// <summary>
        /// Finds any pronunciation problems for a known sentence.
        /// </summary>
        /// <param name="sentence">an expected sentence</param>
        /// <param name="history">a past phoneme history</param>
        /// <returns>any problems found</returns>
        public PronunciationProblem[] 
            FindPronunciationProblems(string sentence, PhonemeQuality[] history)
        {
            int position = 0;
            int wordIndex = 0;
            int phoneIndex = 0;

            WordQuality[] words = Quality.Words;
            string[] spokenWords = RecognizedText.SplitOnBlank();

            PhonemeQuality[] phonemes = Quality.Phonemes;
            List<PronunciationProblem> results = new List<PronunciationProblem>();
            if (Argument.IsEmpty(history)) return results.ToArray();

            while (wordIndex < words.Length)
            {
                // TODO: 
                // the next line of code synchronizes word positioning 
                // if there are extraneous parts in the sentence esp. (parentheticals)
                //position = sentence.IndexOf(spokenWords[wordIndex], position);

                WordQuality word = words[wordIndex];
                if (word.Accepted)
                {
                    while (phoneIndex < phonemes.Length && !phonemes[phoneIndex].IsEmpty())
                    {
                        PhonemeQuality phoneme = phonemes[phoneIndex];
                        phoneme.UpdateAverage(history);
                        if (!Quality.Accepts(phoneme))
                        {
                            results.Add(CreateProblem(phoneme, position));
                        }

                        position += phoneme.Grapheme.Length;
                        phoneIndex++;
                    }
                }
                else
                {
                    while (phoneIndex < phonemes.Length && !phonemes[phoneIndex].IsEmpty())
                    {
                        PhonemeQuality phoneme = phonemes[phoneIndex];
                        phoneme.UpdateAverage(history);
                        if (phoneme.IsIncluded())
                        {
                            results.Add(CreateProblem(phoneme, position));
                        }

                        position += phoneme.Grapheme.Length;
                        phoneIndex++;
                    }
                }

                // advance to the next word's first phoneme
                while (phoneIndex < phonemes.Length &&
                       phonemes[phoneIndex].IsEmpty())
                       phoneIndex++;

                // advance to the next word's first character
                while (position < sentence.Length &&
                       char.IsWhiteSpace(sentence[position]))
                       position++;

                wordIndex++;
            }

            return results.ToArray();
        }

        /// <summary>
        /// Returns a new problem.
        /// </summary>
        private static PronunciationProblem CreateProblem(PhonemeQuality phoneme, int position)
        {
            PronunciationProblem problem = new PronunciationProblem();
            problem.TypeKind = PronunciationProblemKind.Grapheme;
            problem.Phoneme = phoneme.PhoneName;
            problem.Grapheme = phoneme.Grapheme;
            problem.Offset = position;
            return problem;
        }

    } // SentenceMatch

    /// <summary>
    /// Contains the recognized sentence quality measures.
    /// </summary>
    [Serializable]
    [DataContract]
    public class SentenceQuality
    {
        /// <summary>
        /// The number of frames contained in the recognized sentence.
        /// </summary>
        [DataMember]
        public int FrameCount { get; set; }

        /// <summary>
        /// The recognized phrase confidence.
        /// </summary>
        [DataMember]
        public int PhraseConfidence { get; set; }

        /// <summary>
        /// The confidence level of the recognized sentence.
        /// </summary>
        [DataMember]
        public int Confidence { get; set; }

        /// <summary>
        /// The recognized sentence score.
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// The recognized word measures.
        /// </summary>
        [DataMember]
        public WordQuality[] Words { get; set; }

        /// <summary>
        /// The recognized phoneme measures.
        /// </summary>
        [DataMember]
        public PhonemeQuality[] Phonemes { get; set; }

        /// <summary>
        /// The configured phoneme acceptence threshold.
        /// </summary>
        [DataMember]
        public int PhonemeAcceptanceThreshold { get; set; }

        /// <summary>
        /// The configured phoneme experience threshold.
        /// </summary>
        [DataMember]
        public float PhonemeExperienceThreshold { get; set; }

        /// <summary>
        /// The configured phrase acceptance threshold.
        /// </summary>
        [DataMember]
        public int PhraseAcceptanceThreshold { get; set; }

        /// <summary>
        /// The configured sentence acceptance threshold.
        /// </summary>
        [DataMember]
        public int SentenceAcceptanceThreshold { get; set; }

        /// <summary>
        /// Constructs a new SentenceQuality.
        /// </summary>
        public SentenceQuality()
        {
            Phonemes = new PhonemeQuality[0];
            Words = new WordQuality[0];
            PhraseConfidence = 0;
            FrameCount = 0;
            Confidence = 0;
            Score = 0;
        }

        /// <summary>
        /// Indicates whether a recognition has sufficient confidence.
        /// </summary>
        public bool RecognitionAccepted
        {
            get
            {
                return (Confidence >= SentenceAcceptanceThreshold &&
                        PhraseConfidence >= PhraseAcceptanceThreshold);
            }
        }

        /// <summary>
        /// Indicates whether a phoneme was pronounced with acceptable quality.
        /// </summary>
        /// <param name="phoneme">a phoneme</param>
        /// <returns>whether a phoneme was accepted</returns>
        public bool Accepts(PhonemeQuality phoneme)
        {
            if (phoneme.Average == 0) return true;

            // NOTE: acceptance does not include threshold itself for phonemes
            return (phoneme.Score > PhonemeAcceptanceThreshold ||
                    phoneme.Average > PhonemeExperienceThreshold);
        }

    } // SentenceQuality

    /// <summary>
    /// Contains the quality measures for a recognized word.
    /// </summary>
    [Serializable]
    [DataContract]
    public class WordQuality
    {
        /// <summary>
        /// The number of recognized phonemes.
        /// </summary>
        [DataMember]
        public int PhoneCount { get; set; }

        /// <summary>
        /// The start frame of a word.
        /// </summary>
        [DataMember]
        public int StartFrame { get; set; }

        /// <summary>
        /// The end frame of a word.
        /// </summary>
        [DataMember]
        public int EndFrame { get; set; }

        /// <summary>
        /// The number of frames that cover a word.
        /// </summary>
        [DataMember]
        public int FrameCount { get; set; }

        /// <summary>
        /// A word confidence.
        /// </summary>
        [DataMember]
        public int Confidence { get; set; }

        /// <summary>
        /// A word score.
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Indicates whether confidence was acceptable.
        /// </summary>
        [DataMember]
        public bool Accepted { get; set; }

        /// <summary>
        /// Returns a formatted report for a word.
        /// </summary>
        public string FormatMeasures(string word)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(word);
            builder.Append(" = ");
            builder.Append(PhoneCount);
            builder.Append(",");
            builder.Append(Confidence);
            return builder.ToString();
        }

    } // WordQuality

    /// <summary>
    /// Contains the quality measures for a recognized phoneme.
    /// </summary>
    [Serializable]
    [DataContract]
    public class PhonemeQuality
    {
        private static readonly string[] IncludedPhones =
        {
            "aa", "ae", "ah", "ao", "ay", "aw", "ax",
            "b", "ch", "d", "dh", "dx", "eh", "er", "ey", 
            "f", "g", "hh", "ih", "iy", "jh", "k", "l",
            "m", "n", "ng", "ow", "oy", "p", "r", "s", "sh",
            "t", "th", "uh", "uw", "v", "w", "y", "z", "zh"
        };

        private static readonly string[] ExcludedPhones = 
        {
            string.Empty,
            GrammarExtensions.Dash,
            "error"
        };

        /// <summary>
        /// A phoneme name.
        /// </summary>
        [DataMember]
        public string PhoneName { get; set; }

        /// <summary>
        /// The associated grapheme.
        /// </summary>
        [DataMember]
        public string Grapheme { get; set; }

        /// <summary>
        /// The average of the historical phoneme scores.
        /// </summary>
        [DataMember]
        public float Average { get; set; }

        /// <summary>
        /// A phoneme score.
        /// </summary>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Establishes the average given prior phoneme history.
        /// </summary>
        /// <param name="history">prior phoneme history</param>
        public void UpdateAverage(ICollection<PhonemeQuality> history)
        {
            if (!IsIncluded() || history.Count() == 0) return;
            Average = AverageScore(PhoneName, history);
        }

        /// <summary>
        /// Indicates whether this phoneme is included in evaluations.
        /// </summary>
        public bool IsIncluded()
        {
            return !ExcludedPhones.Contains(PhoneName.Trim());
        }

        /// <summary>
        /// Indicates whether this phoneme is empty.
        /// </summary>
        public bool IsEmpty()
        {
            return (PhoneName.Trim().Length == 0);
        }

        /// <summary>
        /// Indicates whether the associated grapheme is silent.
        /// </summary>
        public bool IsSilent()
        {
            return (PhoneName == GrammarExtensions.Dash);
        }

        /// <summary>
        /// Returns a formatted report for a phoneme.
        /// </summary>
        public string FormatMeasures()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");

            if (!IsEmpty())
            {
                builder.Append(PhoneName);
                builder.Append(",");
                builder.Append(Grapheme);
            }

            if (Score > 0)
            {
                builder.Append(",");
                builder.Append(Score.ToString("N0"));
                builder.Append(",");
                builder.Append(Average.ToString("N2"));
            }

            builder.Append("]");
            return builder.ToString();
        }

        /// <summary>
        /// Returns a formatted report for some phonemes.
        /// </summary>
        public static string FormatPhonemes(PhonemeQuality[] phonemes)
        {
            if (phonemes == null || phonemes.Length == 0) return string.Empty;

            int index = 0;
            StringBuilder builder = new StringBuilder();
            foreach (PhonemeQuality phoneme in phonemes)
            {
                if (index > 0) builder.Append(" ");
                builder.Append(phoneme.FormatMeasures());
                index++;
            }

            return builder.ToString();
        }

        /// <summary>
        /// Returns a formatted report of average phoneme scores.
        /// </summary>
        public static string FormatHistory(PhonemeQuality[] history)
        {
            if (history == null || history.Length == 0) return string.Empty;

            string[] phones = (from phoneme in history select phoneme.PhoneName)
                                .Distinct().OrderBy(item => item).ToArray();

            StringBuilder builder = new StringBuilder();
            foreach (string phone in phones)
            {
                if (builder.Length > 0) builder.Append(" ");
                float average = AverageScore(phone, history);
                builder.Append("[");
                builder.Append(phone);
                builder.Append(",");
                builder.Append(average);
                builder.Append("]");
            }
            return builder.ToString();
        }

        /// <summary>
        /// Returns the average score for a given phoneme and score history.
        /// </summary>
        /// <param name="phoneName">a phone name</param>
        /// <param name="history">a score history</param>
        /// <returns>an average score, or zero</returns>
        public static float AverageScore(string phoneName, ICollection<PhonemeQuality> history)
        {
            PhonemeQuality[] phonemeHistory =
                (from priorAttempt in history
                 where priorAttempt.PhoneName == phoneName
                 select priorAttempt).ToArray();

            if (phonemeHistory.Count() > 0)
            {
                return (float)phonemeHistory.Average(item => item.Score);
            }
            else
            {
                return 0.0f;
            }
        }

    } // PhonemeQuality

    /// <summary>
    /// Indicates and locates a pronunciation problem relative to the start of a sentence.
    /// </summary>
    [Serializable]
    [DataContract]
    public class PronunciationProblem
    {
        /// <summary>
        /// Indicates which kind of problem this represents.
        /// </summary>
        public PronunciationProblemKind TypeKind { get; set; }

        [DataMember]
        public string Type
        {
            get { return TypeKind.ToString(); }
            set { TypeKind = value.ToEnum<PronunciationProblemKind>(); }
        }

        /// <summary>
        /// A grapheme or word.
        /// </summary>
        [DataMember]
        public string Grapheme { get; set; }

        /// <summary>
        /// A phoneme or empty.
        /// </summary>
        [DataMember]
        public string Phoneme { get; set; }

        /// <summary>
        /// A character offset within a sentence.
        /// </summary>
        [DataMember]
        public int Offset { get; set; }

        /// <summary>
        /// Constructs a new PronunciationProblem.
        /// </summary>
        public PronunciationProblem()
        {
            TypeKind = PronunciationProblemKind.Word;
            Grapheme = string.Empty;
            Phoneme = string.Empty;
            Offset = 0;
        }

        /// <summary>
        /// Returns a formatted report of this problem.
        /// </summary>
        public string FormatReport()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("[");
            if (Phoneme.Length > 0)
            {
                builder.Append(Phoneme);
                builder.Append(",");
            }

            builder.Append(Grapheme);
            builder.Append(",");
            builder.Append(Offset.ToString());
            builder.Append("]");
            return builder.ToString();
        }

    } // PronunciationProblem
}
