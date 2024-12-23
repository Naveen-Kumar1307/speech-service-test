Deployment of ASR as an Azure Web Role stack on Windows Server 2008 R2 needs the following:

1) .Net Framework 4.0 (use the web installer http://www.microsoft.com/en-us/download/details.aspx?id=17851)
2) Azure libraries in the GAC (use InstallLibs.bat in this folder)
3) Reconfigure ASRPool in IIS to use .Net v4.0
4) Restart the VM (if needed)