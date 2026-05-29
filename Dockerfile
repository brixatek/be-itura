# syntax=docker/dockerfile:1.4

# ── Runtime ───────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs

# ── Build ─────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG SERVICE_DIR
ARG SERVICE_NAME
WORKDIR /src
COPY . .
RUN dotnet restore "src/Services/${SERVICE_DIR}/Itura.${SERVICE_NAME}.API/Itura.${SERVICE_NAME}.API.csproj" \
    --runtime linux-musl-x64
RUN dotnet publish "src/Services/${SERVICE_DIR}/Itura.${SERVICE_NAME}.API/Itura.${SERVICE_NAME}.API.csproj" \
    -c Release -o /app/publish /p:UseAppHost=false \
    --runtime linux-musl-x64 --self-contained false

# ── Final ─────────────────────────────────────────────────────────────────────
FROM base AS final
WORKDIR /app
ARG ASSEMBLY
ENV DOTNET_ASSEMBLY=${ASSEMBLY}
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser
COPY --from=build /app/publish .
HEALTHCHECK --interval=30s --timeout=10s --start-period=20s --retries=3 \
    CMD wget -qO- http://localhost:8080/health || exit 1
ENTRYPOINT ["sh", "-c", "exec dotnet $DOTNET_ASSEMBLY"]
