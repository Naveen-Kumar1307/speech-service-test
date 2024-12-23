using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish
{
    internal class ConfigHelper
    {
        /// <summary>
        /// The threshold to be defined as low score for the phoneme score. 
        /// </summary>
        public static float LowPhonemeScoreThreshold
        {
            get { return ConfiguredValue.Get<float>("LowPhonemeScoreThreshold", 2.0f); }
        }

		/// <summary>
        /// Only consider phonemes that user have spoken at least that many number of times
		/// </summary>
		public static int PhonemeRecordQualifiedThreshold
		{
            get { return ConfiguredValue.Get<int>("PhonemeExperience", 8); }
		}

        /// <summary>
        /// Percentage of phonemes user have to speak for engine to provide confident feedback
        /// </summary>
        public static int EnoughPhonemeHistoryThreshold
        {
            get { return ConfiguredValue.Get<int>("EnoughPhonemeHistoryThreshold", 12); }
        }
    }
}
