# Use the official IIS base image
FROM mcr.microsoft.com/windows/servercore:ltsc2022
# Enable IIS
RUN dism /online /enable-feature /all /featurename:IIS-WebServer /NoRestart
 
# Enable commonly used IIS features
RUN dism /online /enable-feature /all /featurename:IIS-DefaultDocument \
    /featurename:IIS-DirectoryBrowsing \
    /featurename:IIS-HttpErrors \
    /featurename:IIS-StaticContent \
    /featurename:IIS-HttpRedirect \
    /featurename:IIS-HealthAndDiagnostics \
    /featurename:IIS-HttpLogging \
    /featurename:IIS-RequestMonitor \
    /featurename:IIS-ApplicationDevelopment \
    /featurename:IIS-NetFxExtensibility \
    /featurename:IIS-ASPNET \
    /featurename:IIS-ASPNET45 \
    /featurename:IIS-WebSockets \
    /NoRestart
 
# Enable PowerShell (Optional for automation/scripts)
RUN dism /online /enable-feature /all /featurename:MicrosoftWindowsPowerShell /NoRestart
 
# Copy website content to the container (customize path)
COPY . /inetpub/wwwroot/
 
# Expose default IIS port
EXPOSE 80
 
# Start IIS
CMD ["cmd", "/c", "start iis"]