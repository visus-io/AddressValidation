FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:479b9a3f71f169960d909f599f1b0da82af99bbd6b1179216923eb730217db0d

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
