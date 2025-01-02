FROM mcr.microsoft.com/windows/servercore/iis

# Set the working directory
WORKDIR /inetpub/wwwroot

# Copy application files to the container
COPY . .

# Copy PowerShell configuration script
COPY configuration.ps1 /scripts/configuration.ps1

# Install required Windows features (aligned with File A)
RUN powershell -Command \
    Install-WindowsFeature -Name Web-Asp-Net45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-Framework-45-Core,NET-Framework-45-ASPNET -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-WCF-Services45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name Web-Common-Http; \
    Install-WindowsFeature -Name Web-Dyn-Compression; \
    Install-WindowsFeature -Name Web-Default-Doc; \
    Install-WindowsFeature -Name Web-Static-Content; \
    Install-WindowsFeature -Name Web-Request-Monitor; \
    Import-Module ServerManager; \
    Add-WindowsFeature Web-Scripting-Tools; \
    Import-Module WebAdministration; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool -Name enable32BitAppOnWin64 -Value true

# Run additional configuration script
RUN powershell -Command \
    powershell -ExecutionPolicy Bypass -File /scripts/configuration.ps1
