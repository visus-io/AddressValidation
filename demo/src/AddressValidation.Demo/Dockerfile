FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:f27f42210a09b63d8e16e3f2753a7f05983f58bfd2eaa233c7c6a42c5b8cc5b4

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
