using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EduSpeak
{
	///<summary>Returned in NLGetIthSlotNameAndType() and NLGetSlotType().</summary>
	public enum NLValueType {
		NL_INT_VALUE,
		NL_STRING_VALUE,
		NL_STRUCTURE_VALUE,
		NL_LIST_VALUE,
		NL_FLOAT_VALUE
	};

	/// <summary>
	/// Provides access to NL* rcapi methods.
	/// </summary>
	public class NLAPI
	{
		[DllImport("rcapi.dll", EntryPoint="NLInitializeResultPublicWrapper")]
		public static extern IntPtr NLInitializeResult([In,Out]ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NLFreeResultPublicWrapper")]
    	public static extern void NLFreeResult(IntPtr nlresult_nl_result);
		[DllImport("rcapi.dll", EntryPoint="NLGetInterpretationScorePublicWrapper")]
    	public static extern NuanceStatus NLGetInterpretationScore(IntPtr nl_result,
                             [In,Out]ref int num_words,  [In,Out]ref int num_phrases);
		[DllImport("rcapi.dll", EntryPoint="NLIsScorePerfectPublicWrapper")]
    	public static extern int NLIsScorePerfect(IntPtr nl_result, [In,Out]ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NLGetNumberOfInterpretationsPublicWrapper")]
    	public static extern int NLGetNumberOfInterpretations(IntPtr nl_result,
                                 [In,Out]ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NLMakeIthInterpretationActivePublicWrapper")]
    	public static extern NuanceStatus NLMakeIthInterpretationActive(IntPtr nlresult_nl_result, int i);
		[DllImport("rcapi.dll", EntryPoint="NLGetNumberOfFilledSlotsPublicWrapper")]
    	public static extern int NLGetNumberOfFilledSlots(IntPtr nl_result, [In,Out]ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NLGetIthSlotNameAndTypePublicWrapper")]
    	public static extern NuanceStatus NLGetIthSlotNameAndType(IntPtr nl_result, int i,
							StringBuilder slot_name_buffer,
                              int slot_name_buffer_length,
                              [In,Out]ref NLValueType value_type);
		[DllImport("rcapi.dll", EntryPoint="NLGetSlotNamesFromConfigPublicWrapper")]
    	public static extern NuanceStatus NLGetSlotNamesFromConfig(IntPtr nuance_config, IntPtr nlvalue_slot_list);
		[DllImport("rcapi.dll", EntryPoint="NLGetSlotNamesFromPackagePublicWrapper")]
    	public static extern NuanceStatus NLGetSlotNamesFromPackage(string package_dir, IntPtr nlvalue_slot_list);
		[DllImport("rcapi.dll", EntryPoint="NLNewValuePublicWrapper")]
    	public static extern IntPtr NLNewValue([In,Out]ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NLFreeValuePublicWrapper")]
    	public static extern void NLFreeValue(IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLCopyValuePublicWrapper")]
    	public static extern NuanceStatus NLCopyValue(IntPtr nlvalue_from, IntPtr nlvalue_to);
		[DllImport("rcapi.dll", EntryPoint="NLGetSlotValuePublicWrapper")]
    	public static extern NuanceStatus NLGetSlotValue(IntPtr nl_result, string slot_name,
                   ref IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetIntSlotValuePublicWrapper")]
    	public static extern NuanceStatus NLGetIntSlotValue(IntPtr nl_result, string slot_name,
                      [In,Out]ref int int_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetIntFromValuePublicWrapper")]
    	public static extern NuanceStatus NLGetIntFromValue(IntPtr nlvalue_nl_value, [In,Out]ref int int_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetFloatSlotValuePublicWrapper")]
    	public static extern NuanceStatus NLGetFloatSlotValue(IntPtr nl_result, string slot_name,
                         [In,Out]ref double float_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetFloatFromValuePublicWrapper")]
    	public static extern NuanceStatus NLGetFloatFromValue(IntPtr nlvalue_value, [In,Out]ref double float_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetStringSlotValuePublicWrapper")]
    	public static extern NuanceStatus NLGetStringSlotValue(IntPtr nl_result, string slot_name,
                         StringBuilder buffer, int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetStringFromValuePublicWrapper")]
    	public static extern NuanceStatus NLGetStringFromValue(IntPtr nlvalue_nl_value, StringBuilder buffer,
                         int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetLengthOfListPublicWrapper")]
    	public static extern NuanceStatus NLGetLengthOfList(IntPtr nlvalue_list, [In,Out]ref int len);
		[DllImport("rcapi.dll", EntryPoint="NLGetIthValueInListPublicWrapper")]
    	public static extern NuanceStatus NLGetIthValueInList(IntPtr nlvalue_list, int i, IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetIthValueAndTypeInListPublicWrapper")]
    	public static extern NuanceStatus NLGetIthValueAndTypeInList(IntPtr nlvalue_list, int i, IntPtr nlvalue_value,
                               [In,Out]ref NLValueType value_type);
		[DllImport("rcapi.dll", EntryPoint="NLGetIthFeatureNameAndTypePublicWrapper")]
  		public static extern NuanceStatus NLGetIthFeatureNameAndType(IntPtr nlvalue_structure,
                             int i,
                             StringBuilder buffer,
                             int buffer_length,
                             [In,Out]ref NLValueType value_type);
		[DllImport("rcapi.dll", EntryPoint="NLGetFeatureValuePublicWrapper")]
  		public static extern NuanceStatus NLGetFeatureValue(IntPtr nlvalue_structure,
                    string feature,
                    IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetIntFeatureValuePublicWrapper")]
  		public static extern NuanceStatus NLGetIntFeatureValue(IntPtr nlvalue_structure,
                       string feature,
                       [In,Out]ref int int_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetFloatFeatureValuePublicWrapper")]
  		public static extern NuanceStatus NLGetFloatFeatureValue(IntPtr nlvalue_structure,
                         string feature,
                         [In,Out]ref double float_value);
		[DllImport("rcapi.dll", EntryPoint="NLGetStringFeatureValuePublicWrapper")]
  		public static extern NuanceStatus NLGetStringFeatureValue(IntPtr nlvalue_structure,
                          string feature,
                          StringBuilder buffer,
                          int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetValueTypePublicWrapper")]
  		public static extern NuanceStatus NLGetValueType(IntPtr nlvalue_value,
                 [In,Out]ref NLValueType value_type);
		[DllImport("rcapi.dll", EntryPoint="NLGetSlotTypePublicWrapper")]
  		public static extern NuanceStatus NLGetSlotType(IntPtr nl_result,
                string slot_name,
                [In,Out]ref NLValueType value_type);
		[DllImport("rcapi.dll", EntryPoint="NLGetFeatureTypePublicWrapper")]
  		public static extern NuanceStatus NLGetFeatureType(IntPtr nlvalue_structure,
                   string feature,
                   [In,Out]ref NLValueType value_type);
		[DllImport("rcapi.dll", EntryPoint="NLGetInterpretationStringPublicWrapper")]
  		public static extern NuanceStatus NLGetInterpretationString(IntPtr nl_result,
                            StringBuilder buffer,
                            int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetReadableInterpretationStringPublicWrapper")]
  		public static extern NuanceStatus NLGetReadableInterpretationString(IntPtr nl_result,
                                    StringBuilder buffer,
                                    int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetSlotValueAsStringPublicWrapper")]
  		public static extern NuanceStatus NLGetSlotValueAsString(IntPtr nl_result,
                         string slot_name,
                         StringBuilder buffer,
                         int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetFeatureValueAsStringPublicWrapper")]
  		public static extern NuanceStatus NLGetFeatureValueAsString(IntPtr nlvalue_structure,
                            string feature,
                            StringBuilder buffer,
                            int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLGetValueAsStringPublicWrapper")]
  		public static extern NuanceStatus NLGetValueAsString(IntPtr nlvalue_value,
                     StringBuilder buffer,
                     int buffer_length);
		[DllImport("rcapi.dll", EntryPoint="NLClearResultPublicWrapper")]
  		public static extern void NLClearResult(IntPtr nlresult_nl_result);
		[DllImport("rcapi.dll", EntryPoint="NLClearSlotPublicWrapper")]
  		public static extern NuanceStatus NLClearSlot(IntPtr nlresult_nl_result, string slot);
		[DllImport("rcapi.dll", EntryPoint="NLFillSlotWithValuePublicWrapper")]
  		public static extern NuanceStatus NLFillSlotWithValue(IntPtr nlresult_nl_result, string slot,
                      IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLSetValueToIntPublicWrapper")]
  		public static extern NuanceStatus NLSetValueToInt(IntPtr nlvalue_value, int i);
		[DllImport("rcapi.dll", EntryPoint="NLSetValueToFloatPublicWrapper")]
  		public static extern NuanceStatus NLSetValueToFloat(IntPtr nlvalue_value, double d);
		[DllImport("rcapi.dll", EntryPoint="NLSetValueToStringPublicWrapper")]
  		public static extern NuanceStatus NLSetValueToString(IntPtr nlvalue_value, string str);
		[DllImport("rcapi.dll", EntryPoint="NLSetValueToEmptyStructurePublicWrapper")]
  		public static extern NuanceStatus NLSetValueToEmptyStructure(IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLSetValueToEmptyListPublicWrapper")]
  		public static extern NuanceStatus NLSetValueToEmptyList(IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLAddFeatureValuePairToStructurePublicWrapper")]
  		public static extern NuanceStatus NLAddFeatureValuePairToStructure(IntPtr nlvalue_structure,
                                   string feature,
                                   IntPtr nlvalue_value);
		[DllImport("rcapi.dll", EntryPoint="NLAddItemToListPublicWrapper")]
  		public static extern NuanceStatus NLAddItemToList(IntPtr nlvalue_list, IntPtr nlvalue_item);
	}
}
