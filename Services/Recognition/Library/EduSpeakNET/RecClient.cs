using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace EduSpeak
{
	public class RecClient: CPtrRef {
		private static Logger logger = new Logger("EduSpeak.RecClient");

		private volatile RecognizerState state = RecognizerState.SRI_NORECOGNIZER;
		private RCCallbackFnPtr callbackDelegate;
		private volatile bool commonEventsRegistered = false;
		private bool inListenerCallback = false;
		private static volatile bool assertState = true;

		public RecClient(IntPtr cptr, RecognizerState state): base(cptr) {
			callbackDelegate = new RCCallbackFnPtr(RCCallback);
			setCPtr(cptr);
			SetRecognizerState(state);
		}

		public RecClient() {
			callbackDelegate = new RCCallbackFnPtr(RCCallback);
		}

		/// <summary>Initializes the recognizer with a NuanceConfig instance (non-blocking)</summary> 
		public void Initialize(NuanceConfig nuanceConfig) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Initialize: nuanceConfig<"+nuanceConfig+">");
			NuanceStatus nuanceStatus = new NuanceStatus();
			IntPtr cptr = RCAPI.RCInitialize(nuanceConfig.getCPtr(), ref nuanceStatus);
			EduSpeakUtil.checkRetValue(nuanceStatus);
			setCPtr(cptr);
			RegisterCallback(NuanceEvent.NUANCE_EVENT_INIT_COMPLETE);
		}
		
		/// <summary>Initializes the recognizer with a NuanceConfig instance (blocking)</summary> 
		public void InitializeBlocking(NuanceConfig nuanceConfig) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "InitializeBlocking: nuanceConfig<"+nuanceConfig+">");
			NuanceStatus nuanceStatus = new NuanceStatus();
			IntPtr cptr = RCAPI.RCInitializeBlocking(nuanceConfig.getCPtr(), ref nuanceStatus);
			EduSpeakUtil.checkRetValue(nuanceStatus);
			setCPtr(cptr);
			RegisterCommonEvents();
		}
		
		/// <summary>Initializes the recognizer client with a command line string (non-blocking).</summary> 
		public void InitializeCommandLine(string command_line) {
			AsyncCommandLineInit asyncInit = new AsyncCommandLineInit(command_line, this);
			Thread initThread = new Thread(new ThreadStart(asyncInit.InitializeCommandLineAsync));
			initThread.Start();
		}
		
		/// <summary>Initializes the recognizer client with a command line string and blocks until initialization is complete</summary> 
		public void InitializeCommandLineBlocking(string command_line) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "InitializeCommandLineBlocking: command_line<"+command_line+">");
			NuanceStatus nuanceStatus = new NuanceStatus();
			IntPtr cptr = RCAPI.RCInitializeCommandLineBlocking(command_line, ref nuanceStatus);
			EduSpeakUtil.checkRetValue(nuanceStatus);
			setCPtr(cptr);
			RegisterCommonEvents();
		}

		public static void setAssertState(bool doAssert) {
			assertState = doAssert;
		}

		protected void SetRecognizerState(RecognizerState state) {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "RecognizerState: oldState<"+this.state+"> newState<"+state+">");
			this.state = state;
			if (OnStateChange != null) OnStateChange(state);
		}

		private void AssertRecognizerState(RecognizerState targetState) {
			if (assertState && state != targetState) {
				if (!inListenerCallback) {
					throw new IllegalStateException(state, "Recognizer State must be "+targetState);
				}
				inListenerCallback = false;
			}
		}

        /// <summary>You can call Abort at any time to abort the current recognition, recording, or playback operation</summary>
        public void Abort() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Abort");
			EduSpeakUtil.checkRetValue(RCAPI.RCAbort(getCPtr()));
        }

        /// <summary>Unregisters an event callback</summary>
        public void ClearCallback(NuanceEvent nuanceEvent) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ClearCallback: nuanceEvent<"+nuanceEvent+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCRegisterCallback(getCPtr(), nuanceEvent, null, new IntPtr()));
        }

        /// <summary>Calculates a weighted difference bettween the threshold and the score for each phoneme (or grapheme) in the sentence.</summary>
        public void ComputeGraphemeScores(int percentFA) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ComputeGraphemeScores: percentFA<"+percentFA+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCComputeGraphemeScores(getCPtr(), percentFA));
        }

		/// <summary>Retrieves the phoneme, the grapheme corresponding to the phoneme and the score; index is the index of the phoneme or grapheme in the sentence uttered</summary>
		public void ScoringGetVectors(ref string phoneme, ref string grapheme, ref float score, int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ScoringGetVectors: index<"+index+">");
			const int len = 4096;
			StringBuilder phoneme_ret = new StringBuilder(len);
			StringBuilder grapheme_ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(RCAPI.RCScoringGetVectors(getCPtr(), phoneme_ret, grapheme_ret, ref score, index));
			phoneme = phoneme_ret.ToString();
			grapheme = grapheme_ret.ToString();
		}

		/// <summary> Retrieves the phone, its score and the mean of the phone distribution</summary>
		public void GetBoundedPhoneScores(ref string phone, ref float score, ref float mean, int index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetBoundedPhoneScores: phone<"+phone+
				"> index<"+index+">");
			const int len = 4096;
			StringBuilder phone_ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(RCAPI.RCGetBoundedPhoneScores(getCPtr(), phone_ret, ref score, ref mean, index));
			phone = phone_ret.ToString();
		}

        /// <summary>method ComputeScoreForPosterior</summary>
        public float ComputeScoreForPosterior(int posterior) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ComputeScoreForPosterior: posterior<"+posterior+">");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCComputeScoreForPosterior(getCPtr(), posterior, ref ret));
            return ret;
        }

        /// <summary>Sets the recognizer state at the end of playing an external sound</summary>
        public void EndExternalPlay() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "EndExternalPlay");
			AssertRecognizerState(RecognizerState.SRI_PLAYING_EXTERNAL);
			SetRecognizerState(RecognizerState.SRI_READY);
        }

        /// <summary>Retrieves a parameter that has an integer value</summary>
        public int GetIntParameter(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntParameter: paramName<"+paramName+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetIntParameter(getCPtr(), paramName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntParameter: paramName<"+paramName+"> ret<"+ret+">");
            return ret;
        }

        /// <summary>Retrieves a parameter that has a float value</summary>
        public float GetFloatParameter(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFloatParameter: paramName<"+paramName+">");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetFloatParameter(getCPtr(), paramName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFloatParameter: paramName<"+paramName+"> ret<"+ret+">");
            return ret;
        }

        /// <summary>Retrieves a grammar name for a grammar index.</summary>
        public string GetGrammar(int grammar_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetGrammar: grammar_index<"+grammar_index+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(RCAPI.RCGetGrammar(getCPtr(), ret, len, grammar_index));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetGrammar: grammar_index<"+grammar_index+"> ret<"+ret+">");
            return ret.ToString();
        }

        /// <summary>Gets the number of graphemes in the sentence.</summary>
        public int NumGraphemesInSentence {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumGraphemesInSentence");
				int ret = 0;
				EduSpeakUtil.checkRetValue(RCAPI.RCGetNumGraphemesInSentence(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumGraphemesInSentence ret<"+ret+">");
				return ret;
			}
        }

        /// <summary>Gets the number of phones in the sentence.</summary>
        public int NumPhonesInSentence {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumPhonesInSentence");
				int ret = 0;
				EduSpeakUtil.checkRetValue(RCAPI.RCGetNumPhonesInSentence(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumPhonesInSentence ret<"+ret+">");
				return ret;
			}
        }

        /// <summary>Gives the number of word in the sentence</summary>
        public int NumWordsInSentence {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumWordsInSentence");
				int ret = 0;
				EduSpeakUtil.checkRetValue(RCAPI.RCGetNumWordsInSentence(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumWordsInSentence ret<"+ret+">");
				return ret;
			}
        }

        /// <summary>Gives the score of a part the sentence.</summary>
        public float GetPhraseScore(int first_word, int last_word) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhraseScore: first_word<"+first_word+
				"> last_word<"+last_word+">");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetPhraseScore(getCPtr(), first_word, last_word, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhraseScore: first_word<"+first_word+
				"> last_word<"+last_word+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Gives the combine score of a part the sentence.</summary>
        public float GetPhraseScoreCombine(int word_start, int word_end) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhraseScoreCombine: word_start<"+word_start+
				"> word_end<"+word_end+">");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetPhraseScoreCombine(getCPtr(), word_start, word_end, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetPhraseScoreCombine: word_start<"+word_start+
				"> word_end<"+word_end+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Retrieves the sentence duration</summary>
        public int GetSentenceDuration() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceDuration");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetSentenceDuration(getCPtr(), ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceDuration: ret<"+ret+">");
			return ret;
        }

        /// <summary>Retrieves the sentence duration score</summary>
        public float GetSentenceScoreDuration() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceScoreDuration");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetSentenceScoreDuration(getCPtr(), ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceScoreDuration: ret<"+ret+">");
			return ret;
        }

        /// <summary>Gives the duration of the word</summary>
        public int GetWordDuration(int word_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordDuration: word_index<"+word_index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetWordDuration(getCPtr(), word_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordDuration: word_index<"+word_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Gives the end of the word</summary>
        public int GetWordEnd(int word_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordEnd: word_index<"+word_index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetWordEnd(getCPtr(), word_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordEnd: word_index<"+word_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Gives the start of the word</summary>
        public int GetWordStart(int word_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordStart: word_index<"+word_index+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetWordStart(getCPtr(), word_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordStart: word_index<"+word_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary>Retrieves a parameter value encoded in a string variable</summary>
        public string GetParameter(string param) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameter: param<"+param+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(RCAPI.RCGetParameter(getCPtr(), param, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameter: param<"+param+"> ret<"+ret+">");
            return ret.ToString();
        }

        /// <summary>Retrieves the current recognizer state</summary>
        public RecognizerState State {
			get {
				return state;
			}
        }

        /// <summary>Determines whether ro not the recognizer is initialized</summary>
        public bool Initialized {
			get {
				return state != RecognizerState.SRI_NORECOGNIZER;
			}
        }

        /// <summary>Gives the score for the whole sentence</summary>
        public float GetSentenceScore() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceScore");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetSentenceScore(getCPtr(), ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceScore: ret<"+ret+">");
			return ret;
        }

        /// <summary>Gives the combine score of a sentence.</summary>
        public float GetSentenceScoreCombine() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceScoreCombine");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetSentenceScoreCombine(getCPtr(), ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetSentenceScoreCombine: ret<"+ret+">");
			return ret;
        }

        /// <summary>retrieves a parameter that has a string value</summary>
        public string GetStringParameter(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringParameter: paramName<"+paramName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(RCAPI.RCGetStringParameter(getCPtr(), paramName, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringParameter: paramName<"+paramName+"> ret<"+ret+">");
            return ret.ToString();
        }

        /// <summary>Gives the score for the ith word in the sentence</summary>
        public float GetWordScore(int word_index) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordScore: word_index<"+word_index+">");
			float ret = 0;
			EduSpeakUtil.checkRetValue(RCAPI.RCGetWordScore(getCPtr(), word_index, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetWordScore: word_index<"+word_index+"> ret<"+ret+">");
			return ret;
        }

        /// <summary> Interprets the given sentence.</summary>
        public void Interpret(string sentence, string grammar, NLResult inlresult) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Interpret: sentence<"+
					sentence+"> grammar<"+grammar+"> inlresult<"+inlresult+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCInterpret(getCPtr(), sentence, grammar, inlresult.getCPtr()));
        }

        /// <summary> Interprets the given sentence with any grammar.</summary>
        public void InterpretWithAnyGrammar(string sentence, NLResult inlresult) {
			Interpret(sentence, null, inlresult);
        }

        /// <summary>Stops playback</summary>
        public void KillPlayback() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "KillPlayback");
			AssertRecognizerState(RecognizerState.SRI_PLAYING);
			SetRecognizerState(RecognizerState.SRI_STOPPING);
			NuanceStatus status = RCAPI.RCKillPlayback(getCPtr());
			if (!EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>This function informs the Nuance System that subsequent audio will come from a different audio channel than previous audio</summary>
        public void NewAudioChannel() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "NewAudioChannel");
			EduSpeakUtil.checkRetValue(RCAPI.RCNewAudioChannel(getCPtr()));
        }

        /// <summary>Plays one or more digital audio files out the audio device.</summary>
        public void PlayFile(string files) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayFile: files<"+files+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayFile(getCPtr(), files);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

		/// <summary>Plays audio data from an IO stream by reading the stream fully</summary>
		public void PlayData(System.IO.Stream inputStream, string filename)  {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayData");
			byte[] streamData = ReadFully(inputStream);
			PlayData(streamData, streamData.Length, filename);
		}

		/// <summary>Plays audio data from a byte array</summary>
		public void PlayData(byte[] audioData, int length, string filename)  {
			if (length <= 0 || length > audioData.Length) {
				throw new Exception("Invalid length<"+length+"> for audioData.Length<"+audioData.Length+">");
			}
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayData");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayData(getCPtr(), filename, audioData, length);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
		}
		
		/// <summary>
		/// Reads data from a stream until the end is reached. The
		/// data is returned as a byte array. An IOException is
		/// thrown if any of the underlying IO calls fail.
		/// </summary>
		/// <param name="stream">The stream to read data from</param>
		private static byte[] ReadFully (System.IO.Stream stream) {
			byte[] buffer = new byte[10240];
			using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
				while (true) {
					int read = stream.Read(buffer, 0, buffer.Length);
					if (read <= 0)
						return ms.ToArray();
					ms.Write(buffer, 0, read);
				}
			}
		}

        /// <summary>Plays a section of one digital audio file out the audio device</summary>
        public void PlayFileFrame(string fileName, int start_frame, int end_frame) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayFileFrame: fileName<"+
						fileName+"> start_frame<"+start_frame+"> end_frame<"+end_frame+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayFileFrame(getCPtr(), fileName,
				start_frame, end_frame);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Plays a section of one digital audio file out the audio device</summary>
        public void PlayFileSample(string fileName, int start_sample, int end_sample) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayFileSample: fileName<"+
						fileName+"> start_sample<"+start_sample+"> end_sample<"+end_sample+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayFileSample(getCPtr(), fileName,
				start_sample, end_sample);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Plays a section of one digital audio file out the audio device</summary>
        public void PlayFileTime(string fileName, float start_time_sec, float end_time_sec) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayFileTime: fileName<"+
						fileName+"> start_time_sec<"+start_time_sec+"> end_time_sec<"+end_time_sec+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayFileTime(getCPtr(), fileName,
				start_time_sec, end_time_sec);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Plays a recording of the last utterance recognized.</summary>
        public void PlayLastUtterance() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayLastUtterance");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayLastUtterance(getCPtr());
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Plays a section of the last recording</summary>
        public void PlayLastUtteranceFrame(int start_frame, int end_frame) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayLastUtteranceFrame: start_frame<"+
											  start_frame+"> end_frame<"+end_frame+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayLastUtteranceFrame(getCPtr(), start_frame, end_frame);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Plays a section of the last recording</summary>
        public void PlayLastUtteranceSample(int start_sample, int end_sample) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayLastUtteranceSample: start_sample<"+
											  start_sample+"> end_sample<"+end_sample+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayLastUtteranceSample(getCPtr(), start_sample, end_sample);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Plays a section of the last recording </summary>
        public void PlayLastUtteranceTime(float start_time_sec, float end_time_sec) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "PlayLastUtteranceTime: start_time_sec<"+
											  start_time_sec+"> end_time_sec<"+end_time_sec+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCPlayLastUtteranceTime(getCPtr(), start_time_sec, end_time_sec);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_PLAYING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Starts recognition, listening for speech on the implicit audio device</summary>
        public void Recognize(string grammar, float timeout_secs) {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "Recognize: grammar<"+grammar+"> timeout_secs<"+timeout_secs+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus result = RCAPI.RCRecognize(getCPtr(), grammar, timeout_secs);
			if (EduSpeakUtil.NUANCE_SUCCESS(result)) {
				SetRecognizerState(RecognizerState.SRI_RECOGNIZING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "Recognize: grammar<"+grammar+"> timeout_secs<"+timeout_secs+"> result<"+result+">");
			EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>RecognizeFile() performs recognition on a wavfile</summary>
        public void RecognizeFile(string grammar, string filename, bool doEndpoint) {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "RecognizeFile: grammar<"+grammar+"> filename<"+filename+
					"> doEndpoint<"+doEndpoint+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus result = RCAPI.RCRecognizeFile(getCPtr(), grammar, filename, doEndpoint?1:0);
			if (EduSpeakUtil.NUANCE_SUCCESS(result)) {
				SetRecognizerState(RecognizerState.SRI_RECOGNIZING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "RecognizeFile: grammar<"+grammar+"> filename<"+filename+
					"> doEndpoint<"+doEndpoint+"> result<"+result+">");
			EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>RecognizeData() performs recognition on a wavfile buffer</summary>
        public void RecognizeData(string grammar, string filename, byte[] data, bool doEndpoint)
        {
            if (Logger.GetTraceEnabled())
                logger.Trace(this, "RecognizeData: grammar<" + grammar + "> filename<" + filename +
                    "> doEndpoint<" + doEndpoint + ">");
            AssertRecognizerState(RecognizerState.SRI_READY);
            SetRecognizerState(RecognizerState.SRI_NOTREADY);
            NuanceStatus result = RCAPI.RCRecognizeData(getCPtr(), grammar, filename, data, data.Length, doEndpoint ? 1 : 0);
            if (EduSpeakUtil.NUANCE_SUCCESS(result))
            {
                SetRecognizerState(RecognizerState.SRI_RECOGNIZING);
            }
            else
            {
                SetRecognizerState(RecognizerState.SRI_READY);
            }
            if (Logger.GetTraceEnabled())
                logger.Trace(this, "RecognizeData: grammar<" + grammar + "> filename<" + filename +
                    "> doEndpoint<" + doEndpoint + "> result<" + result + ">");
            EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>Used to record a waveform and save it to disk.</summary>
        public void Record(string filename) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Record: filename<"+filename+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCRecord(getCPtr(), filename);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_RECORDING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>method RecordWithTimeout</summary>
        public void RecordWithTimeout(string filename, float timeout_secs) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "RecordWithTimeout: filename<"+
											  filename+"> timeout_secs<"+timeout_secs+">");
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus status = RCAPI.RCRecordWithTimeout(getCPtr(), filename, timeout_secs);
			if (EduSpeakUtil.NUANCE_SUCCESS(status)) {
				SetRecognizerState(RecognizerState.SRI_RECORDING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>Registers for an event callback</summary>
        public void RegisterCallback(NuanceEvent nuanceEvent) {
            if (Logger.GetTraceEnabled())
                logger.Trace(this, "RegisterCallback: eventType<"+nuanceEvent+">");
            // No user data...
            IntPtr userData = new IntPtr();
            NuanceStatus res = RCAPI.RCRegisterCallback(getCPtr(), nuanceEvent, callbackDelegate, userData);
            EduSpeakUtil.checkRetValue(res);
        }

		/// <summary>Registers for common events.</summary>
		/// <remarks>
		/// This method registers for the following common events:
		/// NuanceEvent.NUANCE_EVENT_AUDIO_CHANGE,
		/// NuanceEvent.NUANCE_EVENT_AUDIO_VU_METER,
		/// NuanceEvent.NUANCE_EVENT_PLAYBACK_DONE,
		/// NuanceEvent.NUANCE_EVENT_RECORDING_DONE,
		/// NuanceEvent.NUANCE_EVENT_PARTIAL_RESULT,
		/// NuanceEvent.NUANCE_EVENT_FINAL_RESULT,
		/// NuanceEvent.NUANCE_EVENT_END_OF_SPEECH,
		/// NuanceEvent.NUANCE_EVENT_START_OF_SPEECH
		/// <p>Note that this method is called automatically if a listener is added to
		/// the RecClient, and normally this method does not need to be called by the
		/// client app.</p>
		/// </remarks>
		///<seealso cref="AddRecClientEventListener" />
		public void RegisterCommonEvents() {
			if (!commonEventsRegistered) {
				RegisterCallback(NuanceEvent.NUANCE_EVENT_AUDIO_CHANGE);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_AUDIO_VU_METER);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_PLAYBACK_DONE);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_RECORDING_DONE);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_PARTIAL_RESULT);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_FINAL_RESULT);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_END_OF_SPEECH);
				RegisterCallback(NuanceEvent.NUANCE_EVENT_START_OF_SPEECH);
				commonEventsRegistered = true;
			}
		}

        /// <summary>sets a parameter with an integer value</summary>
        public void SetIntParameter(string paramName, int val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetIntParameter: paramName<"+paramName+"> val<"+val+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetIntParameter(getCPtr(), paramName, val));
        }

        /// <summary>sets a parameter with a float value</summary>
        public void SetFloatParameter(string paramName, float val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetFloatParameter: paramName<"+paramName+"> val<"+val+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetFloatParameter(getCPtr(), paramName, val));
        }

        /// <summary> Sets the value of a Nuance Parameter.</summary>
        public void SetParameter(string param, string val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetParameter: param<"+param+"> val<"+val+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetParameter(getCPtr(), param, val));
        }

        /// <summary>Sets the value of a Nuance Parameter.</summary>
        public void SetParameterFromString(string param_command) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetParameterFromString: param_command<"+param_command+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetParameterFromString(getCPtr(), param_command));
        }

        /// <summary>sets a parameter with a string value</summary>
        public void SetStringParameter(string paramName, string val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetStringParameter: paramName<"+paramName+"> val<"+val+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetStringParameter(getCPtr(), paramName, val));
        }

        /// <summary>Setup a dynamic grammar.</summary>
        public void SetupDynamicGrammarForSentence(string label, string gslStr) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetupDynamicGrammarForSentence: label<"+label+"> gslStr<"+gslStr+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetupDynamicGrammarForSentence(getCPtr(), label, gslStr));
        }

        /// <summary>Setup a forced alignment.</summary>
        public void SetupForcedAlignmentForSentence(string label, string refStr) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetupForcedAlignmentForSentence: label<"+label+"> refStr<"+refStr+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetupForcedAlignmentForSentence(getCPtr(), label, refStr));
        }

        /// <summary>Adds word and its pronunciation to the internal dictionary.</summary>
        public void SetWordAndPron(string asciiword, string pronunciation) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetWordAndPron: asciiword<"+asciiword+"> pronunciation<"+pronunciation+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSetWordAndPron(getCPtr(), asciiword, pronunciation));
        }

        /// <summary>Sets the recognizer state for playing external sounds</summary>
        public void StartExternalPlay() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "StartExternalPlay");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_PLAYING_EXTERNAL);
        }

        /// <summary>Starts recognition, listening for speech on the implicit audio device.</summary>
        public void StartRecognizing(string grammar) {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StartRecognizing: grammar<"+grammar+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus result = RCAPI.RCStartRecognizing(getCPtr(), grammar);
			if (EduSpeakUtil.NUANCE_SUCCESS(result)) {
				SetRecognizerState(RecognizerState.SRI_RECOGNIZING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StartRecognizing: grammar<"+grammar+"> result<"+result+">");
			EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>Used to record a waveform and save it to disk with a timeout.</summary>
        public void StartRecording(string filename) {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StartRecording: filename<"+filename+">");
			AssertRecognizerState(RecognizerState.SRI_READY);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus result = RCAPI.RCStartRecording(getCPtr(), filename);
			if (EduSpeakUtil.NUANCE_SUCCESS(result)) {
				SetRecognizerState(RecognizerState.SRI_RECORDING);
			} else {
				SetRecognizerState(RecognizerState.SRI_READY);
			}
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StartRecording: filename<"+filename+"> result<"+result+">");
			EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>Stops recognition.</summary>
        public void StopRecognizing() {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StopRecognizing");
			AssertRecognizerState(RecognizerState.SRI_RECOGNIZING);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus result = RCAPI.RCStopRecognizing(getCPtr());
			if (!EduSpeakUtil.NUANCE_SUCCESS(result)) {
				SetRecognizerState(RecognizerState.SRI_RECOGNIZING);
			}
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StopRecognizing: result<"+result+">");
			EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>Stops to record a waveform.</summary>
        public void StopRecording() {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StopRecording");
			AssertRecognizerState(RecognizerState.SRI_RECORDING);
			SetRecognizerState(RecognizerState.SRI_NOTREADY);
			NuanceStatus result = RCAPI.RCStopRecording(getCPtr());
			if (!EduSpeakUtil.NUANCE_SUCCESS(result)) {
				SetRecognizerState(RecognizerState.SRI_RECORDING);
			}
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "StopRecording: result<"+result+">");
			EduSpeakUtil.checkRetValue(result);
        }

        /// <summary>Change the recognition package.</summary>
        public void SwitchPackage(string packageStr) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SwitchPackage packageStr<"+packageStr+">");
			EduSpeakUtil.checkRetValue(RCAPI.RCSwitchPackage(getCPtr(), packageStr));
        }

        /// <summary>Terminates the Recognizer Client</summary>
        public void Terminate(bool notifyApp) {
			if (notifyApp) {
				SetRecognizerState(RecognizerState.SRI_TERMINATING);
			}
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Terminate notifyApp<"+notifyApp+">");
			NuanceStatus status = RCAPI.RCTerminate(getCPtr());
			if (notifyApp) {
				SetRecognizerState(RecognizerState.SRI_NORECOGNIZER);
			}
			//setCPtr(new IntPtr(0));	
			EduSpeakUtil.checkRetValue(status);
        }

        /// <summary>The NuanceConfig instance used to initialize the recognizer</summary>
        public NuanceConfig nuanceConfig {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNuanceConfig");
				NuanceStatus status = NuanceStatus.NUANCE_ERROR;
				IntPtr ret = RCAPI.RCGetNuanceConfig(getCPtr(), ref status);
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetNuanceConfig: ret<"+
												  ret+"> status<"+status+">");
				EduSpeakUtil.checkRetValue(status);
				return new NuanceConfig(ret);
            }
        }

        /// <summary>The number of grammars the recognizer knows about</summary>
        public int NumGrammars {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumGrammars");
				int ret = 0;
				EduSpeakUtil.checkRetValue(RCAPI.RCGetNumGrammars(getCPtr(), ref ret));
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumGrammars: ret<"+ret+">");
                return ret;
            }
        }

        /// <summary>The last recognition result</summary>
        public RecResult recResult {
            get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetRecResult");
				NuanceStatus status = NuanceStatus.NUANCE_ERROR;
				IntPtr ret = RCAPI.RCGetRecResult(getCPtr(), ref status);
				if (Logger.GetTraceEnabled()) logger.Trace(this, "GetRecResult: ret<"+
												  ret+"> status<"+status+">");
				EduSpeakUtil.checkRetValue(status);
				return new RecResult(ret);
            }
        }

		public void RCCallback(IntPtr user_data, NuanceEvent eventType, IntPtr eventData) {
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "RCCallback: eventType<"+eventType+"> eventData<"+eventData+">");

			if (eventType == NuanceEvent.NUANCE_EVENT_INIT_COMPLETE) {
				NuanceStatus init_complete_status = (NuanceStatus)eventData.ToInt32();
				NotifyInitComplete(init_complete_status);
			} else if (eventType == NuanceEvent.NUANCE_EVENT_PARTIAL_RESULT) {
				RecResult recResult = new RecResult(eventData);
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnPartialResult");
				if (OnPartialResult != null) OnPartialResult(recResult);
			} else if (eventType == NuanceEvent.NUANCE_EVENT_FINAL_RESULT) {
				RecResult recResult = new RecResult(eventData);
				inListenerCallback = true;
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnFinalResult");
				if (OnFinalResult != null) OnFinalResult(recResult);
				if (inListenerCallback) SetRecognizerState(RecognizerState.SRI_READY);
				inListenerCallback = false;
			} else if (eventType == NuanceEvent.NUANCE_EVENT_START_OF_SPEECH) {
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnStartOfSpeech: eventData<"+eventData.ToInt32()+">");
				if (OnStartOfSpeech != null) OnStartOfSpeech(eventData.ToInt32());
			} else if (eventType == NuanceEvent.NUANCE_EVENT_END_OF_SPEECH) {
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnEndOfSpeech: eventData<"+eventData.ToInt32()+">");
				if (OnEndOfSpeech != null) OnEndOfSpeech(eventData.ToInt32());
			} else if (eventType == NuanceEvent.NUANCE_EVENT_RECORDING_DONE) {
				inListenerCallback = true;
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnRecordingDone: status<"+((RecordingStatus)eventData.ToInt32())+">");
				if (OnRecordingDone != null) OnRecordingDone((RecordingStatus)eventData.ToInt32());
				if (inListenerCallback) SetRecognizerState(RecognizerState.SRI_READY);
				inListenerCallback = false;
			} else if (eventType == NuanceEvent.NUANCE_EVENT_PLAYBACK_DONE) {
				inListenerCallback = true;
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnPlaybackDone: status<"+((PlaybackStatus)eventData.ToInt32())+">");
				if (OnPlaybackDone != null) OnPlaybackDone((PlaybackStatus)eventData.ToInt32());
				if (inListenerCallback) SetRecognizerState(RecognizerState.SRI_READY);
				inListenerCallback = false;
			} else if (eventType == NuanceEvent.NUANCE_EVENT_AUDIO_VU_METER) {
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnAudioVuMeter: eventData<"+eventData.ToInt32()+">");
				if (OnAudioVuMeter != null) OnAudioVuMeter(eventData.ToInt32());
			} else if (eventType == NuanceEvent.NUANCE_EVENT_AUDIO_CHANGE) {
				if (Logger.GetTraceEnabled())
					logger.Trace(this, "RCCallback:OnAudioChange: eventData<"+eventData.ToInt32()+">");
				if (OnAudioChange != null) OnAudioChange(eventData.ToInt32());
			}

			if (OnEvent != null) OnEvent(eventType, eventData);
		}

		/// <summary>
		/// Clears all event handlers for the RecClient.
		/// </summary>
		public void ClearEventHandlers() {
			OnAudioChange = null;
			OnAudioVuMeter = null;
			OnEndOfSpeech = null;
			OnEvent = null;
			OnFinalResult = null;
			OnInitComplete = null;
			OnPartialResult = null;
			OnPlaybackDone = null;
			OnRecordingDone = null;
			OnStartOfSpeech = null;
			OnStateChange = null;
		}

		// Events
		/// <summary>
		/// Fired when an audio control is changed.
		/// </summary>
		public event OnAudioChangeHandler OnAudioChange;
		/// <summary>
		/// Fired when an audio control is changed.
		/// </summary>
		public delegate void OnAudioChangeHandler(int controlID);

		/// <summary>
		/// Fired when audio volume change events are received. Call RegisterCallback with NUANCE_EVENT_AUDIO_VU_METER to receive this callback.
		/// </summary>
		public event OnAudioVuMeterHandler OnAudioVuMeter;
		/// <summary>
		/// Fired when audio volume change events are received. Call RegisterCallback with NUANCE_EVENT_AUDIO_VU_METER to receive this callback.
		/// </summary>
		public delegate void OnAudioVuMeterHandler(int volume);

		/// <summary>
		/// Fired after the end of speech.
		/// </summary>
		public event OnEndOfSpeechHandler OnEndOfSpeech;
		/// <summary>
		/// Fired after the end of speech.
		/// </summary>
		public delegate void OnEndOfSpeechHandler(int lastSample);

		/// <summary>
		/// Generic event handler.
		/// </summary>
		public event OnEventHandler OnEvent;
		/// <summary>
		/// Generic event handler.
		/// </summary>
		public delegate void OnEventHandler(NuanceEvent eventType, IntPtr data);

		/// <summary>
		/// Fired when a final result is obtained from the recognizer.
		/// </summary>
		public event OnFinalResultHandler OnFinalResult;
		/// <summary>
		/// Fired when a final result is obtained from the recognizer.
		/// </summary>
		public delegate void OnFinalResultHandler(RecResult data);

		/// <summary>
		/// Fired when the recognizer client finishes initializing.
		/// </summary>
		public event OnInitCompleteHandler OnInitComplete;
		/// <summary>
		/// Fired when the recognizer client finishes initializing.
		/// </summary>
		public delegate void OnInitCompleteHandler(NuanceStatus status);

		/// <summary>
		/// Fired when a partial result is obtained from the recognizer.
		/// </summary>
		public event OnPartialResultHandler OnPartialResult;
		/// <summary>
		/// Fired when a partial result is obtained from the recognizer.
		/// </summary>
		public delegate void OnPartialResultHandler(RecResult data);

		/// <summary>
		/// Fired when playback is finished.
		/// </summary>
		public event OnPlaybackDoneHandler OnPlaybackDone;
		/// <summary>
		/// Fired when playback is finished.
		/// </summary>
		public delegate void OnPlaybackDoneHandler(PlaybackStatus status);

		/// <summary>
		/// Fired when recording is finished.
		/// </summary>
		public event OnRecordingDoneHandler OnRecordingDone;
		/// <summary>
		/// Fired when recording is finished.
		/// </summary>
		public delegate void OnRecordingDoneHandler(RecordingStatus status);

		/// <summary>
		/// Fired when speech starts.
		/// </summary>
		public event OnStartOfSpeechHandler OnStartOfSpeech;
		/// <summary>
		/// Fired when speech starts.
		/// </summary>
		public delegate void OnStartOfSpeechHandler(int detected);

		/// <summary>
		/// Fired when the recognizer state has changed.
		/// </summary>
		public event OnStateChangeHandler OnStateChange;
		/// <summary>
		/// Fired when the recognizer state has changed.
		/// </summary>
		public delegate void OnStateChangeHandler(RecognizerState newState);

		internal void NotifyInitComplete(NuanceStatus init_complete_status) {
			inListenerCallback = EduSpeakUtil.NUANCE_SUCCESS(init_complete_status);
			if (Logger.GetTraceEnabled())
				logger.Trace(this, "RCCallback:OnInitComplete: init_complete_status<"+init_complete_status+">");
			if (OnInitComplete != null) OnInitComplete(init_complete_status);
			if (EduSpeakUtil.NUANCE_SUCCESS(init_complete_status)) {
				if (inListenerCallback) SetRecognizerState(RecognizerState.SRI_READY);
			} else {
				SetRecognizerState(RecognizerState.SRI_NORECOGNIZER);
			}
			if (EduSpeakUtil.NUANCE_SUCCESS(init_complete_status)) {
				RegisterCommonEvents();
			}
			inListenerCallback = false;
		}
	}

	internal class AsyncCommandLineInit {
		private static Logger logger = new Logger("EduSpeak.RecClient");
		private string command_line;
		private RecClient recClient;

		internal AsyncCommandLineInit(String command_line, RecClient recClient) {
			this.command_line = command_line;
			this.recClient = recClient;
		}

		internal void InitializeCommandLineAsync() {
			if (Logger.GetTraceEnabled()) logger.Trace(recClient, "InitializeCommandLine: command_line<"+command_line+">");
			NuanceStatus nuanceStatus = new NuanceStatus();
			IntPtr cptr = RCAPI.RCInitializeCommandLineBlocking(command_line, ref nuanceStatus);
			if (EduSpeakUtil.NUANCE_SUCCESS(nuanceStatus)) {
				recClient.setCPtr(cptr);
			}
			recClient.NotifyInitComplete(nuanceStatus);
		}
	}
}
