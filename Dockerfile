FROM  mcr.microsoft.com/dotnet/runtime:6.0
COPY /out /app
COPY ./entrypoint.sh /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
