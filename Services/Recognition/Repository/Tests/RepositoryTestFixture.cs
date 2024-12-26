using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

using NUnit.Framework;
using DataContracts = GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Data;

namespace GlobalEnglish.Recognition.Repository
{
	[TestFixture]
	public class RepositoryTestFixture
	{
        private static readonly string RepositoryDB = "SpeechRecognitionConnectionString";

		[TestFixtureSetUp]
		public void Setup()
		{
			Console.WriteLine("setup");
		}

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test]
        public void FetchPhonemeHistory()
        {
            List<PhonemeQuality> results = new List<PhonemeQuality>();
            using (SqlConnection connection = ConnectionFactory.GetConnection(RepositoryDB))
            {
                connection.ExecuteReliably(delegate() 
                {
                    SqlCommand command =
                        connection.CreateProcedure("usp_GE_Asr_GetRecentUserPhonemeScores")
                            .WithInteger("UserId", 25001)
                            .WithInteger("QualifiedThreshold", 8);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            results.Add(
                                new PhonemeQuality()
                                {
                                    PhoneName = reader.Get<string>("Phoneme"),
                                    Score = reader.Get<byte>("PhonemeScore")
                                }
                            );
                        }
                    }
                });
            }
            Assert.IsTrue(results.Count > 0);
        }

		[Test]
		public void TestSaveAsrAttempt()
		{
			SpeechRepository rep = new SpeechRepository();

			DataContracts.RecognitionRequest request = CreatePronunciationRecognitionRequest();

			DataContracts.RecognitionResult result = CreatePronunciationRecognitionResult();

			TimeSpan span = DateTime.Now.TimeToRun(
				delegate() 
				{
					for (int i = 0; i < 1; i++)
					{
						for (int j = 0; j < 1; j++)
						{
							rep.SaveAsrAttempt(i, 1, request, result);
						}
					}
				}
			);

			Console.WriteLine("Total:" + span.TotalMilliseconds);
		}

		[Test]
		public void TestGetRecentPhonemes()
		{
			SpeechRepository rep = new SpeechRepository();

			TimeSpan span = DateTime.Now.TimeToRun(
				delegate()
				{
					for (int i = 0; i < 1; i++)
					{
						List<DataContracts.PhonemeQuality> results = rep.GetRecentPhonemes(i, 5);
					}
				}
			);

			Console.WriteLine("Total:" + span.TotalMilliseconds);
		}

		#region test helper functions

		private static DataContracts.RecognitionResult CreatePronunciationRecognitionResult()
		{
			DataContracts.RecognitionResult recognitionResult = new DataContracts.RecognitionResult()
			{
				AudioMeasure = new DataContracts.AudioQuality()
				{
					LevelKind = DataContracts.AudioLevel.Normal,
					Miscellany = "dsfgfdsh",
					NoiseLevelKind = DataContracts.AudioNoise.Low,
					SignalNoiseRatio = 2,
					TruncationKind = DataContracts.AudioTruncation.None
				},
				Message = "message",
				QueuedRecognitionTime = 500,
				RecognitionTime = 300,
				RecordedFileName = "asdasdfg.dfgsdfgfg.shdfh.fgj",
				RecordedFileType = "flv",
                ResultDetailKinds = new DataContracts.ResultDetailKind[0],
				Sentence = new DataContracts.SentenceMatch()
				{
					Interpretation = "When is the best time to use the facilities in the business center?",
					MatchedIndex = 0,
					RecognizedText = "When is the best time to use the facilities in the business center?",
					Quality = new DataContracts.SentenceQuality()
					{
						Confidence = 90,
						FrameCount = 50,
						PhraseAcceptanceThreshold = 20,
						PhraseConfidence = 85,
						Score = 90,
						SentenceAcceptanceThreshold = 80,
						Words = new DataContracts.WordQuality[]
						{
							new DataContracts.WordQuality() { Accepted = true, Confidence = 90, EndFrame = 80, FrameCount = 00, PhoneCount = 4, Score = 90, StartFrame = 0 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 80, EndFrame = 180, FrameCount = 100, PhoneCount = 2, Score = 60, StartFrame = 80 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 70, EndFrame = 280, FrameCount = 100, PhoneCount = 3, Score = 70, StartFrame = 180 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 75, EndFrame = 380, FrameCount = 100, PhoneCount = 4, Score = 80, StartFrame = 280 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 95, EndFrame = 480, FrameCount = 100, PhoneCount = 4, Score = 50, StartFrame = 380 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 80, EndFrame = 580, FrameCount = 100, PhoneCount = 2, Score = 60, StartFrame = 480 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 70, EndFrame = 680, FrameCount = 100, PhoneCount = 3, Score = 70, StartFrame = 580 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 60, EndFrame = 780, FrameCount = 100, PhoneCount = 3, Score = 80, StartFrame = 680 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 70, EndFrame = 880, FrameCount = 100, PhoneCount = 10, Score = 50, StartFrame = 780 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 80, EndFrame = 980, FrameCount = 100, PhoneCount = 2, Score = 60, StartFrame = 880 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 90, EndFrame = 1080, FrameCount = 100, PhoneCount = 3, Score = 70, StartFrame = 980 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 50, EndFrame = 1180, FrameCount = 100, PhoneCount = 8, Score = 80, StartFrame = 1080 },
							new DataContracts.WordQuality() { Accepted = true, Confidence = 60, EndFrame = 1280, FrameCount = 100, PhoneCount = 6, Score = 60, StartFrame = 1180 },
						},
						Phonemes = new DataContracts.PhonemeQuality[]
						{
							new DataContracts.PhonemeQuality() { Grapheme = "Wh", PhoneName = "W", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "n", PhoneName = "n", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "i", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "s", PhoneName = "s", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "th", PhoneName = "th", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "b", PhoneName = "b", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "s", PhoneName = "s", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "t", PhoneName = "t", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "t", PhoneName = "t", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "i", PhoneName = "i", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "m", PhoneName = "m", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "t", PhoneName = "t", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "u", PhoneName = "u", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "u", PhoneName = "u", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "s", PhoneName = "s", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "th", PhoneName = "th", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "f", PhoneName = "f", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "a", PhoneName = "a", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "c", PhoneName = "c", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "i", PhoneName = "i", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "l", PhoneName = "l", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "i", PhoneName = "i", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "t", PhoneName = "t", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "i", PhoneName = "i", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "s", PhoneName = "s", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "i", PhoneName = "i", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "n", PhoneName = "n", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "th", PhoneName = "th", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "b", PhoneName = "b", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "u", PhoneName = "u", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "s", PhoneName = "s", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "n", PhoneName = "n", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "s", PhoneName = "s", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "c", PhoneName = "c", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "n", PhoneName = "n", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "t", PhoneName = "t", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "e", PhoneName = "e", Score = 5 },
							new DataContracts.PhonemeQuality() { Grapheme = "r", PhoneName = "r", Score = 5 },
						}
					}
				},
				TypeKind = DataContracts.ResultKind.RecognitionSucceeded
			};

			return recognitionResult;
		}

		private static DataContracts.RecognitionRequest CreatePronunciationRecognitionRequest()
		{
			DataContracts.RecognitionRequest recognitionRequest = new DataContracts.RecognitionRequest()
			{
				RecognitionTypeKind = DataContracts.RecognitionKind.PronunciationPractice,
				Grammar = "When is the best time to use the facilities in the business center?",
				UserId = "1",
				ExpectedResults = new DataContracts.ExpectedResult[0]
			};

			return recognitionRequest;
		}
		
		#endregion test helper functions
	} // RepositoryTestFixture
}
