FROM mcr.microsoft.com/dotnet/sdk:9.0-azurelinux3.0@sha256:132f47a01fdedb94fd6b6bf5bfd5aa679abc09d5f1c39330ccbb4b772cec6167 AS build

WORKDIR /source

COPY . .

RUN set -x \
    && dotnet publish ./demo/src/AddressValidation.Demo/AddressValidation.Demo.csproj \
        --use-current-runtime \
        --self-contained false \
        -c release \
        -o /app

# =====================================================================================================================

FROM mcr.microsoft.com/dotnet/aspnet:9.0-azurelinux3.0@sha256:9f58f7f4f324e3e13c8d4d06adf85c3e78815fbbc410cf9f45c6d714b005b1c6

ENV ASPNETCORE_HTTP_PORTS=80

WORKDIR /app
COPY --from=build /app .

EXPOSE 80

ENTRYPOINT ["dotnet", "AddressValidation.Demo.dll"]