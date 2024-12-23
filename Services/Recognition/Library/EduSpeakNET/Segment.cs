using System;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for Segment.
	/// </summary>
	public class Segment: CPtrRef
	{
		private static Logger logger = new Logger("EduSpeak.Segment");

		public Segment(IntPtr cptr): base(cptr) {
		}

        /// <summary>Constructs a new segment.</summary>
        public Segment(string name, CPtrRef model, SegmentType t, int s, int e, int p, int g, int c, int post) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentNew: name<"+name+"> m<"+model+"> t<"+t+
				"> s<"+s+"> e<"+e+"> p<"+p+"> g<"+g+"> c<"+c+"> post<"+post+">");
			setCPtr(SegmentAPI.SegmentNew(name, model.getCPtr(), t, s, e, p, g, c, post));
        }

		protected void Finalize() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SegmentDelete");
			EduSpeakUtil.checkRetValue(SegmentAPI.SegmentDelete(getCPtr()));
		}

        /// <summary></summary>
        public void AllocatePhoneObservationProbs(int num_phones) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "AllocatePhoneObservationProbs: num_phones<"+
											  num_phones+">");
			EduSpeakUtil.checkRetValue(SegmentAPI.SegmentAllocatePhoneObservationProbs(getCPtr(), num_phones));
        }

        /// <summary></summary>
        public void FreePhoneObservationProbs() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "FreePhoneObservationProbs");
			EduSpeakUtil.checkRetValue(SegmentAPI.SegmentFreePhoneObservationProbs(getCPtr()));
        }

        /// <summary>Get segment score properties.</summary>
        public int Confidence {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetConfidence");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetConfidence(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetConfidence ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get segment time boundaries.</summary>
        public int Duration {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetDuration");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetDuration(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetDuration ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get segment time boundaries.</summary>
        public int End {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetEnd");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetEnd(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetEnd ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get segment score properties</summary>
        public int GrammarProb {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetGrammarProb");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetGrammarProb(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetGrammarProb ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get the model pointer (if set).</summary>
        public CPtrRef Model {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetModel");
				IntPtr ret = new IntPtr(0);
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetModel(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetModel ret<"+ret+">");
				return new CPtrRef(ret);
			}
		}

        /// <summary>Get the model name.</summary>
        public string Name {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetName");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetName(getCPtr(), ret, len));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetName ret<"+ret+">");
				return ret.ToString();
			}
		}

        /// <summary>Retrieves the number of elements</summary>
        public int NumElements {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumElements");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetNumElements(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumElements ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Retrieves the number of phones in the segment</summary>
        public int NumPhones {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumPhones");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetNumPhones(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumPhones ret<"+ret+">");
				return ret;
			}
		}

        /// <summary></summary>
        public string Phone {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhone");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetPhone(getCPtr(), ret, len));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhone ret<"+ret+">");
				return ret.ToString();
			}
			set {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "SetPhone: value<"+value+">");
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentSetPhone(getCPtr(), value));
			}
		}

        /// <summary></summary>
        public int PhoneID {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhoneID");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetPhoneID(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhoneID ret<"+ret+">");
				return ret;
			}
			set {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "SetPhoneID: value<"+value+">");
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentSetPhoneID(getCPtr(), value));
			}
		}

        /// <summary></summary>
        public int GetPhoneObservationProbsElement(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhoneObservationProbsElement: index<"+index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetPhoneObservationProbsElement(getCPtr(), index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhoneObservationProbsElement index<"+index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Get segment score properties.</summary>
        public int Posterior {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPosterior");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetPosterior(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPosterior ret<"+ret+">");
				return ret;
			}
		}

        /// <summary> Get segment score properties.</summary>
        public int GetProb {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetProb");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetProb(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetProb ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get segment time boundaries.</summary>
        public int GetStart {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStart");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetStart(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStart ret<"+ret+">");
				return ret;
			}
		}

        /// <summary>Get segment type.</summary>
        public SegmentType GetSegmentType {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegmentType");
				SegmentType ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentGetType(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegmentType ret<"+ret+">");
				return ret;
			}
		}

        /// <summary></summary>
        public void SetPhoneObservationProbsElement(int index, int val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetPhoneObservationProbsElement: index<"+
											  index+"> val<"+val+">");
			EduSpeakUtil.checkRetValue(SegmentAPI.SegmentSetPhoneObservationProbsElement(getCPtr(),
				index, val));
        }

        /// <summary>Get segment type.</summary>
        public bool TypeIsPhone {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTypeIsPhone");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentTypeIsPhone(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTypeIsPhone ret<"+ret+">");
				return ret!=0;
            }
        }

        /// <summary>Get segment type.</summary>
        public bool TypeIsState {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTypeIsState");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentTypeIsState(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTypeIsState ret<"+ret+">");
				return ret!=0;
            }
        }

        /// <summary>Get segment type.</summary>
        public bool TypeIsWord {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTypeIsWord");
				int ret = 0;
				EduSpeakUtil.checkRetValue(SegmentAPI.SegmentTypeIsWord(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTypeIsWord ret<"+ret+">");
				return ret!=0;
            }
        }
	}
}
