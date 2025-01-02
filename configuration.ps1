$InterfaceIndex = (Get-DnsClientServerAddress | Where-Object {$_.ServerAddresses -eq "172.20.0.10"} | Select-Object -First 1).InterfaceIndex

if ($InterfaceIndex) {
    Set-DnsClientServerAddress -InterfaceIndex $InterfaceIndex -ServerAddresses ("8.8.8.8")
    Write-Host "DNS server for InterfaceIndex $InterfaceIndex has been set to 8.8.8.8."
} else {
    Write-Host "No suitable interface found for DNS configuration."
}