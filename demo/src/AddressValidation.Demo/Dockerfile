FROM mcr.microsoft.com/dotnet/aspnet:9.0-noble-chiseled-extra@sha256:0f2b33e9ecff6f06c7c499b3bb088333b7a44dbc34e4195060b80a41d439d63e

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
