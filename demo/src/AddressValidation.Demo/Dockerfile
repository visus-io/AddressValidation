FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:8618360dba69c311da6a13a1ccbd8348df7f056a1352e5e1979ebea486ebbc22

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
