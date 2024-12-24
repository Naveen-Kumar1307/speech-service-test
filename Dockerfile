# Use the official Windows Server Core image with IIS
FROM mcr.microsoft.com/windows/servercore/iis

# Set the working directory
WORKDIR /inetpub/wwwroot

# Copy the local files into the container
COPY . .

# Enable required Windows features using DISM
RUN dism /online /enable-feature /featurename:IIS-WebServerRole /all /norestart && \
    dism /online /enable-feature /featurename:IIS-CommonHttpFeatures /all /norestart && \
    dism /online /enable-feature /featurename:IIS-HttpErrors /all /norestart && \
    dism /online /enable-feature /featurename:IIS-HttpRedirect /all /norestart && \
    dism /online /enable-feature /featurename:IIS-ApplicationDevelopment /all /norestart && \
    dism /online /enable-feature /featurename:IIS-Security /all /norestart && \
    dism /online /enable-feature /featurename:IIS-RequestFiltering /all /norestart && \
    dism /online /enable-feature /featurename:IIS-HealthAndDiagnostics /all /norestart && \
    dism /online /enable-feature /featurename:IIS-HttpLogging /all /norestart && \
    dism /online /enable-feature /featurename:IIS-LoggingLibraries /all /norestart && \
    dism /online /enable-feature /featurename:IIS-RequestMonitor /all /norestart && \
    dism /online /enable-feature /featurename:IIS-HttpTracing /all /norestart && \
    dism /online /enable-feature /featurename:IIS-URLAuthorization /all /norestart && \
    dism /online /enable-feature /featurename:IIS-IPSecurity /all /norestart && \
    dism /online /enable-feature /featurename:IIS-Performance /all /norestart && \
    dism /online /enable-feature /featurename:IIS-ManagementConsole /all /norestart && \
    dism /online /enable-feature /featurename:IIS-ManagementService /all /norestart && \
    dism /online /enable-feature /featurename:IIS-FTPServer /all /norestart && \
    dism /online /enable-feature /featurename:IIS-FTPSvc /all /norestart && \
    dism /online /enable-feature /featurename:IIS-FTPExtensibility /all /norestart && \
    dism /online /enable-feature /featurename:Containers /all /norestart && \
    dism /online /enable-feature /featurename:Microsoft-Hyper-V /all /norestart && \
    dism /online /enable-feature /featurename:Microsoft-Hyper-V-Tools-All /all /norestart && \
    dism /online /enable-feature /featurename:Microsoft-Hyper-V-Management-PowerShell /all /norestart && \
    dism /online /enable-feature /featurename:Microsoft-Hyper-V-Services /all /norestart && \
    dism /online /enable-feature /featurename:Microsoft-Hyper-V-Management-Clients /all /norestart && \
    dism /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart

# Set the port to run the container on
EXPOSE 80

# Start IIS
CMD ["powershell", "-NoProfile", "-Command", "Start-Service -Name W3SVC; while ($true) {Start-Sleep -Seconds 3600}"]
