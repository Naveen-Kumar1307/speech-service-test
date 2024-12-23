using System;
using System.Runtime.InteropServices;
using System.Text;

namespace EduSpeak
{
    public class NuanceConfigAPI {
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigBuildPublicWrapper")]
        public static extern IntPtr NuanceConfigBuild(string package_dir, ref NuanceStatus statusP);
		/*[DllImport("rcapi.dll", EntryPoint="NuanceConfigBuildFromCommandLinePublicWrapper")]
        public static extern unsafe IntPtr NuanceConfigBuildFromCommandLine(int *argc_ptr,
                                           char        **argv,
                                           int           package_required,
                                           ref NuanceStatus statusP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigBuildFromMasterPackageAndCommandLinePublicWrapper")]
        public static extern unsafe IntPtr NuanceConfigBuildFromMasterPackageAndCommandLine(char *master_package,
                                 int          *argc_ptr,
                                 char        **argv,
                                 ref NuanceStatus statusP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigBuildSetDebugLevelPublicWrapper")]
        public static extern unsafe IntPtr NuanceConfigBuildSetDebugLevel(int *argc_ptr, char **argv);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigBuildSimplePublicWrapper")]
        public static extern unsafe IntPtr NuanceConfigBuildSimple(int *argc_ptr, char **argv);*/
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigFromFilenamePublicWrapper")]
        public static extern IntPtr NuanceConfigFromFilename(IntPtr nuance_config,
                               string filename,
                               string  param_source,
                               int           is_user_file,
                               int           must_be_valid,
                               ref NuanceStatus statusP);
		/*[DllImport("rcapi.dll", EntryPoint="NuanceConfigFromStringArrayPublicWrapper")]
        public static extern unsafe IntPtr NuanceConfigFromStringArray(IntPtr nuance_config,
                                      int          *string_countP,
                                      char        **string_array,
                                      char         *param_source,
                                      int           is_from_user,
                                      int           must_be_valid,
                                      int           remove_config_args,
                                      ref NuanceStatus statusP);*/
		[DllImport("rcapi.dll", EntryPoint="PNuanceConfigSetIntParameterublicWrapper")]
        public static extern NuanceStatus NuanceConfigSetIntParameter(IntPtr config,
                                      string param_name,
                                      int           param_value);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigSetFloatParameterPublicWrapper")]
        public static extern NuanceStatus NuanceConfigSetFloatParameter(IntPtr config,
                                        string param_name,
                                        float         param_value);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigSetStringParameterPublicWrapper")]
        public static extern NuanceStatus NuanceConfigSetStringParameter(IntPtr config,
                                         string param_name,
                                         string param_value);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetIntParameterPublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetIntParameter(IntPtr config,
                                      string  param_name,
                                      [In,Out]ref int          param_valueP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetFloatParameterPublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetFloatParameter(IntPtr config,
                                        string param_name,
                                        [In,Out]ref float        param_valueP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetStringParameterPublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetStringParameter(IntPtr config,
                                         string param_name,
                                         StringBuilder output_buffer,
                                         int           buffer_size);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetParameterAsStringPublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetParameterAsString(IntPtr config,
                                           string param_name,
                                           StringBuilder output_buffer,
                                           int           buffer_size);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetParameterSourcePublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetParameterSource(IntPtr config,
                                         string param_name,
                                         StringBuilder output_buffer,
                                         int           buffer_size,
                                         [In,Out]ref int          is_from_userP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigParameterExistsPublicWrapper")]
        public static extern int NuanceConfigParameterExists(IntPtr config,
                                      string param_name);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigParameterIsFromSourcePublicWrapper")]
        public static extern int NuanceConfigParameterIsFromSource(IntPtr config,
                                            string param_name,
                                            string source);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetParameterListPublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetParameterList(IntPtr config,
                                           string initial_substring,
                                           int           user_params_only,
                                           [In,Out]ref int          i_searchP,
                                           StringBuilder output_buffer,
                                           int           buffer_size);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetParameterListForOwnerPublicWrapper")]
        public static extern NuanceStatus NuanceConfigGetParameterListForOwner(IntPtr config,
                                               string owner,
                                               int           user_params_only,
                                               [In,Out]ref int          i_searchP,
                                               StringBuilder output_buffer,
                                               int           buffer_size);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetNumPackagesPublicWrapper")]
        public static extern int NuanceConfigGetNumPackages(IntPtr config,
                                     ref NuanceStatus statusP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigGetConfigForIthPackagePublicWrapper")]
        public static extern IntPtr NuanceConfigGetConfigForIthPackage(IntPtr config,
                                             int           package_index,
                                             ref NuanceStatus statusP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigCopyPublicWrapper")]
        public static extern IntPtr NuanceConfigCopy(IntPtr in_config,
                           ref NuanceStatus statusP);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigPrintPublicWrapper")]
        public static extern void NuanceConfigPrint(IntPtr config);
		[DllImport("rcapi.dll", EntryPoint="NuanceConfigFreePublicWrapper")]
        public static extern void NuanceConfigFree(IntPtr config);
    }
}
