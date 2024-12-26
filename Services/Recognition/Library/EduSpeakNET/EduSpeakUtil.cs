using System;

namespace EduSpeak
{
	public abstract class EduSpeakException: Exception {
		public EduSpeakException(string message) : base(message) {
		}
	}

	public class NuanceErrorException: EduSpeakException {
		private NuanceStatus result;

		public NuanceErrorException(NuanceStatus result) : base("Received NuanceStatus: "+result) {
			this.result = result;
		}

		public NuanceStatus getNuanceStatus() { return result; }
	}

	public class IllegalStateException: EduSpeakException {
		private RecognizerState illegalState;

		public IllegalStateException(RecognizerState illegalState, string message) : base(message) {
			this.illegalState = illegalState;
		}

		public RecognizerState getIllegalState() {
			return illegalState;
		}
	}

	public class UnimplementedException: EduSpeakException {
		public UnimplementedException(string message): base(message) {}
	}

	public class NotInitializedException: EduSpeakException {
		public NotInitializedException(string message): base(message) {}
	}

	/// <summary>
	/// Summary description for EduSpeakUtil.
	/// </summary>
	public class EduSpeakUtil {
		public static bool NUANCE_SUCCESS(NuanceStatus status) {
			return (status == NuanceStatus.NUANCE_OK);
		}

		public static void checkRetValue(NuanceStatus result) {
			if (!NUANCE_SUCCESS(result)) {
				throw new NuanceErrorException(result);
			}
		}
	}
}
