using System;
using System.Runtime.InteropServices;

namespace EduSpeak
{    
	// Status codes from nuance-status-codes.h
	public enum NuanceStatus {
		/// <summary>No error.</summary>
		NUANCE_OK,

		/// <summary>General error.</summary>
		NUANCE_ERROR,

		/// <summary>Memory allocation error.</summary>
		NUANCE_ALLOCATE_FAILED,

		/// <summary>File I/O errors.</summary>
		NUANCE_CANNOT_OPEN_FILE,
		NUANCE_CANNOT_REDIRECT_IO,
		NUANCE_CANNOT_REDIRECT_STDIN,
		NUANCE_CANNOT_REDIRECT_STDOUT,
		NUANCE_CANNOT_REDIRECT_STDERR,
		NUANCE_CANNOT_CHANGE_DIRECTORY,

		/// <summary>Subprogram errors.</summary>
		NUANCE_CHILD_CAUGHT_SIGNAL,
		NUANCE_CHILD_STOPPED,
		NUANCE_CHILD_DIED,

		NUANCE_TRAPPED_SIGINT,
		NUANCE_TRAPPED_SIGSEGV,
		NUANCE_TRAPPED_SIGBUS,
		NUANCE_TRAPPED_SIGTERM,

		/// <summary>Subprogram execution errors.</summary>
		NUANCE_CANNOT_EXEC_REC,

		/// <summary>Subprogram termination errors.</summary>
		NUANCE_CANNOT_KILL_REC,

		/// <summary>Other errors.</summary>
		NUANCE_NOT_INITIALIZED,
		NUANCE_NOT_READY,
		NUANCE_NETWORK_CONNECTION_ERROR,
		/// <summary>The specified output buffer size is too
		/// small for the requested output
		/// Or: the buffer is NULL</summary>
		NUANCE_OUTPUT_BUFFER_TOO_SMALL,
		 /// <summary>A problem of some sort occurred when attempting to set a variable</summary>
		NUANCE_SET_VARIABLE_ERROR, 
		  /// <summary>A problem of some sort occurred when attempting to get a variable</summary>
		NUANCE_GET_VARIABLE_ERROR,

		     /// <summary>The event is not relevant to the API   </summary>
		NUANCE_IRRELEVANT_EVENT, 
		 /// <summary>A request was made to wait for an event
		/// that cannot be waited for in the current API </summary>
		NUANCE_CANNOT_WAIT_FOR_EVENT,


		/// <summary>Error code corresponding to an information/warning/error
		/// message received from the recognizer.</summary>
		NUANCE_RECOGNIZER_ERROR,

		/// <summary>The recognition server can only process one
		///   utterance at a time per client.  Recognition
		///   for this client is already in progress, so
		///   another recognition cannot be started.</summary>
		NUANCE_RECSERVER_BUSY,      

		/// <summary>The function timed out before the requested event took place</summary>
		NUANCE_TIMED_OUT,           

		/// <summary>Argument passed to the function was not a RecEngine struct </summary>
		NUANCE_RECENGINE_INVALID, 
		/// <summary>Argument passed to the function was not a RecOobject struct</summary>
		NUANCE_RECOBJECT_INVALID,  
		/// <summary>Argument passed to the function was not a RecResult struct </summary>
		NUANCE_RECRESULT_INVALID, 
		/// <summary>The RecResult structure has been cleared.
		/// This probably indicates that you were only
		/// allowed to access its contents at the time
		/// it was passed to you in a callback</summary>  
		NUANCE_RECRESULT_EMPTY, 
		 /// <summary>Some object passed into a Nuance API was not
		/// valid</summary>
		NUANCE_INVALID_OBJECT,     

		/// <summary>Calls to any of the ProcessSamples functions
		/// must be bracketed by calls to StartUtterance
		/// and EndUtterance.  One or more calls to a
		/// ProcessSamples function must come between
		/// the start and end calls.
		/// AbortUtterance functions may be called at
		/// any time.  Any other sequence of calls will
		/// produce this error.
		///
		/// This error code has been replaced with
		/// nuance_out_of_sequence_call, but is
		/// still used in sapi-client.c and rapi.c.
		/// </summary>
		NUANCE_INVALID_SEQUENCE, 
	     /// <summary>The grammar name supplied to some API
		/// function (e.g. a StartUtterance function)
		/// was unknown.  In the case of a
		/// StartUtterance function, the library will
		/// reuse the last correctly specified grammar.
		/// </summary>
		NUANCE_UNKNOWN_GRAMMAR,
		 /// <summary>Generally indicates that the user
		/// requested an item by number, but the
		/// number was out of range.  For instance, an
		/// invalid enumerated type was passed to
		/// Nuance.
		/// </summary>
		NUANCE_ARGUMENT_OUT_OF_RANGE,

