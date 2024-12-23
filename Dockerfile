FROM mcr.microsoft.com/windows/servercore/iis:latest

WORKDIR /inetpub/wwwroot

COPY . .