using System;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for NLValue.
	/// </summary>
	public class NLValue: CPtrRef
	{
		private static Logger logger = new Logger("EduSpeak.NLValue");

		public NLValue(IntPtr cptr): base(cptr) {}


		public NLValue() {
			NuanceStatus status = NuanceStatus.NUANCE_ERROR;
			if (Logger.GetTraceEnabled()) logger.Trace(this, "NLNewValue");
			IntPtr nlValueRef = NLAPI.NLNewValue(ref status);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "NLNewValue: status<"+status+">");
			EduSpeakUtil.checkRetValue(status);
			setCPtr(nlValueRef);
		}

		protected void Finalize() {
			Free();
		}

		/// <summary>Adds a feature-value pair to the structure in the given NLValue object.</summary>
		public void AddFeatureValuePairToStructure(string feature, NLValue inlvalue) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "AddFeatureValuePairToStructure: "+
											  "feature<"+feature+"> inlvalue<"+inlvalue+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLAddFeatureValuePairToStructure(getCPtr(), feature,
				inlvalue.getCPtr()));
		}

		/// <summary> Adds an item to the list in the given NLValue object.</summary>
		public void AddItemToList(NLValue inlvalue) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "AddItemToList: "+
											  "nlvalue<"+inlvalue+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLAddItemToList(getCPtr(), inlvalue.getCPtr()));
		}

		/// <summary>Copies the contents of another INLValue into this INLValue</summary>
		public void CopyFrom(NLValue inlvalue) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "CopyFrom: "+
											  "nlvalue<"+inlvalue+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLCopyValue(inlvalue.getCPtr(), getCPtr()));
		}

		/// <summary>This function frees an NLValue object.</summary>
		public void Free() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Free");
			NLAPI.NLFreeValue(getCPtr());
		}

		/// <summary>Gets the type of a feature value.</summary>
		public NLValueType GetFeatureType(string feature) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFeatureType");
			NLValueType ret = 0;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetFeatureType(getCPtr(), feature, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFeatureType: ret<"+ret+">");
			return ret;
		}
		
		/// <summary>This function is used to get the value of the specified feature of a specified structure.</summary>
		public void GetFeatureValue(string feature, NLValue inlvalue) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFeatureValue: feature<"+feature+
				"> inlvalue<"+inlvalue+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLGetFeatureValue(getCPtr(), feature, inlvalue.getCPtr()));
		}

		/// <summary>This function produces a string representation of the value of the specified feature of a structure (even if that value is, say, an</summary>
		public string GetFeatureValueAsString(string feature) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFeatureValueAsString: feature<"+feature+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(NLAPI.NLGetFeatureValueAsString(getCPtr(), feature, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFeatureValueAsString: feature<"+feature+
				"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>This function is used to get the floating point value of a specified feature.</summary>
		public double GetFloatFeatureValue(string feature) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntSlotValue: feature<"+feature+">");
			double ret = 0;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetFloatFeatureValue(getCPtr(), feature, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntSlotValue: feature<"+feature+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary>This function is used to get the floating point value of a specified NLValue object.</summary>
		public double FloatFromValue {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "FloatFromValue");
				double ret = 0;
				EduSpeakUtil.checkRetValue(NLAPI.NLGetFloatFromValue(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "FloatFromValue: ret<"+ret+">");
				return ret;
			}
		}

		/// <summary> This function is used to get the int point value of a specified feature.</summary>
		public int GetIntFeatureValue(string feature) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntFeatureValue: feature<"+feature+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIntFeatureValue(getCPtr(), feature, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntFeatureValue: feature<"+feature+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary>This function is used to get the integer value of a specified NLValue object.</summary>
		public int IntFromValue {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "IntFromValue");
				int ret = 0;
				EduSpeakUtil.checkRetValue(NLAPI.NLGetIntFromValue(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "IntFromValue: ret<"+ret+">");
				return ret;
			}
		}

		/// <summary>Call this function to get the name of the feature from the ith feature of a structure.</summary>
		public string GetIthFeatureName(int i) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthFeatureName: i<"+i+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			NLValueType type = NLValueType.NL_FLOAT_VALUE;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIthFeatureNameAndType(getCPtr(), i, ret, len, ref type));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthFeatureName: i<"+i+
				"> name<"+ret+">");
			return ret.ToString();
		}

		/// <summary>Call this function to get the type of the feature from the ith feature of a structure.</summary>
		public NLValueType GetIthFeatureType(int i) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthFeatureType: i<"+i+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			NLValueType type = NLValueType.NL_FLOAT_VALUE;
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIthFeatureNameAndType(getCPtr(), i, ret, len, ref type));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthFeatureType: i<"+i+
				"> type<"+type+">");
			return type;
		}

		/// <summary>This function gets the ith value in a list and the type of that value.</summary>
		public NLValueType GetIthValueAndTypeInList(int i, NLValue inlvalue) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthValueAndTypeInList: i<"+i+
				"> inlvalue<"+inlvalue+">");
			NLValueType type = NLValueType.NL_FLOAT_VALUE;	
			EduSpeakUtil.checkRetValue(NLAPI.NLGetIthValueAndTypeInList(getCPtr(), i, inlvalue.getCPtr(), ref type));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIthValueAndTypeInList: i<"+i+
				"> inlvalue<"+inlvalue+"> type<"+type+">");
			return type;
			
		}

		/// <summary>Returns the length of the given list.</summary>
		public int LengthOfList {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "LengthOfList");
				int ret = 0;
				EduSpeakUtil.checkRetValue(NLAPI.NLGetLengthOfList(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "LengthOfList: ret<"+ret+">");
				return ret;
			}
		}

		/// <summary>Retrieves the number of features in a structure NLValue</summary>
		public int NumFeatures {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumFeatures");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				NuanceStatus status = NuanceStatus.NUANCE_OK;
				NLValueType type = NLValueType.NL_FLOAT_VALUE;
				int i = 0;
				while (status != NuanceStatus.NUANCE_ARGUMENT_OUT_OF_RANGE && EduSpeakUtil.NUANCE_SUCCESS(status)) {
					status = NLAPI.NLGetIthFeatureNameAndType(getCPtr(), i, ret, len, ref type);
					i++;
				}
				if (status == NuanceStatus.NUANCE_ARGUMENT_OUT_OF_RANGE) {
					status = NuanceStatus.NUANCE_OK;
				}
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumFeatures: ret<"+ret+
					"> status<"+status+">");
				EduSpeakUtil.checkRetValue(status);
				return i-1;
			}
		}

		/// <summary>This function gets the list of all slot names in the package specified by the given NuanceConfig object.</summary>
		public void GetSlotNamesFromConfig(NuanceConfig inuanceconfig) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotNamesFromConfig: "+
											  "inuanceconfig<"+inuanceconfig+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLGetSlotNamesFromConfig(inuanceconfig.getCPtr(), getCPtr()));
		}

		/// <summary>This function gets the list of all slot names in the given package.</summary>
		public void GetSlotNamesFromPackage(string packageDir) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSlotNamesFromPackage: "+
											  "packageDir<"+packageDir+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLGetSlotNamesFromPackage(packageDir, getCPtr()));
		}

		/// <summary> This function is used to get the string value of a specified feature.</summary>
		public string GetStringFeatureValue(string feature) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringFeatureValue: feature<"+feature+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(NLAPI.NLGetStringFeatureValue(getCPtr(), feature, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringFeatureValue: feature<"+feature+
				"> name<"+ret+">");
			return ret.ToString();
		}

		/// <summary>This function is used to get the string value of a specified NLValue object.</summary>
		public string StringFromValue {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "StringFromValue");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				EduSpeakUtil.checkRetValue(NLAPI.NLGetStringFromValue(getCPtr(), ret, len));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "StringFromValue: ret<"+ret+">");
				return ret.ToString();
			}
		}

		/// <summary>This function produces a string representation of an NLValue object (even if that value is, say, an integer).</summary>
		public string ValueAsString {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "ValueAsString");
				const int len = 4096;
				StringBuilder ret = new StringBuilder(len);
				EduSpeakUtil.checkRetValue(NLAPI.NLGetValueAsString(getCPtr(), ret, len));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "ValueAsString: ret<"+ret+">");
				return ret.ToString();
			}
		}

		/// <summary>Gets the type of a value.</summary>
		public NLValueType ValueType {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "ValueType");
				NLValueType ret = 0;
				EduSpeakUtil.checkRetValue(NLAPI.NLGetValueType(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "ValueType: ret<"+ret+">");
				return ret;
			}
		}

		/// <summary>Sets the specified NLValue object to an empty list.</summary>
		public void SetValueToEmptyList() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetValueToEmptyList");
			EduSpeakUtil.checkRetValue(NLAPI.NLSetValueToEmptyList(getCPtr()));
		}

		/// <summary>Sets the specified NLValue object to an empty structure.</summary>
		public void SetValueToEmptyStructure() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetValueToEmptyStructure");
			EduSpeakUtil.checkRetValue(NLAPI.NLSetValueToEmptyStructure(getCPtr()));
		}

		/// <summary>Sets the specified NLValue object to the given floating point value.</summary>
		public void SetValueToFloat(double val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetValueToFloat: val<"+val+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLSetValueToFloat(getCPtr(), val));
		}

		/// <summary>Sets the specified NLValue object to the given integer value.</summary>
		public void SetValueToInt(int val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetValueToInt: val<"+val+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLSetValueToInt(getCPtr(), val));
		}

		/// <summary>Sets the specified NLValue object to the given string value.</summary>
		public void SetValueToString(string val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetValueToString: val<"+val+">");
			EduSpeakUtil.checkRetValue(NLAPI.NLSetValueToString(getCPtr(), val));
		}
	}
}
