using System;
using System.IO;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Microsoft.ServiceModel.Web;
using System.Collections.Generic;

using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.ServiceContracts
{
    /// <summary>
    /// Recognizes speech audio and provide feedback.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>translates a spoken utterance into the equivalent textual words</item>
    /// <item>provides feedback based on pronunciation history</item>
    /// </list>
    /// </remarks>
    [ServiceContract]
    public interface ISimpleRecognitionService : IDisposable
    {
        /// <summary>
        /// Recognizes a spoken utterance given its context.
        /// </summary>
        /// <param name="request">contains the context payload</param>
        /// <param name="audioData">contains the encoded speech data</param>
        /// <returns>a RecognitionResult</returns>
        [WebHelp(
            Comment = "Recognizes speech from a supplied request and audio data (both required).")]
        [WebInvoke(Method = "POST", UriTemplate = "requests", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        RecognitionResult RecognizeSpeechStream(Stream audioData);

        /// <summary>
        /// Recognizes a spoken utterance given its context.
        /// </summary>
        /// <param name="request">contains the context payload</param>
        /// <param name="audioData">contains the encoded speech data</param>
        /// <returns>a RecognitionResult</returns>
        [WebHelp(
            Comment = "Recognizes speech from a supplied request and audio data (both required).")]
        [WebInvoke(Method = "POST",
            BodyStyle=WebMessageBodyStyle.WrappedRequest,
            UriTemplate = "requests/json",
            RequestFormat = WebMessageFormat.Json,
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        RecognitionResult RecognizeSpeech(RecognitionRequest request, string audioData);

        /// <summary>
        /// Indicates whether a given user has enough phoneme history (for evaluations).
        /// </summary>
        /// <param name="userId">identifies a user</param>
        /// <returns>whether a given user has enough phoneme history</returns>
        [WebHelp(
            Comment = "Indicates whether sufficient phoneme history exists to support feedback for a user.")]
        [WebGet(
            UriTemplate = "Users/{userId}/phonemes/exist",
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        bool HasEnoughPhonemeHistory(string userId);

        /// <summary>
        /// Returns the phonemes with the worst pronunciation history.
        /// </summary>
        /// <param name="userId">identifies a user</param>
        /// <param name="resultLimit">the maximum number of results to be returned</param>
        /// <returns>the phonemes with the worst pronunciations</returns>
        [WebHelp(
            Comment = "Returns the phonemes with the worst pronunciation history.")]
        [WebGet(
            UriTemplate = "users/{userId}/phonemes/troubled?limit={resultLimit}",
            ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        List<string> GetTroublePhonemes(string userId, int resultLimit);

    } // SimpleRecognitionService
}
