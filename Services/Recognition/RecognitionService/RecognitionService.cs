using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataContracts = GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.Repository;
using Common.Logging;
using System.ServiceModel;

namespace GlobalEnglish
{
	public class RecognitionService
	{
		#region Declarations

		struct PhonemeAverageScore
		{
			public string Phoneme;
			public float AverageScore;
		}

		private ISpeechRepository _speechRepository;
		public ISpeechRepository SpeechRepository
		{
			get { return _speechRepository; }
			set { _speechRepository = value; }
		}

        private static readonly ILog Logger = LogManager.GetLogger(typeof(RecognitionService));

		#endregion Declarations

		#region Methods

		/// <summary>
		/// Returns a list of phonemes user had trouble pronouncing
		/// Returns entire list of phonemes if maxReturned is not specified
		/// </summary>
		/// <param name="userId">User ID</param>
		/// <param name="maxReturned">number of trouble phonemes to return</param>
		/// <returns></returns>
        public List<string> GetTroublePhonemes(string clsUserIdStr, int? maxReturned)
		{
            try
            {
                int clsUserId = int.Parse(clsUserIdStr);

                ISpeechRepository speechRepository = GetSpeechRepository();

                return GetTroublePhonemesImpl(clsUserId, maxReturned, speechRepository, ConfigHelper.LowPhonemeScoreThreshold, ConfigHelper.PhonemeRecordQualifiedThreshold);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw new FaultException(ex.Message);
            }
        }

        public bool HasEnoughPhonemeHistory(string clsUserIdStr)
        {
            try
            {
                int clsUserId = int.Parse(clsUserIdStr);

                if (clsUserId == 0) return false;

                ISpeechRepository speechRepository = GetSpeechRepository();

                return HasEnoughPhonemeHistoryImpl(clsUserId, speechRepository, ConfigHelper.PhonemeRecordQualifiedThreshold, ConfigHelper.EnoughPhonemeHistoryThreshold);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw new FaultException(ex.Message);
            }
        }

        #endregion Methods

		#region Implementation

		private ISpeechRepository GetSpeechRepository()
		{
			return SpeechRepository == null ? new SpeechRepository() : SpeechRepository;
		}

		private List<string> GetTroublePhonemesImpl(int clsUserId, int? maxReturned, ISpeechRepository speechRepository, float lowScore, int phonemeRecordQualifiedThreshold)
		{
            List<DataContracts.PhonemeQuality> phonemeScoreList = speechRepository.GetRecentPhonemes(clsUserId, phonemeRecordQualifiedThreshold);

			List<PhonemeAverageScore> averageScores =
                (from p in phonemeScoreList
                 group p by p.PhoneName into g
                 select new PhonemeAverageScore
                 {
                     Phoneme = g.Key,
                     AverageScore = (float)g.Average(item => item.Score)
                 }).ToList();

            if (Logger.IsDebugEnabled)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Phoneme History ID = ");
                builder.Append(clsUserId);
                builder.Append(" ");

                PhonemeAverageScore[] orderedScores = 
                    (from s in averageScores orderby s.Phoneme select s).ToArray();

                foreach (PhonemeAverageScore score in orderedScores)
                {
                    builder.Append("[");
                    builder.Append(score.Phoneme);
                    builder.Append(",");
                    builder.Append(score.AverageScore.ToString("N2"));
                    builder.Append("] ");
                }

                Logger.Debug(builder.ToString());
            }

			IEnumerable<string> allPhonemes = (from p in averageScores
											   where p.AverageScore <= lowScore
											   orderby p.AverageScore ascending
											   select p.Phoneme);

			return maxReturned.HasValue ? allPhonemes.Take(maxReturned.Value).ToList() : allPhonemes.ToList();
		}

        private bool HasEnoughPhonemeHistoryImpl(int clsUserId, ISpeechRepository speechRepository, int phonemeRecordQualifiedThreshold, int enoughPhonemeHistoryThreshold)
        {
            List<DataContracts.PhonemeQuality> phonemeScoreList = speechRepository.GetRecentPhonemes(clsUserId, phonemeRecordQualifiedThreshold);

            int distinctPhonemesNo = (from p in phonemeScoreList select p.PhoneName).Distinct().Count();

            return distinctPhonemesNo >= enoughPhonemeHistoryThreshold;
        }
        
		#endregion Implementation
	}
}
