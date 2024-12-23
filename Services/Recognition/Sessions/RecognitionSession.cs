using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Common.Logging;
using GlobalEnglish.Recognition.Repository;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Spring;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.Sessions
{
    /// <summary>
    /// A speech recognition session.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>maintains a registry of active sessions</item>
    /// <item>analyzes the results of a recognition</item>
    /// <item>maintains a phoneme history for a user</item>
    /// <item>knows the location (folder) of the client audio files</item>
    /// <item>knows the file path for a client audio file (recorded utterance)</item>
    /// </list>
    /// </remarks>
    public abstract class RecognitionSession
    {
        private static readonly Type ClassType = typeof(RecognitionSession);
        private static readonly ILog Logger = LogManager.GetLogger(ClassType);

        private static readonly string WildType = ".*";
        public static readonly string OggSuffix = ".ogg";

        /// <summary>
        /// The standard audio file type.
        /// </summary>
        public static string StandardAudioFileType =
            ConfiguredValue.Named("StandardAudioFileType", OggSuffix);

        private static readonly bool RecycleGarbage = 
            ConfiguredValue.Get<bool>("RecycleGarbage", false);

        private static readonly int ExperienceCount = 
            ConfiguredValue.Get<int>("PhonemeExperience", 8);

        protected static int ActiveRecognitionCount = 0;

        /// <summary>
        /// The active session registry.
        /// </summary>
        protected static readonly Dictionary<string, RecognitionSession> Registry = 
                              new Dictionary<string, RecognitionSession>();

        /// <summary>
        /// A speech repository.
        /// </summary>
        public static ISpeechRepository Repository = new SpeechRepository();

        /// <summary>
        /// The audio sample storage folder path.
        /// </summary>
        public static string AudioFolderPath { get; private set; }


        /// <summary>
        /// A session ID.
        /// </summary>
        public string ClientId { get; protected set; }

        /// <summary>
        /// An audio file name.
        /// </summary>
        public string AudioFileName { get; protected set; }

        /// <summary>
        /// A recognition request.
        /// </summary>
        public RecognitionRequest Request { get; protected set; }

        /// <summary>
        /// A recognition result.
        /// </summary>
        public RecognitionResult Result { get; protected set; }

        /// <summary>
        /// A cached snapshot of the latest phoneme history.
        /// </summary>
        private PhonemeQuality[] PhonemeHistory { get; set; }

        /// <summary>
        /// A practice analyst.
        /// </summary>
        private IPracticeAnalyst PracticeAnalyst { get; set; }

        #region creating instances
        /// <summary>
        /// Initializes this class.
        /// </summary>
        static RecognitionSession()
        {
            if (Argument.IsAbsent(AudioFolderPath))
            {
                AudioFolderPath = @"D:\Speech";
            }
        }

        /// <summary>
        /// Returns a known recognition session.
        /// </summary>
        /// <param name="clientId">a client ID</param>
        /// <returns>a RecognitionSession, or null</returns>
        public static SessionType Named<SessionType>(string clientId) 
                where SessionType : RecognitionSession
        {
            if (Argument.IsAbsent(clientId)) return null;

            lock (Registry)
            {
                return (Registry.ContainsKey(clientId) ? 
                        Registry[clientId] : null) as SessionType;
            }
        }

        /// <summary>
        /// Constructs a new RecognitionSession.
        /// </summary>
        protected RecognitionSession()
        {
            Request = new RecognitionRequest() { UserId = "-1" };
            ClientId = GetHashCode().ToString();
        }
        #endregion

        #region managing sessions
        /// <summary>
        /// Registers this session.
        /// </summary>
        public virtual RecognitionSession RegisterSession()
        {
            lock (Registry)
            {
                Registry[ClientId] = this;
            }

            return this;
        }

        /// <summary>
        /// Removes this session from the registry.
        /// </summary>
        public virtual bool RemoveSession()
        {
            bool removed = RemoveRegistry();

            if (removed)
            {
                DeleteSampleFileIfConfigured();
            }
            return removed;
        }

        public bool RemoveRegistry()
        {
            bool removed = false;
            lock (Registry)
            {
                removed = Registry.Remove(ClientId);
            }
            return removed;
        }

        public virtual bool RemoveSessionOnUploadCompleted(bool DoesBlobAudioUploadRequired)
        {
            bool removed = RemoveRegistry();

            if (removed && !DoesBlobAudioUploadRequired)
            {
                DeleteSampleFileIfConfigured();
            }

            return removed;
        }

        /// <summary>
        /// Returns all registered session of a given type.
        /// </summary>
        /// <typeparam name="SessionType">a session type</typeparam>
        /// <returns>the registered sessions</returns>
        public static IList<SessionType> GetSessions<SessionType>() 
            where SessionType : RecognitionSession
        {
            lock (Registry)
            {
                List<SessionType> results = new List<SessionType>();
                foreach (SessionType session in Registry.Values)
                {
                    if (session != null) results.Add(session);
                }
                return results;
            }
        }
        #endregion

        #region managing audio files
        /// <summary>
        /// Initializes the location and type of audio files.
        /// </summary>
        /// <param name="audioFolderPath">a folder path</param>
        /// <param name="fileType">an audio file type</param>
        public static void Initialize(string audioFolderPath, string fileType)
        {
            StandardAudioFileType = fileType;
            AudioFolderPath = ResourceFile.ConvertPath(audioFolderPath);
            DirectoryInfo folder = new DirectoryInfo(AudioFolderPath).CreateIfMissing();
        }

        /// <summary>
        /// Removes the sample audio file if so configured.
        /// </summary>
        protected void DeleteSampleFileIfConfigured()
        {
            if (RecycleGarbage)
            {
                DeleteSampleFiles();
            }
        }

        /// <summary>
        /// Removes the sample audio file.
        /// </summary>
        private void DeleteSampleFiles()
        {
            try
            {
                DirectoryInfo folder = new DirectoryInfo(AudioFolderPath);
                FileInfo[] files = folder.GetFiles(AudioFileName + WildType);
                if (files.Length > 0)
                {
                    foreach (FileInfo file in files)
                    {
                        if (file.Exists) file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                ReportFileRemovalFailure(ex);
            }
        }
        #endregion

        #region analyzing speech
        /// <summary>
        /// Analyzes a recognition result.
        /// </summary>
        protected void AnalyzeResult()
        {
            if (Result.WasSuccess())
            {
                ConfigureAnalyst();
                PracticeAnalyst.AnalyzePractice(Request, Result);
            }
        }

        /// <summary>
        /// Configures an appropriate practice analyst.
        /// </summary>
        private void ConfigureAnalyst()
        {
            switch (Request.RecognitionTypeKind)
            {
                case RecognitionKind.CommunicationPractice:
                    PracticeAnalyst = new CommunicationAnalyst();
                    return;

                case RecognitionKind.PronunciationPractice:
                    PhonemeHistory = GetPhonemeHistory();
                    PracticeAnalyst = PronunciationAnalyst.With(PhonemeHistory);
                    return;
            }
        }
        #endregion

        #region maintaining phoneme history
        /// <summary>
        /// Returns the recent phoneme history for the user.
        /// </summary>
        public PhonemeQuality[] GetPhonemeHistory()
        {
            PhonemeQuality[] empty = { };
            if (Repository == null) return empty;

            try
            {
                return Repository.GetRecentPhonemes(UserId, ExperienceCount).ToArray();
            }
            catch (Exception ex)
            {
                ReportDatabaseProblem(ex);
                return empty;
            }
        }

        /// <summary>
        /// Saves the results of a recognition.
        /// </summary>
        private void SaveResultIfNeeded()
        {
            if (Repository == null) return;

            if (Result.NeedsSave())
            {
                try
                {
                    Repository.SaveAsrAttempt(UserId, 1, Request, Result);
                }
                catch (Exception ex)
                {
                    ReportDatabaseProblem(ex);
                }
            }
        }

        /// <summary>
        /// Saves the results of a recognition.
        /// </summary>
        private void SaveHistoryIfNeeded()
        {
            if (Repository == null) return;

            if (Result.NeedsSave())
            {
                try
                {
                    Repository.SaveAsrAttemptHistory(UserId, 1, Request, Result);
                }
                catch (Exception ex)
                {
                    ReportTableProblem(ex);
                }
            }
        }
        #endregion

        #region accessing values
        /// <summary>
        /// The registered session count.
        /// </summary>
        public static int RegisteredSessionCount
        {
            get { return Registry.Count; }
        }

        /// <summary>
        /// Indicates whether this session is registered.
        /// </summary>
        public bool IsRegistered
        {
            get { return Registry.ContainsKey(ClientId); }
        }

        /// <summary>
        /// The user ID.
        /// </summary>
        public int UserId
        {
            get { return int.Parse(Request.UserId); }
        }

        /// <summary>
        /// Indicates whether the expected audio file is present.
        /// </summary>
        public bool AudioPresent
        {
            get
            {
                string filePath = GetFilePath();
                FileInfo file = new FileInfo(filePath);
                return (file.Exists && file.Length > 0);
            }
        }

        /// <summary>
        /// Returns the recorded audio file path.
        /// </summary>
        public virtual string GetFilePath()
        {
            return Path.Combine(AudioFolderPath, AudioFileName + Request.AudioFormat);
        }

        /// <summary>
        /// Returns the grammar associated with a given client.
        /// </summary>
        public string GetGrammar()
        {
            if (Request == null) return GrammarExtensions.FixedGrammar;
            return Request.GetNormalizedGrammar();
        }

        #endregion

        #region logging operations
        /// <summary>
        /// Formats and logs an ERROR message.
        /// </summary>
        public void FormatError(string message)
        {
            string messageText = string.Format(message, UserId, ClientId);
            LogError(messageText);
        }

        /// <summary>
        /// Formats and logs an ERROR message.
        /// </summary>
        public void FormatError(string message, Exception ex)
        {
            string messageText = string.Format(message, UserId, ClientId);
            LogError(messageText, ex);
        }

        /// <summary>
        /// Formats and logs a WARN message.
        /// </summary>
        public void FormatWarn(string message)
        {
            string messageText = string.Format(message, UserId, ClientId);
            LogWarn(messageText);
        }

        /// <summary>
        /// Formats and logs an INFO message.
        /// </summary>
        public void FormatInfo(string message)
        {
            string messageText = string.Format(message, UserId, ClientId);
            LogInfo(messageText);
        }

        /// <summary>
        /// Formats and logs a DEBUG message.
        /// </summary>
        public void FormatDebug(string message)
        {
            string messageText = string.Format(message, UserId, ClientId);
            LogDebug(messageText);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        protected virtual void LogError(string message)
        {
            Logger.Error(message);
        }

        /// <summary>
        /// Logs a formatted error message.
        /// </summary>
        protected virtual void LogError(string message, Exception ex)
        {
            Logger.Error(message, ex);
        }

        /// <summary>
        /// Logs a formatted warning message.
        /// </summary>
        protected virtual void LogWarn(string message)
        {
            Logger.Warn(message);
        }

        /// <summary>
        /// Logs a formatted informative message.
        /// </summary>
        protected virtual void LogInfo(string message)
        {
            Logger.Info(message);
        }

        /// <summary>
        /// Logs a formatted debug message.
        /// </summary>
        protected virtual void LogDebug(string message)
        {
            Logger.Debug(message);
        }
        #endregion

        #region reporting operations
        /// <summary>
        /// Reports recognition start.
        /// </summary>
        protected void ReportRecognitionStart()
        {
            FormatInfo("started recognition user {0} file " + AudioFileName.SinglyQuoted());
        }

        /// <summary>
        /// Reports a recognition file and grammar.
        /// </summary>
        protected void ReportFileGrammar(string grammar)
        {
            string[] args = { UserId.ToString(), AudioFileName.SinglyQuoted(), grammar };
            LogInfo(string.Format("user {0} file = {1}, grammar = {2}", args));
        }


        /// <summary>
        /// Reports recognition results.
        /// </summary>
        protected void ReportRecognitionResults(TimeSpan duration)
        {
            SaveHistoryIfNeeded();

            WorkerThread.SpawnBackground(delegate()
            {
                SaveResultIfNeeded();
                ReportRecognitionTime((int)duration.TotalMilliseconds);
                ReportRecognitionDetails();

            });
        }

        /// <summary>
        /// Reports a recognition response time.
        /// </summary>
        /// <param name="responseTime">a response time (in msecs)</param>
        protected void ReportRecognitionTime(int responseTime)
        {
            string[] args = 
            {
                Result.TypeKind.ToString(), responseTime.ToString("N0"),
                UserId.ToString(), ClientId, AudioFileName.SinglyQuoted()
            };

            string message =
            string.Format("{0} took {1} msecs user {2} on connection {3} file {4}", args);

            if (Result.WasError() || Result.WasUnavailable())
            {
                LogError(message);
            }
            else
            {
                LogInfo(message);
            }
        }

        /// <summary>
        /// Reports the recognition result details.
        /// </summary>
        protected void ReportRecognitionDetails()
        {
            if (Result.Sentence == null) return;
            if (Result.Sentence.Quality != null)
            {
                LogDebug(Result.Sentence.FormatConfidence());
                FormatDebug("user {0} words: " + Result.Sentence.FormatWords());

                if (Request.IsPronunciationPractice())
                {
                    if (Request.ExpectedResults.Length > 0) FormatDebug("user {0} answer: " + Request.ExpectedResults[0].FullAnswer);
                    FormatDebug("user {0} phonemes: " + Result.Sentence.FormatPhonemes());
                    FormatDebug("user {0} history: " + PhonemeQuality.FormatHistory(PhonemeHistory));
                }

                if (Result.Sentence.Problems.Length > 0)
                {
                    FormatDebug("user {0} problems: " + Result.Sentence.FormatProblems());
                }
            }

            if (Result.Sentence.MatchedIndex >= 0)
            {
                ReportSentenceMatch();
            }
        }

        /// <summary>
        /// Reports a sentence match.
        /// </summary>
        protected void ReportSentenceMatch()
        {
            int index = Result.Sentence.MatchedIndex;
            string answer = Request.ExpectedResults.Length == 0 ? "" : Request.ExpectedResults[index].Answer.SinglyQuoted();
            string sentence = Result.Sentence.RecognizedText.SinglyQuoted();
            object[] arguments = { UserId, sentence, answer };
            string message = string.Format("user {0} recognized result {1} matches {2}", arguments);
            LogDebug(message);
        }

        /// <summary>
        /// Reports residual file removal.
        /// </summary>
        protected static void ReportResidualFileRemoval(int count)
        {
            object[] arguments = { count, AudioFolderPath };
            string message = string.Format("deleted {0} residual audio files from {1}", arguments);
            Logger.Info(message);
        }

        /// <summary>
        /// Reports a file removal failure.
        /// </summary>
        protected void ReportFileRemovalFailure(Exception ex)
        {
            object[] arguments = { UserId, GetFilePath() };
            string message = string.Format("audio delete failed for user {0} path {1}", arguments);
            LogWarn(message);
        }

        /// <summary>
        /// Reports a database access problem.
        /// </summary>
        protected void ReportDatabaseProblem(Exception ex)
        {
            FormatError("database access failed for user {0}", ex);
        }

        /// <summary>
        /// Reports a table access problem.
        /// </summary>
        protected void ReportTableProblem(Exception ex)
        {
            FormatError("table access failed for user {0}", ex);
        }
        #endregion

    } // RecognitionSession
}