		/// <summary>Functions having to do with Nuance Parameters</summary>
		NUANCE_NO_PACKAGE,            /// <summary>Package, or (if allowed) signal to     </summary>
									/// <summary> to omit package, was not specified    </summary>
		NUANCE_UNKNOWN_MODULE,        /// <summary>The module portion of                  </summary>
					/// <summary><module.ParameterName> is not a Nuance </summary>
									/// <summary> module                                </summary>
		NUANCE_UNKNOWN_PARAMETER,     /// <summary>The string does not represent a known  </summary>
					/// <summary>nuance parameter                       </summary>
		NUANCE_VALUE_MUST_BE_INT,     /// <summary>Parameter type is 'int', but another   </summary>
					/// <summary>type of value was given                </summary>
		NUANCE_VALUE_MUST_BE_FLOAT,   /// <summary>Parameter type is 'float', but another </summary>
					/// <summary>type of value was given                </summary>
		NUANCE_VALUE_MUST_BE_BOOL,    /// <summary>Parameter type is boolean, but another </summary>
					/// <summary>type of value was given                </summary>
		NUANCE_VALUE_MUST_BE_STRING,  /// <summary>Parameter type is (char ///), but another</summary>
					/// <summary>type of value was given                </summary>
		NUANCE_VALUE_TOO_SMALL,       /// <summary>Specifed value was less than allowable </summary>
					/// <summary>minimum                                </summary>
		NUANCE_VALUE_TOO_LARGE,       /// <summary>Specifed value was greater than        </summary>
					/// <summary>allowable maximum                      </summary>
		NUANCE_VALUE_NOT_ALLOWABLE,   /// <summary>The supplied value was not one of a set</summary>
					/// <summary>of allowable values                    </summary>
		NUANCE_IRRELEVANT_PARAMETER,  /// <summary>Parameter exists in the Nuance system, </summary>
					/// <summary>but is irrelevant to this API          </summary>
		NUANCE_NOT_A_CONFIG_PARAM,    /// <summary>Parameter does not match the syntax of </summary>
									/// <summary> a command line configuration parameter</summary>
		NUANCE_PARAM_SETABLE_ONLY_AT_INIT,/// <summary>This parameter can be set before   </summary>
						/// <summary>initialization begins (ie. from    </summary>
						/// <summary>the command line or a              </summary>
						/// <summary>nuance-resources file) but not on  </summary>
										/// <summary>the fly                            </summary>
		NUANCE_PARAM_READ_ONLY,       /// <summary>This parameter can be read, but not set</summary>
		NUANCE_PARAM_WRITE_ONLY,      /// <summary>This parameter can be set, but not     </summary>
					/// <summary>  queried                              </summary>
		NUANCE_PARAMETER_NOT_SET,     /// <summary>Requested parameter was not found in   </summary>
									/// <summary>  the configuration object             </summary>
		NUANCE_CONFIGURATION_ERROR,   /// <summary>The configuration object is            </summary>
									/// <summary>  inconsistent or corrupted            </summary>
		NUANCE_END_OF_PARAM_LIST,     /// <summary>There are no more parameters to return </summary>
		NUANCE_NO_CONFIG_FOR_PACKAGE, /// <summary>The configuration for the specified    </summary>
									/// <summary>  package is neither the specified     </summary>
									/// <summary>  configuration object nor a           </summary>
									/// <summary>  configuration object in its associated////
									/// <summary>  list                                 </summary>
		NUANCE_SAMPLING_RATE_NOT_SPECIFIED,
									/// <summary>Parameter specifying sampling rate is  </summary>
									/// <summary>  required                             </summary>
		NUANCE_PRUNING_NOT_SPECIFIED, /// <summary>Parameter specifying pruning is        </summary>
					/// <summary>    required                           </summary>

		NUANCE_GRAMMAR_PROCESSOR_METHOD_NOT_SPECIFIED,
									/// <summary>Parameter specifying grammar processor </summary>
									/// <summary>  method is required                   </summary>
		NUANCE_SHUTDOWN_ERROR,      /// <summary>When asked to terminate, Nuance failed to
					/// do so cleanly</summary>
		NUANCE_NOT_IMPLEMENTED,     /// <summary>The requested function or feature has not
					/// yet been implemented</summary>

