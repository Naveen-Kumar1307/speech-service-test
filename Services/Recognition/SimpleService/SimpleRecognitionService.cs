using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using log4net;
using log4net.Config;
using log4net.Appender;

using GlobalEnglish.Recognition.Services;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.Sessions;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Spring;
using GlobalEnglish.Utility.Xml;
using GlobalEnglish.Denali.Util;

namespace GlobalEnglish.Recognition.SimpleService
{
    /// <summary>
    /// Recognizes speech audio and provides feedback.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>translates a spoken utterance into the equivalent textual words</item>
    /// <item>provides feedback based on pronunciation history</item>
    /// </list>
    /// </remarks>
    [ServiceBehavior(
        ConcurrencyMode=ConcurrencyMode.Multiple,
        InstanceContextMode = InstanceContextMode.Single)]
    public class SimpleRecognitionService : ISimpleRecognitionService
    {
        private static readonly string Dot = ".";
        private static readonly string Dash = "-";
        private static readonly string Slash = "/";
        private static readonly string BarSeparator = "|";
        private static readonly string AnswerSeparator = ";";
        private static readonly string AudioHeader = "audio";

        private static readonly Type ClassType = typeof(SimpleRecognitionService);

        private static readonly string ConversionFolderPath =
                                ConfiguredValue.Named("AudioConversionFolder", "D:/Speech");

        private static ILog Logger = null;
        private static RecognitionService PhonemeService = null;

        #region initializing service
        /// <summary>
        /// Prepares this recognizer process for use.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        static SimpleRecognitionService()
        {
            RegisterFaultHandlers();
            InitializeLogging();

            try
            {
                RecognitionSession.Initialize(ConversionFolderPath, StandardAudioFileType);
                SimpleRecognitionSession.GetConfiguredRecognizer();
                PhonemeService = new RecognitionService();

                Logger.Warn("Started SimpleRecognitionService");
                Denali.Util.Logger.WriteWarning("Started SimpleRecognitionService");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
            }
        }

        /// <summary>
        /// Initializes the logging for this process.
        /// </summary>
        private static void InitializeLogging()
        {
            int processID = Process.GetCurrentProcess().Id;

            // customize each file appender with the process ID
            foreach (FileAppender appender in GetFileAppenders())
            {
                string fileName = Path.GetFileNameWithoutExtension(appender.File);
                appender.File = appender.File.Replace(fileName, fileName + Dash + processID);
                appender.ActivateOptions();
            }

            Logger = LogManager.GetLogger(ClassType);
        }

        /// <summary>
        /// Returns the configured file appenders.
        /// </summary>
        private static IEnumerable<IAppender> GetFileAppenders()
        {
            XmlConfigurator.Configure();
            return from appender in LogManager.GetRepository().GetAppenders()
                   where appender is FileAppender
                   select appender;
        }
        #endregion

        #region handling faults
        /// <summary>
        /// Registers the standard fault handlers.
        /// </summary>
        private static void RegisterFaultHandlers()
        {
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(HandleUnload);
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(HandleExit);
            AppDomain.CurrentDomain.UnhandledException +=
                                new UnhandledExceptionEventHandler(HandleFault);
        }

        /// <summary>
        /// Handles an otherwise unhandled exception.
        /// </summary>
        private static void HandleFault(object source, UnhandledExceptionEventArgs payload)
        {
            ReportFault(payload.ExceptionObject);
        }

        private static void HandleUnload(object source, EventArgs payload)
        {
            ReportExit();
        }

        /// <summary>
        /// Handles an exit.
        /// </summary>
        private static void HandleExit(object source, EventArgs payload)
        {
            ReportExit();
        }
        #endregion

        #region ISimpleRecognitionService Members
        /// <inheritdoc/>
        public RecognitionResult RecognizeSpeechStream(Stream audioData)
        {
            try
            {
                RecognitionRequest request = GetRequestFromCurrentContext();
                return RecognizeSpeech(request, ReadStreamCompletely(audioData));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                return RecognitionResult.WithError(ex);
            }
        }

        /// <inheritdoc/>
        public RecognitionResult RecognizeSpeech(RecognitionRequest request, string audioData)
        {
            if (audioData.Length < 20)
            {
                return RecognitionResult.WithMissingFile(MissingFileName);
            }

            if (Argument.IsAbsent(request.AudioFormat))
            {
                request.AudioFormat = StandardAudioFileType;
            }

            byte[] audioBuffer = Convert.FromBase64String(audioData);
            return RecognizeSpeech(request, audioBuffer);
        }

