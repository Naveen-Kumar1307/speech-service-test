using System;
using System.Text;

namespace EduSpeak
{
	/// <summary>
	/// Summary description for NuanceConfig.
	/// </summary>
	public class NuanceConfig: CPtrRef {
		private static Logger logger = new Logger("EduSpeak.NuanceConfig");
		public const string NUANCE_PARAMETER_SOURCE_COMMAND_LINE = "Command line";

		private int i_search = 0;
		private bool endOfParamList = false;

		public NuanceConfig(IntPtr cptr): base(cptr) {
		}

		public NuanceConfig() {
		}

		protected void Finalize() {
			if (getCPtrAsInt() != 0) {
				Free();
			}
		}

		/// <summary>Builds the NuanceConfig using a package directory</summary>
		public void Build(string package_dir) {
			NuanceStatus status = NuanceStatus.NUANCE_OK;
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Build: package_dir<"+package_dir+">");
			IntPtr ret = NuanceConfigAPI.NuanceConfigBuild(package_dir, ref status);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Build: package_dir<"+package_dir+
				"> status<"+status+">");
			EduSpeakUtil.checkRetValue(status);
			if (getCPtrAsInt() != 0) {
				Free();
			}
			setCPtr(ret);
		}

		/// <summary>builds the NuanceConfig instance from a command line array</summary>
		public void BuildFromCommandLine(string[] cmdLine, bool package_required) {
			throw new UnimplementedException("BuildFromCommandLine is not implemented");
		}

