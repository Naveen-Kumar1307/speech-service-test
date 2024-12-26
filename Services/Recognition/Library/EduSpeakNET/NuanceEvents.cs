using System;

namespace EduSpeak
{

	///<summary>The enumerated list of Nuance events.</summary>
	///<remarks>
	///<p>
    /// The Nuance Speech Recognition System allows users to handle or ignore a
    /// number of asynchronous events which occur in the process of recognition
    /// and playback.  The NuanceEvent type enumerates the possible events.
    /// </p><p>
    /// A user can either ignore, wait for, or ask to be notified of a given event.
    /// By default, all events are ignored.  Each of the Nuance API's offer users
    /// functions with which they can register a callback function to be called
    /// when a given event occurs.  Or, users can call a function which will block
    /// until a given event occurs.
    /// </p><p>
    /// In either case, there is data associated with most events, which must
    /// be conveyed to the user.  For each event the type of data is different.
    /// The following table indicates the type of data associated with each event.
    /// Since the functions to wait for events and to call a user's callback are
    /// used for all events, the event-specific data must be coerced to and from
    /// type (void    ///).
    /// </p><p>
    /// So if the data type is int, the user's callback function should expect
    /// to be called with a pointer to an int, and the user must supply a pointer
    /// to an int when waiting for the event to occur.  Events whose data type
    /// is NuanceStatus are handled in the same way.
    /// </p><p>
    /// Similarly, if the data type is RecResult, callback functions will be
    /// called with a (RecResult    ///) which points to a RecResult structure owned
    /// by the API (and which will be cleared after the event has been handled).
    /// When waiting for the event, the caller must supply a pointer to a RecResult
    /// structure obtained by a call to RecResultNew().  The 'wait' function will
    /// fill out the RecResult structure before returning.
    /// </p><pre>
    /// -----
    /// Event: NUANCE_EVENT_INIT_COMPLETE
    /// Data:  Status code indicating success (NUANCE_OK) or reason for failure.
    /// Type:  NuanceStatus
    ///
    /// Event: NUANCE_EVENT_START_OF_SPEECH
    /// Data:  Indexes of first sample in utterance, relative to time at which
    ///          listening began.  0th index is the "safe" start-of-speech, 1st
    ///          is the "actual" one.  Units are 1/sampling_rate.  Therefore if
    ///          start of speech occured 1.5 seconds into a 8KHz audio stream,
    ///          "actual" will equal 12,000 (1.5    /// 8000), and "safe" will equal
    ///          ( (1.5 - A)    /// 8000) where A is the value of ep.AdditionalStartSilence
    ///          (default 0.3 seconds).
    /// Type:  int[2]
    ///
    /// Event: NUANCE_EVENT_END_OF_SPEECH
    /// Data:  Index of last sample in utterance, relative to time at which
    ///          listening began.
    /// Type:  int
    ///
    /// Event: NUANCE_EVENT_PARTIAL_RESULT
    /// Data:  RecResult structure containing the best recognition hypothesis so far.
    /// Type:  RecResult
    ///
    /// Event: NUANCE_EVENT_FINAL_RESULT
    /// Data:  RecResult structure containing the final recognition hypothesis,
    ///          probability, etc.
    /// Type:  RecResult
    ///
    /// Event: NUANCE_EVENT_PLAYBACK_DONE
    /// Data:  none
    /// Type:  NULL
    ///
    /// Event: NUANCE_EVENT_PROCESS_DIED
    /// Data:  Reason for the death, if available
    /// Type:  NuanceStatus
    ///
    /// Event: NUANCE_EVENT_INCOMING_CALL
    /// Data:  A pointer to an incoming call info. structure which contains two
    ///          null-terminated strings with the intended local phone number and
    ///          caller-id (if supported).
    /// Type:  NuanceEventIncomingCallInfo
    ///
    /// Event: NUANCE_EVENT_REMOTE_HANGUP
    /// Data:  the phone number of the remote call that generated the hangup
    /// Type:  char[]  -- That is:
    ///                    - Wait for the event by passing in a (char    ///)
    ///                      pointer to an array of characters.
    ///                    - If you register a callback, your callback
    ///                      function will be called with a (char    ///)pointer to a
    ///                      NULL-terminated character string.
    ///
    /// Event: NUANCE_EVENT_OUTGOING_CALL_COMPLETED
    /// Data:  status of outgoing call completion (either NUANCE_OK,
    ///          NUANCE_TELEPHONY_REMOTE_BUSY, or NUANCE_TELEPHONY_ERROR)
    /// Type:  NuanceStatus
    ///
    /// Event: NUANCE_EVENT_DTMF
    /// Data:  The telephone key that was pressed
    /// Type:  char
    /// </pre>
    /// </remarks>
	public enum NuanceEvent {
		///<summary>Subprocess completed background initialization</summary>
		NUANCE_EVENT_INIT_COMPLETE,

		///<summary>Recognition events</summary>
		NUANCE_EVENT_START_OF_SPEECH,
		NUANCE_EVENT_END_OF_SPEECH,
		NUANCE_EVENT_PARTIAL_RESULT,
		NUANCE_EVENT_FINAL_RESULT,

		///<summary>Recording events</summary>
		NUANCE_EVENT_AUDIO_CHANGE,
		NUANCE_EVENT_AUDIO_VU_METER,
		NUANCE_EVENT_RECORDING_DONE,

		///<summary>Playback events</summary>
		NUANCE_EVENT_PLAYBACK_DONE,

		///<summary>Communication/information events</summary>
		NUANCE_EVENT_PROCESS_DIED,

		///<summary>Nuance Variable Set/Get result and version/packageID events.</summary>
		///<summary>Not intended for use by end-users. - rhs</summary>
		NUANCE_PRIVATE_EVENT_SET_VAR,
		NUANCE_PRIVATE_EVENT_GET_VAR,
		NUANCE_PRIVATE_EVENT_REMOTE_STATUS,

		///<summary>Telephony events</summary>
		NUANCE_EVENT_INCOMING_CALL,
		NUANCE_EVENT_REMOTE_HANGUP,
		NUANCE_EVENT_OUTGOING_CALL_COMPLETED,

		///<summary>Touch tone event</summary>
		NUANCE_EVENT_DTMF,

		///<summary>Used in RCInterpret()</summary>
		NUANCE_EVENT_NL_RESULT,
		///<summary>Used internally in RCAbort()</summary>
		NUANCE_PRIVATE_EVENT_ABORT_MARKER,
		///<summary>Used internally in RCKillPlayback()</summary>
		NUANCE_PRIVATE_EVENT_KILL_PLAYBACK_MARKER,

		///<summary>Used to ack sending of WGIL's to server</summary>
		NUANCE_EVENT_SET_WORD_GRAMMAR,
		NUANCE_EVENT_SET_CLASH_PRONS,
		NUANCE_EVENT_SET_CONSISTENCY_PRONS,

		NUM_NUANCE_EVENTS
	};
}
