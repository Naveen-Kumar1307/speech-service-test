using System;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for SegmentInfo.
	/// </summary>
	public class SegmentInfo: CPtrRef
	{
		private static Logger logger = new Logger("EduSpeak.SegmentInfo");

		public SegmentInfo(IntPtr cptr): base(cptr) {
		}

		public SegmentInfo(int size) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentInfoNew");
			IntPtr nlRef = SegmentInfoAPI.SegmentInfoNew(size);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentInfoNew: nlRef<"+nlRef+">");
			setCPtr(nlRef);
		}

        /// <summary>Creates a segment info that is a copy of another segment info</summary>
		public SegmentInfo(SegmentInfo toCopy) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentInfoNewCopy");
			IntPtr nlRef = SegmentInfoAPI.SegmentInfoNewCopy(toCopy.getCPtr());
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentInfoNewCopy: nlRef<"+nlRef+">");
			setCPtr(nlRef);
		}

		protected void Finalize() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentInfoDelete");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoDelete(getCPtr()));
		}

        /// <summary>Add a segment to the SegmentInfo object.</summary>
        public void AddSegment(string b, CPtrRef model, SegmentType t, int s, int e, int p, int g, int c, int post) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "AddSegment: b<"+b+"> m<"+model+"> t<"+t+
				"> s<"+s+"> e<"+e+"> p<"+p+"> g<"+g+"> c<"+c+"> post<"+post+">");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoAddSegment(getCPtr(),
				b, model.getCPtr(), t, s, e, p, g, c, post));
        }

        /// <summary>Add a segment to the SegmentInfo object.</summary>
        public void AddSegmentSegment(Segment srcSegment) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "AddSegmentSegment: srcSegment<"+srcSegment+">");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoAddSegmentSegment(getCPtr(), srcSegment.getCPtr()));
        }

        /// <summary>Add a segment to the SegmentInfo object.</summary>
        public void AddSegmentSegmentDontCopy(Segment srcSegment) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "AddSegmentSegmentDontCopy: srcSegment<"+srcSegment+">");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoAddSegmentSegmentDontCopy(getCPtr(), srcSegment.getCPtr()));
        }

        /// <summary>Clear the contents of the SegmentInfo, without deallocating it.</summary>
        public void Clear() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Clear");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoClear(getCPtr()));
        }

        /// <summary>Copy contents of input SegmentInfo object into this SegmentInfo</summary>
        public void CopyFrom(SegmentInfo isegmentinfo) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "CopyFrom: isegmentinfo<"+isegmentinfo+">");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoCopy(getCPtr(), isegmentinfo.getCPtr()));
        }

        /// <summary>Frees the SegmentInfo object.</summary>
        public void Delete() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Delete");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoDelete(getCPtr()));
        }

        /// <summary>Get top-level sentence information.</summary>
        public int Confidence {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetConfidence");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetConfidence(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetConfidence ret<"+ret+">");
				return ret;
			}
			set {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "SetConfidence: value<"+value+">");
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoSetConfidence(getCPtr(), value));
			}
		}

        /// <summary>Return the number of words in the segmentation that we have confidence values for.</summary>
        public int ConfidenceNumWords {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetConfidenceNumWords");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetConfidenceNumWords(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetConfidenceNumWords ret<"+ret+">");
				return ret;
			}
			set {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "SetConfidenceNumWords: value<"+value+">");
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoSetConfidenceNumWords(getCPtr(), value));
			}
        }

        /// <summary>Get top-level sentence information.</summary>
        public int Duration {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "Duration");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetDuration(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "Duration ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get top-level sentence information.</summary>
        public int EndFrame {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "EndFrame");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetEndFrame(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "EndFrame ret<"+ret+">");
				return ret;
			}
		}

        /// <summary> Get top-level sentence information.</summary>
        public int GrammarProbability {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GrammarProbability");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetGrammarProbability(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GrammarProbability ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Retrieves the number of phones</summary>
        public int NumPhones {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumPhones");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetNumPhones(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumPhones ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>How many in the i'th word.</summary>
        public int GetNumPhonesInWord(int word_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumPhonesInWord: word_index<"+
						word_index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetNumPhonesInWord(getCPtr(), word_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumPhonesInWord: word_index<"+
						word_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>If so, how many?</summary>
        public int NumStates {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumStates");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetNumStates(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumStates ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>How many states in this phone? Returns -1 on error.</summary>
        public int GetNumStatesInPhone(int word_index, int phone_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumStatesInPhone: word_index<"+
						word_index+"> phone_index<"+phone_index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetNumStatesInPhone(getCPtr(), word_index,
				phone_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumStatesInPhone: word_index<"+
						word_index+"> phone_index<"+phone_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Retrieves the number of words.</summary>
        public int NumWords {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumWords");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetNumWords(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumWords ret<"+ret+">");
				return ret;
			}
		}

        /// <summary> Get the i'th phone in the word_index'th word.</summary>
        public void GetPhoneSegmentInWord(int word_index, int phone_index, Segment destSegment) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhoneSegmentInWord: word_index<"+
						word_index+"> phone_index<"+phone_index+"> destSegment<"+destSegment+">");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetPhoneSegmentInWord(getCPtr(), word_index,
				phone_index, destSegment.getCPtr()));
        }

        /// <summary>Get top-level sentence information.</summary>
        public int Posterior {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPosterior");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetPosterior(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPosterior ret<"+ret+">");
				return ret;
			}
			set {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "SetPosterior: value<"+value+">");
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoSetPosterior(getCPtr(), value));
			}
		}

        /// <summary>Get top-level sentence information.</summary>
        public int Probability {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetProbability");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetProbability(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetProbability ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get the index'th segment, whatever it is.</summary>
        public Segment GetSegment(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegment: index<"+index+">");
			IntPtr ret = new IntPtr(0);
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetSegment(getCPtr(), index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegment: index<"+index+"> ret<"+ret+">");
			return new Segment(ret);
        }

        /// <summary>Get top-level sentence information.</summary>
        public int StartFrame {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStartFrame");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetStartFrame(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStartFrame ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get the state.</summary>
        public Segment GetStateSegmentInPhone(int word_index, int phone_index, int state_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegment: word_index<"+word_index+
				"> phone_index<"+phone_index+"> state_index<"+state_index+">");
			IntPtr ret = new IntPtr(0);
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetStateSegmentInPhone(getCPtr(), 
				word_index, phone_index, state_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegment: word_index<"+word_index+
				"> phone_index<"+phone_index+"> state_index<"+state_index+"> ret<"+ret+">");
			return new Segment(ret);
        }

        /// <summary>Returns the wordIndex'th word confidence value.</summary>
        public int GetWordConfidence(int word_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordConfidence: word_index<"+word_index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoGetWordConfidence(getCPtr(), word_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordConfidence: word_index<"+
											  word_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Get the i'th word.</summary>
        public Segment GetWordSegment(int wordIndex) {
            return null;
        }

        /// <summary>Does the SegmentInfo contain phones?</summary>
        public bool HasPhones {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetHasPhones");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoHasPhones(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetHasPhones ret<"+ret+">");
				return ret!=0;
			}
		}

        /// <summary>Does the SegmentInfo contain states?</summary>
        public bool HasStates {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetHasStates");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoHasStates(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetHasStates ret<"+ret+">");
				return ret!=0;
			}
		}

        /// <summary>Does the SegmentInfo contain words?</summary>
        public bool HasWords {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetHasWords");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoHasWords(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetHasWords ret<"+ret+">");
				return ret!=0;
            }
        }

        /// <summary>Returns NUANCE_OK if the the segment info is valid.</summary>
        public bool IsValid {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIsValid");
				NuanceStatus ret = SegmentInfoAPI.SegmentInfoIsValid(getCPtr());
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIsValid: ret<"+ret+">");
				return ret == NuanceStatus.NUANCE_OK;
            }
        }

        /// <summary>Return the number of segments contained in the sentence.</summary>
        public int NumSegments {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumSegments");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoNumSegments(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumSegments ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>We're done adding stuff, connect everything</summary>
        public void SetComplete() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetComplete");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoSetComplete(getCPtr()));
        }

        /// <summary>Sets a word confidence</summary>
        public void SetWordConfidence(int index, int confidence) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetWordConfidence: index<"+index+
				"> confidence<"+confidence+">");
			EduSpeakUtil.checkRetValue(SegmentInfoAPI.SegmentInfoSetWordConfidence(getCPtr(),
				index, confidence));
        }
	}
}
