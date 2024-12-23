%windir%\system32\inetsrv\appcmd set config -section:applicationPools -applicationPoolDefaults.enable32BitAppOnWin64:true 
rem %windir%\system32\inetsrv\appcmd set config -section:applicationPools -applicationPoolDefaults.processModel.maxProcesses:2

mkdir C:\SiteLogs
icacls C:\SiteLogs /grant "NETWORK SERVICE":(OI)(CI)F

%windir%\Microsoft.Net\Framework\v2.0.50727\aspnet_regiis -pa "NetFrameworkConfigurationKey" "NT Authority\Network Service"
%windir%\Microsoft.Net\Framework\v2.0.50727\aspnet_regiis -pef "connectionStrings" %CD:~0,2%\approot
%windir%\Microsoft.Net\Framework\v2.0.50727\aspnet_regiis -pef "connectionStrings" %CD:~0,2%\sitesroot\0

:: Install New Relic client  - Requires restart
msiexec /i %CD:~0,2%\sitesroot\0\bin\Installers\NewRelicServerMonitor_x64_3.1.17.0.msi /L*v %CD:~0,2%\sitesroot\0\bin\Installers\install_server_mon.log /qn NR_LICENSE_KEY=ded53620f3c7e62598dfaddc6553c4c2589872b0
