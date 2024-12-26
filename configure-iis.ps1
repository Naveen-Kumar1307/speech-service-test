# Import the IIS WebAdministration module
Import-Module WebAdministration

# Create necessary directories if they don't already exist
if (-not (Test-Path "C:\inetpub\wwwroot\Services")) { 
    New-Item -Path "C:\inetpub\wwwroot\Services" -ItemType Directory 
}

if (-not (Test-Path "C:\inetpub\wwwroot\Services\Recognition")) { 
    New-Item -Path "C:\inetpub\wwwroot\Services\Recognition" -ItemType Directory 
}

if (-not (Test-Path "C:\inetpub\wwwroot\Services\Recognition\Web")) { 
    New-Item -Path "C:\inetpub\wwwroot\Services\Recognition\Web" -ItemType Directory 
}

# Configure the Default Web Site to point to the new physical path
$site = Get-Item "IIS:\Sites\Default Web Site"
$site.physicalPath = "C:\inetpub\wwwroot\Services\Recognition\Web"
$site | Set-Item

Write-Host "IIS configuration completed successfully."


Set-ItemProperty -Path "IIS:\Sites\Default Web Site" -Name physicalPath -Value "C:\inetpub\wwwroot\Services\Recognition\Web"

Write-Host "Updated Default Web Site physicalPath to 'C:\inetpub\wwwroot\Services\Recognition\Web'."
