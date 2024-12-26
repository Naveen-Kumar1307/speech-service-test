using System;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;

using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.ServiceContracts
{
    /// <summary>
    /// Recognizes speech encoded in a file.
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a particular kind of speech encoding</item>
    /// <item>translates an utterance into the equivalent textual words</item>
    /// </list>
    /// </remarks>
    [ServiceContract]
    public interface ISpeechFileRecognitionService : IDisposable
    {
        /// <summary>
        /// Recognizes an utterance using a given grammar.
        /// </summary>
        /// <param name="filePath">a speech file path</param>
        /// <param name="grammar">a grammar</param>
        /// <returns>a recognition result, or null</returns>
        [OperationContract]
        RecognitionResult RecognizeFile(string filePath, string grammar);

    } // ISpeechFileRecognitionService
}
