using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using Common.Logging;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.StorageClient;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Data;

namespace GlobalEnglish.Recognition.Repository
{
	public partial class SpeechRepository : ISpeechRepository
	{
		private static readonly string RepositoryDB = "SpeechRecognitionConnectionString";

		private static readonly bool SaveHistoryAsynchronously =
			ConfiguredValue.Get<bool>("SaveHistoryAsynchronously", true);

		#region Declarations

		private static readonly ILog Logger = LogManager.GetLogger(typeof(SpeechRepository));

		#endregion Declarations

		#region Interface implementation

		public void SaveAsrAttempt(int userId, int engineId,
			DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
		{
			TimeSpan span = DateTime.Now.TimeToRun(() =>
			{
				SaveAsrAttemptImpl(engineId, userId, request, result);
			});

			string fileName = result.RecordedFileName.SinglyQuoted();
			string saveTime = span.TotalMilliseconds.ToString("N0");
			Logger.Debug("saved phoneme data to sql database in " + saveTime + " msecs for " + fileName);
		}

		public void SaveAsrAttemptHistory(int userId, int engineId,
			DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
		{
			TimeSpan span = DateTime.Now.TimeToRun(() =>
			{
				SaveAsrAttemptHistoryImpl(engineId, userId, request, result);
			});

			string fileName = result.RecordedFileName.SinglyQuoted();
			string saveTime = span.TotalMilliseconds.ToString("N0");
			Logger.Debug("saved recognition data to azure table in " + saveTime + " msecs for " + fileName);
		}

		public List<DataContracts.PhonemeQuality> GetRecentPhonemes(int userId, int? qualifiedThreshold)
		{
			GEAzureSqlRetryPolicy policy = new GEAzureSqlRetryPolicy();
			return policy.ExecuteAction(() =>
			{
				using (SqlConnection connection = ConnectionFactory.GetConnection(RepositoryDB))
				{
					if (connection.State != ConnectionState.Open) connection.Open();
					return GetRecentPhonemesImpl(connection, userId, qualifiedThreshold);
				}
			});
		}

		#endregion Interface methods

		#region Implementation

		private static void SaveAsrAttemptImpl(int engineId, int userId,
			DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
		{
			if (request == null) throw new ArgumentNullException("No RecognitionRequest was supplied");
			if (result == null) throw new ArgumentNullException("No RecognitionResult was supplied");

			if (result.Sentence != null && request.IsPronunciationPractice())
			{
				if (result.Sentence.Quality.Phonemes.Length > 0)
				{
					StringBuilder builder = new StringBuilder();
					GEAzureSqlRetryPolicy policy = new GEAzureSqlRetryPolicy();
					policy.ExecuteAction(() =>
					{
						using (SqlConnection connection = ConnectionFactory.GetConnection(RepositoryDB))
						{
							if (connection.State != ConnectionState.Open) connection.Open();

							for (int i = 0; i < result.Sentence.Quality.Phonemes.Length; i++)
							{
								DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[i];

								if (phonemeQuality.IsIncluded())
								{
									builder.Append(phonemeQuality.PhoneName);
									builder.Append(", ");

									using (SqlCommand recentPhonemesCmd = connection.CreateProcedure("usp_GE_Asr_UpdateRecentUserPhonemeScores"))
									{
										recentPhonemesCmd.AddInteger("UserId", userId);
										recentPhonemesCmd.AddString("Phoneme", phonemeQuality.PhoneName);
										recentPhonemesCmd.AddByte("PhonemeScore", phonemeQuality.Score);
										recentPhonemesCmd.ExecuteNonQuery();
									}
								}
							}
						}
					});
					Logger.Debug("saved phonemes: " + builder.ToString());
				}
			}
		}

		private static void SaveAsrAttemptHistoryImpl(int engineId, int userId,
			DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
		{
			if (request == null) throw new ArgumentNullException("No RecognitionRequest was supplied");
			if (result == null) throw new ArgumentNullException("No RecognitionResult was supplied");

			AsrAttemptEntity attempt = new AsrAttemptEntity(request.UserId);
			attempt.EngineId = engineId;
			attempt.RecognitionType = (int)request.RecognitionTypeKind;
			attempt.Grammar = request.Grammar;

			if (result.Sentence != null)
			{
				string scores = string.Empty;
				string confidences = string.Empty;
				if (result.Sentence.Quality.Words.Length > 0)
				{
					scores = result.Sentence.Quality.Words.Select(i => i.Score.ToString()).Aggregate((i, j) => i + "," + j);
					confidences =
						result.Sentence.Quality.Words.Select(i => i.Confidence.ToString()).Aggregate((i, j) => i + "," + j);
				}
				attempt.SentenceConfidence = result.Sentence.Quality.Confidence;
				attempt.SentenceScore = result.Sentence.Quality.Score;
				attempt.WordConfidenceList = confidences;
				attempt.WordScoreList = scores;

				if (result.Sentence.Quality.Phonemes.Length > 0)
				{
					List<Dictionary<string, int>> phonemeTrackingDetail = new List<Dictionary<string, int>>();
					for (int i = 0; i < result.Sentence.Quality.Phonemes.Length; i++)
					{
						DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[i];

						if (phonemeQuality.IsIncluded())
						{
							Dictionary<string, int> phonemeAndScore = new Dictionary<string, int>();
							phonemeAndScore.Add(phonemeQuality.PhoneName, phonemeQuality.Score);
							phonemeTrackingDetail.Add(phonemeAndScore);
						}
					}

					if (phonemeTrackingDetail.Count > 0)
					{
						string jsonStr = JsonConvert.SerializeObject(phonemeTrackingDetail);
						attempt.PhonemeTrackingDetail = jsonStr;
					}
				}
			}

			CloudTableClient client = AzureTableHelper.GetAzureTableClient();
			TableServiceContext context = client.GetDataServiceContext();
			context.AddObject(AzureTableHelper.TableName, attempt);

			if (SaveHistoryAsynchronously)
			{
				context.BeginSaveChangesWithRetries(new AsyncCallback(HandleCompletion), result);
			}
			else
			{
				context.SaveChangesWithRetries();
			}
		}

		private static void HandleCompletion(IAsyncResult result)
		{
			DataContracts.RecognitionResult recResult = (DataContracts.RecognitionResult)result.AsyncState;
			Logger.Debug("completed recognition data save from " + recResult.RecordedFileName);
		}

		private static List<DataContracts.PhonemeQuality> GetRecentPhonemesImpl(
			SqlConnection connection, int userId, int? qualifiedThreshold)
		{
			List<DataContracts.PhonemeQuality> result = new List<DataContracts.PhonemeQuality>();
			using (SqlCommand recentPhonemes =
					connection.CreateProcedure("usp_GE_Asr_GetRecentUserPhonemeScores"))
			{
				recentPhonemes.AddInteger("UserId", userId);

				if (qualifiedThreshold.HasValue)
				{
					recentPhonemes.AddInteger("QualifiedThreshold", qualifiedThreshold.Value);
				}

				using (IDataReader reader = recentPhonemes.ExecuteReader())
				{
					while (reader.Read())
					{
						result.Add(new DataContracts.PhonemeQuality()
						{
							PhoneName = reader.Get<string>("Phoneme"),
							Score = reader.Get<byte>("PhonemeScore")
						});
					}
				}

				return result;
			}
		}

		#endregion Implementation
	}


}
