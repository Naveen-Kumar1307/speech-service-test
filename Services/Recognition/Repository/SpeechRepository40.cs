using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Common.Logging;
using DataContracts = GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.SpeechRecognition.Repository;
using GlobalEnglish.Recognition.Repository.ServiceContract;
using System.Data.Objects.DataClasses;
using System.Data.Objects;

namespace GlobalEnglish.Recognition.Repository
{
	public partial class SpeechRepository40 : ISpeechRepository
	{
		#region Declarations

		private static readonly ILog Logger = LogManager.GetLogger(typeof(SpeechRepository));

		#endregion Declarations

		#region Interface implementation

		public void SaveAsrAttempt(string clsUserId, int engineId, DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
        {
			using (RepositoryModelContainer repository = new RepositoryModelContainer())
			{
				//find CLS user in database
				User clsUser = repository.Users.FirstOrDefault(u => u.ClsUserId == clsUserId);

				//if CLS user does not exist in database yet - create new
				if (null == clsUser)
				{
					clsUser = repository.Users.CreateObject();
					clsUser.ClsUserId = clsUserId;
					repository.Users.AddObject(clsUser);
				}

				//create recognition attempt object to persist into database
				RecognitionAttempt dbRecognitionAttempt = CreateRecognitionAttempt(engineId, request, result, clsUser);

				//attach recognition attempt to a user
				clsUser.RecognitionAttempts.Add(dbRecognitionAttempt);

				repository.SaveChanges();
			}
		}

		public List<DataContracts.PhonemeQuality> GetRecentPhonemes(string clsUserId, int qualifiedThreshold, int phonemeCount)
		{
			List<DataContracts.PhonemeQuality> result = new List<DataContracts.PhonemeQuality>();

			using (RepositoryModelContainer repository = new RepositoryModelContainer())
			{
				//find user in database
				User clsUser = repository.Users.FirstOrDefault(u => u.ClsUserId == clsUserId);

				//return if requested user is not found in database
				if (clsUser == null)
				{
					return null;
				}

				ObjectResult<PhonemeQuality> recentPhonemes = repository.GetRecentPhonemes(clsUser.Id, qualifiedThreshold, phonemeCount);

				foreach (PhonemeQuality phonemeQuality in recentPhonemes)
				{
					result.Add(new DataContracts.PhonemeQuality()
					{
						Grapheme = phonemeQuality.Grapheme,
						PhoneName = phonemeQuality.PhoneName,
						Score = phonemeQuality.Score
					});
				}
			}

			return result;
		}

		#endregion Interface implementation

		#region Private methods

		private RecognitionAttempt CreateRecognitionAttempt(int engineId, DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result, User user)
		{
			RecognitionAttempt dbRecognitionAttempt = new RecognitionAttempt()
			{
				EngineId = engineId,
				RecognitionRequest = RecognitionRequest2DbRecognitionRequest(request),
				RecognitionResult = RecognitionResult2DbRecognitionResult(result, user)
			};

			return dbRecognitionAttempt;
		}

		private RecognitionRequest RecognitionRequest2DbRecognitionRequest(DataContracts.RecognitionRequest request)
		{
			RecognitionRequest dbRecognitionRequest = new RecognitionRequest()
			{
				Grammar = request.Grammar,
				RecognitionType = (int)request.RecognitionType,
				ExpectedResults = ExpectedResults2DbExpectedResults(request.ExpectedResults)
			};

			return dbRecognitionRequest;
		}

		private EntityCollection<ExpectedResult> ExpectedResults2DbExpectedResults(DataContracts.ExpectedResult[]  expectedResults)
		{
			EntityCollection<ExpectedResult> dbExpectedResults = new EntityCollection<ExpectedResult>();

			foreach (DataContracts.ExpectedResult expectedResult in expectedResults)
			{
				dbExpectedResults.Add(new ExpectedResult() 
				{ 
					Answer = expectedResult.Answer, 
					FullAnswer = expectedResult.FullAnswer 
				});
			}

			return dbExpectedResults;
		}

		private RecognitionResult RecognitionResult2DbRecognitionResult(DataContracts.RecognitionResult result, User user)
		{
			AudioQuality dbAudioQuality = AudioQuality2DbAudioQuality(result.AudioMeasure);

			SentenceMatch sentenceMatch = SentenceMatch2DbSentenceMatch(result.Sentence, user);

			RecognitionResult dbRecognitionResult = new RecognitionResult()
			{
				AudioMeasure = dbAudioQuality,
				Message = result.Message,
				QueuedRecognitionTime = result.QueuedRecognitionTime,
				RecognitionTime = result.RecognitionTime,
				RecordedFileName = result.RecordedFileName,
				RecordedFileType = result.RecordedFileType,
				ResultDetail = (int)result.ResultDetail,
				Type = (int)result.Type,
				Sentence = sentenceMatch
			};

			return dbRecognitionResult;
		}

		private AudioQuality AudioQuality2DbAudioQuality(DataContracts.AudioQuality audioQuality)
		{
			AudioQuality dbAudioQuality = new AudioQuality()
			{
				Level = (int)audioQuality.Level,
				Miscellany = audioQuality.Miscellany,
				NoiseLevel = (int)audioQuality.NoiseLevel,
				SignalNoiseRatio = audioQuality.SignalNoiseRatio,
				Truncation = (int)audioQuality.Truncation
			};

			return dbAudioQuality;
		}

		private SentenceMatch SentenceMatch2DbSentenceMatch(DataContracts.SentenceMatch sentenceMatch, User user)
		{
			SentenceQuality dbSentenceQuality = SentenceQuality2DbSentenceQuality(sentenceMatch.Quality, user);

			SentenceMatch dbSentenceMatch = new SentenceMatch()
			{
				Interpretation = sentenceMatch.Interpretation,
				MatchedIndex = sentenceMatch.MatchedIndex,
				Quality = dbSentenceQuality,
				RecognizedText = sentenceMatch.RecognizedText
			};

			return dbSentenceMatch;
		}

		private SentenceQuality SentenceQuality2DbSentenceQuality(DataContracts.SentenceQuality sentenceQuality, User user)
		{
			EntityCollection<PhonemeQuality> dbPhonemes = PhonemeQualities2DbPhonemeQualities(sentenceQuality.Phonemes, user);

			EntityCollection<WordQuality> dbWordQualities = WordQualities2DbWordQualities(sentenceQuality.Words);

			SentenceQuality dbSentenceQuality = new SentenceQuality()
			{
				Confidence = sentenceQuality.Confidence,
				FrameCount = sentenceQuality.FrameCount,
				PhonemeQualities = dbPhonemes,
				PhraseAcceptanceThreshold = sentenceQuality.PhraseAcceptanceThreshold,
				PhraseConfidence = sentenceQuality.PhraseConfidence,
				Score = sentenceQuality.Score,
				SentenceAcceptanceThreshold = sentenceQuality.SentenceAcceptanceThreshold,
				Words = dbWordQualities
			};

			return dbSentenceQuality;
		}

		private EntityCollection<WordQuality> WordQualities2DbWordQualities(DataContracts.WordQuality[] wordQualities)
		{
			EntityCollection<WordQuality> dbWordQualities = new EntityCollection<WordQuality>();

			foreach (DataContracts.WordQuality wordQuality in wordQualities)
			{
				dbWordQualities.Add(new WordQuality() 
				{ 
					Accepted = wordQuality.Accepted, 
					Confidence = wordQuality.Confidence, 
					EndFrame = wordQuality.EndFrame, 
					FrameCount = wordQuality.FrameCount, 
					PhoneCount = wordQuality.PhoneCount, 
					Score = wordQuality.Score, 
					StartFrame = wordQuality.StartFrame 
				});
			}

			return dbWordQualities;
		}

		private EntityCollection<PhonemeQuality> PhonemeQualities2DbPhonemeQualities(DataContracts.PhonemeQuality[] phonemeQualities, User user)
		{
			EntityCollection<PhonemeQuality> dbPhonemes = new EntityCollection<PhonemeQuality>();

			DateTime dtNow = DateTime.Now;
			foreach (DataContracts.PhonemeQuality phonemeQuality in phonemeQualities)
			{
				dbPhonemes.Add(new PhonemeQuality() 
				{ 
					Grapheme = phonemeQuality.Grapheme, 
					PhoneName = phonemeQuality.PhoneName, 
					Score = phonemeQuality.Score, 
					CreatedDate = dtNow,
					UserId = user.Id 
				});
			}

			return dbPhonemes;
		}

		#endregion Private methods

		#region old way to save user attempt

		//refactored old way to save user state optimized (a bit) for performance
		//usp_GE_Asr_UpdateRecentUserPhonemeScores keeps running table of 8 recent phonemes for faster retrieval
		//keepeng that one temporary for faster revival/reference in case entity framework fails to perform
		private void saveByLegacyWay(int engineId, DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
		{
			//legacy ADO.NET way
			using (SqlConnection connection = new SqlConnection(ConfigHelper.ConnectionString))
			{
				connection.Open();
				// insert ASR attempt and phoneme tracking data in a transaction
				SqlTransaction transaction = connection.BeginTransaction();
				try
				{
					SqlCommand command = new SqlCommand("usp_GE_Asr_SaveAsrAttempt", connection, transaction);
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add(new SqlParameter("UserId", int.Parse(request.UserId)));
					command.Parameters.Add(new SqlParameter("EngineId", engineId));
					command.Parameters.Add(new SqlParameter("RecognitionType", (int)request.RecognitionType));
					command.Parameters.Add(new SqlParameter("AsrDataJsonStr", request.Grammar));
					SqlParameter outParam = new SqlParameter();
					outParam.Direction = ParameterDirection.Output;
					outParam.Size = 4;
					outParam.ParameterName = "AsrAttemptId";
					command.Parameters.Add(outParam);
					command.ExecuteNonQuery();

					int asrAttemptId = Convert.ToInt32(outParam.Value);

					SqlCommand command2 = new SqlCommand("usp_GE_Asr_SavePhonemeTracking", connection, transaction);
					command2.CommandType = CommandType.StoredProcedure;
					command2.Parameters.Add(new SqlParameter("AsrAttemptId", asrAttemptId));
					command2.Parameters.Add(new SqlParameter("Grammar", request.Grammar));
					command2.Parameters.Add(new SqlParameter("SentenceConfidence", result.Sentence.Quality.Confidence));
					command2.Parameters.Add(new SqlParameter("SentenceScore", result.Sentence.Quality.Score));
					command2.Parameters.Add(new SqlParameter("WordConfidenceList", result.Sentence.Quality.Words.Select(i => i.Score.ToString()).Aggregate((i, j) => i + "," + j)));
					command2.Parameters.Add(new SqlParameter("WordScoreList", result.Sentence.Quality.Words.Select(i => i.Score.ToString()).Aggregate((i, j) => i + "," + j)));
					SqlParameter outParam2 = new SqlParameter();
					outParam2.Direction = ParameterDirection.Output;
					outParam2.ParameterName = "PhonemeTrackingId";
					outParam2.Size = 4;
					command2.Parameters.Add(outParam2);
					command2.ExecuteNonQuery();

					int phonemeTrackingId = Convert.ToInt32(outParam2.Value);

					for (int i = 0; i < result.Sentence.Quality.Phonemes.Length; i++)
					{
						DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[i];

						SqlCommand command3 = new SqlCommand("usp_GE_Asr_SavePhonemeTrackingDetail", connection, transaction);
						command3.CommandType = CommandType.StoredProcedure;
						command3.Parameters.Add(new SqlParameter("PhonemeTrackingId", phonemeTrackingId));
						command3.Parameters.Add(new SqlParameter("Phoneme", phonemeQuality.PhoneName));
						command3.Parameters.Add(new SqlParameter("PhonemeScore", phonemeQuality.Score));
						command3.ExecuteNonQuery();

						SqlCommand command4 = new SqlCommand("usp_GE_Asr_UpdateRecentUserPhonemeScores", connection, transaction);
						command4.CommandType = CommandType.StoredProcedure;
						command4.Parameters.Add(new SqlParameter("UserId", int.Parse(request.UserId)));
						command4.Parameters.Add(new SqlParameter("Phoneme", phonemeQuality.PhoneName));
						command4.Parameters.Add(new SqlParameter("PhonemeScore", phonemeQuality.Score));
						command4.ExecuteNonQuery();
					}

					transaction.Commit();
				}
				catch (Exception ex)
				{
					transaction.Rollback();
					Logger.Fatal(ex.Message, ex);
					throw new FaultException(ex.Message);
				}
			}
		}

		#endregion old way to save user attempt
	}
}
