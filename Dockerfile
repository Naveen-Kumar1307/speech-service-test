FROM mcr.microsoft.com/windows/servercore/iis

WORKDIR /inetpub/wwwroot

COPY . .

RUN powershell -Command \
    Install-WindowsFeature -Name Web-Asp-Net45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-Framework-45-Core,NET-Framework-45-ASPNET -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-WCF-Services45 -IncludeAllSubFeature
