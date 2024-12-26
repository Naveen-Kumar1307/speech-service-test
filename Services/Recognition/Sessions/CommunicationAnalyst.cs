using System;
using System.Text;
using System.Collections.Generic;

using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.Sessions
{
    /// <summary>
    /// Analyzes the results of a communication activity.
    /// </summary>
    public class CommunicationAnalyst : IPracticeAnalyst
    {
        #region IPracticeAnalyst Members
        /// <summary>
        /// Analyzes the results of a communication activity.
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="result">a recognition result</param>
        public void AnalyzePractice(RecognitionRequest request, RecognitionResult result)
        {
            int matchedIndex = request.FindMatchIndex(result.Sentence.RecognizedText);
            result.Sentence.MatchedIndex = matchedIndex;
            if (matchedIndex < 0) return; // no match found!!

            string matchedText = request.GetExpectedPhraseGrammar(matchedIndex);
            result.UpdateConfidence(matchedText);
        }

        #endregion

    } // CommunicationAnalyst
}
