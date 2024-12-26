using System;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using Common.Logging;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Media.ServiceContracts;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Diagnostics;
using System.IO;

namespace GlobalEnglish.Recognition.Services
{
    /// <summary>
    /// Transcodes speech audio into another format and translates it into its text equivalent.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>converts speech audio between formats</item>
    /// <item>passes the transcoded data to an another recognizer</item>
    /// </list>
    /// </remarks>
    public class TranscodeSpeechRecognizer : ISpeechRecognitionService
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TranscodeSpeechRecognizer));

        /// <summary>
        /// The TCP port for this process.
        /// </summary>
        private static readonly string ProcessPort = Environment.GetEnvironmentVariable("RECPORT");

        /// <summary>
        /// Recognizes utterances coded in the target format.
        /// </summary>
        public ISpeechRecognitionService TargetSpeechRecognizer { get; set; }

        /// <summary>
        /// Converts audio data from a source format to a target format.
        /// </summary>
        public IFormatConversionService AudioConverter { get; set; }


        #region ISpeechRecognitionService Members

        /// <summary>
        /// Transcodes audio data into a target format, and passes the target format
        /// audio data to another recognizer.
        /// </summary>
        /// <param name="audioData">encoded audio data</param>
        /// <param name="grammar">a grammar that describes the expected content</param>
        /// <param name="fileName">an optional name</param>
        /// <returns>the recognized text</returns>
        public RecognitionResult RecognizeSpeech(string grammar, string fileName, byte[] audioData)
        {
            byte[] targetData = { };
            TimeSpan duration = DateTime.Now.TimeToRun(delegate()
            {
                targetData = AudioConverter.ConvertFormat(audioData, fileName);
            });

            ReportConversionTime(audioData, fileName, duration);
            return TargetSpeechRecognizer.RecognizeSpeech(grammar, fileName, targetData);
        }

        /// <summary>
        /// Reports on conversion performance.
        /// </summary>
        private void ReportConversionTime(byte[] utterance, string filePath, TimeSpan duration)
        {
            if (utterance == null) utterance = new byte[0];
            string length = utterance.Length.ToString("N0");
            string time = duration.TotalMilliseconds.ToString("N0");
            string typeName = AudioConverter.GetType().Name;
            string fileName = Path.GetFileName(filePath);
            object[] arguments = { typeName, fileName, length, time };

            if (utterance.Length == 0)
            {
                string message = string.Format("{0} converted {1} in {3} msecs", arguments);
                LogMessage(message);
            }
            else
            {
                string message = string.Format("{0} converted {2} bytes in {3} msecs", arguments);
                LogMessage(message);
            }
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


        #region IDisposable Members

        /// <inheritdoc/>
        public void Dispose()
        {
            if (TargetSpeechRecognizer != null)
            {
                DisposableTarget.Dispose();
                TargetSpeechRecognizer = null;
            }
        }

        /// <summary>
        /// Converts the recognizer to IDisposable.
        /// </summary>
        private IDisposable DisposableTarget
        {
            get { return TargetSpeechRecognizer as IDisposable; }
        }

        #endregion

    } // TranscodeSpeechRecognizer
}
