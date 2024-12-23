$storage_account_name = 'geprodasr'
$container_name = 'asr'
$source_path = 'SPEECH.zip'
$destination_path = 'E:\sitesroot\0'
$download_path = 'E:\downloads'
$logFilePath = "E:\sitesroot\0\log.txt"
$sas_token = 'XXX_BLOB_SAS_TOKEN_XXX'

# Function to write logs
Function Write-Log {
    param (
        [string]$message
    )
    $timestamp = (Get-Date).ToString("yyyy-MM-dd HH:mm:ss")
    $logMessage = "$timestamp - $message"
    Add-Content -Path $logFilePath -Value $logMessage
}

# Start logging
Write-Log "Script execution started."

# Stop the Web Service
Write-Log "Stopping web service."
iisreset /stop
Write-Log "Web service stopped."

# Prepare Download Folder
Write-Log "Preparing download folder."
if (-not (Test-Path $download_path)) {
    New-Item -Path $download_path -ItemType Directory -Force
    Write-Log "Download folder created."
}

# Clean Document Root
Write-Log "Cleaning document root."
if (Test-Path $destination_path) {
    Remove-Item -Recurse -Force "$destination_path\*" -ErrorAction Stop
    Write-Log "Document root cleaned."
} else {
    Write-Log "Document root does not exist. Creating directory."
    New-Item -Path $destination_path -ItemType Directory -Force
}

# Download the ZIP file
Write-Log "Starting download of document root."
try {
    # Add the SAS token to the URL
    $uri = "https://" + $storage_account_name + ".blob.core.windows.net/" + $container_name + "/" + $source_path + "?" + $sas_token
    $outFile = "$download_path\$source_path"

    Invoke-WebRequest -Uri $uri -OutFile $outFile -ErrorAction Stop
    Write-Log "Download completed successfully."

    # Extract the ZIP file to document root
    Write-Log "Expanding archive."
    Expand-Archive -Path $outFile -DestinationPath $destination_path -Force
    Write-Log "Archive expanded successfully."
} catch {
    Write-Log "Error during download or extraction: $_"
}

# Start the Web Service
Write-Log "Starting web service."
iisreset
Write-Log "Web service started."

# End logging
Write-Log "Script execution completed."
