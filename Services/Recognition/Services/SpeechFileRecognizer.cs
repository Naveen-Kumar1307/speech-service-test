using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;

using Common.Logging;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.Services
{
    /// <summary>
    /// Recognizes speech contained in an audio file.
    /// </summary>
    public class SpeechFileRecognizer : ISpeechRecognitionService, 
                                        ISpeechFileRecognitionService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SpeechFileRecognizer));
        private static readonly string NumberFormat = "N0";

        private static readonly string OggFileType = ".ogg";
        private static readonly string Mpeg3FileType = ".mp3";
        private static readonly string SpeechFileType = ".wav";
        private static readonly string FlashFileType = ".flv";
        private static readonly string SpeexFileType = ".spx";
        private static readonly string SampleFileType = ".pcm.wav";

        private static readonly string StandardAudioFileType =
                                ConfiguredValue.Named("StandardAudioFileType", SpeechFileType);

        private static readonly bool ServiceLaunchedRecognizer =
                                ConfiguredValue.Get<bool>("ServiceLaunchedRecognizer", true);

        /// <summary>
        /// The TCP port for this process.
        /// </summary>
        private static readonly string ProcessPort = Environment.GetEnvironmentVariable("RECPORT");

        #region creating instances
        /// <summary>
        /// Constructs a new SpeechRecognizer.
        /// </summary>
        public SpeechFileRecognizer()
        {
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (SpeechRecognizers.Count > 0)
            {
                Dictionary<string, ISpeechRecognitionService> copy = CopyRecognizers();
                SpeechRecognizers.Clear();
                foreach (ISpeechRecognitionService recognizer in copy.Values) recognizer.Dispose();
            }
        }

        /// <summary>
        /// Returns a copy of the recognizers.
        /// </summary>
        private Dictionary<string, ISpeechRecognitionService> CopyRecognizers()
        {
            return new Dictionary<string, ISpeechRecognitionService>(SpeechRecognizers);
        }
        #endregion

        #region accessing recognizers
        /// <summary>
        /// A recognizer map (file type -> recognizer).
        /// </summary>
        private Dictionary<string, ISpeechRecognitionService> SpeechRecognizers =
            new Dictionary<string, ISpeechRecognitionService>();

        /// <summary>
        /// A recognition service for speech data with an unknown format (ogg, mp3, ...).
        /// </summary>
        public ISpeechRecognitionService SpeechRecognizer
        {
            get { return SpeechRecognizers[SpeechFileType]; }
            set
            {
                SpeechRecognizers[SpeechFileType] = value;
                SpeechRecognizers[Mpeg3FileType] = value;
            }
        }

        /// <summary>
        /// A recognition service for Flash formatted data.
        /// </summary>
        public ISpeechRecognitionService FlashRecognizer
        {
            get { return SpeechRecognizers[FlashFileType]; }
            set { SpeechRecognizers[FlashFileType] = value; }
        }

        /// <summary>
        /// A recognition service for Speex formatted data.
        /// </summary>
        public ISpeechRecognitionService SpeexRecognizer
        {
            get { return SpeechRecognizers[SpeexFileType]; }
            set { SpeechRecognizers[SpeexFileType] = value; }
        }

        /// <summary>
        /// A recognition service for PCM formatted data.
        /// </summary>
        public ISpeechRecognitionService SampleRecognizer
        {
            get { return SpeechRecognizers[SampleFileType]; }
            set
            {
                SpeechRecognizers[SampleFileType] = value;
                SpeechRecognizers[OggFileType] = value;
            }
        }
        #endregion

        #region recognizing speech
        /// <inheritdoc/>
        public RecognitionResult RecognizeFile(string filePath, string grammar)
        {
            try
            {
                if (Argument.IsAbsent(filePath)) // connection test
                {
                    ReportPing();
                    return RecognitionResult.WithAvailable();
                }

                if (Argument.IsAbsent(grammar))
                    grammar = GrammarExtensions.FixedGrammar;

                ResourceFile sampleFile = GetSampleFile(filePath);
                if (sampleFile == null) // failure already reported
                    return RecognitionResult.WithMissingFile(filePath);

                return RecognizeSpeech(sampleFile, grammar);
            }
            catch (Exception ex)
            {
                ReportRecognitionFailure(filePath, grammar, ex);
                return RecognitionResult.WithError(ex);
            }
        }

        /// <summary>
        /// Recognizes the speech contained in a file.
        /// </summary>
        /// <param name="sampleFile">a sample audio file</param>
        /// <param name="grammar">a grammar</param>
        /// <returns>a recognition result</returns>
        private RecognitionResult RecognizeSpeech(ResourceFile sampleFile, string grammar)
        {
            byte[] speechData = { };
            if (!sampleFile.FullName.EndsWith(OggFileType))
            {
                speechData = sampleFile.GetResourceBinary();
            }

            return RecognizeSpeech(grammar, sampleFile.FullName, speechData);
        }

        /// <summary>
        /// Recognizes the supplied speech data.
        /// </summary>
        /// <param name="grammar">a grammar</param>
        /// <param name="fileName">a sample file name</param>
        /// <param name="speechData">a speech sample</param>
        /// <returns>a recognition result</returns>
        public RecognitionResult RecognizeSpeech(string grammar, string filePath, byte[] speechData)
        {
            ReportRecognition(filePath);

            if (speechData == null) speechData = new byte[0];

            return TimeRecognition(grammar, filePath, speechData);
        }


        /// <summary>
        /// Times a recognition.
        /// </summary>
        private RecognitionResult TimeRecognition(
            string grammar, string filePath, byte[] speechData)
        {
            RecognitionResult result = new RecognitionResult();
            TimeSpan span = DateTime.Now.TimeToRun(delegate()
            {
                result = Recognize(grammar, filePath, speechData);
            });

            result.RecognitionTime = (int)span.TotalMilliseconds;
            ReportRecognition(filePath, span, speechData.Length, 0);

            return result;
        }

        /// <summary>
        /// Recognizes speech with an appropriate recognizer.
        /// </summary>
        private RecognitionResult Recognize(string grammar, string filePath, byte[] speechData)
        {
            Logger.Debug("Loaded audio data from " + filePath.SinglyQuoted());
            ISpeechRecognitionService recognizer = GetRecognizer(filePath);

            if (recognizer == null)
            {
                ReportMissingRecognizer(filePath);
                string message = "Missing recognizer for " + filePath;
                return RecognitionResult.WithError(message);
            }

            Logger.Debug("Recognizing with " + recognizer.GetType().Name);
            return recognizer.RecognizeSpeech(grammar, filePath, speechData);
        }

        /// <summary>
        /// Returns an appropriate recognizer.
        /// </summary>
        /// <param name="sampleFile">a sample file</param>
        /// <returns>an appropriate recognizer, or null</returns>
        private ISpeechRecognitionService GetRecognizer(string fileName)
        {
            if (fileName.EndsWith(SampleFileType))
                return SpeechRecognizers[SampleFileType];

            string fileType = Path.GetExtension(fileName);
            return (SpeechRecognizers.ContainsKey(fileType) ? 
                    SpeechRecognizers[fileType] : null);
        }
        #endregion

        #region accessing resources
        /// <summary>
        /// Returns the sample audio file.
        /// </summary>
        /// <param name="filePath">a file path</param>
        /// <returns>a ResourceFile, or null</returns>
        private ResourceFile GetSampleFile(string filePath)
        {
            FileInfo sampleFile = new FileInfo(filePath);
            if (!sampleFile.Exists)
            {
                sampleFile = ResourceFile.FindFile(filePath);
                if (sampleFile == null)
                {
                    ReportSampleLocationFailure(filePath);
                    return null;
                }
            }

            return ResourceFile.Named(sampleFile.FullName);
        }

        /// <summary>
        /// Returns the grammar from a specified file.
        /// </summary>
        private string GetGrammar(string grammarFile)
        {
            FileInfo grammarPath = ResourceFile.FindFile(grammarFile);
            return ResourceFile.Named(grammarPath.FullName).GetResourceText();
        }
        #endregion

        #region reporting operations
        /// <summary>
        /// Reports a ping from a client.
        /// </summary>
        private void ReportPing()
        {
            Logger.Debug("Ping test: process is alive.");
        }

        /// <summary>
        /// Reports a recognition start.
        /// </summary>
        private void ReportRecognition(string fileName)
        {
            LogMessage("Starting recognition " + fileName.SinglyQuoted());
        }

        /// <summary>
        /// Reports a recognition.
        /// </summary>
        private void ReportRecognition(
            string fileName, TimeSpan duration, int bufferLength, long peakSpace)
        {
            object[] arguments = {
                GetType().Name, fileName.SinglyQuoted(), 
                duration.TotalMilliseconds.ToString(NumberFormat)
            };

            string message = string.Format("{0} recognized {1} in {2} msecs", arguments);
            LogMessage(message);
        }

        /// <summary>
        /// Logs a message at either the INFO or DEBUG level.
        /// </summary>
        private void LogMessage(string message)
        {
            if (Argument.IsAbsent(ProcessPort))
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
        /// Reports a missing recognizer.
        /// </summary>
        private void ReportMissingRecognizer(string filePath)
        {
            Logger.Error("Recognizer missing for file " + filePath.SinglyQuoted());
        }

        /// <summary>
        /// Reports a recognition failure.
        /// </summary>
        private void ReportRecognitionFailure(string filePath, string grammar, Exception ex)
        {
            Logger.Error("Recognition failed on file " + 
                         filePath.SinglyQuoted() + " with grammar " + grammar, ex);
        }

        /// <summary>
        /// Reports a failure to locate a sample audio file.
        /// </summary>
        private void ReportSampleLocationFailure(string filePath)
        {
            Logger.Error("Can't located file " + filePath.SinglyQuoted());
        }
        #endregion

    } // SpeechFileRecognizer
}
