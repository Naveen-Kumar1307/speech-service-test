using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

using GlobalEnglish.Recognition.DataContracts;
using GlobalEnglish.Utility.Parameters;
using GlobalEnglish.Utility.Xml;

namespace GlobalEnglish.Recognition.DataContracts.Tests
{
    /// <summary>
    /// Contains configured test data.
    /// </summary>
    [Serializable]
    public class ConfiguredTest
    {
        [XmlAttribute]
        public int Recognizers { get; set; }

        [XmlAttribute]
        public int QueueSize { get; set; }

        [XmlAttribute]
        public int Threads { get; set; }

        [XmlAttribute]
        public int Repeats { get; set; }

        [XmlAttribute]
        public int ThreadSleep { get; set; }

        [XmlElement("RecognitionRequest", typeof(RecognitionRequest))]
        public RecognitionRequest[] Requests { get; set; }

        [XmlElement("RecognitionResult", typeof(RecognitionResult))]
        public RecognitionResult[] Results { get; set; }

        public static ConfiguredTest LoadSamples(string sampleFile, Type classType)
        {
            if (File.Exists(sampleFile))
            {
                return ResourceFile.Named(sampleFile).LoadResource<ConfiguredTest>();
            }
            else
            {
                return ResourceFile.From(sampleFile, classType).LoadResource<ConfiguredTest>();
            }
        }

    } // ConfiguredTest
}
