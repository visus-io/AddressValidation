FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra@sha256:1ec4c9605a8fc97d3c2045d46557e51d9ca54ba4f91339205be2e44bfb184732

WORKDIR /app

COPY --chown=$APP_UID:$APP_UID .nuke/temp/AddressValidation.Demo/ .

ENV ASPNETCORE_HTTP_PORTS=3000
EXPOSE 3000

USER $APP_UID

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]
