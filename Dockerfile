# Use the official IIS base image
FROM mcr.microsoft.com/windows/servercore/iis:latest

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

# Copy website content to the container (customize path)
COPY . /inetpub/wwwroot/

# Expose default IIS port
EXPOSE 80

# Start IIS
CMD ["cmd", "/c", "iisreset && ping -t localhost"]
