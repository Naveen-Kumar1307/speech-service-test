using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EduSpeak
{
	public delegate void RCCallbackFnPtr(IntPtr user_data, NuanceEvent eventType, IntPtr eventData);

	public delegate void CallbackFnPtr(object user_data, NuanceEvent eventType, object eventData);

	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class RCAPI {
		private static Logger logger = new Logger("EduSpeak.RCAPI");

		[DllImport("rcapi.dll", EntryPoint="RCInitializePublicWrapper")]
		public static extern IntPtr RCInitialize(IntPtr nuance_config, ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="RCInitializeBlockingPublicWrapper")]
		public static extern IntPtr RCInitializeBlocking(IntPtr nuance_config, ref NuanceStatus status);

		[DllImport("rcapi.dll", EntryPoint="RCInitializeCommandLineBlockingPublicWrapper")]
		public static extern IntPtr RCInitializeCommandLineBlocking(string cmdLine, [In,Out]ref NuanceStatus nuanceStatus);
		[DllImport("rcapi.dll", EntryPoint="RCInitializeCommandLinePublicWrapper")]
		public static extern IntPtr RCInitializeCommandLine([MarshalAs(UnmanagedType.LPTStr)]string cmdLine, [In,Out]ref NuanceStatus nuanceStatus);

		[DllImport("rcapi.dll", EntryPoint="RCTerminatePublicWrapper")]
		public static extern NuanceStatus RCTerminate(IntPtr recClientPtr);
		
		[DllImport("rcapi.dll", EntryPoint="RCRegisterCallbackPublicWrapper")]
		public static extern NuanceStatus RCRegisterCallback(IntPtr recClientPtr, NuanceEvent eventType, 
			RCCallbackFnPtr callback, IntPtr user_data);


		[DllImport("rcapi.dll", EntryPoint="NuanceStatusFromStringPublicWrapper")]
		public static extern NuanceStatus NuanceStatusFromString(string status_string);
		[DllImport("rcapi.dll", EntryPoint="NuanceStatusToStringPublicWrapper")]
		public static extern string NuanceStatusToString(NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NuanceStatusMessagePublicWrapper")]
		public static extern string NuanceStatusMessage(NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="NuanceEventNamePublicWrapper")]
		public static extern string NuanceEventName(NuanceEvent eventType);
		
		[DllImport("rcapi.dll", EntryPoint="RCGetEnvironmentVariablePublicWrapper")]
		public static extern NuanceStatus RCGetEnvironmentVariable(string variable_name, StringBuilder valueStr, int len);
		[DllImport("rcapi.dll", EntryPoint="RCSetEnvironmentVariablePublicWrapper")]
		public static extern NuanceStatus RCSetEnvironmentVariable(string variable_name, string valueStr);
		[DllImport("rcapi.dll", EntryPoint="RCGetRegistryPublicWrapper")]
		public static extern NuanceStatus RCGetRegistry(string key, StringBuilder valueStr, int len);
		[DllImport("rcapi.dll", EntryPoint="RCSetRegistryPublicWrapper")]
		public static extern NuanceStatus RCSetRegistry(string key, string valueStr);

		
		[DllImport("rcapi.dll", EntryPoint="RCSetIntParameterPublicWrapper")]
		public static extern NuanceStatus RCSetIntParameter(IntPtr recClientPtr, string param, int val);
		[DllImport("rcapi.dll", EntryPoint="RCSetFloatParameterPublicWrapper")]
		public static extern NuanceStatus RCSetFloatParameter (IntPtr recClientPtr, string param, float val);
		[DllImport("rcapi.dll", EntryPoint="RCSetStringParameterPublicWrapper")]
		public static extern NuanceStatus RCSetStringParameter(IntPtr recClientPtr, string param, string val);
		[DllImport("rcapi.dll", EntryPoint="RCGetIntParameterPublicWrapper")]
		public static extern NuanceStatus RCGetIntParameter(IntPtr recClientPtr, string param, [In,Out]ref int val);
		[DllImport("rcapi.dll", EntryPoint="RCGetFloatParameterPublicWrapper")]
		public static extern NuanceStatus RCGetFloatParameter (IntPtr recClientPtr, string param, [In,Out]ref float val);
		[DllImport("rcapi.dll", EntryPoint="RCGetStringParameterPublicWrapper")]
		public static extern NuanceStatus RCGetStringParameter(IntPtr recClientPtr, string param, StringBuilder val,
								int maxlen);

		[DllImport("rcapi.dll", EntryPoint="RCGetNumGrammarsPublicWrapper")]
		public static extern NuanceStatus RCGetNumGrammars(IntPtr recClientPtr, [In,Out]ref int num_grammars);
		[DllImport("rcapi.dll", EntryPoint="RCGetGrammarPublicWrapper")]
		public static extern NuanceStatus RCGetGrammar(IntPtr recClientPtr, StringBuilder grammar_buf, int grammar_buf_len, int grammar_index);

		
		[DllImport("rcapi.dll", EntryPoint="RCGetNumPhonesInSentencePublicWrapper")]
		public static extern NuanceStatus RCGetNumPhonesInSentence(IntPtr recClientPtr, [In,Out]ref int num_phones);
		[DllImport("rcapi.dll", EntryPoint="RCGetNumGraphemesInSentencePublicWrapper")]
		public static extern NuanceStatus RCGetNumGraphemesInSentence(IntPtr recClientPtr, [In,Out]ref int num_graphemes);
		[DllImport("rcapi.dll", EntryPoint="RCGetBoundedPhoneScoresPublicWrapper")]
		public static extern NuanceStatus RCGetBoundedPhoneScores(IntPtr recClientPtr, StringBuilder phone, [In,Out]ref float score, [In,Out]ref float mean, int index);
		[DllImport("rcapi.dll", EntryPoint="RCGetBoundedPhoneScoresPhonePublicWrapper")] 
		public static extern NuanceStatus RCGetBoundedPhoneScoresPhone(IntPtr recClientPtr, StringBuilder phone, int index);
		[DllImport("rcapi.dll", EntryPoint="RCGetBoundedPhoneScoresScorePublicWrapper")]
		public static extern NuanceStatus RCGetBoundedPhoneScoresScore(IntPtr recClientPtr, [In,Out]ref float score, int index); 
		[DllImport("rcapi.dll", EntryPoint="RCGetBoundedPhoneScoresMeanPublicWrapper")]
		public static extern NuanceStatus RCGetBoundedPhoneScoresMean(IntPtr recClientPtr, [In,Out]ref float mean, int index); 
		[DllImport("rcapi.dll", EntryPoint="RCComputeGraphemeScoresPublicWrapper")]
		public static extern NuanceStatus RCComputeGraphemeScores(IntPtr recClientPtr, int percentFA);
		[DllImport("rcapi.dll", EntryPoint="RCScoringGetVectorsPublicWrapper")]
		public static extern NuanceStatus RCScoringGetVectors(IntPtr recClientPtr, StringBuilder phoneme, StringBuilder grapheme, [In,Out]ref float score, int index);
		[DllImport("rcapi.dll", EntryPoint="RCScoringGetVectorsPhonemePublicWrapper")]
		public static extern NuanceStatus RCScoringGetVectorsPhoneme(IntPtr recClientPtr, StringBuilder phoneme, int index);
		[DllImport("rcapi.dll", EntryPoint="RCScoringGetVectorsGraphemePublicWrapper")]
		public static extern NuanceStatus RCScoringGetVectorsGrapheme(IntPtr recClientPtr, StringBuilder grapheme, int index);
		[DllImport("rcapi.dll", EntryPoint="RCScoringGetVectorsScorePublicWrapper")]
		public static extern NuanceStatus RCScoringGetVectorsScore(IntPtr recClientPtr, [In,Out]ref float score, int index);

		[DllImport("rcapi.dll", EntryPoint="RCGetSentenceScorePublicWrapper")]
		public static extern NuanceStatus RCGetSentenceScore(IntPtr recClientPtr, [In,Out]ref float sentence_score);
		[DllImport("rcapi.dll", EntryPoint="RCGetSentenceScoreCombinePublicWrapper")]
		public static extern NuanceStatus RCGetSentenceScoreCombine(IntPtr recClientPtr, [In,Out]ref float combine_score);
		[DllImport("rcapi.dll", EntryPoint="RCGetSentenceDurationPublicWrapper")]
		public static extern NuanceStatus RCGetSentenceDuration(IntPtr recClientPtr, [In,Out]ref int sentence_duration);
		[DllImport("rcapi.dll", EntryPoint="RCGetSentenceScoreDurationPublicWrapper")]
		public static extern NuanceStatus RCGetSentenceScoreDuration(IntPtr recClientPtr, [In,Out]ref float score);
		[DllImport("rcapi.dll", EntryPoint="RCGetNumWordsInSentencePublicWrapper")]
		public static extern NuanceStatus RCGetNumWordsInSentence(IntPtr recClientPtr, [In,Out]ref int num_words);
		[DllImport("rcapi.dll", EntryPoint="RCGetWordScorePublicWrapper")]
		public static extern NuanceStatus RCGetWordScore(IntPtr recClientPtr, int word_index, [In,Out]ref float score);
		[DllImport("rcapi.dll", EntryPoint="RCGetWordDurationPublicWrapper")]
		public static extern NuanceStatus RCGetWordDuration(IntPtr recClientPtr, int word_index, [In,Out]ref int duration);
		[DllImport("rcapi.dll", EntryPoint="RCGetWordStartPublicWrapper")]
		public static extern NuanceStatus RCGetWordStart(IntPtr recClientPtr, int word_index, [In,Out]ref int start);
		[DllImport("rcapi.dll", EntryPoint="RCGetWordEndPublicWrapper")]
		public static extern NuanceStatus RCGetWordEnd(IntPtr recClientPtr, int word_index, [In,Out]ref int end);
		[DllImport("rcapi.dll", EntryPoint="RCGetPhraseScorePublicWrapper")]
		public static extern NuanceStatus RCGetPhraseScore(IntPtr recClientPtr, int first_word, int last_word, [In,Out]ref float score);
		[DllImport("rcapi.dll", EntryPoint="RCGetPhraseScoreCombinePublicWrapper")]
		public static extern NuanceStatus RCGetPhraseScoreCombine(IntPtr recClientPtr, int first_word, int last_word, [In,Out]ref float combine_score);
		[DllImport("rcapi.dll", EntryPoint="RCComputeScoreForPosteriorPublicWrapper")]
		public static extern NuanceStatus RCComputeScoreForPosterior(IntPtr recClientPtr, int posterior, [In,Out]ref float score);
		
		[DllImport("rcapi.dll", EntryPoint="RCSetParameterPublicWrapper")]
    	public static extern NuanceStatus RCSetParameter(IntPtr recClientPtr, string param, string val);
		[DllImport("rcapi.dll", EntryPoint="RCSetParameterFromStringPublicWrapper")]
    	public static extern NuanceStatus RCSetParameterFromString(IntPtr recClientPtr, string param_command);
		[DllImport("rcapi.dll", EntryPoint="RCGetParameterPublicWrapper")]
    	public static extern NuanceStatus RCGetParameter(IntPtr recClientPtr, string param, StringBuilder value_str, int value_str_len);

		
		[DllImport("rcapi.dll", EntryPoint="RCInterpretPublicWrapper")]
		public static extern NuanceStatus RCInterpret(IntPtr recClientPtr, string sentence, string grammar,
              IntPtr nl_result);

        /// <summary>Retrieves RCAPI version number</summary>
		[DllImport("rcapi.dll", EntryPoint="RCAPIVersionNumberPublicWrapper")]
    	public static extern int RCAPIVersionNumber();
        /// <summary>Retrieves RCAPI version name</summary>
		[DllImport("rcapi.dll", EntryPoint="RCAPIVersionNamePublicWrapper")]
    	public static extern string RCAPIVersionName();
        /// <summary>Retrieves RCAPI version date</summary>
		[DllImport("rcapi.dll", EntryPoint="RCAPIVersionDatePublicWrapper")]
    	public static extern string RCAPIVersionDate();

		[DllImport("rcapi.dll", EntryPoint="RCPlayLastUtteranceFramePublicWrapper")]
    	public static extern NuanceStatus RCPlayLastUtteranceFrame(IntPtr recClientPtr, int start_frame, int end_frame);
		[DllImport("rcapi.dll", EntryPoint="RCPlayLastUtteranceSamplePublicWrapper")]
    	public static extern NuanceStatus RCPlayLastUtteranceSample(IntPtr recClientPtr, int start_sample, int end_sample);
		[DllImport("rcapi.dll", EntryPoint="RCPlayLastUtteranceTimePublicWrapper")]
    	public static extern NuanceStatus RCPlayLastUtteranceTime(IntPtr recClientPtr, float start_time_sec, float end_time_sec);
		[DllImport("rcapi.dll", EntryPoint="RCPlayFileFramePublicWrapper")]
    	public static extern NuanceStatus RCPlayFileFrame(IntPtr recClientPtr, string file, int start_frame, int end_frame);
		[DllImport("rcapi.dll", EntryPoint="RCPlayFileSamplePublicWrapper")]
    	public static extern NuanceStatus RCPlayFileSample(IntPtr recClientPtr, string file, int start_sample, int end_sample);
		[DllImport("rcapi.dll", EntryPoint="RCPlayFileTimePublicWrapper")]
    	public static extern NuanceStatus RCPlayFileTime(IntPtr recClientPtr, string file, float start_time_sec, float end_time_sec);

		[DllImport("rcapi.dll", EntryPoint="RCPlayFilePublicWrapper")]
    	public static extern NuanceStatus RCPlayFile(IntPtr recClientPtr, string files);
		[DllImport("rcapi.dll", EntryPoint="RCPlayDataPublicWrapper")]
    	public static extern NuanceStatus RCPlayData(IntPtr recClientPtr, string filename, byte[] data, int dataLength);
		[DllImport("rcapi.dll", EntryPoint="RCPlayLastUtterancePublicWrapper")]
    	public static extern NuanceStatus RCPlayLastUtterance(IntPtr recClientPtr);
		[DllImport("rcapi.dll", EntryPoint="RCKillPlaybackPublicWrapper")]
    	public static extern NuanceStatus RCKillPlayback(IntPtr recClientPtr);

		
		[DllImport("rcapi.dll", EntryPoint="RCLoadForcedAlignmentDictionaryPublicWrapper")]
		public static extern NuanceStatus RCLoadForcedAlignmentDictionary(IntPtr recClientPtr, string dictionary);
		[DllImport("rcapi.dll", EntryPoint="RCSetupForcedAlignmentForSentencePublicWrapper")]
		public static extern NuanceStatus RCSetupForcedAlignmentForSentence(IntPtr recClientPtr, string label, string ref_str);
		
		[DllImport("rcapi.dll", EntryPoint="RCNewAudioChannelPublicWrapper")]
		public static extern NuanceStatus RCNewAudioChannel(IntPtr recClientPtr);

		
		[DllImport("rcapi.dll", EntryPoint="RCRecognizePublicWrapper")]
		public static extern NuanceStatus RCRecognize(IntPtr recClientPtr, string grammar, float timeout_secs);
		[DllImport("rcapi.dll", EntryPoint="RCRecognizeFilePublicWrapper")]
		public static extern NuanceStatus RCRecognizeFile(IntPtr recClientPtr, string grammar, string filename,
						int do_endpoint);
        [DllImport("rcapi.dll", EntryPoint = "RCRecognizeDataPublicWrapper")]
        public static extern NuanceStatus RCRecognizeData(IntPtr recClientPtr, string grammar, string filename,
                        byte[] data, int dataLength, int do_endpoint);
        [DllImport("rcapi.dll", EntryPoint = "RCStartRecognizingWrapper")]
		public static extern NuanceStatus RCStartRecognizing(IntPtr recClientPtr, string grammar);
		[DllImport("rcapi.dll", EntryPoint="RCStopRecognizingPublicWrapper")]
		public static extern NuanceStatus RCStopRecognizing(IntPtr recClientPtr);

		
		[DllImport("rcapi.dll", EntryPoint="RCRecordPublicWrapper")]
		public static extern NuanceStatus RCRecord(IntPtr recClientPtr, string filename);
		[DllImport("rcapi.dll", EntryPoint="RCRecordWithTimeoutPublicWrapper")]
		public static extern NuanceStatus RCRecordWithTimeout(IntPtr recClientPtr, string filename, float timeout_secs);
		[DllImport("rcapi.dll", EntryPoint="RCStartRecordingPublicWrapper")]
		public static extern NuanceStatus RCStartRecording(IntPtr recClientPtr, string filename);
		[DllImport("rcapi.dll", EntryPoint="RCStopRecordingPublicWrapper")]
		public static extern NuanceStatus RCStopRecording(IntPtr recClientPtr);

		[DllImport("rcapi.dll", EntryPoint="RCAbortPublicWrapper")]
		public static extern NuanceStatus RCAbort(IntPtr recClientPtr);

		
		[DllImport("rcapi.dll", EntryPoint="RCSetupDynamicGrammarForSentencePublicWrapper")]
		public static extern NuanceStatus RCSetupDynamicGrammarForSentence(IntPtr recClientPtr, string label, string gsl_str);
		[DllImport("rcapi.dll", EntryPoint="RCSwitchPackagePublicWrapper")]
		public static extern NuanceStatus RCSwitchPackage(IntPtr recClientPtr, string package);
		[DllImport("rcapi.dll", EntryPoint="RCGetRecResultPublicWrapper")]
		public static extern IntPtr RCGetRecResult (IntPtr recClientPtr, ref NuanceStatus status);
		[DllImport("rcapi.dll", EntryPoint="RCGetNuanceConfigPublicWrapper")]
		public static extern IntPtr RCGetNuanceConfig (IntPtr recClientPtr, ref NuanceStatus status);
			
		//[DllImport("rcapi.dll", EntryPoint="RCSetWindowsHandlesPublicWrapper")]
		//public static extern NuanceStatus RCSetWindowsHandles(HWND hDlg, HANDLE hInst);
			
		[DllImport("rcapi.dll", EntryPoint="RCSetWordAndPronPublicWrapper")]
		public static extern NuanceStatus RCSetWordAndPron(IntPtr recClientPtr, string ascii_word, string pronunciation_str);

		/// <summary>Initializes the recognizer with a NuanceConfig instance (non-blocking)</summary> 
		public static RecClient Initialize(NuanceConfig nuanceConfig) {
			RecClient rc = new RecClient();
			rc.Initialize(nuanceConfig);
			return rc;
		}
		
		/// <summary>Initializes the recognizer with a NuanceConfig instance (blocking)</summary> 
		public static RecClient InitializeBlocking(NuanceConfig nuanceConfig) {
			RecClient rc = new RecClient();
			rc.InitializeBlocking(nuanceConfig);
			return rc;
		}
		
		/// <summary>Initializes the recognizer client with a command line string (non-blocking).</summary> 
		public static RecClient InitializeCommandLine(string command_line) {
			RecClient rc = new RecClient();
			rc.InitializeCommandLine(command_line);
			return rc;
		}
		
		/// <summary>Initializes the recognizer client with a command line string and blocks until initialization is complete</summary> 
		public static RecClient InitializeCommandLineBlocking(string command_line) {
			RecClient rc = new RecClient();
			rc.InitializeCommandLineBlocking(command_line);
			return rc;
		}

        /// <summary>Gets an environment variable</summary>
		public static string GetEnvironmentVariable(string variable_name) {
			if (Logger.GetTraceEnabled()) logger.Trace("GetEnvironmentVariable: variable_name<"+variable_name+">");
			const int len = 2048;
			StringBuilder ret = new StringBuilder(len, len);
			EduSpeakUtil.checkRetValue(RCGetEnvironmentVariable(variable_name, ret, len));
			return ret.ToString();
		}

        /// <summary>Sets an environment variable</summary>
		public static void SetEnvironmentVariable(string variable_name, string valueStr) {
			if (Logger.GetTraceEnabled()) logger.Trace("SetEnvironmentVariable: variable_name<"+
											  variable_name+"> valueStr<"+valueStr+">");
			EduSpeakUtil.checkRetValue(RCSetEnvironmentVariable(variable_name, valueStr));
		}

        /// <summary>Retrieves a Windows registry value</summary>
		public static string GetRegistry(string key) {
			if (Logger.GetTraceEnabled()) logger.Trace("GetRegistry: key<"+key+">");
			const int len = 2048;
			StringBuilder ret = new StringBuilder(len, len);
			EduSpeakUtil.checkRetValue(RCGetRegistry(key, ret, len));
			return ret.ToString();
		}

        /// <summary>Sets a Windows registry value</summary>
		public static void SetRegistry(string key, string valueStr) {
			if (Logger.GetTraceEnabled()) logger.Trace("SetRegistry: key<"+
											  key+"> valueStr<"+valueStr+">");
			EduSpeakUtil.checkRetValue(RCSetRegistry(key, valueStr));
		}
	}
}
