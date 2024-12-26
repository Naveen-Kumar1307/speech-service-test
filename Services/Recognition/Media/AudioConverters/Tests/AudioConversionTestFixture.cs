using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using NUnit.Framework;
using GlobalEnglish.Media.ServiceContracts;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Media.AudioConverters
{
    /// <summary>
    /// Verifies that the Speex / Wave converter works.
    /// </summary>
    [TestFixture]
    public class AudioConversionTestFixture
    {
        private static readonly string Dash = "-";
        private static readonly string[] SampleNames = 
        {
            "SampleOGG", "SampleMP3", "SampleSPX", "SampleFLV"
        };

        private static readonly Dictionary<string, IFormatConversionService> Converters = 
                            new Dictionary<string, IFormatConversionService>();

        static AudioConversionTestFixture()
        {
            Converters[".spx"] = new Mpeg3AudioConverter(); // SpeexWaveConverter();
            Converters[".mp3"] = new Mpeg3AudioConverter();
            Converters[".ogg"] = new Mpeg3AudioConverter();
            Converters[".flv"] = new FlashAudioConverter();
        }

        /// <summary>
        /// Converts sample audio files to WAV format.
        /// </summary>
        [Test]
        public void ConvertSamplesToWave()
        {
            foreach (string sampleName in SampleNames)
            {
                string samplePath = ConfiguredValue.Named(sampleName);
                string resampleName = Path.GetFileNameWithoutExtension(samplePath);
                byte[] audioData = ConvertData(samplePath);
                Assert.IsTrue(audioData.Length > 0, "missing data converted from " + samplePath);
            }
        }

        private byte[] ConvertData(string samplePath)
        {
            string baseName = Path.GetFileNameWithoutExtension(samplePath);
            string sampleFile = baseName + Dash + DateTime.Now.Ticks;
            byte[] sampleAudio = GetSampleData(samplePath);
            return GetConverter(samplePath).ConvertFormat(sampleAudio, sampleFile);
        }

        private IFormatConversionService GetConverter(string filePath)
        {
            string fileType = Path.GetExtension(filePath);
            return Converters[fileType];
        }

        private byte[] GetSampleData(string samplePath)
        {
            FileInfo sampleFile = ResourceFile.FindFile(samplePath);
            Assert.IsTrue(sampleFile.Exists);
            return ResourceFile.Named(sampleFile.FullName).GetResourceBinary();
        }

    } // AudioConversionTestFixture
}