        /// <inheritdoc/>
        public bool HasEnoughPhonemeHistory(string userId)
        {
            return PhonemeService.HasEnoughPhonemeHistory(userId);
        }

        /// <inheritdoc/>
        public List<string> GetTroublePhonemes(string userId, int resultLimit)
        {
            return PhonemeService.GetTroublePhonemes(userId, resultLimit);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            ReportExit();
        }
        #endregion

        #region accessing values
        /// <summary>
        /// The standard audio file type.
        /// </summary>
        public static string StandardAudioFileType
        {
            get { return RecognitionSession.StandardAudioFileType; }
        }

        /// <summary>
        /// The name for missing files.
        /// </summary>
        public string MissingFileName
        {
            get { return "missing" + StandardAudioFileType; }
        }
        #endregion

        #region recognizing speech
        /// <summary>
        /// Recognizes the supplied speech.
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="audioBuffer">an audio buffer containing speech data</param>
        /// <returns>a RecognitionResult</returns>
        private RecognitionResult RecognizeSpeech(RecognitionRequest request, byte[] audioBuffer)
        {
            Logger.Debug("received data length = " + audioBuffer.Length);
            return RecognizeSpeech(SimpleRecognitionSession.From(request, audioBuffer));
        }

        /// <summary>
        /// Recognizes the speech included in the supplied session.
        /// </summary>
        /// <param name="session">a recognition session</param>
        /// <returns>a RecognitionResult</returns>
        private RecognitionResult RecognizeSpeech(SimpleRecognitionSession session)
        {
            if (session == null)
            {
                return RecognitionResult.WithMissingFile(MissingFileName);
            }
            else
            {
                return session.RecognizeSample();
            }
        }

        /// <summary>
        /// Builds a recognition request from the current web operation context.
        /// </summary>
        private RecognitionRequest GetRequestFromCurrentContext()
        {
            IncomingWebRequestContext context = WebOperationContext.Current.IncomingRequest;

            string fileType = StandardAudioFileType;
            string[] segments = context.ContentType.Split(Slash[0]);
            if (segments[0].ToLower() == AudioHeader)
            {
                fileType = Dot + segments[1].ToLower();
            }

            RecognitionRequest request = new RecognitionRequest();
            request.AudioFormat = fileType;
            request.UserId = context.Headers["UserId"];
            request.Grammar = context.Headers["Grammar"];
            request.RecognitionType = context.Headers["RecognitionType"];

            if (context.Headers["ExpectedResults"] != null)
            {
                string[] expectations = context.Headers["ExpectedResults"].Split(BarSeparator[0]);
                List<ExpectedResult> results = new List<ExpectedResult>();
                foreach (string expectation in expectations)
                {
                    string[] answers = expectation.Split(AnswerSeparator[0]);
                    ExpectedResult result = new ExpectedResult();
                    result.Answer = answers[0].Trim();
                    result.FullAnswer = answers[1].Trim();
                    results.Add(result);
                }

                request.ExpectedResults = results.ToArray();
            }

            return request;
        }

        /// <summary>
        /// Reads an audio stream from a submitted recognition request.
        /// </summary>
        /// <param name="audioStream">an audio stream</param>
        /// <returns>the audio data read from the stream</returns>
        private byte[] ReadStreamCompletely(Stream audioStream)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                byte[] buffer = new byte[32768];
                int bytesRead, totalBytesRead = 0;
                do
                {
                    bytesRead = audioStream.Read(buffer, 0, buffer.Length);
                    totalBytesRead += bytesRead;
                    stream.Write(buffer, 0, bytesRead);
                }
                while (bytesRead > 0);
                stream.Flush();

                return stream.ToArray();
            }
        }
        #endregion

        #region reporting operations
        /// <summary>
        /// Reports a process shutdown.
        /// </summary>
        private static void ReportExit()
        {
            Logger.Info("Terminating server");
            SimpleRecognitionSession.GetConfiguredRecognizer().Dispose();
        }

        /// <summary>
        /// Reports an unhandled exception (fault).
        /// </summary>
        private static void ReportFault(object ex)
        {
            if (ex == null)
            {
                Logger.Error("Unhandled exception caught ... ");
            }
            else
            {
                Logger.Error("Unhandled exception caught ... " + ex.ToString());
            }
        }
        #endregion

    } // SimpleRecognitionService
}
