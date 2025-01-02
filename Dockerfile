FROM mcr.microsoft.com/windows/servercore/iis
WORKDIR /inetpub/wwwroot
COPY . .
COPY configuration.ps1 /scripts/configuration.ps1
RUN powershell -Command \
    Install-WindowsFeature -Name Web-Asp-Net45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-Framework-45-Core,NET-Framework-45-ASPNET -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-WCF-Services45 -IncludeAllSubFeature; \
    Import-Module ServerManager; \
    Add-WindowsFeature Web-Scripting-Tools; \
    Import-Module WebAdministration; \
    Set-ItemProperty IIS:\AppPools\DefaultAppPool -Name enable32BitAppOnWin64 -Value true

RUN powershell -Command \
    powershell -ExecutionPolicy Bypass -File /scripts/configuration.ps1
