FROM  mcr.microsoft.com/dotnet/runtime:10.0
COPY /out /app
COPY ./entrypoint.sh /entrypoint.sh
ENTRYPOINT ["/entrypoint.sh"]
