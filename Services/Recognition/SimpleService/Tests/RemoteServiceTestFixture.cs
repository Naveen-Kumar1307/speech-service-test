using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;

using Common.Logging;
using NUnit.Framework;
using GlobalEnglish.Recognition.Clients;
using GlobalEnglish.Recognition.Services;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.DataContracts.Tests;
using GlobalEnglish.Recognition.Sessions;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Spring;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.SimpleService
{
    /// <summary>
    /// Remotely tests the operation of the simple recognition service.
    /// </summary>
    [TestFixture, Ignore]
    public class RemoteServiceTestFixture
    {
        private static readonly Type ClassType = typeof(RemoteServiceTestFixture);
        private static readonly ILog Logger = LogManager.GetLogger(ClassType);

        private static readonly string FolderPath = ConfiguredValue.Named("AudioFolder");
        private static readonly string SampleRequestFile = "SampleRecognitionRequest.xml";
        private static ConfiguredTest Test = null;

        private string SampleFileType { get; set; }
        private SpringContext Context { get; set; }

        public RemoteServiceTestFixture()
        {
            SampleFileType = RecognitionSession.OggSuffix;
        }

        [TestFixtureSetUp]
        public void InitializeTests()
        {
            Test = ConfiguredTest.LoadSamples(SampleRequestFile, ClassType);
            foreach (RecognitionRequest request in Test.Requests)
            {
                request.AudioFormat = SampleFileType;
            }
        }

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test]
        public void RecognizeRemotely()
        {
            bool useStream = false;
            for (int index = 0; index < 2; index++)
            {
                byte[] bytes = LoadSample(index); // LoadAudio(index);
                Logger.Debug("sending bytes length = " + bytes.Length);
                RecognitionResult result = RecognizeSpeech(index, bytes, useStream);

                Assert.IsTrue(result != null);
                Logger.Info(result.FormatAllResults());
            }
        }

        /// <summary>
        /// Verifies that concurrent fetches work.
        /// </summary>
        [Test]
        public void FetchConcurrently()
        {
            List<IPerformance> performers = new List<IPerformance>();
            for (int index = 0; index < 10; index++)
            {
                int clientID = 25001 + index;
                performers.Add(Performance.With(TestClient, clientID));
            }

            int sleepTime = 5;
            int repetitions = 100;
            ThreadCoordinator coordinator =
                ThreadCoordinator.With(performers)
                                 .With(repetitions, sleepTime)
                                 .SpawnThreadsWaitForCompletion();

            ReportStatistics(coordinator);
        }

        /// <summary>
        /// Verifies that concurrent recognitions work.
        /// </summary>
        [Test]
        public void RecognizeConcurrently()
        {
            List<IPerformance> performers = new List<IPerformance>();
            for (int index = 0; index < 4; index++)
            {
                performers.Add(Performance.With(TestRecognition, index));
            }

            int repetitions = Test.Threads;
            ThreadCoordinator coordinator =
                ThreadCoordinator.With(performers)
                                 .With(repetitions, Test.ThreadSleep)
                                 .SpawnThreadsWaitForCompletion();

            ReportStatistics(coordinator);
        }

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test]
        public void RecognizeRepeatedly()
        {
            int count = 20;
            List<TimeSpan> results = new List<TimeSpan>();
            bool useStream = false;

            byte[] bytes = LoadAudio(0);
            for (int index = 0; index < count; index++)
            {
                TimeSpan duration = DateTime.Now.TimeToRun(delegate() 
                {
                    RecognitionResult result = RecognizeSpeech(0, bytes, useStream);
                });

                if (index > 0)
                    results.Add(duration);
            }

            double avg = results.Average(item => item.TotalMilliseconds);
            Logger.Info("average duration = " + avg);
        }

        private void TestRecognition(int index)
        {
            TestRecognition(index, true);
        }

        private void TestRecognition(int index, bool useStream)
        {
            byte[] bytes = LoadAudio(index);
            Logger.Debug("sending bytes length = " + bytes.Length);

            TimeSpan span = DateTime.Now.TimeToRun(delegate() 
            {
                RecognitionResult result = RecognizeSpeech(index, bytes, useStream);
                Assert.IsTrue(result != null);
                Logger.Debug(result.FormatAllResults());
            });

            Logger.Debug("recognized in " + span.TotalMilliseconds.ToString("N0") + " msecs");
        }

        private void TestClient()
        {
            TestClient(25001);
        }

        private void TestClient(int clientID)
        {
            using (StreamRecognitionClient service = new StreamRecognitionClient())
            {
                bool history = service.HasEnoughPhonemeHistory(clientID.ToString());
            }
        }

        private RecognitionResult RecognizeSpeech(int index, byte[] bytes, bool useStream)
        {
            using (StreamRecognitionClient service = new StreamRecognitionClient())
            {
                if (useStream)
                {
                    return service.RecognizeSpeechStream(Test.Requests[index], bytes);
                }
                else
                {
                    return service.RecognizeSpeech(Test.Requests[index], bytes);
                }
            }
        }

        private byte[] LoadAudio(int index)
        {
            byte[] empty = { };
            string audioPath = GetAudioPath(index);
            if (audioPath.Length == 0) return empty;
            return ResourceFile.Named(audioPath).GetResourceBinary();
        }

        private string GetAudioPath(int index)
        {
            DirectoryInfo audioFolder = ResourceFile.FindFolder(FolderPath);
            FileInfo[] files = audioFolder.GetFiles("*" + SampleFileType);
            if (index < files.Length) return files[index].FullName;
            return string.Empty;
        }

        private byte[] LoadSample(int index)
        {
            byte[] empty = { };
            string audioPath = GetSamplePath(index);
            if (audioPath.Length == 0) return empty;
            return ResourceFile.Named(audioPath).GetResourceBinary();
        }

        private string GetSamplePath(int index)
        {
            string fileName = Test.Requests[index].ExpectedResults[0].Answer + ".ogg";
            DirectoryInfo audioFolder = ResourceFile.FindFolder(FolderPath);
            FileInfo[] files = audioFolder.GetFiles(fileName);
            if (0 < files.Length) return files[0].FullName;
            return string.Empty;
        }

        public void ReportStatistics(ThreadCoordinator coordinator)
        {
            Logger.Info("");
            Logger.Info("");

            StringBuilder builder = new StringBuilder();
            builder.Append("total requests = ");
            builder.Append(coordinator.TotalPerformanceCount.ToString("N0"));
            Logger.Info(builder.ToString());

            builder = new StringBuilder();
            builder.Append("average response time = ");
            builder.Append(coordinator.AveragePerformanceTime.ToString("N0"));
            builder.Append(" msecs");
            Logger.Info(builder.ToString());

            builder = new StringBuilder();
            builder.Append("maximum response time = ");
            builder.Append(coordinator.MaximumPerformanceTime.ToString("N0"));
            builder.Append(" msecs");
            Logger.Info(builder.ToString());

            builder = new StringBuilder();
            builder.Append("average response rate = ");
            builder.Append(coordinator.AveragePerformanceRate.ToString("N3"));
            builder.Append("/sec");
            Logger.Info(builder.ToString());
        }

    } // RemoteServiceTestFixture
}
