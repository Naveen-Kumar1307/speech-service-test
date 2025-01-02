FROM mcr.microsoft.com/windows/servercore/iis

# Set the working directory
WORKDIR /inetpub/wwwroot

# Copy application files to the container
COPY . .

# Copy PowerShell configuration script
COPY configuration.ps1 /scripts/configuration.ps1

# Install required Windows features
Install-WindowsFeature -Name Web-Asp-Net -IncludeAllSubFeature; \
    Install-WindowsFeature -Name Web-Asp-Net45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-Framework-Core; \
    Install-WindowsFeature -Name NET-Framework-45-Core,NET-Framework-45-ASPNET -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-WCF-Services45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name Web-Common-Http -IncludeAllSubFeature; \
    Install-WindowsFeature -Name Web-Dyn-Compression; \
    Install-WindowsFeature -Name Web-Default-Doc; \
    Install-WindowsFeature -Name Web-Static-Content; \
    Install-WindowsFeature -Name Web-Request-Monitor; \
    Install-WindowsFeature -Name Web-Dir-Browsing; \
    Install-WindowsFeature -Name Web-Http-Errors; \
    Install-WindowsFeature -Name Web-Http-Redirect; \
    Install-WindowsFeature -Name Web-DAV-Publishing; \
    Install-WindowsFeature -Name Web-Http-Logging; \
    Install-WindowsFeature -Name Web-Custom-Logging; \
    Install-WindowsFeature -Name Web-Log-Libraries; \
    Install-WindowsFeature -Name Web-Stat-Compression; \
    Install-WindowsFeature -Name Web-Filtering; \
    Install-WindowsFeature -Name Web-Basic-Auth; \
    Install-WindowsFeature -Name Web-CertProvider; \
    Install-WindowsFeature -Name Web-Client-Auth; \
    Install-WindowsFeature -Name Web-Digest-Auth; \
    Install-WindowsFeature -Name Web-Cert-Auth; \
    Install-WindowsFeature -Name Web-IP-Security; \
    Install-WindowsFeature -Name Web-Url-Auth; \
    Install-WindowsFeature -Name Web-Windows-Auth; \
    Install-WindowsFeature -Name Web-Net-Ext45; \
    Install-WindowsFeature -Name Web-AppInit; \
    Install-WindowsFeature -Name Web-ASP; \
    Install-WindowsFeature -Name Web-CGI; \
    Install-WindowsFeature -Name Web-ISAPI-Ext; \
    Install-WindowsFeature -Name Web-ISAPI-Filter; \
    Install-WindowsFeature -Name Web-Includes; \
    Install-WindowsFeature -Name Web-WebSockets; \
    Install-WindowsFeature -Name Web-Mgmt-Tools -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-HTTP-Activation; \
    Install-WindowsFeature -Name NET-Non-HTTP-Activ; \
    Install-WindowsFeature -Name NET-WCF-HTTP-Activation45; \
    Install-WindowsFeature -Name NET-WCF-MSMQ-Activation45; \
    Install-WindowsFeature -Name NET-WCF-Pipe-Activation45; \
    Install-WindowsFeature -Name NET-WCF-TCP-Activation45; \
    Install-WindowsFeature -Name NET-WCF-TCP-PortSharing45; \
    Install-WindowsFeature -Name BitLocker; \
    Install-WindowsFeature -Name EnhancedStorage; \
    Install-WindowsFeature -Name Server-Media-Foundation; \
    Install-WindowsFeature -Name MSMQ -IncludeAllSubFeature; \
    Install-WindowsFeature -Name RSAT -IncludeAllSubFeature; \
    Install-WindowsFeature -Name System-DataArchiver; \
    Install-WindowsFeature -Name Windows-Defender; \
    Install-WindowsFeature -Name PowerShellRoot -IncludeAllSubFeature; \
    Install-WindowsFeature -Name WAS -IncludeAllSubFeature; \
    Install-WindowsFeature -Name WoW64-Support; \
    Install-WindowsFeature -Name XPS-Viewer; \
    Import-Module ServerManager; \
    Import-Module WebAdministration; \
    Get-ChildItem IIS:\AppPools | ForEach-Object { Set-ItemProperty IIS:\AppPools\$($_.Name) -Name enable32BitAppOnWin64 -Value true }


# Run additional configuration script
RUN powershell -Command \
    powershell -ExecutionPolicy Bypass -File /scripts/configuration.ps1
