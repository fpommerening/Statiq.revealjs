FROM  mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY /out /app
ENTRYPOINT ["/app/Statiq.RevealJS"]
