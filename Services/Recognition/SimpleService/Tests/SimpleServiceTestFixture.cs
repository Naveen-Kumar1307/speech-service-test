using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Common.Logging;
using NUnit.Framework;
using GlobalEnglish.Recognition.Services;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.DataContracts.Tests;
using GlobalEnglish.Recognition.Sessions;
using GlobalEnglish.Utility.Diagnostics;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.SimpleService
{
    /// <summary>
    /// Verifies that a recognition works with a full stack recognizer.
    /// </summary>
    [TestFixture]
    public class SimpleServiceTestFixture
    {
        private static readonly Type ClassType = typeof(SimpleServiceTestFixture);
        private static readonly ILog Logger = LogManager.GetLogger(ClassType);

        private static readonly string WildCard = "*";
        private static readonly string FolderPath = ConfiguredValue.Named("AudioFolder");
        private static readonly string SampleRequestFile = "SampleRecognitionRequest.xml";
        private static ConfiguredTest Test = null;

        private string SampleFileType { get; set; }

        public SimpleServiceTestFixture()
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
        /// Verifies that a recognition works with a full stack recognizer.
        /// </summary>
        [Test]
        public void RecognizeAudioWithService()
        {
            using (ISimpleRecognitionService service = GetService())
            {
                int count = Test.Requests.Length;
                for (int index = 0; index < count; index++)
                {
                    byte[] audioData = LoadAudio(index);
                    if (audioData.Length > 0)
                    {
                        RecognitionRequest request = Test.Requests[0];
                        string audioSample = Convert.ToBase64String(audioData);
                        RecognitionResult result = service.RecognizeSpeech(request, audioSample);
                        Assert.IsTrue(result != null);
                    }
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
            FileInfo[] files = audioFolder.GetFiles(WildCard + SampleFileType);
            if (index < files.Length) return files[index].FullName;
            return string.Empty;
        }

        /// <summary>
        /// Returns a new (local) instances of ISimpleRecognitionService.
        /// </summary>
        private ISimpleRecognitionService GetService()
        {
            return new SimpleRecognitionService();
        }

    } // SimpleServiceTestFixture
}
