using System;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Wraps RecResult from rcapi.
	/// </summary>
	public class RecResult: CPtrRef {
		private static Logger logger = new Logger("EduSpeak.RecResult");

		public RecResult(IntPtr cptr): base(cptr) {
		}

		public RecResult() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "RecResultNew");
			IntPtr ret = RecResultAPI.RecResultNew();
			if (Logger.GetTraceEnabled()) logger.Trace(this, "RecResultNew: ret<"+ret+">");
			setCPtr(ret);
		}
		
		protected void Finalize() {
			Delete();
		}

		/// <summary>Retrieves a copy of the RecResult instance</summary>
		public RecResult Copy() {
			RecResult target = new RecResult();
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Copy");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultCopy(getCPtr(), target.getCPtr()));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Copy: target<"+target+">");
			return target;
		}

		/// <summary>Copies data from an IRecResult instance into this IRecResult</summary> 
		public void CopyFrom(RecResult irecresult) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "CopyFrom: irecresult<"+irecresult+">");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultCopy(irecresult.getCPtr(), getCPtr()));
		}

		/// <summary>Frees memory allocated for specified RecResult structure.</summary> 
		public void Delete() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Delete");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultDelete(getCPtr()));
		}

		/// <summary>This is a convenience wrapper function around getting the NL result for the 0th answer and extracting the 'exception' structure slot.</summary> 
		public string GetException() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetException");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			NuanceStatus status = NuanceStatus.NUANCE_OK;
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultException(getCPtr(), ref status, ret, len));
			return ret.ToString();
		}

		/// <summary>asks for the NLResult for the specified recognition result to be copied into the supplied INLResult object.</summary> 
		public void GetNLResult(int index, NLResult inlresult) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNLResult: index<"+index+
				"> inlresult<"+inlresult+">");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultNLResult(getCPtr(), index, inlresult.getCPtr()));
		}

		/// <summary>Gets the number of words separated by space in the result string</summary> 
		public int GetNumWords(int result) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumWords result<"+result+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultNumWords(getCPtr(), result, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNumWords result<"+result+"> ret<"+ret+">");
			return ret;
		}

		/// <summary>Gets overall confidence of specified result.</summary> 
		public int GetOverallConfidence(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetOverallConfidence index<"+index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultOverallConfidence(getCPtr(), index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetOverallConfidence index<"+index+"> ret<"+ret+">");
			return ret;
		}

		/// <summary>Gets segmentation and scoring information of specified result (Doesn't exist under Unix).</summary> 
		public void GetSegmentInfo(int index, SegmentInfo destInfo) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSegmentInfo index<"+
											  index+"> destInfo<"+destInfo+">");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultSegmentInfo(getCPtr(), index, destInfo.getCPtr()));
		}

		/// <summary>GetString() asks for the ascii result string to be copied into the supplied buffer.</summary> 
		public string GetString(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetString index<"+index+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultString(getCPtr(), index, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetString index<"+index+"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>Gets total log probability of specified result.</summary> 
		public int GetTotalProbability(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTotalProbability index<"+index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultTotalProbability(getCPtr(), index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetTotalProbability index<"+index+"> ret<"+ret+">");
			return ret;
		}

		/// <summary>Gets the confidence of the specified word in the specified result</summary> 
		public int GetWordConfidence(int resultI, int wordI) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordConfidence resultI<"+resultI+
				"> wordI<"+wordI+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultWordConfidence(getCPtr(), resultI, wordI, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordConfidence resultI<"+resultI+
				"> wordI<"+wordI+"> ret<"+ret+">");
			return ret;
		}

		/// <summary>Prints the segment info to stderr</summary> 
		public void PrintSegmentInfoToStderr(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PrintSegmentInfoToStderr: index<"+index+">");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultPrintSegmentInfoToStderr(getCPtr(), index));
		}

		/// <summary>Prints the segment info to stdout</summary> 
		public void PrintSegmentInfoToStdout(int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PrintSegmentInfoToStdout: index<"+index+">");
			EduSpeakUtil.checkRetValue(RecResultAPI.RecResultPrintSegmentInfoToStdout(getCPtr(), index));
		}

		/// <summary>Prints the RecResult to stderr</summary> 
		public void PrintToStderr() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PrintToStderr");
			RecResultAPI.RecResultPrintToStderr(getCPtr());
		}

		/// <summary>Prints the RecResult to stdout</summary> 
		public void PrintToStdout() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PrintToStdout");
			RecResultAPI.RecResultPrintToStdout(getCPtr());
		}

		/// <summary>the number of result answers stored in the specified RecResult structure (N-best processing can provide a set of possible results).</summary>
		public int NumAnswers {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumAnswers");
				int ret = 0;
				EduSpeakUtil.checkRetValue(RecResultAPI.RecResultNumAnswers(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumAnswers ret<"+ret+">");
				return ret;
			}
		}

		/// <summary>the number of frames of data that were processed to get the specified result.</summary>
		public int NumFrames {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumFrames");
				int ret = 0;
				EduSpeakUtil.checkRetValue(RecResultAPI.RecResultNumFrames(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumFrames ret<"+ret+">");
				return ret;
			}
		}
	}
}
