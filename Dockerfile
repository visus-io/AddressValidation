FROM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0@sha256:a5a4fbba750e1ea13c419236293651d347e691c9a03fb35a4231c853abd77ae7 AS build

WORKDIR /source

COPY . .

RUN set -x \
    && dotnet publish ./demo/src/AddressValidation.Demo/AddressValidation.Demo.csproj \
        --use-current-runtime \
        --self-contained false \
        -c release \
        -o /app

# =====================================================================================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0@sha256:338f596560cea519f6dc51429a48d1acb71b9cb16d08cd203b8ce67b418ee4a4

ENV ASPNETCORE_HTTP_PORTS=80

WORKDIR /app
COPY --from=build /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]