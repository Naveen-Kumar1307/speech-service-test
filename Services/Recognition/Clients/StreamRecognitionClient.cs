using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Recognition.Clients
{
    /// <summary>
    /// A client-side proxy for a remote speech recognition service.
    /// </summary>
    public class StreamRecognitionClient : 
        ClientBase<ISimpleRecognitionService>, 
        ISimpleRecognitionService
    {
        private static readonly string AUDIO = "audio/";
        private static readonly string POST = "POST";

        #region ISimpleRecognitionService Members

        /// <summary>
        /// Recognizes an audio utterance as a stream of bytes.
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="bytes">an audio utterance</param>
        /// <returns>a RecognitionResult</returns>
        /// <remarks>Encodes the supplied request as HTTP headers and POSTs the bytes.</remarks>
        public RecognitionResult RecognizeSpeechStream(
            RecognitionRequest request, byte[] bytes)
        {
            string methodURL = base.Endpoint.Address + "/requests";
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(methodURL);
            webRequest.ContentType = AUDIO + request.AudioFormat.Substring(1);
            webRequest.Method = POST;

            // encode the request as HTTP headers
            webRequest.Headers["UserId"] = request.UserId;
            webRequest.Headers["Grammar"] = request.Grammar.WithoutHiddenCrap();
            webRequest.Headers["RecognitionType"] = request.RecognitionType;
            webRequest.Headers["ExpectedResults"] = request.EncodeExpectedResults();

            using (Stream stream = webRequest.GetRequestStream())
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
            }

            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                return GetJsonCodec().ReadObject(stream) as RecognitionResult;
            }
        }

        /// <summary>
        /// Returns a JSON codec configured to decode a RecognitionResult.
        /// </summary>
        private DataContractJsonSerializer GetJsonCodec()
        {
            return new DataContractJsonSerializer(typeof(RecognitionResult));
        }

        /// <inheritdoc/>
        public RecognitionResult RecognizeSpeechStream(Stream audioStream)
        {
            throw new InvalidOperationException(
                "Unimplemented operation, instead use " +
                "RecognizeSpeechStream(RecognitionRequest request, byte[] bytes)");
        }

        /// <summary>
        /// Recognizes an audio utterance.
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="bytes">an audio utterance</param>
        /// <returns>a RecognitionResult</returns>
        /// <remarks>Encodes the supplied bytes as base 64 content in a POST.</remarks>
        public RecognitionResult RecognizeSpeech(RecognitionRequest request, byte[] bytes)
        {
            string audioData = Convert.ToBase64String(bytes);
            return RecognizeSpeech(request, audioData);
        }

        /// <summary>
        /// Recognizes an audio utterance (encoded in base 64).
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="bytes">an encoded audio utterance</param>
        /// <returns>a RecognitionResult</returns>
        public RecognitionResult RecognizeSpeech(RecognitionRequest request, string audioData)
        {
            return base.Channel.RecognizeSpeech(request, audioData);
        }
        
        /// <summary>
        /// Indicates whether a user has enough phoneme history to provide feedback.
        /// </summary>
        /// <param name="userId">a user ID</param>
        /// <returns>whether a user has enough phoneme history to provide feedback</returns>
        public bool HasEnoughPhonemeHistory(string userId)
        {
            return base.Channel.HasEnoughPhonemeHistory(userId);
        }

        /// <summary>
        /// Returns the phonemes that have poor pronunciation history for a user.
        /// </summary>
        /// <param name="userId">a user ID</param>
        /// <param name="resultLimit">a result limit</param>
        /// <returns>the user's trouble phonemes</returns>
        public List<string> GetTroublePhonemes(string userId, int resultLimit)
        {
            return base.Channel.GetTroublePhonemes(userId, resultLimit);
        }

        #endregion

    } // StreamRecognitionClient
}
