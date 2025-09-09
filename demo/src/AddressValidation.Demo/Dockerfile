FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:e396ddb631cb148b3db17b8a464e2a306f4e802381c14e779362fb44aeb3cf5c

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
