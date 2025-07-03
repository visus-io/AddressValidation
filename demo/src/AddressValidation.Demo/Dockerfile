FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:db790153de8350a1809b62aa495364372df4c9b551e9606a1bd857908299e2b9

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
