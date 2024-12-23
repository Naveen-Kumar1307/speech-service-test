using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using GlobalEnglish.Recognition.Services;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.Sessions;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Spring;
using System.ServiceModel.Web;
using GlobalEnglish.Recognition.Repository;
using GlobalEnglish.Utility.Data;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using GlobalEnglish.Denali.Util;

namespace GlobalEnglish.Recognition.SimpleService
{
    /// <summary>
    /// A speech recognition session for a simple recognition service.
    /// </summary>
    public class SimpleRecognitionSession : RecognitionSession
    {        
        private static SpringContext CachedContext { get; set; }
        delegate void HandleCompletion(IAsyncResult result);
        private bool registoryRemoved = false;

        /// <summary>
        /// The context for this session.
        /// </summary>
        public static SpringContext Context
        {
            set { CachedContext = value; }
            get
            {
                if (CachedContext == null)
                    CachedContext = SpringContext.From(typeof(SimpleRecognitionSession));

                return CachedContext;
            }
        }

        /// <summary>
        /// An audio data buffer.
        /// </summary>
        public byte[] AudioData { get; private set; }

        /// <summary>
        /// An audio folder path.
        /// </summary>
        public string SampleAudioFolder { get; private set; }

        #region creating instances
        /// <summary>
        /// Returns a new (registered) recognition session.
        /// </summary>
        /// <param name="request">a request</param>
        /// <param name="audioData">client audio data</param>
        /// <returns>a RecognitionSession, or null</returns>
        public static SimpleRecognitionSession From(
            RecognitionRequest request, byte[] audioData)
        {
            if (Argument.IsAbsent(request)) return null;
            if (Argument.IsEmpty(audioData)) return null;

            SimpleRecognitionSession result = new SimpleRecognitionSession();
            result.AudioFileName = request.GetUniqueFileName();
            result.Request = request;

            if (request.AudioFormat == StandardAudioFileType)
            {
                // save a copy of the data in a file
                string sampleName = result.AudioFileName + request.AudioFormat;
                string samplePath = Path.Combine(AudioFolderPath, sampleName);
                FileInfo sampleFile = new FileInfo(samplePath);

                using (FileStream stream = sampleFile.OpenWrite())
                {
                    stream.Write(audioData, 0, audioData.Length);
                    stream.Flush();
                    stream.Close();
                }
            }
            else
            {
                result.AudioData = audioData;
            }

            result.RegisterSession();
            return result;
        }

        /// <summary>
        /// Constructs a new SimpleRecognitionSession.
        /// </summary>
        private SimpleRecognitionSession() : base()
        {
            AudioData = new byte[0];
            SampleAudioFolder = string.Empty;
        }
        #endregion

        #region accessing values
        /// <inheritdoc/>
        public override string GetFilePath()
        {
            if (SampleAudioFolder.Length > 0)
            {
                return Path.Combine(SampleAudioFolder, AudioFileName + Request.AudioFormat);
            }
            else
            {
                return base.GetFilePath();
            }
        }
        #endregion

        #region recognizing speech
        /// <summary>
        /// Recognizes a speech sample.
        /// </summary>
        /// <returns>a RecognitionResult</returns>
        public RecognitionResult RecognizeSample()
        {
            IncomingWebRequestContext context = WebOperationContext.Current.IncomingRequest;
            string userId  = context.Headers["UserId"].WithoutQuotes();
            string fileName = context.Headers["FileName"].WithoutQuotes();
            bool DoesBlobAudioUploadRequired = ConfiguredValue.Get<bool>("DoesBlobAudioUploadRequired", false);
            try
            {
                MeasureRecognition();
                if (DoesBlobAudioUploadRequired && Result.WasSuccess())
                { 
                    if((!userId.IsNullOrEmpty() && Convert.ToInt32(userId) > 0) && !fileName.IsNullOrEmpty())
                        SaveAudioToBlob(userId, fileName, GetFilePath());
                }
                return Result;
            }
            finally
            {
                if (Result.WasSuccess())
                    registoryRemoved = RemoveSessionOnUploadCompleted(DoesBlobAudioUploadRequired);
                else
                    RemoveSession();
            }
        }

