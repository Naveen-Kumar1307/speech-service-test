# Import the IIS WebAdministration module
Import-Module WebAdministration

# Create necessary directories if they don't already exist
if (-not (Test-Path "C:\inetpub\wwwroot\Services")) { 
    New-Item -Path "C:\inetpub\wwwroot\Services" -ItemType Directory 
}

if (-not (Test-Path "C:\inetpub\wwwroot\")) { 
    New-Item -Path "C:\inetpub\wwwroot\" -ItemType Directory 
}

if (-not (Test-Path "C:\inetpub\wwwroot\")) { 
    New-Item -Path "C:\inetpub\wwwroot\" -ItemType Directory 
}

# Configure the Default Web Site to point to the new physical path
$site = Get-Item "IIS:\Sites\Default Web Site"
$site.physicalPath = "C:\inetpub\wwwroot\"
$site | Set-Item

Write-Host "IIS configuration completed successfully."

Set-DnsClientServerAddress `    -InterfaceIndex 18 `    -ServerAddresses ("8.8.8.8")

Set-ItemProperty -Path "IIS:\Sites\Default Web Site" -Name physicalPath -Value "C:\inetpub\wwwroot\"

Write-Host "Updated Default Web Site physicalPath to 'C:\inetpub\wwwroot\'."
