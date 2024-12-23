# Use the official Windows Server Core image with IIS
FROM mcr.microsoft.com/windows/servercore:ltsc2019

# Enable IIS and required features in smaller chunks for better debugging
RUN dism /online /enable-feature /all /featurename:IIS-WebServerRole /NoRestart > C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-WebServer /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-CommonHttpFeatures /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-HttpErrors /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-HttpRedirect /NoRestart >> C:\install-log.txt 2>&1

RUN dism /online /enable-feature /all /featurename:IIS-ApplicationDevelopment /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-ASPNET45 /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-ISAPIExtensions /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-ISAPIFilter /NoRestart >> C:\install-log.txt 2>&1

RUN dism /online /enable-feature /all /featurename:IIS-Security /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-WindowsAuthentication /NoRestart >> C:\install-log.txt 2>&1 \
    && dism /online /enable-feature /all /featurename:IIS-HttpCompressionStatic /NoRestart >> C:\install-log.txt 2>&1

# Start IIS service to ensure it's running
RUN powershell -Command "Start-Service W3SVC"

# Copy website content
COPY . /inetpub/wwwroot/

# Keep the container running indefinitely
CMD ["powershell", "-Command", "Start-Service W3SVC; while ($true) {Start-Sleep -Seconds 3600}"]
