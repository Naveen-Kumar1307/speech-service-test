using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using EduSpeak;
using Common.Logging;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.Services
{
    /// <summary>
    /// Recognizes a wave encoded speech utterance.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a recognition initialization package</item>
    /// <item>translates a speech utterance into text from a known grammar</item>
    /// </list>
    /// </remarks>
    public class EduSpeakRecognizer : ISpeechRecognitionService
    {
        /// <summary>
        /// The recognizer factory.
        /// </summary>
        public static readonly Factory ServiceFactory = new Factory();

        private static readonly ILog Logger = LogManager.GetLogger(typeof(EduSpeakRecognizer));

        private static readonly string Equal = "=";
        private static readonly string Comma = ",";
        private static readonly string Blank = " ";
        private static readonly string Slash = "/";
        private static readonly string BackSlash = "\\";
        private static readonly string Separators = Comma + Blank;
        private static readonly string OggFileType = ".ogg"; // ogg-speex format file

        private static readonly string DynamicGrammar = "DynamicGrammar";
        private static readonly string EduSpeakPrefix = "EduSpeak.";
        private static readonly string EduSpeakPackage = "EduSpeak.package";

        private static readonly string OperationalPrefix = "client.";
        private static readonly string ScoringInitialized = "client.ScoringInitialized";
        private static readonly string SegmentationAvailable = "result.SegmentationAvailable";
        private static readonly string PhonemeCounts = "result.SegmentationNumPhonesInWords";
        private static readonly string PhonemeScores = "result.SegmentationPhoneScores";
        private static readonly string WorkFolderName = "client.DynamicGrammarOutputDirectory";

        private static readonly string AudioValid = "audiocheck.valid";
        private static readonly string AudioLevel = "audiocheck.level";
        private static readonly string AudioTruncation = "audiocheck.truncation";
        private static readonly string AudioSignalLevel = "audiocheck.SNR";
        private static readonly string AudioMiscellany = "result.AudioCheckString";
        private static readonly string EnableEndpoints = "EnableEndpoints";

        private static readonly string NoiseAcceptance = "NoiseAcceptanceThreshold";
        private static readonly string PhraseAcceptance = "PhraseAcceptanceThreshold";
        private static readonly string SentenceAcceptance = "SentenceAcceptanceThreshold";
        private static readonly string PhonemeAcceptance = "PhonemeAcceptanceThreshold";
        private static readonly string PhonemeExperience = "PhonemeExperienceThreshold";
        private static readonly string PhonemeOccurrence = "PhonemeOccurrenceThreshold";
        private static readonly string PhonemeCoverage = "RequiredPhonemeCoveragePercentage";
        private static readonly string WordConfidence = "WordConfidenceThresholds";

        private static readonly string WorkFolderPath =
                                ConfiguredValue.Named("RecognizerWorkFolder", "/SiteLogs/EduSpeak");

        /// <summary>
        /// Returns the configured word confidence thresholds.
        /// </summary>
        /// <returns></returns>
        private static int[] GetWordConfidenceThresholds()
        {
            string[] thresholds = ConfiguredValue.Named(WordConfidence)
                                    .Split(", ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            return (from threshold in thresholds select threshold.To<int>()).ToArray();
        }

        /// <summary>
        /// The default recognition package folder path.
        /// </summary>
        public static string DefaultPackagePath = "/Data/grammars/GE";

        /// <summary>
        /// The default initializations (command line settings).
        /// </summary>
        public static string DefaultInitializations =
            "config.DebugLevel=4 " +
            "rec.SegmentResults=2 " +
            "rec.Interpret=TRUE " +
            "audio.Provider=null " +
            "audio.native.ControlVolume=0 " +
            "";

        /// <summary>
        /// The default operational settings (post-initialization).
        /// </summary>
        public static string DefaultOperationalSettings =
            "client.RecordDirectory={0} " + 
            "client.DynamicGrammarOutputDirectory={0} " + 
            "client.DynamicGrammarCanContainNL=1 " +
            "client.DynamicGrammarCanCallStaticGrammars=1 " +
            "client.ScoringDirectory=english-p16-na-050930.s101004 " +
            //"client.WriteLogfile=1 " +
            "";

        public static string ExtraOperationalSettings = string.Empty;

        private static readonly int InitializationTimeout = 10000; // msecs
        private static readonly int RecognitionTimeout = 15000; // msecs
        private static readonly int[] NoiseThresholds = { 10, 21, 30 };

        private AutoResetEvent CoordinationEvent { get; set; }
        private RecClient SpeechRecognizer { get; set; }
        private DirectoryInfo WorkFolder { get; set; }

        private int[] WordConfidenceThresholds { get; set; }
        private int SentenceAcceptanceThreshold { get; set; }
        private int PhraseAcceptanceThreshold { get; set; }
        private int PhonemeAcceptanceThreshold { get; set; }
        private float PhonemeExperienceThreshold { get; set; }
        private int PhonemeOccurrenceThreshold { get; set; }
        private int RequiredPhonemeCoveragePercentage { get; set; }
        private float NoiseAcceptanceThreshold { get; set; }

        #region creating instances
        /// <summary>
        /// Returns an initialized EduSpeakRecognizer.
        /// </summary>
        /// <returns>an initialized EduSpeakRecognizer</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// if the supplied recognition package folder doesn't exist</exception>
        public static EduSpeakRecognizer InitializePackage()
        {
            return ServiceFactory.CreateRecognizer();
        }

        /// <summary>
        /// Returns an initialized EduSpeakRecognizer.
        /// </summary>
        /// <returns>an initialized EduSpeakRecognizer</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// if the default recognition package folder doesn't exist</exception>
        public static EduSpeakRecognizer InitializeWithDefaultPackage()
        {
            return Initialize(DefaultPackagePath);
        }

        /// <summary>
        /// Returns an initialized EduSpeakRecognizer.
        /// </summary>
        /// <param name="packagePath">a recognition package path</param>
        /// <returns>an initialized EduSpeakRecognizer</returns>
        /// <exception cref="DirectoryNotFoundException">
        /// if the supplied recognition package folder doesn't exist</exception>
        public static EduSpeakRecognizer Initialize(string packagePath)
        {
            return ServiceFactory.CreateRecognizer(packagePath);
        }

        /// <summary>
        /// Constructs a new EduSpeakRecognizer.
        /// </summary>
        /// <param name="commandLine">engine initializations</param>
        private EduSpeakRecognizer(string commandLine) : this()
        {
            SpeechRecognizer.InitializeCommandLine(commandLine);
            WaitForInitialization();
            ReportInitialization(commandLine);
        }

        /// <summary>
        /// Constructs a new EduSpeakRecognizer.
        /// </summary>
        private EduSpeakRecognizer()
        {
            CoordinationEvent = new AutoResetEvent(false);
            SpeechRecognizer = new RecClient();

            NoiseAcceptanceThreshold = ConfiguredValue.Get<float>(NoiseAcceptance, 30.0f);
            SentenceAcceptanceThreshold = ConfiguredValue.Get<int>(SentenceAcceptance);
            PhraseAcceptanceThreshold = ConfiguredValue.Get<int>(PhraseAcceptance);
            PhonemeAcceptanceThreshold = ConfiguredValue.Get<int>(PhonemeAcceptance);
            PhonemeExperienceThreshold = ConfiguredValue.Get<float>(PhonemeExperience);
            PhonemeOccurrenceThreshold = ConfiguredValue.Get<int>(PhonemeOccurrence);
            RequiredPhonemeCoveragePercentage = ConfiguredValue.Get<int>(PhonemeCoverage);
            WordConfidenceThresholds = GetWordConfidenceThresholds();

            RegisterEventListeners();
        }

        /// <summary>
        /// Terminates and disposes of this recognizer.
        /// </summary>
        public void Dispose()
        {
            if (SpeechRecognizer != null)
            {
                ReportShutdown();

                SpeechRecognizer.Terminate(true);
                CoordinationEvent.WaitOne();
                SpeechRecognizer = null;

                CoordinationEvent.Close();
                CoordinationEvent = null;

                if (WorkFolder != null && WorkFolder.Exists)
                    WorkFolder.Delete(true);
            }
        }

        /// <summary>
        /// Prepares this recognizer for use.
        /// </summary>
        private void Prepare(string[] namedValues)
        {
            foreach (string namedValue in namedValues)
            {
                PrepareValue(namedValue);
            }

            ReportOperationalSettings(namedValues);
        }

        /// <summary>
        /// Adds a configured value to the recognizer.
        /// </summary>
        private void PrepareValue(string namedValue)
        {
            string[] parts = namedValue.Split(Equal[0]);
            string valueName = parts[0];
            string valuePart = parts[1];
            if (valuePart.Length == 0) return;

            if (char.IsDigit(valuePart[0]))
            {
                SpeechRecognizer.SetIntParameter(valueName, int.Parse(valuePart));
            }
            else
            {
                SpeechRecognizer.SetStringParameter(valueName, valuePart);
            }
        }
        #endregion

        #region handling events
        /// <summary>
        /// Registers the recognizer event handlers.
        /// </summary>
        private void RegisterEventListeners()
        {
            SpeechRecognizer.OnInitComplete +=
                new RecClient.OnInitCompleteHandler(HandleInitialization);

            SpeechRecognizer.OnFinalResult +=
                new RecClient.OnFinalResultHandler(HandleResult);

            SpeechRecognizer.OnEvent += new RecClient.OnEventHandler(HandleEvent);
        }

        /// <summary>
        /// Note a state change during initialization.
        /// </summary>
        /// <param name="newState">a new state</param>
        private void HandleInitialState(RecognizerState newState)
        {
            ReportState(newState);
            if (IsReady) SignalCompletion();
        }

        /// <summary>
        /// Note completion of the initialization.
        /// </summary>
        /// <param name="status">engine status</param>
        private void HandleInitialization(NuanceStatus status)
        {
            ReportStatus(status);
        }

        /// <summary>
        /// Note completion of a recognition.
        /// </summary>
        /// <param name="result">a recognition result</param>
        private void HandleResult(RecResult result)
        {
            ReportRecognition(result);
        }

        /// <summary>
        /// Note an event.
        /// </summary>
        /// <param name="eventType">an event type</param>
        /// <param name="data">event payload</param>
        private void HandleEvent(NuanceEvent eventType, IntPtr data)
        {
            ReportEvent(eventType);
            if (eventType == NuanceEvent.NUANCE_EVENT_FINAL_RESULT)
            {
                SignalCompletion();
            }
        }

        /// <summary>
        /// Note a state change.
        /// </summary>
        /// <param name="newState">a new state</param>
        private void HandleState(RecognizerState newState)
        {
            ReportState(newState);
            if (newState == RecognizerState.SRI_NORECOGNIZER)
            {
                SignalCompletion();
            }
        }
        #endregion

        #region coordinating operations
        /// <summary>
        /// Waits (only so long) for initialization.
        /// </summary>
        private void WaitForInitialization()
        {
            ReportInitializing(InitializationTimeout);

            SpeechRecognizer.OnStateChange +=
                new RecClient.OnStateChangeHandler(HandleInitialState);

            bool done = CoordinationEvent.WaitOne(InitializationTimeout);
            if (done)
            {
                // swap state change listener
                SpeechRecognizer.OnStateChange -=
                    new RecClient.OnStateChangeHandler(HandleInitialState);

                SpeechRecognizer.OnStateChange +=
                    new RecClient.OnStateChangeHandler(HandleState);
            }
            else
            {
                ReportInitializationTimeout(InitializationTimeout);
            }
        }

        /// <summary>
        /// Waits for recognition completion.
        /// </summary>
        private void WaitForRecognition()
        {
            bool completed = CoordinationEvent.WaitOne(RecognitionTimeout);
            if (!completed)
            {
                Logger.Warn("Recognition timed out after " + RecognitionTimeout + " msecs");
            }
        }

        /// <summary>
        /// Signals completion of a recognizer activity.
        /// </summary>
        private void SignalCompletion()
        {
            CoordinationEvent.Set();
        }

        /// <summary>
        /// Indicates whether the recognizer is ready.
        /// </summary>
        private bool IsReady
        {
            get { return GetRecognizerState() == RecognizerState.SRI_READY; }
        }

        /// <summary>
        /// Indicates that no recognizer was properly initialized.
        /// </summary>
        private bool NoRecognizer
        {
            get { return GetRecognizerState() == RecognizerState.SRI_NORECOGNIZER; }
        }

        /// <summary>
        /// Returns the recognizer state.
        /// </summary>
        private RecognizerState GetRecognizerState()
        {
            return SpeechRecognizer.State;
        }

        /// <summary>
        /// Returns the recognizer result.
        /// </summary>
        private RecResult GetRecognitionResult()
        {
            return SpeechRecognizer.recResult;
        }
        #endregion

        #region accessing values
        /// <summary>
        /// Indicates whether scoring data is available.
        /// </summary>
        public bool ScoringEnabled
        {
            get { return (SpeechRecognizer.GetIntParameter(ScoringInitialized) > 0); }
        }

        /// <summary>
        /// Indicates whether segmentation data is available.
        /// </summary>
        public bool SegmentationEnabled
        {
            get { return (GetSegmentationAvailability().ToLower().To<bool>()); }
        }

        /// <summary>
        /// Returns the segmentation availability setting.
        /// </summary>
        private string GetSegmentationAvailability()
        {
            return SpeechRecognizer.GetStringParameter(SegmentationAvailable);
        }

        /// <summary>
        /// Returns the sentence score.
        /// </summary>
        private int GetSentenceScore()
        {
            return (int)SpeechRecognizer.GetSentenceScore();
        }

        /// <summary>
        /// Returns the number of phonemes for each word in the recognized sentence.
        /// </summary>
        private int[] GetPhonemeCounts()
        {
            return SpeechRecognizer.GetStringParameter(PhonemeCounts).SplitAsIntegers();
        }

        /// <summary>
        /// Returns the number of phonemes for each word in the recognized sentence.
        /// </summary>
        private int[] GetPhonemeScores()
        {
            return SpeechRecognizer.GetStringParameter(PhonemeScores).SplitAsIntegers();
        }

        /// <summary>
        /// Indicates whether audio quality values are valid.
        /// </summary>
        private bool IsAudioValid
        {
            get
            {
                try
                {
                    return (SpeechRecognizer.GetIntParameter(AudioValid) == 1);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message);
                    return false;
                }
            }
        }

        /// <summary>
        /// Returns the detected audio level.
        /// </summary>
        private AudioLevel GetAudioLevel()
        {
            int result = SpeechRecognizer.GetIntParameter(AudioLevel);
            return result.ConstrainedToEnum<AudioLevel>();
        }

        /// <summary>
        /// Returns whether the audio was truncated.
        /// </summary>
        private AudioTruncation GetAudioTruncation()
        {
            int result = SpeechRecognizer.GetIntParameter(AudioTruncation);
            return result.ConstrainedToEnum<AudioTruncation>();
        }

        /// <summary>
        /// Returns the detected audio signal to noise ratio.
        /// </summary>
        private float GetSignalNoiseRatio()
        {
            return SpeechRecognizer.GetFloatParameter(AudioSignalLevel);
        }

        /// <summary>
        /// Returns the miscellaneous audio quality measures.
        /// </summary>
        private string GetAudioMiscellany()
        {
            return SpeechRecognizer.GetStringParameter(AudioMiscellany);
        }
        #endregion

        #region recognizing speech
        /// <summary>
        /// Indicates whether to use endpoint detection during recognition.
        /// </summary>
        private static readonly bool EndpointingEnabled =
                                     ConfiguredValue.Get<bool>(EnableEndpoints, false);

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public RecognitionResult RecognizeSpeech(string grammar, string filePath, byte[] audioData)
        {
            if (Argument.IsPresent(filePath) && filePath.EndsWith(OggFileType))
            {
                return RecognizeFile(grammar, filePath);
            }
            else
            {
                return RecognizeData(grammar, filePath, audioData);
            }
        }

        /// <summary>
        /// Recognizes audio from a given file.
        /// </summary>
        /// <param name="grammar">a grammar</param>
        /// <param name="filePath">a file path</param>
        /// <returns>a RecognitionResult</returns>
        private RecognitionResult RecognizeFile(string grammar, string filePath)
        {
            Argument.Check("filePath", filePath);
            if (!File.Exists(filePath))
                return RecognitionResult.WithMissingFile(Path.GetFileName(filePath));

            string preparedGrammar = PrepareGrammar(grammar);
            if (preparedGrammar.Length == 0)
                return RecognitionResult.WithInvalidGrammar();

            try
            {
                // EduSpeak prefers '/' for folder name separators
                filePath = filePath.Replace(BackSlash[0], Slash[0]);

                Logger.Debug("Starting recognition ...");
                SpeechRecognizer.RecognizeFile(preparedGrammar, filePath, EndpointingEnabled);
                WaitForRecognition();
                return BuildResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RecognitionResult.WithError(ex);
            }
        }

        /// <summary>
        /// Recognizes audio from supplied PCM data.
        /// </summary>
        /// <param name="grammar">a grammar</param>
        /// <param name="filePath">a file path</param>
        /// <param name="audioData">PCM audio data</param>
        /// <returns>a RecognitionResult</returns>
        private RecognitionResult RecognizeData(string grammar, string filePath, byte[] audioData)
        {
            string fileName = "missing.wav";
            if (Argument.IsPresent(filePath))
                fileName = Path.GetFileName(filePath);

            if (Argument.IsEmpty(audioData))
                return RecognitionResult.WithMissingFile(fileName);

            string preparedGrammar = PrepareGrammar(grammar);
            if (preparedGrammar.Length == 0)
                return RecognitionResult.WithInvalidGrammar();

            try
            {
                Logger.Debug("Starting recognition ...");
                SpeechRecognizer.RecognizeData(
                    preparedGrammar, filePath, audioData, EndpointingEnabled);

                WaitForRecognition();
                return BuildResult();
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RecognitionResult.WithError(ex);
            }
        }

        /// <summary>
        /// Prepares a grammar for use (if needed).
        /// </summary>
        /// <param name="grammar">a grammar (or grammar name)</param>
        /// <returns>a grammar name, or empty</returns>
        private string PrepareGrammar(string grammar)
        {
            return (grammar.IsGrammarName() ? grammar : PrepareDynamicGrammar(grammar));
        }

        /// <summary>
        /// Prepares a dynamic grammar for use (if accepted by the recognizer).
        /// </summary>
        /// <param name="grammar">a grammar</param>
        /// <returns>a grammar name, or empty</returns>
        private string PrepareDynamicGrammar(string grammar)
        {
            if (Argument.IsAbsent(grammar))
            {
                ReportGrammarProblem(string.Empty, null);
                return string.Empty; // reject missing grammar
            }

            try
            {
                // detect whether grammar has alternatives
                string grammarText = grammar.WithoutClosures();
                if (grammarText.Length < grammar.Length)
                {
                    Logger.Debug("Dynamic Grammar " + grammar);
                    SpeechRecognizer.SetupDynamicGrammarForSentence(DynamicGrammar, grammar.ToLower());
                }
                else
                {
                    // only one statement, so force alignment
                    grammar = grammar.WithoutTermEnds();
                    Logger.Debug("Forced Alignment Grammar " + grammar);
                    SpeechRecognizer.SetupForcedAlignmentForSentence(DynamicGrammar, grammar.ToLower());
                }

                return GrammarExtensions.FixedGrammar;
            }
            catch (Exception ex)
            {
                ReportGrammarProblem(grammar, ex);
                return string.Empty; // grammar was rejected
            }
        }

        /// <summary>
        /// Builds a recognition result from a completed recognition.
        /// </summary>
        private RecognitionResult BuildResult()
        {
            RecResult recognition = GetRecognitionResult();
            RecognitionResult result = new RecognitionResult();
            result.AudioMeasure = BuildAudioQuality();
            result.Sentence = BuildMatch();

            if (result.AudioMeasure.WarrantsWarning())
            {
                result.AddDetail(ResultDetailKind.AudioQualityWarning);
            }

            if (result.Sentence.RecognizedText.Length > 0)
            {
                result.TypeKind = ResultKind.RecognitionSucceeded;
            }
            else
            {
                result.TypeKind = ResultKind.RecognitionFailed;
                result.AddDetail(ResultDetailKind.NoRecognition);
            }

            ReportAnalysis(result);
            return result;
        }

        /// <summary>
        /// Indicates whether the recognition result warrants analysis.
        /// </summary>
        /// <param name="recognitionText">a recognition result</param>
        private bool ResultWarrantsAnalysis(string recognitionText)
        {
            return (recognitionText.Length > 0 &&
                    ScoringEnabled && SegmentationEnabled);
        }

        /// <summary>
        /// Builds an audio quality result.
        /// </summary>
        private AudioQuality BuildAudioQuality()
        {
            AudioQuality result = new AudioQuality();
            result.NoiseAcceptanceThreshold = NoiseAcceptanceThreshold;

            if (IsAudioValid)
            {
                result.LevelKind = GetAudioLevel();
                //result.Truncation = GetAudioTruncation();
                result.SignalNoiseRatio = GetSignalNoiseRatio();
                result.NoiseLevelKind = GetNoiseLevel((int)result.SignalNoiseRatio);
                result.Miscellany = GetAudioMiscellany();
            }

            return result;
        }

        /// <summary>
        /// Returns the noise level given a signal / noise ratio.
        /// </summary>
        private AudioNoise GetNoiseLevel(int ratio)
        {
            for (int index = 0; index < NoiseThresholds.Length; index++)
            {
                if (ratio < NoiseThresholds[index])
                    return (AudioNoise)(NoiseThresholds.Length - index);
            }

            return AudioNoise.None;
        }

        /// <summary>
        /// Builds a match result from a completed recognition.
        /// </summary>
        private SentenceMatch BuildMatch()
        {
            RecResult recognition = GetRecognitionResult();
            SentenceMatch result = new SentenceMatch();
            result.RecognizedText = recognition.GetString(0);

            NLResult nlResult = new NLResult();
            recognition.GetNLResult(0, nlResult);
            if (nlResult.NumberOfFilledSlots > 0)
            {
                result.Interpretation = nlResult.InterpretationString;
                ReportInterpretation(result.Interpretation);
            }

            if (result.RecognizedText.Length > 0)
            {
                result.Quality = BuildSentenceQuality();
            }

            return result;
        }

        /// <summary>
        /// Builds a sentence quality from a completed recognition.
        /// </summary>
        private SentenceQuality BuildSentenceQuality()
        {
            RecResult recognition = GetRecognitionResult();
            SentenceQuality result = new SentenceQuality();
            result.Confidence = recognition.GetOverallConfidence(0);
            result.PhonemeAcceptanceThreshold = PhonemeAcceptanceThreshold;
            result.PhonemeExperienceThreshold = PhonemeExperienceThreshold;
            result.PhraseAcceptanceThreshold = PhraseAcceptanceThreshold;
            result.SentenceAcceptanceThreshold = SentenceAcceptanceThreshold;

            // determine whether further analysis is possible
            if (!ResultWarrantsAnalysis(recognition.GetString(0))) return result;

            result.Score = GetSentenceScore();
            //SpeechRecognizer.ComputeGraphemeScores(19);

            // build the word measures
            int[] phoneCounts = GetPhonemeCounts();
            int wordCount = SpeechRecognizer.NumWordsInSentence;
            if (wordCount > 0)
            {
                result.Words = new WordQuality[wordCount];
                for (int word = 0; word < wordCount; word++)
                {
                    result.Words[word] = BuildWord(word, phoneCounts[word], recognition);
                }
            }

            // determine how many items are available
            int phoneCount = SpeechRecognizer.NumPhonesInSentence;
            int graphCount = SpeechRecognizer.NumGraphemesInSentence;
            int count = Math.Max(phoneCount, graphCount);

            if (count > 0)
            {
                // get the available phoneme scores
                int[] phoneScores = GetPhonemeScores();
                int scoreIndex = 0;

                // build the phoneme measures
                result.Phonemes = new PhonemeQuality[count];
                for (int index = 0; index < count; index++)
                {
                    PhonemeQuality phoneme = BuildPhoneme(index);
                    if (!phoneme.IsEmpty() && !phoneme.IsSilent())
                    {
                        phoneme.Score = phoneScores[scoreIndex];

                        // advance scoreIndex without exceeding the bounds of phoneScores
                        if (scoreIndex < phoneScores.Length - 1) scoreIndex++;
                    }

                    result.Phonemes[index] = phoneme;
                }
            }

            return result;
        }

        /// <summary>
        /// Builds a word quality for the indicated word.
        /// </summary>
        private WordQuality BuildWord(int index, int phoneCount, RecResult recognition)
        {
            WordQuality result = new WordQuality();
            result.PhoneCount = phoneCount;
            result.Confidence = recognition.GetWordConfidence(0, index);
            result.Score = (int)SpeechRecognizer.GetWordScore(index);

            int threshold = Math.Min(result.PhoneCount, WordConfidenceThresholds.Length - 1);
            result.Accepted = !(result.Confidence < WordConfidenceThresholds[threshold]);
            return result;
        }

        /// <summary>
        /// Builds a phoneme quality for the indicated phoneme.
        /// </summary>
        private PhonemeQuality BuildPhoneme(int index)
        {
            float score = 0.0f;
            string phoneName = "";
            string graphName = "";
            SpeechRecognizer.ScoringGetVectors(ref phoneName, ref graphName, ref score, index);

            PhonemeQuality result = new PhonemeQuality();
            result.PhoneName = phoneName.Trim();
            result.Grapheme = graphName.Trim();
            return result;
        }
        #endregion

        #region reporting operations
        /// <summary>
        /// Reports initialization start.
        /// </summary>
        private void ReportInitializing(int timeout)
        {
            Logger.Debug("Initializing EduSpeak, waiting " + timeout + " msecs " + GetHashCode());
        }

        /// <summary>
        /// Reports initialization completion.
        /// </summary>
        private void ReportInitialization(string commandLine)
        {
            if (IsReady)
                Logger.Info("Started EduSpeak with " + commandLine);
        }

        /// <summary>
        /// Reports initialization timeout.
        /// </summary>
        private void ReportInitializationTimeout(int timeout)
        {
            Logger.Warn("EduSpeak initialization timed out after " + timeout + " msecs");
        }

        /// <summary>
        /// Reports the operational settings used to configure EduSpeak.
        /// </summary>
        private void ReportOperationalSettings(string[] namedValues)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("Settings ");
            foreach (string namedValue in namedValues)
            {
                builder.Append(namedValue);
                builder.Append(Blank);
            }
            Logger.Info(builder.ToString());
        }

        /// <summary>
        /// Reports shutdown.
        /// </summary>
        private void ReportShutdown()
        {
            Logger.Info("Shutting down EduSpeak");
        }

        /// <summary>
        /// Reports an event.
        /// </summary>
        private void ReportEvent(NuanceEvent eventType)
        {
            Logger.Debug("Event = " + eventType.ToString());
        }

        /// <summary>
        /// Reports a state change.
        /// </summary>
        private void ReportState(RecognizerState state)
        {
            Logger.Debug("State = " + state.ToString());
        }

        /// <summary>
        /// Reports a status change.
        /// </summary>
        private void ReportStatus(NuanceStatus status)
        {
            Logger.Debug("Status = " + status.ToString());
        }

        /// <summary>
        /// Reports a recognition completion.
        /// </summary>
        private void ReportRecognition(RecResult result)
        {
            Logger.Debug("Result = " + result.GetString(0).SinglyQuoted());
        }

        /// <summary>
        /// Reports analysis of a recognition.
        /// </summary>
        private void ReportAnalysis(RecognitionResult result)
        {
            LogMessage(result.AudioMeasure.FormatResults());
            LogMessage(result.AudioMeasure.Miscellany);

            StringBuilder builder = new StringBuilder();
            builder.Append(result.FormatReport());
            if (ResultWarrantsAnalysis(result.Sentence.RecognizedText))
            {
                builder.Append(", ");
                builder.Append(result.Sentence.FormatWords());
                LogMessage(builder.ToString());
                LogMessage(result.Sentence.FormatPhonemes());
            }
            else
            {
                LogMessage(builder.ToString());
            }
        }

        /// <summary>
        /// Reports a natural language interpretation.
        /// </summary>
        private void ReportInterpretation(string text)
        {
            Logger.Debug("Interpretation = " + text);
        }

        /// <summary>
        /// Logs a message at either the INFO or DEBUG level.
        /// </summary>
        private void LogMessage(string message)
        {
            string processPort = Environment.GetEnvironmentVariable("RECPORT");
            if (Argument.IsAbsent(processPort))
            {
                Logger.Debug(message);
            }
            else
            {
                Logger.Info(message);
            }
        }
        #endregion

        #region reporting problems
        /// <summary>
        /// Reports a startup failure.
        /// </summary>
        private static Exception ReportStartupFailure()
        {
            return new InvalidOperationException("EduSpeak could not be properly initialized");
        }

        /// <summary>
        /// Reports a grammar problem.
        /// </summary>
        private void ReportGrammarProblem(string grammar, Exception ex)
        {
            if (Logger.IsDebugEnabled && ex != null)
            {
                Logger.Error("Recognition rejected grammar " + grammar.SinglyQuoted(), ex);
            }
            else
            {
                Logger.Error("Recognition rejected grammar " + grammar.SinglyQuoted());
            }
        }
        #endregion


        /// <summary>
        /// Creates and initializes EduSpeak instances.
        /// </summary>
        public class Factory
        {
            private static readonly string NUANCE = "NUANCE";
            private static readonly string PackagePrefix = "-package ";

            private Dictionary<string, string> Configuration = new Dictionary<string,string>();
            private EduSpeakRecognizer CachedRecognizer = null; // a lazy singleton

            public string ProcessWorkFolderPath { get; private set; }

            /// <summary>
            /// Clears the cached recognizer.
            /// </summary>
            public void ClearCache()
            {
                if (CachedRecognizer != null)
                {
                    CachedRecognizer.Dispose();
                    CachedRecognizer = null;
                }
            }

            /// <summary>
            /// Returns a new EduSpeakRecognizer.
            /// </summary>
            /// <returns>a new EduSpeakRecognizer</returns>
            [MethodImpl(MethodImplOptions.Synchronized)]
            public EduSpeakRecognizer CreateRecognizer()
            {
                string configuredPackage = 
                        ConfiguredValue.Named(EduSpeakPackage, DefaultPackagePath);

                if (CachedRecognizer == null)
                    CachedRecognizer = CreateRecognizer(configuredPackage);

                return CachedRecognizer;
            }

            /// <summary>
            /// Returns a new EduSpeakRecognizer.
            /// </summary>
            /// <param name="packagePath">a package path</param>
            /// <returns>a new EduSpeakRecognizer</returns>
            public EduSpeakRecognizer CreateRecognizer(string packagePath)
            {
                BuildConfiguration();

                DirectoryInfo packageFolder = FindPackageFolder(packagePath);
                SetNuanceFolder(packageFolder);

                string commandLine = BuildCommandLine(packageFolder);
                EduSpeakRecognizer result = new EduSpeakRecognizer(commandLine);

                if (result.NoRecognizer)
                {
                    result.Dispose();
                    throw ReportStartupFailure();
                }
                else
                {
                    result.Prepare(GetConfiguredOperationalValues());
                    result.WorkFolder = new DirectoryInfo(GetConfiguredValue(WorkFolderName));
                }

                return result;
            }

            /// <summary>
            /// Finds a package folder.
            /// </summary>
            /// <param name="packagePath">a package folder path</param>
            /// <returns>a package folder</returns>
            /// <exception cref="DirectoryNotFoundException">
            /// if the supplied path cannot be used to locate a folder in the file system
            /// </exception>
            private DirectoryInfo FindPackageFolder(string packagePath)
            {
                DirectoryInfo folder = new DirectoryInfo(ResourceFile.ConvertPath(packagePath));
                if (!folder.Exists) // check for absolute path
                {
                    folder = ResourceFile.FindFolder(packagePath);
                    if (folder == null || !folder.Exists) // check for relative path
                        throw new DirectoryNotFoundException("Can't find " + packagePath);
                }

                return folder;
            }

            /// <summary>
            /// Verifies that EduSpeak can load its master packages.
            /// </summary>
            /// <param name="packageFolder">a package folder</param>
            private void SetNuanceFolder(DirectoryInfo packageFolder)
            {
                string folderPath = packageFolder.Parent.FullName;
                RCAPI.SetEnvironmentVariable(NUANCE, folderPath);
                string nuancePath = Environment.GetEnvironmentVariable(NUANCE);
                if (nuancePath == folderPath)
                    Logger.Info("Set NUANCE=" + folderPath);
                else
                    Logger.Warn("NUANCE not set to " + folderPath);
            }

            /// <summary>
            /// Builds the EduSpeak command line for initialization.
            /// </summary>
            /// <param name="packageFolder">a package folder</param>
            /// <returns>the command line arguments for initialization</returns>
            private string BuildCommandLine(DirectoryInfo packageFolder)
            {
                string packagePath = packageFolder.FullName;
                if (packagePath.Contains(Blank)) packagePath = packagePath.Quoted();
                return PackagePrefix + packagePath + BuildCommandLineSettings();
            }

            /// <summary>
            /// Builds the EduSpeak configuration.
            /// </summary>
            private void BuildConfiguration()
            {
                string operationalSettings = 
                    string.Format(DefaultOperationalSettings, GetWorkFolderPath());

                Configuration.Clear();
                BuildConfiguration(DefaultInitializations);
                BuildConfiguration(operationalSettings);
                BuildConfiguration(GetConfiguredValues());

                if (ExtraOperationalSettings.Length > 0)
                {
                    BuildConfiguration(ExtraOperationalSettings);
                }
            }

            /// <summary>
            /// Adds some settings to the EduSpeak configuration.
            /// </summary>
            /// <param name="settings">some settings</param>
            private void BuildConfiguration(string settings)
            {
                BuildConfiguration(GetNamedValues(settings));
            }

            /// <summary>
            /// Returns the named values contained in some settings.
            /// </summary>
            private string[] GetNamedValues(string settings)
            {
                return settings.Split(Separators.ToCharArray(), 
                                StringSplitOptions.RemoveEmptyEntries);
            }

            /// <summary>
            /// Adds named values to the configuration.
            /// </summary>
            /// <param name="namedValues">a named value</param>
            private void BuildConfiguration(string[] namedValues)
            {
                foreach (string namedValue in namedValues)
                {
                    string[] parts = namedValue.Split(Equal[0]);
                    Configuration[parts[0].Trim()] = namedValue.Trim();
                }
            }

            /// <summary>
            /// Builds the command line settings.
            /// </summary>
            private string BuildCommandLineSettings()
            {
                List<string> excluded = new List<string>(GetConfiguredOperationalValueNames());
                excluded.Add(EduSpeakPackage.Substring(EduSpeakPrefix.Length));

                StringBuilder builder = new StringBuilder();
                foreach (string valueName in Configuration.Keys)
                {
                    if (!excluded.Contains(valueName))
                    {
                        builder.Append(Blank); // always
                        builder.Append(Configuration[valueName]);
                    }
                }

                return builder.ToString();
            }

            /// <summary>
            /// Returns the configured operational values.
            /// </summary>
            private string[] GetConfiguredOperationalValues()
            {
                List<string> results = new List<string>();
                foreach (string valueName in GetConfiguredOperationalValueNames())
                {
                    results.Add(Configuration[valueName]);
                }
                return results.ToArray();
            }

            /// <summary>
            /// Returns the configured operational value names.
            /// </summary>
            private string[] GetConfiguredOperationalValueNames()
            {
                return (from valueName in Configuration.Keys
                        where valueName.StartsWith(OperationalPrefix)
                        select valueName).ToArray();
            }

            /// <summary>
            /// Returns the configured EduSpeak values.
            /// </summary>
            private string[] GetConfiguredValues()
            {
                List<string> results = new List<string>();
                foreach (string valueName in GetConfiguredValueNames())
                {
                    string namedValue = ConfiguredValue.Named(valueName);
                    string revisedName = valueName.Substring(EduSpeakPrefix.Length);
                    results.Add(revisedName + Equal + namedValue);
                }
                return results.ToArray();
            }

            /// <summary>
            /// Returns the names of the configured EduSpeak settings.
            /// </summary>
            private string[] GetConfiguredValueNames()
            {
                return (from valueName in ConfiguredValue.ConfiguredNames 
                        where valueName.StartsWith(EduSpeakPrefix) 
                        select valueName).ToArray();
            }

            /// <summary>
            /// Returns a configured value.
            /// </summary>
            /// <param name="valueName">a value name</param>
            /// <returns>a configured value</returns>
            private string GetConfiguredValue(string valueName)
            {
                if (Argument.IsAbsent(valueName) ||
                    !Configuration.ContainsKey(valueName)) 
                    return string.Empty;

                string namedValue = Configuration[valueName.Trim()];
                return namedValue.Substring(valueName.Length + 1).Trim();
            }

            /// <summary>
            /// Returns the EduSpeak work folder path.
            /// </summary>
            private string GetWorkFolderPath()
            {
                string basePath = ResourceFile.ConvertPath(WorkFolderPath);
                string processFolder = Process.GetCurrentProcess().Id.ToString();
                string workFolderPath = Path.Combine(basePath, processFolder);
                return new DirectoryInfo(workFolderPath).CreateIfMissing().FullName;
            }

        } // Factory

    } // EduSpeakRecognizer
}
