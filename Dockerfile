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
    New-Item -Path "C:\inetpub\wwwroot\Services" -ItemType Directory; \
    New-Item -Path "C:\inetpub\wwwroot\Services\Recognition" -ItemType Directory; \
    New-Item -Path "C:\inetpub\wwwroot\Services\Recognition\Web" -ItemType Directory; \
    Set-ItemProperty -Path "IIS:\Sites\Default Web Site" -Name physicalPath -Value "C:\inetpub\wwwroot\Services\Recognition\Web"