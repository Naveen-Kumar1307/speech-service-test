using System;

namespace EduSpeak
{
	public enum RecognizerState
	{
		SRI_NORECOGNIZER,
		SRI_INITIALIZING,
		SRI_TERMINATING,
		SRI_READY,
		SRI_NOTREADY,
		SRI_STOPPING,
		SRI_PLAYING,
		SRI_RECORDING,
		SRI_RECOGNIZING,
		SRI_PLAYING_EXTERNAL,
		SRI_NUM_STATES
	};
	
	public enum RecordingStatus {
		RECORD_OK,
		RECORD_ABORTED,
		RECORD_TIMEOUT,
		RECORD_NO_SPEACH
	};

	public enum PlaybackStatus {
		PLAYBACK_OK,
		PLAYBACK_ABORTED
	};
}
