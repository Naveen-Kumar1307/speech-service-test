using System;

namespace EduSpeak
{
	/// <summary>
	/// Describes a class that refers to a C pointer in rcapi.dll, stored as an IntPtr.
	/// </summary>
	public class CPtrRef {
		private IntPtr cptr = new IntPtr(0);

		public CPtrRef(IntPtr cptr) {
			this.cptr = cptr;
		}


		public CPtrRef() {
		}

		public virtual void setCPtr(IntPtr cptr) {
			this.cptr = cptr;
		}

		public IntPtr getCPtr() { 
			if (cptr.ToInt32() == 0) {
				throw new NotInitializedException("Object not initialized");
			}
			return cptr;
		}

		public int getCPtrAsInt() { 
			return cptr.ToInt32();
		}

		public override String ToString() {
			return "CPtrRef@"+cptr;
		}
	}
}
