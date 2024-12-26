using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.Objects.DataClasses;
using System.Data.Objects;
using System.Configuration;

using Common.Logging;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.SpeechRecognition.Repository;
using DataContracts = GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.Repository
{
    public partial class SpeechRepository : ISpeechRepository
    {
        private static readonly string RepositoryDB = 
            ConfiguredValue.ConnectionNamed("SpeechRecognitionConnectionString");

        #region Declarations

        private static readonly ILog Logger = LogManager.GetLogger(typeof(SpeechRepository));

        #endregion Declarations

        #region Interface implementation

        public void SaveAsrAttempt(int userId, int engineId, 
            DataContracts.RecognitionRequest request, DataContracts.RecognitionResult result)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(RepositoryDB))
                {
                    connection.Open();
                    try
                    {
                        SaveAsrAttemptImpl(connection, engineId, userId, request, result);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public List<DataContracts.PhonemeQuality> GetRecentPhonemes(int userId, int? qualifiedThreshold)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(RepositoryDB))
                {
                    connection.Open();
                    try
                    {
                        return GetRecentPhonemesImpl(connection, userId, qualifiedThreshold);
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw ex;
            }
        }

        #endregion Interface methods
        
        #region Implementation

        private static void SaveAsrAttemptImpl(
            SqlConnection connection, int engineId, int userId,
            DataContracts.RecognitionRequest request, 
            DataContracts.RecognitionResult result)
        {
            if (request == null) throw new ArgumentNullException("No RecognitionRequest was supplied");
            if (result == null) throw new ArgumentNullException("No RecognitionResult was supplied");

            StringBuilder builder = new StringBuilder();

            // insert ASR attempt and phoneme tracking data in a transaction
            using (SqlTransaction transaction = connection.BeginTransaction())
            {
                try
                {
                    using (SqlCommand command = 
                        CreateSaveAttemptCommand(engineId, request, connection, transaction))
                    {
                        SqlParameter outParam = CreateSqlOutputInteger("AsrAttemptId");
                        command.Parameters.Add(outParam);
                        command.ExecuteNonQuery();

                        int asrAttemptId = Convert.ToInt32(outParam.Value);
                        if (result.Sentence != null && request.IsPronunciationPractice())
                        {
                            using (SqlCommand command2 = 
                                CreateSavePhonemeCommand(asrAttemptId, request, result, connection, transaction, builder))
                            {
                                //SqlParameter outParam2 = CreateSqlOutputInteger("PhonemeTrackingId");
                                //command2.Parameters.Add(outParam2);
                                command2.ExecuteNonQuery();

                                //int phonemeTrackingId = Convert.ToInt32(outParam2.Value);

                                //if (result.Sentence.Quality.Phonemes.Length > 0)
                                //{
                                //    for (int i = 0; i < result.Sentence.Quality.Phonemes.Length; i++)
                                //    {
                                //        DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[i];
                                //        if (phonemeQuality.IsIncluded())
                                //        {
                                //            using (SqlCommand command3 = 
                                //                CreateSaveDetailCommand(phonemeTrackingId, i, result, connection, transaction))
                                //            {
                                //                command3.ExecuteNonQuery();
                                //            }

                                //            using (SqlCommand command4 = 
                                //                CreateUpdateScoresCommand(i, request, result, connection, transaction))
                                //            {
                                //                command4.ExecuteNonQuery();
                                //            }

                                //            builder.Append(phonemeQuality.PhoneName);
                                //            builder.Append(", ");
                                //        }
                                //    }
                                //}
                            }
                        }
                    }

                    transaction.Commit();
                    Logger.Debug("saved phonemes: " + builder.ToString());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        private static List<DataContracts.PhonemeQuality> GetRecentPhonemesImpl(
            SqlConnection connection, int userId, int? qualifiedThreshold)
        {
            List<DataContracts.PhonemeQuality> result = new List<DataContracts.PhonemeQuality>();
            using (SqlCommand command = new SqlCommand("usp_GE_Asr_GetRecentUserPhonemeScores", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(CreateSqlInteger("UserId", userId));

                if (qualifiedThreshold.HasValue)
                {
                    command.Parameters.Add(CreateSqlInteger("QualifiedThreshold", qualifiedThreshold.Value));
                }

                using (IDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new DataContracts.PhonemeQuality()
                        {
                            PhoneName = DataHelper.GetString(reader, "Phoneme"),
                            Score = DataHelper.GetInt(reader, "PhonemeScore")
                        });
                    }
                }

                return result;
            }
        }

        private static SqlCommand CreateSaveAttemptCommand(
            int engineId,
            DataContracts.RecognitionRequest request, 
            SqlConnection connection,
            SqlTransaction transaction)
        {
            SqlCommand command = new SqlCommand("usp_GE_Asr_SaveAsrAttempt", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(CreateSqlInteger("UserId", int.Parse(request.UserId)));
            command.Parameters.Add(CreateSqlInteger("EngineId", engineId));
            command.Parameters.Add(CreateSqlInteger("RecognitionType", (int)request.RecognitionTypeKind));
            command.Parameters.Add(CreateSqlString("AsrDataJsonStr", request.Grammar));
            return command;
        }

        private static SqlCommand CreateSavePhonemeCommand(
            int asrAttemptId,
            DataContracts.RecognitionRequest request, 
            DataContracts.RecognitionResult result,
            SqlConnection connection,
            SqlTransaction transaction,
            StringBuilder builder)
        {
            string scores = string.Empty;
            string confidences = string.Empty;
            if (result.Sentence.Quality.Words.Length > 0)
            {
                scores = result.Sentence.Quality.Words.Select(i => i.Score.ToString()).Aggregate((i, j) => i + "," + j);
                confidences = 
                    result.Sentence.Quality.Words.Select(i => i.Confidence.ToString()).Aggregate((i, j) => i + "," + j);
            }

            SqlCommand command = new SqlCommand("usp_GE_Asr_SavePhonemes", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(CreateSqlInteger("UserId", int.Parse(request.UserId)));
            command.Parameters.Add(CreateSqlInteger("AsrAttemptId", asrAttemptId));
            command.Parameters.Add(CreateSqlString("Grammar", request.Grammar));
            command.Parameters.Add(CreateSqlInteger("SentenceConfidence", result.Sentence.Quality.Confidence));
            command.Parameters.Add(CreateSqlTinyInteger("SentenceScore", result.Sentence.Quality.Score));
            command.Parameters.Add(CreateSqlString("WordConfidenceList", confidences));
            command.Parameters.Add(CreateSqlString("WordScoreList", scores));

            if (result.Sentence.Quality.Phonemes.Length > 0)
            {
                DataTable phonemeTable = new DataTable("PhonemeScores");
                phonemeTable.Columns.Add("Phoneme", typeof(string));
                phonemeTable.Columns.Add("PhonemeScore", typeof(byte));

                for (int i = 0; i < result.Sentence.Quality.Phonemes.Length; i++)
                {
                    DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[i];
                    if (phonemeQuality.IsIncluded())
                    {
                        phonemeTable.Rows.Add(phonemeQuality.PhoneName, (byte)phonemeQuality.Score);
                        builder.Append(phonemeQuality.PhoneName);
                        builder.Append(", ");
                    }
                }

                command.Parameters.Add(CreateSqlTable("PhonemeScores", phonemeTable));
            }

            return command;
        }

        private static SqlCommand CreateSaveDetailCommand(
            int phonemeTrackingId, int phonemeIndex,
            DataContracts.RecognitionResult result,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[phonemeIndex];

            SqlCommand command = new SqlCommand("usp_GE_Asr_SavePhonemeTrackingDetail", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(CreateSqlInteger("PhonemeTrackingId", phonemeTrackingId));
            command.Parameters.Add(CreateSqlString("Phoneme", phonemeQuality.PhoneName));
            command.Parameters.Add(CreateSqlTinyInteger("PhonemeScore", phonemeQuality.Score));
            return command;
        }

        private static SqlCommand CreateUpdateScoresCommand(
            int phonemeIndex,
            DataContracts.RecognitionRequest request,
            DataContracts.RecognitionResult result,
            SqlConnection connection,
            SqlTransaction transaction)
        {
            DataContracts.PhonemeQuality phonemeQuality = result.Sentence.Quality.Phonemes[phonemeIndex];

            SqlCommand command = new SqlCommand("usp_GE_Asr_UpdateRecentUserPhonemeScores", connection, transaction);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(CreateSqlInteger("UserId", int.Parse(request.UserId)));
            command.Parameters.Add(CreateSqlString("Phoneme", phonemeQuality.PhoneName));
            command.Parameters.Add(CreateSqlTinyInteger("PhonemeScore", phonemeQuality.Score));
            return command;
        }

        private static SqlParameter CreateSqlOutputInteger(string argumentName)
        {
            SqlParameter result = CreateSqlInteger(argumentName, 0);
            result.Direction = ParameterDirection.Output;
            return result;
        }

        private static SqlParameter CreateSqlInteger(string argumentName, int value)
        {
            SqlParameter result = new SqlParameter(argumentName, SqlDbType.Int);
            result.Value = value;
            result.Size = 4;
            return result;
        }

        private static SqlParameter CreateSqlTinyInteger(string argumentName, int value)
        {
            SqlParameter result = new SqlParameter(argumentName, SqlDbType.TinyInt);
            result.Value = value;
            result.Size = 1;
            return result;
        }

        private static SqlParameter CreateSqlString(string argumentName, string value)
        {
            SqlParameter result = new SqlParameter(argumentName, SqlDbType.NVarChar);
            result.Value = value;
            return result;
        }

        private static SqlParameter CreateSqlTable(string argumentName, DataTable table)
        {
            SqlParameter result = new SqlParameter(argumentName, SqlDbType.Structured);
            result.Value = table;
            return result;
        }

        #endregion Implementation
    }
}
