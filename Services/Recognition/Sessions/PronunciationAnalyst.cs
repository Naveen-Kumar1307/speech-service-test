using System;
using System.Text;
using System.Collections.Generic;

using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.Sessions
{
    /// <summary>
    /// Analyzes the results of a pronunciation activity.
    /// </summary>
    public class PronunciationAnalyst : IPracticeAnalyst
    {
        private PhonemeQuality[] PhonemeHistory { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a new PronunciationAnalyst.
        /// </summary>
        /// <param name="phonemeHistory">a phoneme history</param>
        /// <returns>a new PronunciationAnalyst</returns>
        public static PronunciationAnalyst With(PhonemeQuality[] phonemeHistory)
        {
            PronunciationAnalyst result = new PronunciationAnalyst();
            result.PhonemeHistory = phonemeHistory;
            return result;
        }

        private PronunciationAnalyst() { }
        #endregion

        #region IPracticeAnalyst Members
        /// <summary>
        /// Analyzes the results of a pronunciation activity.
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="result">a recognition result</param>
        public void AnalyzePractice(RecognitionRequest request, RecognitionResult result)
        {
            int phoneAcceptance = result.Sentence.Quality.PhonemeAcceptanceThreshold;
            result.Sentence.UpdateProblems(request.GetNormalizedAnswer(), PhonemeHistory);
        }
        #endregion

    } // PronunciationAnalyst
}
