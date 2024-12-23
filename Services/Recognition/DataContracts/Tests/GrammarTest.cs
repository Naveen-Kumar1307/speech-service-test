using System;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Recognition.DataContracts.Tests
{
    [Serializable]
    public class GrammarTest
    {
        [XmlElement("Package", typeof(Package))]
        public Package[] Packages { get; set; }

    } // GrammarTest

    [Serializable]
    public class Package
    {
        [XmlAttribute("Folder")]
        public string PackageFolder { get; set; }

        [XmlAttribute("Command")]
        public string CommandLine { get; set; }

        [XmlAttribute("Settings")]
        public string OperationalSettings { get; set; }

        [XmlAttribute]
        public string AudioFolder { get; set; }

        [XmlElement("Recognition", typeof(RecognitionTest))]
        public RecognitionTest[] Recognitions { get; set; }

    } // Package

    [Serializable]
    public class RecognitionTest
    {
        [XmlAttribute]
        public ResultKind Type { get; set; }

        [XmlElement]
        public Statement Expected { get; set; }

        [XmlElement]
        public Grammar Grammar { get; set; }

    } // RecognitionTest

    [Serializable]
    public class Grammar
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlElement("Statement", typeof(Statement))]
        public Statement[] Statements { get; set; }

        public string BuildGrammar()
        {
            if (Argument.IsEmpty(Statements)) return string.Empty;
            StringBuilder builder = new StringBuilder();
            foreach (Statement statement in Statements)
            {
                builder.Append(statement.BuildGrammar());
            }

            return (Statements.Length < 2 ?
                    builder.ToString() :
                    builder.ToString().Enclosed());
        }

    } // Grammar

    [Serializable]
    public class Statement
    {
        [XmlAttribute]
        public string Text { get; set; }

        [XmlAttribute]
        public string Result { get; set; }

        private string[] ResultParts
        {
            get { return Result.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); }
        }

        public string BuildGrammar()
        {
            return Text.AsTerm() + BuildCommands();
        }

        public string BuildCommands()
        {
            if (Result == null || Result.Length == 0) return string.Empty;

            string[] parts = ResultParts;

            StringBuilder builder = new StringBuilder();
            foreach (string part in parts)
            {
                if (builder.Length > 0) builder.Append(" ");
                builder.Append(part.Trim().Bracketed());
            }
            return builder.ToString().Embraced();
        }

    } // Statement
}
