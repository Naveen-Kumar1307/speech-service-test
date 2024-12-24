# Use Windows Server 2022 (LTSC 2022) as the base image
FROM --platform=windows/amd64 mcr.microsoft.com/windows/server:ltsc2022

# Set the working directory to the IIS root directory
WORKDIR /inetpub/wwwroot

# Copy the local files into the container
COPY . .

# Enable IIS and other required features for the application
RUN powershell -Command \
    # Install IIS Web Server Role and associated features
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-LoggingLibraries -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestMonitor -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpTracing -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-URLAuthorization -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-IPSecurity -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementService -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-FTPServer -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-FTPSvc -All -NoRestart -Source D:\sources\sxs; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-FTPExtensibility -All -NoRestart -Source D:\sources\sxs; \
    # Enable Hyper-V and Containers (if required)
    #Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All -NoRestart -Source D:\sources\sxs; \
    #Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Tools-All -All -NoRestart -Source D:\sources\sxs; \
   # Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Management-PowerShell -All -NoRestart -Source D:\sources\sxs; \
    #Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Services -All -NoRestart -Source D:\sources\sxs; \
    #Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Management-Clients -All -NoRestart -Source D:\sources\sxs; \
    #Enable-WindowsOptionalFeature -Online -FeatureName Containers -All -NoRestart -Source D:\sources\sxs

# Expose port 80 for web traffic
#EXPOSE 80

# Start IIS service and keep the container running
CMD ["powershell", "-NoProfile", "-Command", "Start-Service -Name W3SVC; while ($true) {Start-Sleep -Seconds 3600}"]
