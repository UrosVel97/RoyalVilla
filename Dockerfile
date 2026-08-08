# syntax=docker/dockerfile:1

ARG DOTNET_VERSION=10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION}-noble AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["RoyalVilla.slnx", "./"]
COPY ["RoyalVilla_API/RoyalVilla_API.csproj", "RoyalVilla_API/"]
COPY ["RoyalVillaWeb/RoyalVillaWeb.csproj", "RoyalVillaWeb/"]
COPY ["RoyalVIlla.DTO/RoyalVIlla.DTO.csproj", "RoyalVIlla.DTO/"]
RUN dotnet restore "RoyalVilla.slnx"

COPY . .
RUN dotnet build "RoyalVilla.slnx" -c "$BUILD_CONFIGURATION" --no-restore

FROM build AS publish-api
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RoyalVilla_API/RoyalVilla_API.csproj" \
    -c "$BUILD_CONFIGURATION" \
    --no-build \
    --no-restore \
    -o /app/publish/api \
    /p:UseAppHost=false

FROM build AS publish-web
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RoyalVillaWeb/RoyalVillaWeb.csproj" \
    -c "$BUILD_CONFIGURATION" \
    --no-build \
    --no-restore \
    -o /app/publish/web \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}-noble AS runtime
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/* \
    && mkdir -p /home/app/.aspnet/DataProtection-Keys \
    && chown -R app:app /home/app/.aspnet

WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=true
EXPOSE 8080
USER $APP_UID
HEALTHCHECK --interval=15s --timeout=5s --start-period=15s --retries=5 \
    CMD curl --fail --silent --show-error http://127.0.0.1:8080/health || exit 1

FROM runtime AS api
COPY --from=publish-api /app/publish/api .
ENTRYPOINT ["dotnet", "RoyalVilla_API.dll"]

FROM runtime AS web
COPY --from=publish-web /app/publish/web .
ENTRYPOINT ["dotnet", "RoyalVillaWeb.dll"]