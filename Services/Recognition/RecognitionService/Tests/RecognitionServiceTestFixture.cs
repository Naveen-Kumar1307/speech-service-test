using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using GlobalEnglish;

namespace Tests
{
	[TestFixture]
	public class RecognitionServiceTestFixture
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			Console.WriteLine("setup");
		}

		[Test]
		public void SampleTestMethod()
		{
			RecognitionService recognitionService = new RecognitionService();

            List<string> result = recognitionService.GetTroublePhonemes(968714, 10);
		}

        [Test]
        public void HasEnoughPhonemeHistory()
        {
            RecognitionService recognitionService = new RecognitionService();

            bool b = recognitionService.HasEnoughPhonemeHistory(968714);
        }
    }//RecognitionServiceTestFixture
}
