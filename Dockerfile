FROM mcr.microsoft.com/windows/servercore/iis

# Set the working directory
WORKDIR /inetpub/wwwroot

# Copy application files to the container
COPY . .

# Install required Windows features
RUN powershell -Command \
    Install-WindowsFeature -Name Web-Asp-Net45 -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-Framework-45-Core,NET-Framework-45-ASPNET -IncludeAllSubFeature; \
    Install-WindowsFeature -Name NET-WCF-Services45 -IncludeAllSubFeature

# Map IIS physical path
RUN powershell -Command \
    Import-Module WebAdministration; \
    if (-not (Test-Path "C:\inetpub\wwwroot\Services")) { New-Item -Path "C:\inetpub\wwwroot\Services" -ItemType Directory }; \
    if (-not (Test-Path "C:\inetpub\wwwroot\Services\Recognition")) { New-Item -Path "C:\inetpub\wwwroot\Services\Recognition" -ItemType Directory }; \
    if (-not (Test-Path "C:\inetpub\wwwroot\Services\Recognition\Web")) { New-Item -Path "C:\inetpub\wwwroot\Services\Recognition\Web" -ItemType Directory }; \
    $site = Get-Item "IIS:\Sites\Default Web Site"; \
    $site.physicalPath = "C:\inetpub\wwwroot\Services\Recognition\Web"; \
    $site | Set-Item