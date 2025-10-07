FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra@sha256:83795c4b066ec5f0610d8c3fdd977b65e7536c9bbade4f594e80e5fa24fb533a

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
