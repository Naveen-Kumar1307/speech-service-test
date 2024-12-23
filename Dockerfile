# Use the official IIS base image
FROM mcr.microsoft.com/windows/servercore:ltsc2019

# Enable IIS and required features in smaller chunks for better debugging
RUN dism /online /enable-feature /all /featurename:IIS-WebServerRole /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WebServer /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-CommonHttpFeatures /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpErrors /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpRedirect /NoRestart

RUN dism /online /enable-feature /all /featurename:IIS-ApplicationDevelopment /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ASPNET45 /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ISAPIExtensions /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-ISAPIFilter /NoRestart

RUN dism /online /enable-feature /all /featurename:IIS-Security /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-WindowsAuthentication /NoRestart \
    && dism /online /enable-feature /all /featurename:IIS-HttpCompressionStatic /NoRestart

# Copy website content
COPY . /inetpub/wwwroot/

# Expose HTTP port
EXPOSE 80

# Default CMD to run IIS
#CMD ["cmd", "/c", "iisreset", "/start"]