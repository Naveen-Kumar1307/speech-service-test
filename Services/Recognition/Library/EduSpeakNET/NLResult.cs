using System;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for NLResult.
	/// </summary>
	public class NLResult: CPtrRef
	{
		private static Logger logger = new Logger("EduSpeak.NLResult");

		public NLResult(IntPtr cptr): base(cptr) {
		}

		public NLResult() {
			NuanceStatus status = NuanceStatus.NUANCE_ERROR;
			if (Logger.GetTraceEnabled()) logger.Trace(this, "NLInitializeResult");
			IntPtr nlRef = NLAPI.NLInitializeResult(ref status);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "NLInitializeResult: status<"+status+">");
			EduSpeakUtil.checkRetValue(status);
			setCPtr(nlRef);
		}

		protected void Finalize() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "NLFreeResult");
			NLAPI.NLFreeResult(getCPtr());
		}

		/// <summary>Clears the given NL result object.")]
		public void ClearResult() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ClearResult");
			NLAPI.NLClearResult(getCPtr());
		}

		/// <summary>Clears the specified slot in the active interpretation of the given NLResult object.</summary>
		public void ClearSlot(string slot) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ClearSlot: slot<"+slot+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLClearSlot(getCPtr(), slot));
		}

		/// <summary>Fills the specified slot with the specified INLValue object.</summary>
		public void FillSlotWithValue(string slot, NLValue inlvalue) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "FillSlotWithValue: slot<"+slot+
				"> inlvalue<"+inlvalue+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLFillSlotWithValue(getCPtr(), slot, inlvalue.getCPtr()));
		}

		/// <summary>This function is used to get the floating point value of a specified slot.</summary>
		public double GetFloatSlotValue(string slotName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFloatSlotValue: slotName<"+slotName+">");
			double ret = 0;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetFloatSlotValue(getCPtr(), slotName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFloatSlotValue: slotName<"+slotName+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary> This function is used to get the integer value of a specified slot.</summary>
		public int GetIntSlotValue(string slotName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntSlotValue: slotName<"+slotName+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIntSlotValue(getCPtr(), slotName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntSlotValue: slotName<"+slotName+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary>Call this function to get the name of the slot from the ith filled slot of an interpretation.</summary>
		public string GetIthSlotName(int i) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthSlotName: i<"+i+">");
			const int len = 4096;
			StringBuilder name = new StringBuilder(len);
			NLValueType type = NLValueType.NL_FLOAT_VALUE;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIthSlotNameAndType(getCPtr(), i, name, len, ref type));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthSlotName: i<"+i+
				"> name<"+name+">");
			return name.ToString();
		}

		/// <summary>Call this function to get the type of the filler from the ith filled slot of an interpretation.</summary>
		public NLValueType GetIthSlotType(int i) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthSlotName: i<"+i+">");
			const int len = 4096;
			StringBuilder name = new StringBuilder(len);
			NLValueType type = NLValueType.NL_FLOAT_VALUE;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIthSlotNameAndType(getCPtr(), i, name, len, ref type));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthSlotName: i<"+i+
				"> type<"+type+">");
			return type;
		}

		/// <summary>Gets the type of a filled slot.</summary>
		public NLValueType GetSlotType(string slotName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotType: slotName<"+slotName+">");
			NLValueType ret = 0;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetSlotType(getCPtr(), slotName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotType: slotName<"+slotName+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary> This function is used to get the value of a specified slot.</summary>
		public NLValue GetSlotValue(string slotName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotValue: slotName<"+slotName+">");
			IntPtr ret = new IntPtr(0);
			EduSpeakUtil.checkRetValue(NLAPI.NLGetSlotValue(getCPtr(), slotName, ref ret));
			return new NLValue(ret);
		}

		/// <summary>This function produces a string representation of the filler of the specified slot (even if that filler is, say, an integer).</summary>
		public string GetSlotValueAsString(string slotName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotValueAsString: slotName<"+slotName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(NLAPI.NLGetSlotValueAsString(getCPtr(), slotName, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotValueAsString: slotName<"+slotName+
				"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>This function is used to get the string value of a specified slot.</summary>
		public string GetStringSlotValue(string slotName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringSlotValue: slotName<"+slotName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(NLAPI.NLGetStringSlotValue(getCPtr(), slotName, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringSlotValue: slotName<"+slotName+
				"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>Call this function to make the ith interpretation active.</summary>
		public void MakeIthInterpretationActive(int i) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "MakeIthInterpretationActive: i<"+i+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLMakeIthInterpretationActive(getCPtr(), i));
		}

		/// <summary>The score for the interpretation(s) that was (were) produced for the last processed sentence.</summary>
		public int InterpretationScorePhrases {
		    get {
				int numWords = 0;
				int numPhrases = 0;
				if (Logger.GetTraceEnabled()) logger.Trace(this, "InterpretationScorePhrases");
				EduSpeakUtil.checkRetValue(NLAPI.NLGetInterpretationScore(getCPtr(), ref numWords, ref numPhrases));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "InterpretationScorePhrases: numPhrases<"+numPhrases+">");
		        return numPhrases;
		    }
		}

		/// <summary>The score for the interpretation(s) that was (were) produced for the last processed sentence.</summary>
		public int InterpretationScoreWords {
		    get {
				int numWords = 0;
				int numPhrases = 0;
				if (Logger.GetTraceEnabled()) logger.Trace(this, "InterpretationScoreWords");
				EduSpeakUtil.checkRetValue(NLAPI.NLGetInterpretationScore(getCPtr(), ref numWords, ref numPhrases));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "InterpretationScoreWords: numPhrases<"+numPhrases+">");
		        return numWords;
		    }
		}

		/// <summary>Produces a string representation of the current interpretation.</summary>
		public string InterpretationString {
		    get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "InterpretationString");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				EduSpeakUtil.checkRetValue(NLAPI.NLGetInterpretationString(getCPtr(), ret, len));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "InterpretationString: ret<"+ret+">");
				return ret.ToString();
		    }
		}

		/// <summary>Determines if the score is perfect</summary>
		public bool IsScorePerfect {
		    get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "IsScorePerfect");
				NuanceStatus status = NuanceStatus.NUANCE_ERROR;
				int ret = NLAPI.NLIsScorePerfect(getCPtr(), ref status);
				if (Logger.GetTraceEnabled()) logger.Trace(this, "IsScorePerfect: status<"+status+"> ret<"+ret+">");
				EduSpeakUtil.checkRetValue(status);
		        return ret != 0;
		    }
		}

		/// <summary>the number of slots filled in the currently active interpretation.</summary>
		public int NumberOfFilledSlots {
		    get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumberOfFilledSlots");
				NuanceStatus status = NuanceStatus.NUANCE_ERROR;
				int ret = NLAPI.NLGetNumberOfFilledSlots(getCPtr(), ref status);
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumberOfFilledSlots: status<"+status+"> ret<"+ret+">");
				EduSpeakUtil.checkRetValue(status);
		        return ret;
		    }
		}

		/// <summary>the number of interpretations that were produced for the last processed sentence.</summary>
		public int NumberOfInterpretations {
		    get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumberOfInterpretations");
				NuanceStatus status = NuanceStatus.NUANCE_ERROR;
				int ret = NLAPI.NLGetNumberOfInterpretations(getCPtr(), ref status);
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumberOfInterpretations: status<"+status+"> ret<"+ret+">");
				EduSpeakUtil.checkRetValue(status);
		        return ret;
		    }
		}

		/// <summary>Produces a string representation of the current interpretation in a form that can be read from a file by scoring code.</summary>
		public string ReadableInterpretationString {
		    get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "ReadableInterpretationString");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				EduSpeakUtil.checkRetValue(NLAPI.NLGetReadableInterpretationString(getCPtr(), ret, len));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "ReadableInterpretationString: ret<"+ret+">");
				return ret.ToString();
		    }
		}
		
	}
}
