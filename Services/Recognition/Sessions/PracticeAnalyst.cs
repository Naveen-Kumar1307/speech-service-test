using System;
using System.Text;
using System.Collections.Generic;

using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.Sessions
{
    /// <summary>
    /// Analyzes the results of a speech practice activity.
    /// </summary>
    public interface IPracticeAnalyst
    {
        /// <summary>
        /// Performs analysis of a recognition practice activity.
        /// </summary>
        /// <param name="request">a recognition request</param>
        /// <param name="result">a recognition result</param>
        void AnalyzePractice(RecognitionRequest request, RecognitionResult result);

    } // IPracticeAnalyst
}
