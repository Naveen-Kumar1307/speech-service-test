# Use the official Windows Server Core image with IIS
FROM mcr.microsoft.com/windows/servercore/iis

# Set the working directory to IIS root
WORKDIR /inetpub/wwwroot

# Copy the local files into the container
COPY . .

# Enable IIS-related features using PowerShell and Enable-WindowsOptionalFeature
RUN powershell -Command \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServerRole -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-CommonHttpFeatures -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpErrors -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpRedirect -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ApplicationDevelopment -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Security -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestFiltering -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HealthAndDiagnostics -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpLogging -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-LoggingLibraries -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-RequestMonitor -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-HttpTracing -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-URLAuthorization -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-IPSecurity -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-Performance -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementConsole -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-ManagementService -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-FTPServer -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-FTPSvc -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName IIS-FTPExtensibility -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Containers -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Tools-All -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Management-PowerShell -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Services -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V-Management-Clients -All -NoRestart; \
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Windows-Subsystem-Linux -All -NoRestart

# Expose the default HTTP port
EXPOSE 80

# Start IIS service
CMD ["powershell", "-NoProfile", "-Command", "Start-Service -Name W3SVC; while ($true) {Start-Sleep -Seconds 3600}"]