		NUANCE_NOT_CONNECTED,       /// <summary>Client-Server control connection not in
					/// place</summary>
		NUANCE_SERVER_NOT_ACCESSIBLE,  /// <summary>A client was unable to connect to a
						/// server</summary>
		NUANCE_SERVER_AT_FULL_CAPACITY, /// <summary>tried to connect to a server but
						/// all allowable client connections
						/// connections are already allocated</summary>
		NUANCE_SERVER_QUIESCING,     /// <summary>the recognition server is
						/// no longer accepting any
						/// clients, because it has been
						/// asked to quiesce</summary>
		NUANCE_CONTROL_CON_LOST,       /// <summary>Control port connection lost</summary>
		NUANCE_DATA_CON_FAILED,        /// <summary>Data port connection could not be
						/// established</summary>
		NUANCE_DATA_CON_NOT_SETUP,     /// <summary>Data port connection not established</summary>
		NUANCE_PASSIVE_TCP_FAILURE,    /// <summary>Could not establish a passive open</summary>
		NUANCE_CANNOT_LOOKUP_HOSTNAME, /// <summary>Cannot ascertain this machine's hostname.
						/// gethostname() failed.</summary>
		NUANCE_GETHOSTBYNAME_FAILED,        /// <summary>gethostbyname() failed</summary>
		NUANCE_WRITE_CONTROL_PORT_FAILED,   /// <summary>Write failure on control port</summary>
		NUANCE_WRITE_DATA_PORT_FAILED,      /// <summary>Write failure on data port</summary>
		NUANCE_BAD_ARG_IN_TIMEVAL_CONV,     /// <summary>Bad argument detected when
						/// converting to a struct timeval</summary>
		NUANCE_SELECT_ERROR,                /// <summary>Error detected by select() call</summary>
		NUANCE_SELECT_DATA_CON_TIMEOUT,     /// <summary>Could not establish data port</summary>
		NUANCE_SOCKET_ACCEPT_ERROR,         /// <summary>accept() failed</summary>
		NUANCE_BAD_SAMPLING_VALUE,          /// <summary>illegal value specified for sampling
						/// rate</summary>
		NUANCE_BAD_SAMPLE_TYPE,             /// <summary>illegal sample type</summary>
		NUANCE_MULAW_NOT_SPECIFIED_IN_START_UTTERANCE,   /// <summary>cannot process mulaw
								/// samples after
								/// specifying linear</summary>
		NUANCE_LINEAR_NOT_SPECIFIED_IN_START_UTTERANCE,  /// <summary>cannot process linear
								/// samples after
								/// specifying mulaw</summary>
		NUANCE_INVALID_EVENT,                      /// <summary>illegitimate event
							/// specified</summary>
		NUANCE_SERVER_MESSAGE_READ_ERROR,          /// <summary>error reading message from
							/// server</summary>
		NUANCE_SERVER_MESSAGE_READ_EOF,            /// <summary>EOF read on a message from
							/// server</summary>
		NUANCE_SERVER_MESSAGE_SIZE_UNREADABLE,     /// <summary>size field of message from
							/// server unreadable</summary>
		NUANCE_UNRECOGNIZED_SERVER_MESSAGE,        /// <summary>Client couldn't decipher
							/// message from server.
													/// This should never happen.</summary>
		NUANCE_SERVER_UNABLE_TO_CONNECT_TO_CLIENT, /// <summary>server tells us (via control
							/// port) it couldn't connect</summary>
		NUANCE_CLIENT_MESSAGE_READ_ERROR,          /// <summary>server encountered error
							/// reading message from client</summary>
		NUANCE_CLIENT_MESSAGE_READ_EOF,            /// <summary>server encountered EOF in a
							/// client message</summary>
		NUANCE_NO_ACK_ABORT_BEFORE_TIMEOUT,        /// <summary>No abort ACK received from
													/// server within timeout
							/// period</summary>
		NUANCE_EP_ERROR,                        /// <summary>Non-specific endpointer error</summary>

		NUANCE_LICENSE_CHECK_FAILED,              /// <summary>License codes mismatch     </summary>
		NUANCE_REC_OBJECT_INIT_FAILED,            /// <summary>Couldn't initialize a      </summary>
												/// <summary>  RecObject                </summary>
		NUANCE_ENDPOINTER_INIT_FAILED,            /// <summary>Couldn't initialize an     </summary>
												/// <summary>  endpointer               </summary>
		NUANCE_BACKTRACE_INIT_FAILED,             /// <summary>Couldn't initialize the    </summary>
												/// <summary>  recognition backtrace    </summary>
		NUANCE_PHONE_PROCESSOR_INIT_FAILED,       /// <summary>Couldn't initialize the    </summary>
												/// <summary>  recognition phone        </summary>
												/// <summary>  processor                </summary>
		NUANCE_GRAMMAR_PROCESSOR_INIT_FAILED,     /// <summary>Couldn't initialize the    </summary>
												/// <summary>  recognition grammar      </summary>
												/// <summary>  processor                </summary>

