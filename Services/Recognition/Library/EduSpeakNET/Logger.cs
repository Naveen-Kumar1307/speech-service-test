using System;

namespace EduSpeak
{
	/// <summary>
	/// Used for logging EduSpeak C# methods. Note that these method will be replaced with Log4Net
	/// which can be found at http://logging.apache.org/log4net/downloads.html. Log4Net is still
	/// in incubation mode at the ASF, so until then it won't be implemented here.
	/// </summary>
	public class Logger {
		private string category;
		private static volatile bool traceEnabled = false;
		private static volatile bool outputToConsole = false;

		public Logger(string category) {
			this.category = category;
		}

		/// <summary>
		/// Retrieves whether or not the EduSpeak classes will output function traces.
		/// </summary>
		/// <returns>whether or not function traces are enabled</returns>
		public static bool GetTraceEnabled() {
			return traceEnabled;
		}

		/// <summary>
		/// Sets whether or not the EduSpeak classes will output function traces.
		/// </summary>
		///<param name=enabled>
		///Whether or not to output function traces.
		///</param>
		public static void SetTraceEnabled(bool enabled) {
			traceEnabled = enabled;
		}

		/// <summary>
		/// Retrieves whether or not the EduSpeak classes will output logging to the console.
		/// </summary>
		/// <returns>whether or not console logging is enabled</returns>
		public static bool GetOutputToConsole() {
			return outputToConsole;
		}

		/// <summary>
		/// Retrieves whether or not the EduSpeak classes will output logging to the console.
		/// </summary>
		///<param name=output>
		///whether or not console logging is enabled
		///</param>
		public static void SetOutputToConsole(bool output) {
			outputToConsole = output;
		}

		/// <summary>
		/// Traces function execution. This method calls System.Diagnostice.Trace.WriteLine.
		/// Note that this method only outputs a trace if tracing is enabled (see the
		/// static method setTraceEnabled).
		/// </summary>
		///<param name=src>
		///The source object tracing.
		///</param>
		///<param name=message>
		///The trace message.
		///</param>
		///<seealso cref="SetTraceEnabled" />
		public void Trace(object src, string message) {
			if (traceEnabled) {
				System.Diagnostics.Trace.WriteLine("[source="+src.ToString()+"]: "+message, category);
				if (outputToConsole) System.Console.WriteLine(category+"[source="+src.ToString()+"]: "+message);
			}
		}

		/// <summary>
		/// Traces function execution. This method calls System.Diagnostice.Trace.WriteLine.
		/// Note that this method only outputs a trace if tracing is enabled (see the
		/// static method setTraceEnabled).
		/// </summary>
		///<param name=src>
		///The source class tracing.
		///</param>
		///<param name=message>
		///The trace message.
		///</param>
		///<seealso cref="SetTraceEnabled" />
		public void Trace(string message) {
			if (traceEnabled) {
				System.Diagnostics.Trace.WriteLine(message, category);
				if (outputToConsole) System.Console.WriteLine(category+": "+message);
			}
		}

		/// <summary>
		/// Traces function execution. This method calls System.Diagnostice.Trace.Fail.
		/// </summary>
		///<param name=message>
		///The error message.
		///</param>
		///<param name=detailMessage>
		///A detailed error message.
		///</param>
		public void Error(string message, string detailMessage) {
			System.Diagnostics.Trace.Fail(message, detailMessage);
			if (outputToConsole) System.Console.WriteLine("ERROR: "+category+":"+message);
			if (outputToConsole) System.Console.WriteLine("ERROR: "+detailMessage);
		}
	}
}
