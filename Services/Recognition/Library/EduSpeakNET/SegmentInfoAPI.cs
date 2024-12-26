using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for RecResultAPI.
	/// </summary>
	public class SegmentInfoAPI
	{
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoNewPublicWrapper")]
        public static extern IntPtr SegmentInfoNew(int size);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoNewCopyPublicWrapper")]
        public static extern IntPtr SegmentInfoNewCopy(IntPtr src_segmentinfo);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoDeletePublicWrapper")]
        public static extern NuanceStatus SegmentInfoDelete(IntPtr seg_info);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoClearPublicWrapper")]
        public static extern NuanceStatus SegmentInfoClear(IntPtr seg_info);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoCopyPublicWrapper")]
        public static extern NuanceStatus SegmentInfoCopy(IntPtr dest_seg_info, IntPtr src_seg_info);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoAddSegmentPublicWrapper")]
        public static extern NuanceStatus SegmentInfoAddSegment(IntPtr seg_info, string n, IntPtr m, SegmentType t, int s, int e, int p, int g, int c, int post);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoAddSegmentSegmentPublicWrapper")]
        public static extern NuanceStatus SegmentInfoAddSegmentSegment(IntPtr seg_info, IntPtr src_segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoAddSegmentSegmentDontCopyPublicWrapper")]
        public static extern NuanceStatus SegmentInfoAddSegmentSegmentDontCopy(IntPtr seg_info, IntPtr src_segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetSegmentPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetSegment(IntPtr seg_info, int index, [In,Out]ref IntPtr segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoNumSegmentsPublicWrapper")]
        public static extern NuanceStatus SegmentInfoNumSegments(IntPtr seg_info, [In,Out]ref int num_segments);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetStartFramePublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetStartFrame(IntPtr seg_info, [In,Out]ref int start_frame);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetEndFramePublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetEndFrame(IntPtr seg_info, [In,Out]ref int end_frame);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetDurationPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetDuration(IntPtr seg_info, [In,Out]ref int duration);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetProbabilityPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetProbability(IntPtr seg_info, [In,Out]ref int prob);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetGrammarProbabilityPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetGrammarProbability(IntPtr seg_info, [In,Out]ref int grammar_prob);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetConfidencePublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetConfidence(IntPtr seg_info, [In,Out]ref int confidence);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoSetConfidencePublicWrapper")]
        public static extern NuanceStatus SegmentInfoSetConfidence(IntPtr seg_info, int confidence);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetConfidenceNumWordsPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetConfidenceNumWords(IntPtr seg_info, [In,Out]ref int num_words);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetWordConfidencePublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetWordConfidence(IntPtr seg_info, int index, [In,Out]ref int confidence);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoSetConfidenceNumWordsPublicWrapper")]
        public static extern NuanceStatus SegmentInfoSetConfidenceNumWords(IntPtr seg_info, int num_words);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoSetWordConfidencePublicWrapper")]
        public static extern NuanceStatus SegmentInfoSetWordConfidence(IntPtr seg_info, int index, int confidence);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetPosteriorPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetPosterior(IntPtr seg_info, [In,Out]ref int posterior);
		[DllImport("rcapi.dll", EntryPoint="PSegmentInfoSetPosteriorublicWrapper")]
        public static extern NuanceStatus SegmentInfoSetPosterior(IntPtr seg_info, int posterior);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoHasWordsPublicWrapper")]
        public static extern NuanceStatus SegmentInfoHasWords(IntPtr seg_info, [In,Out]ref int has_words);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetNumWordsPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetNumWords(IntPtr seg_info, [In,Out]ref int num_words);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetWordSegmentPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetWordSegment(IntPtr seg_info, int word_index, IntPtr segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoHasPhonesPublicWrapper")]
        public static extern NuanceStatus SegmentInfoHasPhones(IntPtr seg_info, [In,Out]ref int has_phones);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetNumPhonesPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetNumPhones(IntPtr seg_info, [In,Out]ref int num_phones);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetNumPhonesInWordPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetNumPhonesInWord(IntPtr seg_info, int word_index, [In,Out]ref int num_phones_in_word);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetPhoneSegmentInWordPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetPhoneSegmentInWord(IntPtr seg_info, int word_index, int phone_index, IntPtr segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoHasStatesPublicWrapper")]
        public static extern NuanceStatus SegmentInfoHasStates(IntPtr seg_info, [In,Out]ref int has_states);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetNumStatesPublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetNumStates(IntPtr seg_info, [In,Out]ref int num_states);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetNumStatesInPhonePublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetNumStatesInPhone(IntPtr seg_info, int word_index, int phone_index, [In,Out]ref int num_states);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoGetStateSegmentInPhonePublicWrapper")]
        public static extern NuanceStatus SegmentInfoGetStateSegmentInPhone(IntPtr seg_info, int word_index, int phone_index, int state_index, [In,Out]ref IntPtr segment);
		//[DllImport("rcapi.dll", EntryPoint="SegmentInfoPrintOldStylePublicWrapper")]
        //public static extern NuanceStatus SegmentInfoPrintOldStyle(IntPtr seg_info, FILE *fp);
		//[DllImport("rcapi.dll", EntryPoint="SegmentInfoPrintPublicWrapper")]
        //public static extern NuanceStatus SegmentInfoPrint(IntPtr seg_info, FILE *fp);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoSetCompletePublicWrapper")]
        public static extern NuanceStatus SegmentInfoSetComplete(IntPtr seg_info);
		[DllImport("rcapi.dll", EntryPoint="SegmentInfoIsValidPublicWrapper")]
        public static extern NuanceStatus SegmentInfoIsValid(IntPtr seg_info);
	}
}
