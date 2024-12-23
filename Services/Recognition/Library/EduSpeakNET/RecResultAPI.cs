using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for RecResultAPI.
	/// </summary>
	public class RecResultAPI
	{
		[DllImport("rcapi.dll", EntryPoint="RecResultNewPublicWrapper")]
        public static extern IntPtr RecResultNew();
		[DllImport("rcapi.dll", EntryPoint="RecResultDeletePublicWrapper")]
        public static extern NuanceStatus RecResultDelete(IntPtr rr);
		[DllImport("rcapi.dll", EntryPoint="RecResultNumAnswersPublicWrapper")]
        public static extern NuanceStatus RecResultNumAnswers(IntPtr rr, [In,Out]ref int num);
		[DllImport("rcapi.dll", EntryPoint="RecResultNumFramesPublicWrapper")]
        public static extern NuanceStatus RecResultNumFrames(IntPtr rr, [In,Out]ref int num_frames);
		[DllImport("rcapi.dll", EntryPoint="RecResultStringPublicWrapper")]
        public static extern NuanceStatus RecResultString(IntPtr rr, int index, StringBuilder buffer, int len);
		[DllImport("rcapi.dll", EntryPoint="RecResultNLResultPublicWrapper")]
        public static extern NuanceStatus RecResultNLResult(IntPtr rr, int index, IntPtr nl_result);
		[DllImport("rcapi.dll", EntryPoint="RecResultTotalProbabilityPublicWrapper")]
        public static extern NuanceStatus RecResultTotalProbability(IntPtr rr, int index, [In,Out]ref int total_prob);
		[DllImport("rcapi.dll", EntryPoint="RecResultOverallConfidencePublicWrapper")]
        public static extern NuanceStatus RecResultOverallConfidence(IntPtr rr, int index, [In,Out]ref int conf);
		[DllImport("rcapi.dll", EntryPoint="RecResultSegmentInfoPublicWrapper")]
        public static extern NuanceStatus RecResultSegmentInfo(IntPtr rr, int index, IntPtr seg_info);
		//[DllImport("rcapi.dll", EntryPoint="RecResultPrintSegmentInfoPublicWrapper")]
        //public static extern NuanceStatus RecResultPrintSegmentInfo(IntPtr rr, int index, FILE *fp);
		[DllImport("rcapi.dll", EntryPoint="RecResultPrintSegmentInfoToStdoutPublicWrapper")]
        public static extern NuanceStatus RecResultPrintSegmentInfoToStdout(IntPtr rr, int index);
		[DllImport("rcapi.dll", EntryPoint="RecResultPrintSegmentInfoToStderrPublicWrapper")]
        public static extern NuanceStatus RecResultPrintSegmentInfoToStderr(IntPtr rr, int index);
		[DllImport("rcapi.dll", EntryPoint="RecResultNumWordsPublicWrapper")]
        public static extern NuanceStatus RecResultNumWords(IntPtr rr, int result_i, [In,Out]ref int num_words);
		[DllImport("rcapi.dll", EntryPoint="RecResultWordConfidencePublicWrapper")]
        public static extern NuanceStatus RecResultWordConfidence(IntPtr rr, int result_i, int word_i, [In,Out]ref int conf);
		[DllImport("rcapi.dll", EntryPoint="RecResultConsistencyStatusPublicWrapper")]
        public static extern NuanceStatus RecResultConsistencyStatus(IntPtr rr, [In,Out]ref int cons_value);
		[DllImport("rcapi.dll", EntryPoint="RecResultClashStatusPublicWrapper")]
        public static extern NuanceStatus RecResultClashStatus(IntPtr rr, [In,Out]ref int clash_value);
		[DllImport("rcapi.dll", EntryPoint="RecResultExceptionPublicWrapper")]
        public static extern NuanceStatus RecResultException(IntPtr rr, ref NuanceStatus statusp, StringBuilder buffer,
                             int buffer_size);
		[DllImport("rcapi.dll", EntryPoint="RecResultCopyPublicWrapper")]
        public static extern NuanceStatus RecResultCopy(IntPtr rr_source, IntPtr rr_target);
		[DllImport("rcapi.dll", EntryPoint="RecResultSizePublicWrapper")]
        public static extern int RecResultSize();
		//[DllImport("rcapi.dll", EntryPoint="RecResultPrintPublicWrapper")]
        //public static extern void RecResultPrint(IntPtr rr, FILE *fp);
		[DllImport("rcapi.dll", EntryPoint="RecResultPrintToStdoutPublicWrapper")]
        public static extern void RecResultPrintToStdout(IntPtr rr);
		[DllImport("rcapi.dll", EntryPoint="RecResultPrintToStderrPublicWrapper")]
        public static extern void RecResultPrintToStderr(IntPtr rr);
	}
}
