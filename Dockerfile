# Use the official IIS base image
FROM mcr.microsoft.com/windows/servercore:ltsc2019-amd64

# Enable IIS and its features
RUN dism /online /enable-feature /all /featurename:IIS-WebServerRole /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WebServer /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-CommonHttpFeatures /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpErrors /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpRedirect /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ApplicationDevelopment /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-Security /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-RequestFiltering /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-NetFxExtensibility /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-NetFxExtensibility45 /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HealthAndDiagnostics /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpLogging /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-LoggingLibraries /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-RequestMonitor /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpTracing /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-URLAuthorization /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-IPSecurity /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-Performance /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WebServerManagementTools /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ManagementScriptingTools /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-IIS6ManagementCompatibility /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-Metabase /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HostableWebCore /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-StaticContent /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-DefaultDocument /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-DirectoryBrowsing /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WebDAV /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WebSockets /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ApplicationInit /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ASPNET /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ASPNET45 /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ASP /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-CGI /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ISAPIExtensions /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ISAPIFilter /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ServerSideIncludes /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-CustomLogging /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-BasicAuthentication /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpCompressionStatic /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ManagementConsole /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ManagementService /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-FTPServer /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-FTPSvc /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-CertProvider /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WindowsAuthentication /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-DigestAuthentication /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ClientCertificateMappingAuthentication /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-IISCertificateMappingAuthentication /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ODBCLogging /NoRestart

# Expose IIS port
EXPOSE 80

# Copy website content (replace ./site with your website directory)
COPY ./site /inetpub/wwwroot/

# Start IIS
CMD ["cmd", "/c", "start iis"]