        //upload audio files to blob
        public void SaveAudioToBlob(string userId, string fileName, string filePath)
        {
            try
            {
                string conn = ConnectionFactory.GetConnectionString("StorageConnectionString");
                var account = CloudStorageAccount.Parse(conn);

                // Create the blob client.
                CloudBlobClient blobClient = account.CreateCloudBlobClient();

                // Retrieve reference to a previously created container.
                CloudBlobContainer container = blobClient.GetContainerReference(ConfiguredValue.Get<string>("BlobContainerName", "asr"));
                string[] filenameArry = fileName.Split('-');

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(userId + "/" + ((filenameArry.Length > 2) ? fileName.Split('-')[2] : "default") + "/" + fileName + ".ogg");

                FileStream fileStream = System.IO.File.OpenRead(filePath);

                HandleCompletion handleCompletion = delegate(IAsyncResult result)
                {
                    blockBlob.EndUploadFromStream(result);
                    fileStream.Close();
                    fileStream.Dispose();
                    if (registoryRemoved)
                        DeleteSampleFileIfConfigured();
                };

                blockBlob.BeginUploadFromStream(fileStream, new AsyncCallback(handleCompletion), null);
            }
            catch (Exception ex)
            {
                Logger.WriteException("Exception has thrown when uploading user audio to blob, audio name: "
                + fileName + "\nException Message: " + ex.Message + "\n Stack trace: " + ex.StackTrace, ex);
            }
        }

        /// <summary>
        /// Measures the time to recognize a speech sample and analyze the results.
        /// </summary>
        public void MeasureRecognition()
        {
            ReportRecognitionStart();

            TimeSpan span = DateTime.Now.TimeToRun(delegate()
            {
                Result = RecognizeAudio();
                AnalyzeResult();
            });

            ReportRecognitionResults(span);
        }

        /// <summary>
        /// Returns the result of a recognition.
        /// </summary>
        private RecognitionResult RecognizeAudio()
        {
            string filePath = GetFilePath();
            string grammar = GetGrammar();
            ReportFileGrammar(grammar);

            try
            {
                return GetConfiguredRecognizer()
                            .RecognizeSpeech(grammar, filePath, AudioData)
                            .With(AudioFileName, StandardAudioFileType);
            }
            catch (Exception ex)
            {
                ReportRecognitionProblem(ex);
                return RecognitionResult.WithError(ex);
            }
        }

        /// <summary>
        /// Returns the configured recognizer.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static ISpeechRecognitionService GetConfiguredRecognizer()
        {
            if (Service == null)
            {
                Service = Context.Get<ISpeechRecognitionService>();
            }

            return Service;
        }

        private static ISpeechRecognitionService Service = null;
        #endregion

        #region logging operations
        /// <summary>
        /// Reports a recognition problem.
        /// </summary>
        protected void ReportRecognitionProblem(Exception ex)
        {
            LogError(ex.Message, ex);
            LogDebug(ex.Message + "\n ... with " + Request.FormatData());
        }

        protected override void LogError(string message)
        {
            base.LogError(message);
            Denali.Util.Logger.WriteError(UserId, message);
        }

        protected override void LogError(string message, Exception ex)
        {
            base.LogError(message, ex);
            Denali.Util.Logger.WriteError(UserId, message + " " + ex.ToString());
        }

        protected override void LogWarn(string message)
        {
            base.LogWarn(message);
            Denali.Util.Logger.WriteWarning(UserId, message);
        }

        protected override void LogInfo(string message)
        {
            base.LogInfo(message);
            Denali.Util.Logger.WriteInformation(UserId, message);
        }

        protected override void LogDebug(string message)
        {
            base.LogDebug(message);
        }
        #endregion

    } // SimpleRecognitionSession
}
