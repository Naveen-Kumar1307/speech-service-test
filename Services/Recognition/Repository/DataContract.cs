using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using GlobalEnglish.Recognition.DataContracts;

namespace GlobalEnglish.Recognition.Repository.DataContract
{
	[DataContract]
	public class SpeechRecognitionAttempt
	{
		int engineId;
		RecognitionRequest request;
		RecognitionResult result;
	}

	[DataContract]
	public class PhonemeScore
	{
		public char Phoneme;
		public byte Score;
	}

	[DataContract]
	public class WordScore
	{
		public string Word;
		public byte Score;
	}

	[DataContract]
	public class RecentUserPhonemeScore
	{
		public int UserId;
		public string Phoneme;
		public int PhonemeScore;
		public DateTime CreateDate;
	}
}