		/// <summary>builds the NuanceConfig instance from a package file name</summary>
		public void BuildFromFilename(string fileName, string paramSource,
									bool isUserFile, bool mustBeValid) {
			NuanceStatus status = NuanceStatus.NUANCE_OK;
			if (Logger.GetTraceEnabled()) logger.Trace(this, "BuildFromFilename: fileName<"+fileName+
				"> paramSource<"+paramSource+"> isUserFile<"+isUserFile+"> mustBeValid<"+mustBeValid+">");
			int cptr = getCPtrAsInt();
			IntPtr ret = NuanceConfigAPI.NuanceConfigFromFilename(new IntPtr(cptr), fileName, paramSource,
				isUserFile?1:0, mustBeValid?1:0, ref status);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "BuildFromFilename: fileName<"+fileName+
				"> paramSource<"+paramSource+"> isUserFile<"+isUserFile+
				"> mustBeValid<"+mustBeValid+"> ret<"+ret+">");
			EduSpeakUtil.checkRetValue(status);
			if (cptr != 0 && ret.ToInt32() != cptr) {
				Free();
			}
			setCPtr(ret);
		}

		/// <summary>builds the NuanceConfig instance from a string array</summary>
		public void BuildFromStringArray(string[] stringArray, string paramSource,
										bool isUserFile, bool mustBeValid,
										bool removeConfigArgs) {
			throw new UnimplementedException("BuildFromStringArray is not implemented");
		}

		/// <summary>builds the NuanceConfig instance from a command line array and sets the debug level to 4</summary>
		public void BuildSetDebugLevel(string[] cmdLine) {
			throw new UnimplementedException("BuildSetDebugLevel is not implemented");
		}

		/// <summary>builds the NuanceConfig instance from a command line array</summary>
		public void BuildSimple(string[] cmdLine) {
			throw new UnimplementedException("BuildSimple is not implemented");
		}

		/// <summary>copies the INuanceConfig instance</summary>
		public NuanceConfig Copy() {
			NuanceStatus status = NuanceStatus.NUANCE_ERROR;
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Copy");
			IntPtr ret = NuanceConfigAPI.NuanceConfigCopy(getCPtr(), ref status);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Copy: status<"+status+
				"> ret<"+ret+">");
			EduSpeakUtil.checkRetValue(status);
			return new NuanceConfig(ret);
		}

		/// <summary>frees the internal data of an INuanceConfig object</summary>
		public void Free() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Free");
			NuanceConfigAPI.NuanceConfigFree(getCPtr());
		}

		/// <summary>retrieves a new NuanceConfig instance for the specified package index</summary>
		public NuanceConfig GetConfigForIthPackage(int package_index) {
			return null;
		}

		/// <summary>retrieves a parameter that has an integer value</summary>
		public int GetIntParameter(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntParameter: paramName<"+paramName+">");
			int ret = 0;
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigGetIntParameter(getCPtr(), paramName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetIntParameter: paramName<"+paramName+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary>retrieves a parameter that has a float value</summary>
		public float GetFloatParameter(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFloatParameter: paramName<"+paramName+">");
			float ret = 0;
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigGetFloatParameter(getCPtr(), paramName, ref ret));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetFloatParameter: paramName<"+paramName+
				"> ret<"+ret+">");
			return ret;
		}

		/// <summary>retrieves a parameter that has a string value</summary>
		public string GetStringParameter(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringParameter: paramName<"+paramName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigGetStringParameter(getCPtr(), paramName, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetStringParameter: paramName<"+paramName+
				"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>retrieves a parameter converted to a string</summary>
		public string GetParameterAsString(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterAsString: paramName<"+paramName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigGetParameterAsString(getCPtr(), paramName, ret, len));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterAsString: paramName<"+paramName+
				"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>searches the parameters by substring</summary>
		public string GetParameterList(string initial_substring, bool user_params_only) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterList: initial_substring<"+
											  initial_substring+"> user_params_only<"+user_params_only+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			NuanceStatus status = NuanceConfigAPI.NuanceConfigGetParameterList(getCPtr(), 
				initial_substring, user_params_only?1:0, ref i_search, ret, len);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterList: initial_substring<"+
						initial_substring+"> user_params_only<"+user_params_only+
						"> ret<"+ret+"> status<"+status+">");
			if (status == NuanceStatus.NUANCE_END_OF_PARAM_LIST) {
				endOfParamList = true;
			} else {
				endOfParamList = false;
				EduSpeakUtil.checkRetValue(status);
			}
			return ret.ToString();
		}

		/// <summary>searches the parameters by owner</summary>
		public string GetParameterListForOwner(string owner, bool user_params_only) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterListForOwner: owner<"+
											  owner+"> user_params_only<"+user_params_only+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			NuanceStatus status = NuanceConfigAPI.NuanceConfigGetParameterListForOwner(getCPtr(), 
				owner, user_params_only?1:0, ref i_search, ret, len);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterListForOwner: owner<"+
						owner+"> user_params_only<"+user_params_only+"> ret<"+ret+"> status<"+status+">");
			if (status == NuanceStatus.NUANCE_END_OF_PARAM_LIST) {
				endOfParamList = true;
			} else {
				endOfParamList = false;
				EduSpeakUtil.checkRetValue(status);
			}
			return ret.ToString();
		}

		/// <summary>retrieves the source of a parameter</summary>
		public string GetParameterSource(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterSource: paramName<"+paramName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			int is_from_userP = 0;
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigGetParameterSource(getCPtr(), 
				paramName, ret, len, ref is_from_userP));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "GetParameterSource: paramName<"+
											  paramName+"> ret<"+ret+">");
			return ret.ToString();
		}

		/// <summary>determines if a parameter was defined by the user</summary>
		public bool IsParameterFromUser(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "IsParameterFromUser: paramName<"+paramName+">");
			const int len = 4096;
			StringBuilder ret = new StringBuilder(len);
			int is_from_userP = 0;
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigGetParameterSource(getCPtr(), 
				paramName, ret, len, ref is_from_userP));
			if (Logger.GetTraceEnabled()) logger.Trace(this, "IsParameterFromUser: paramName<"+
											  paramName+"> is_from_userP<"+is_from_userP+">");
			return is_from_userP!=0;
		}

		/// <summary>returns TRUE only if the specified parameter is defined</summary>
		public bool ParameterExists(string paramName) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ParameterExists: paramName<"+paramName+">");
			int ret = NuanceConfigAPI.NuanceConfigParameterExists(getCPtr(), paramName);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ParameterExists: paramName<"+paramName+
				"> ret<"+ret+">");
			return ret!=0;
		}

		/// <summary>determines if a parameter is from the command line</summary>
		public bool ParameterIsFromCommandLine(string paramName) {
			return ParameterIsFromSource(paramName, NUANCE_PARAMETER_SOURCE_COMMAND_LINE);
		}

		/// <summary>determines if a parameter comes from a specific source</summary>
		public bool ParameterIsFromSource(string paramName, string source) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ParameterIsFromSource: paramName<"+paramName+
				"> source<"+source+">");
			int ret = NuanceConfigAPI.NuanceConfigParameterIsFromSource(getCPtr(), paramName, source);
			if (Logger.GetTraceEnabled()) logger.Trace(this, "ParameterIsFromSource: paramName<"+paramName+
				"> source<"+source+"> ret<"+ret+">");
			return ret!=0;
		}

		/// <summary>prints the NuanceConfig to std_out</summary>
		public void Print() {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "Print");
			NuanceConfigAPI.NuanceConfigPrint(getCPtr());
		}

		/// <summary>sets a parameter with an integer value</summary>
		public void SetIntParameter(string paramName, int val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetIntParameter paramName<"+paramName+
				"> val<"+val+">");
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigSetIntParameter(getCPtr(), paramName, val));
		}

		/// <summary>sets a parameter with a float value</summary>
		public void SetFloatParameter(string paramName, float val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetFloatParameter paramName<"+paramName+
				"> val<"+val+">");
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigSetFloatParameter(getCPtr(), paramName, val));
		}

		/// <summary>sets a parameter with a string value</summary>
		public void SetStringParameter(string paramName, string val) {
			if (Logger.GetTraceEnabled()) logger.Trace(this, "SetStringParameter paramName<"+paramName+
				"> val<"+val+">");
			EduSpeakUtil.checkRetValue(NuanceConfigAPI.NuanceConfigSetStringParameter(getCPtr(), paramName, val));
		}

		/// <summary>EndOfParamList is set to TRUE when a parameter search has ended</summary>
		public bool EndOfParamList {
			get {
				return endOfParamList;
			}
		}

		/// <summary>Search index used for searching through the parameter list</summary>
		public int ISearch {
			get {
				return i_search;
			}
			set {
				i_search = value;
			}
		}
		/// <summary>specifies the number of packages</summary>
		public int NumPackages {
			get {
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumPackages");
				NuanceStatus status = NuanceStatus.NUANCE_ERROR;
				int ret = NuanceConfigAPI.NuanceConfigGetNumPackages(getCPtr(), ref status);
				if (Logger.GetTraceEnabled()) logger.Trace(this, "NumPackages: ret<"+ret+
					"> status<"+status+">");
				EduSpeakUtil.checkRetValue(status);
				return ret;
			}
		}
	}
}