		////------------------------------------------------------------------------
		/// The following are audio and/or telephony status codes.
		///-----------------------------------------------------------------------////
		NUANCE_UNKNOWN_AUDIO_PROVIDER,      /// <summary>The specified audio provider is
												/// unknown to the audio library.</summary>

		NUANCE_AUDIO_PROVIDER_NOT_SUPPORTED_ON_THIS_SYSTEM,
											/// <summary>The specified audio provider is
											/// 	not supported on the current
											/// 	platform or machine.</summary>

		NUANCE_AUDIO_DEVICE_NOT_INSTALLED,  /// <summary>No hardware or device drivers for
											/// 	the given provider and device
											/// 	could be found.</summary>

		NUANCE_AUDIO_DEVICE_BUSY,           /// <summary>The specified audio device for the
											/// 	provider is already in use.</summary>

		NUANCE_AUDIO_FORMAT_NOT_SUPPORTED,  /// <summary>The specified audio format does
											/// 	not match any of the audio
											/// 	provider's known formats.</summary>

		NUANCE_AUDIO_ERROR,                 /// <summary>This is a catch-all error which is
											/// 	returned, along with a message
											/// 	printed using nonfatal_error(), to
											/// 	the caller when a provider-
											/// 	specific error occurs that is too
											/// 	internal to pass back to the
											/// 	application.  In this case, the
											/// 	calling functions should simply
											/// 	return this error without printing
											/// 	anything, since the lowest level
											/// 	(the one that generated this error)
											/// 	is the only one that knows what the
											/// 	meaning of the error was.</summary>

		NUANCE_OUT_OF_SEQUENCE_CALL,        /// <summary>Call to playback, recording, or
											/// 	telephony being made at the wrong
											/// 	point in the state machine, i.e.
											/// 	write-samples before start-
											/// 	playback, hangup before place-
											/// 	call, etc.
											/// This should be considered a
											/// 	programming error.</summary>

		NUANCE_TELEPHONY_BAD_PHONE_NUMBER,  /// <summary>The phone number specified does
											/// 	not match the one to be answered
											/// 	in the function call to answer an
											/// 	incoming call.
											/// 	(Use "///" as a wildcard string.)</summary>

		NUANCE_TELEPHONY_CHANNEL_CLOSED,    /// <summary>Playback or recording is being
											/// 	attempted after a hangup occurred
											/// 	or before a call connection was
											/// 	established.</summary>

		NUANCE_TELEPHONY_REMOTE_BUSY,       /// <summary>Unable to connect to remote
											/// 	number because it is in use</summary>

		NUANCE_TELEPHONY_DEVICE_NOT_INSTALLED,/// <summary>The telephone hardware could not
											/// 	be found, i.e. it isn't installed.</summary>

		NUANCE_TELEPHONY_DEVICE_BUSY,       /// <summary>The telephone hardware is already
											/// 	in use.</summary>

		NUANCE_TELEPHONY_ERROR,             /// <summary>This is a catch-all telephony error
											/// 	that is used analogously to the
											/// 	catch-all audio error.</summary>
		////------------------------------------------------------------------------
		/// End of audio error codes.
		///-----------------------------------------------------------------------////

		NUANCE_VERSION_OR_PACKAGE_MISMATCH,     /// <summary>The RECSERVER version and
							/// package ID must equal those of
							/// the sapi client</summary>
		NUANCE_PIPE_CREATION_FAILED,            /// <summary>pipe() failed.  Use plumber()</summary>
		NUANCE_FORK_FAILED,                     /// <summary>fork() failed.  Try spoon()</summary>

		NUANCE_ALREADY_LISTENING,         /// <summary>This error indicates that the Nuance
										/// Recognition Client is already
										/// listening to the input stream because
										/// of a prior 'record' or 'recognize'
						/// command.  The command in progress must
										/// either finish naturally or be
										/// terminated with an 'end' or 'abort'
						/// before another record/recognize
										/// command can be issued.
						////
		NUANCE_WRITE_FAILED,              /// <summary>a write() call failed</summary>
		NUANCE_READ_FAILED,               /// <summary>a read() call failed</summary>

