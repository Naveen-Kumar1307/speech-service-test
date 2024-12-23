using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EduSpeak
{
    public enum SegmentType { UnknownSegment, WordSegment, PhoneSegment, StateSegment };

	/// <summary>
	/// Summary description for RecResultAPI.
	/// </summary>
	public class SegmentAPI
	{
		[DllImport("rcapi.dll", EntryPoint="SegmentNewPublicWrapper")]
        public static extern IntPtr SegmentNew(string name, IntPtr model, SegmentType type, int start, int end, int prob, int gprob, int conf, int post);
		[DllImport("rcapi.dll", EntryPoint="SegmentDeletePublicWrapper")]
        public static extern NuanceStatus SegmentDelete(IntPtr segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetModelPublicWrapper")]
        public static extern NuanceStatus SegmentGetModel(IntPtr segment, [In,Out]ref IntPtr model);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetNamePublicWrapper")]
        public static extern NuanceStatus SegmentGetName(IntPtr segment, StringBuilder name, int len);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetTypePublicWrapper")]
        public static extern NuanceStatus SegmentGetType(IntPtr segment, ref SegmentType type);
		[DllImport("rcapi.dll", EntryPoint="SegmentTypeIsWordPublicWrapper")]
        public static extern NuanceStatus SegmentTypeIsWord(IntPtr segment, [In,Out]ref int isWord);
		[DllImport("rcapi.dll", EntryPoint="SegmentTypeIsPhonePublicWrapper")]
        public static extern NuanceStatus SegmentTypeIsPhone(IntPtr segment, [In,Out]ref int isPhone);
		[DllImport("rcapi.dll", EntryPoint="SegmentTypeIsStatePublicWrapper")]
        public static extern NuanceStatus SegmentTypeIsState(IntPtr segment, [In,Out]ref int isState);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetStartPublicWrapper")]
        public static extern NuanceStatus SegmentGetStart(IntPtr segment, [In,Out]ref int start);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetEndPublicWrapper")]
        public static extern NuanceStatus SegmentGetEnd(IntPtr segment, [In,Out]ref int end);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetDurationPublicWrapper")]
        public static extern NuanceStatus SegmentGetDuration(IntPtr segment, [In,Out]ref int duration);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetProbPublicWrapper")]
        public static extern NuanceStatus SegmentGetProb(IntPtr segment, [In,Out]ref int prob);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetGrammarProbPublicWrapper")]
        public static extern NuanceStatus SegmentGetGrammarProb(IntPtr segment, [In,Out]ref int gprob);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetConfidencePublicWrapper")]
        public static extern NuanceStatus SegmentGetConfidence(IntPtr segment, [In,Out]ref int conf);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetPosteriorPublicWrapper")]
        public static extern NuanceStatus SegmentGetPosterior(IntPtr segment, [In,Out]ref int post);
		[DllImport("rcapi.dll", EntryPoint="SegmentFreePhoneObservationProbsPublicWrapper")]
        public static extern NuanceStatus SegmentFreePhoneObservationProbs(IntPtr segment);
		[DllImport("rcapi.dll", EntryPoint="SegmentAllocatePhoneObservationProbsPublicWrapper")]
        public static extern NuanceStatus SegmentAllocatePhoneObservationProbs(IntPtr segment, int num_phones);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetNumPhonesPublicWrapper")]
        public static extern NuanceStatus SegmentGetNumPhones(IntPtr segment, [In,Out]ref int num_phones);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetNumElementsPublicWrapper")]
        public static extern NuanceStatus SegmentGetNumElements(IntPtr segment, [In,Out]ref int num_elements);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetPhoneObservationProbsElementPublicWrapper")]
        public static extern NuanceStatus SegmentGetPhoneObservationProbsElement(IntPtr segment, int index, [In,Out]ref int val);
		[DllImport("rcapi.dll", EntryPoint="SegmentSetPhoneObservationProbsElementPublicWrapper")]
        public static extern NuanceStatus SegmentSetPhoneObservationProbsElement(IntPtr segment, int index, int val);
		[DllImport("rcapi.dll", EntryPoint="SegmentSetPhonePublicWrapper")]
        public static extern NuanceStatus SegmentSetPhone(IntPtr segment, string hone);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetPhonePublicWrapper")]
        public static extern NuanceStatus SegmentGetPhone(IntPtr segment, StringBuilder phone, int len);
		[DllImport("rcapi.dll", EntryPoint="SegmentSetPhoneIDPublicWrapper")]
        public static extern NuanceStatus SegmentSetPhoneID(IntPtr segment, int phone_id);
		[DllImport("rcapi.dll", EntryPoint="SegmentGetPhoneIDPublicWrapper")]
        public static extern NuanceStatus SegmentGetPhoneID(IntPtr segment, [In,Out]ref int phone_id);
	}
}
