# Use the official IIS base image
FROM mcr.microsoft.com/windows/servercore/iis:latest

# Enable IIS features selectively
RUN dism /online /get-features /format:table && \
    dism /online /enable-feature /all /featurename:IIS-HttpRedirect /NoRestart || echo "Feature already enabled or unavailable"

# Copy website content to the container (customize path)
COPY . /inetpub/wwwroot/

# Expose default IIS port
EXPOSE 80

# Start IIS
CMD ["cmd", "/c", "iisreset && ping -t localhost"]