		NUANCE_RECSERVER_INIT_FAILED,     /// <summary>The recserver forked off by the
											/// recclient failed to initialize
											/// successfully</summary>
		NUANCE_RECSERVER_DIED,            /// <summary>The recserver died.  A process
											/// communicating with the recserver may
											/// exit for this reason.</summary>

		NUANCE_NL_SYSTEM_NOT_INITIALIZED, /// <summary>Name pretty much says it all</summary>
		NUANCE_UNKNOWN_SLOT_NAME,         /// <summary>A slot name passed in is one that is
											/// undefined.</summary>
		NUANCE_SLOT_NOT_FILLED,           /// <summary>The specified slot does not appear in
											/// the specified template.</summary>
		NUANCE_FEATURE_NOT_SET,           /// <summary>The specified feature does not appear
											/// in the specified structure.</summary>
		NUANCE_VALUE_NOT_INT,             /// <summary>An integer value has been requested
											/// but the value in question is not,
											/// in fact, an integer.</summary>
		NUANCE_VALUE_NOT_FLOAT,           /// <summary>A floating point number has been
						/// requested but the value in question is
						/// not, in fact, an floating point
											/// number.</summary>
		NUANCE_VALUE_NOT_STRING,          /// <summary>A string value has been requested
											/// but the value in question is not,
											/// in fact, a string.</summary>
		NUANCE_VALUE_NOT_STRUCTURE,       /// <summary>A structure value has been requested
											/// but the value in question is not,
											/// in fact, a structure.</summary>
		NUANCE_VALUE_NOT_LIST,            /// <summary>A list value has been requested
											/// but the value in question is not,
											/// in fact, a list.</summary>

		////------------------------------------------------------------------------
		/// Both of the following are possible RCPlay///() return values and have
		/// nothing to do with the lower-level audio modules.
		///-----------------------------------------------------------------------////
		NUANCE_PLAYBACK_FAILED, /// <summary>RCPlayLastUtterance() was called before any
									/// utterance was recorded.</summary>

		NUANCE_PLAYBACK_BUSY, /// <summary>Once a 'play' command has been issued, another
								/// play command cannot be issued until the first has
								/// completed. This limitation is imposed to avoid a
								/// race condition in which the number of
								/// playback-done events to expect cannot be
								/// determined.  Note however that the RCPlayfile()
					/// command allow you to specify multiple files at
								/// once.  These files will be played, and will
								/// generate a single playback-done event at
								/// completion.</summary>

		/// <summary>Status codes for dynamic grammar modification</summary>
		NUANCE_GRAMMAR_NOT_DYNAMIC,
		NUANCE_BAD_NL_COMMAND_STRING,
		NUANCE_CANNOT_DO_DYNAMIC_ADD,
		NUANCE_CANNOT_DO_DYNAMIC_DELETE,
		NUANCE_CANNOT_DO_DYNAMIC_ACTIVATE,
		NUANCE_CANNOT_DO_DYNAMIC_DEACTIVATE,

		/// <summary>Status codes relating to SQL query translation and execution</summary>
		NUANCE_COULD_NOT_PRODUCE_SQL_QUERY,
		NUANCE_BAD_SQL_QUERY,
		NUANCE_DB_QUERY_FAILED,
		NUANCE_DB_CONNECT_FAILED,
		NUANCE_DB_QUERY_ABORTED,

		/// <summary>Status codes relating to the Prompter object</summary>
		NUANCE_INVALID_PROMPTER,
		NUANCE_INVALID_PROMPT_NAME,

		/// <summary>Status codes related to Piecewise Playback</summary>
		NUANCE_END_OF_SAMPLES,

		/// <summary>necessary to know when the client has shut down</summary>
		NUANCE_CLIENT_DISCONNECT,

		/// <summary>Utterance and playback ID sequencing</summary>
		NUANCE_INVALID_PLAYBACK_ID,
		NUANCE_INVALID_RECORD_ID,
		NUANCE_INVALID_UTTERANCE_ID,

		/// <summary>Speaker verification status codes. More codes will be added in
		/// the near future.
		////
		NUANCE_VERIFIER_OBJ_INVALID,
		NUANCE_VERIFIER_CLAIMANT_OBJ_INVALID,

		/// <summary>RecClient behaviors</summary>
		NUANCE_UNKNOWN_BEHAVIOR,
		NUANCE_UNABLE_TO_LOAD_BEHAVIOR,

		/// <summary>Generic error code for ambiguity.  For example, used in SQL query
		/// generation.</summary>
		NUANCE_AMBIGUITY,

		/// <summary>The number of NUANCE error codes.</summary>
		NUM_NUANCE_ERROR_CODES
		/// <summary>No more status codes</summary>
	};
}
