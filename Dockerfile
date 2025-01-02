FROM mcr.microsoft.com/windows/servercore:ltsc2019

# Set the working directory

# Copy application files to the container
COPY . .

# Copy PowerShell configuration script
COPY configuration.ps1 /scripts/configuration.ps1

# Install required Windows features
RUN powershell -Command \
    Install-WindowsFeature -name Web-Server -IncludeManagementTools; \
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
    Get-ChildItem IIS:\AppPools ^| ForEach-Object { Set-ItemProperty IIS:\AppPools\$($_.Name) -Name enable32BitAppOnWin64 -Value true }

WORKDIR /inetpub/wwwroot

# Run additional configuration script
RUN powershell -Command \
    powershell -ExecutionPolicy Bypass -File /scripts/configuration.ps1

# Keep the container running
CMD ["powershell.exe", "-NoLogo", "-Command", "while ($true) { Start-Sleep -Seconds 3600 }"]
