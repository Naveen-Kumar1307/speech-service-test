using System;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;

using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.ServiceContracts
{
    /// <summary>
    /// Recognizes encoded speech utterances.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a particular kind of speech encoding</item>
    /// <item>translates an utterance into the equivalent textual words</item>
    /// </list>
    /// </remarks>
    [ServiceContract]
    public interface ISpeechRecognitionService : IDisposable
    {
        /// <summary>
        /// Recognizes an utterance using a given grammar.
        /// </summary>
        /// <param name="grammar">a grammar</param>
        /// <param name="fileName">optional file name</param>
        /// <param name="audioData">optional encoded audio data</param>
        /// <returns>a recognition result, or null</returns>
        /// <remarks>The service requires either audio data or a file name.</remarks>
        [OperationContract]
        RecognitionResult RecognizeSpeech(string grammar, string fileName, byte[] audioData);

    } // ISpeechRecognitionService
}
