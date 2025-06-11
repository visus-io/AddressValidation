FROM mcr.microsoft.com/dotnet/sdk:8.0-azurelinux3.0@sha256:32c0f0db277220891763553b2f315f146e4553ced6c98aa2e72fdf15cb9c6f3b AS build

WORKDIR /source

COPY . .

RUN set -x \
    && dotnet publish ./demo/src/AddressValidation.Demo/AddressValidation.Demo.csproj \
        --use-current-runtime \
        --self-contained false \
        -c release \
        -o /app

# =====================================================================================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0@sha256:3e427de07aa2b28d64aadb9caa15e3354961da3e5747c3536e5fabbae62c9810

ENV ASPNETCORE_HTTP_PORTS=80

WORKDIR /app
COPY --from=build /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]