using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using NUnit.Framework;
using GlobalEnglish.Recognition.ServiceContracts;
using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Recognition.DataContracts.Tests;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Spring;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.Services
{
    /// <summary>
    /// Verifies the full speech recognition configuration works.
    /// </summary>
    [TestFixture]
    public class RecognitionChainTestFixture
    {
        private static readonly Type ClassType = typeof(RecognitionChainTestFixture);

        private static readonly string WildCard = ".*";
        private static readonly string PreferredType = ".ogg";

        private static readonly bool ServiceLaunchedRecognizer =
                                ConfiguredValue.Get<bool>("ServiceLaunchedRecognizer", true);

        private ISpeechRecognitionService Recognizer { get; set; }
        private DirectoryInfo AudioFolder { get; set; }

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test, Ignore]
        public void RecognizeRepeatedly()
        {
            int count = 100;
            Package[] packages = LoadTestData().Packages;
            while (count > 0)
            {
                RecognizeSamples(packages);
                count--;
            }
        }

        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test, Ignore]
        public void RecognizeRepeatedlyWithSameRecognizer()
        {
            Package[] packages = LoadTestData().Packages;
            RecognizeSamples(packages[1], 100);
        }

        /// <summary>
        /// Verifies that various recognition scenarios work.
        /// </summary>
        [Test]
        public void RecognizeWithSampleGrammars()
        {
            //RecognizeSamples(LoadPackage());
            if (ServiceLaunchedRecognizer)
                RecognizeSamples(LoadTestData().Packages);
        }

        /// <summary>
        /// Verifies that file-based recognitions work.
        /// </summary>
        [Test]
        public void RecognizeWithMultipleSampleFiles()
        {
            if (ServiceLaunchedRecognizer) return;
            Package[] packages = LoadTestData().Packages;
            Recognizer = CreateFileRecognizer(packages[0]);

            FileInfo[] files = GetSamples();
            foreach (FileInfo file in files)
            {
                RecognizeSample(packages[0].Recognitions[0], file);
            }

            Recognizer.Dispose();
            Recognizer = null;
        }

        private void RecognizeSamples(Package[] packages)
        {
            foreach (Package package in packages)
            {
                if (package.AudioFolder.EndsWith("test-samples"))
                {
                    RecognizeSamples(package);
                }
            }
        }

        private void RecognizeSamples(Package package)
        {
            RecognizeSamples(package, 1);
        }

        private void RecognizeSamples(Package package, int count)
        {
            PrepareRecognizer(package);
            RecognizeSamples(package.Recognitions, count);
            Recognizer.Dispose();
            Recognizer = null;
        }

        private void RecognizeSamples(RecognitionTest[] recognitions, int count)
        {
            while (count-- > 0)
            {
                foreach (RecognitionTest recognition in recognitions)
                    RecognizeSample(recognition);
            }
        }

        private void RecognizeSamples(RecognitionTest[] recognitions)
        {
            foreach (RecognitionTest recognition in recognitions)
                RecognizeSample(recognition);
        }

        private void RecognizeSample(RecognitionTest recognition)
        {
            string expectedText = recognition.Expected.Text;
            FileInfo sampleFile = FindSampleFile(expectedText);
            RecognizeSample(recognition, sampleFile);
        }

        private void RecognizeSample(RecognitionTest recognition, FileInfo sampleFile)
        {
            string expectedText = recognition.Expected.Text;

            string grammar = recognition.Grammar.BuildGrammar();
            string grammarName = recognition.Grammar.Name;
            if (Argument.IsPresent(grammarName))
            {
                grammar = grammarName;
            }

            byte[] audioData = { };
            if (ServiceLaunchedRecognizer)
                audioData = ResourceFile.Named(sampleFile.FullName).GetResourceBinary();

            RecognitionResult result =
                Recognizer.RecognizeSpeech(grammar, sampleFile.FullName, audioData);

            Assert.IsTrue(result.TypeKind == recognition.Type,
                "Expected " + recognition.Type.ToString() +
                ", but " + result.TypeKind.ToString() +
                " with " + expectedText.SinglyQuoted());

            if (recognition.Type != ResultKind.RecognitionSucceeded) return;

            string resultText = result.Sentence.RecognizedText.WithoutQuotes();
            Assert.IsTrue(resultText == expectedText,
                          "Failed to recognize " + expectedText.SinglyQuoted());

            string expectedResult = recognition.Expected.BuildCommands();
            if (expectedResult.Length > 0)
            {
                string actualResult = result.Sentence.Interpretation;
                Assert.IsTrue(actualResult == expectedResult,
                              "Found " + actualResult + " instead of " +
                                         expectedResult.SinglyQuoted());
            }
        }

        private FileInfo FindSampleFile(string fileName)
        {
            FileInfo[] files = AudioFolder.GetFiles(fileName + WildCard);
            return (files.Length > 0 ? files[0] : null);
        }

        private FileInfo[] GetSamples()
        {
            return AudioFolder.GetFiles(WildCard[1] + PreferredType);
        }

        private void PrepareRecognizer(Package package)
        {
            AudioFolder = ResourceFile.FindFolder(package.AudioFolder);
            EduSpeakRecognizer.DefaultPackagePath = package.PackageFolder;
            EduSpeakRecognizer.ServiceFactory.ClearCache();

            if (Argument.IsPresent(package.CommandLine))
            {
                EduSpeakRecognizer.DefaultInitializations = package.CommandLine;
            }

            if (Argument.IsPresent(package.OperationalSettings))
            {
                EduSpeakRecognizer.ExtraOperationalSettings = package.OperationalSettings;
            }

            Recognizer = GetRecognizer();
        }

        private ISpeechRecognitionService CreateFileRecognizer(Package package)
        {
            AudioFolder = ResourceFile.FindFolder(package.AudioFolder);
            EduSpeakRecognizer.DefaultPackagePath = package.PackageFolder;
            EduSpeakRecognizer.ServiceFactory.ClearCache();

            if (Argument.IsPresent(package.CommandLine))
            {
                EduSpeakRecognizer.DefaultInitializations = package.CommandLine;
            }

            if (Argument.IsPresent(package.OperationalSettings))
            {
                EduSpeakRecognizer.ExtraOperationalSettings = package.OperationalSettings;
            }

            return EduSpeakRecognizer.InitializePackage();
        }

        private ISpeechRecognitionService GetRecognizer()
        {
            return SpringContext.GetConfigured<ISpeechRecognitionService>();
        }

        private GrammarTest LoadTestData()
        {
            return ResourceFile.LoadEmbedded<GrammarTest>("TestData.xml", GetType());
        }

        private Package LoadPackage()
        {
            return ResourceFile.LoadEmbedded<Package>("TestData.xml", GetType());
        }

    } // RecognitionChainTestFixture
}
