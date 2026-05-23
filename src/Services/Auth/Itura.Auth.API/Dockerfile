FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Shared/Itura.SharedKernel/Itura.SharedKernel.csproj", "src/Shared/Itura.SharedKernel/"]
COPY ["src/Shared/Itura.Contracts/Itura.Contracts.csproj", "src/Shared/Itura.Contracts/"]
COPY ["src/Services/Auth/Itura.Auth.Domain/Itura.Auth.Domain.csproj", "src/Services/Auth/Itura.Auth.Domain/"]
COPY ["src/Services/Auth/Itura.Auth.Application/Itura.Auth.Application.csproj", "src/Services/Auth/Itura.Auth.Application/"]
COPY ["src/Services/Auth/Itura.Auth.Infrastructure/Itura.Auth.Infrastructure.csproj", "src/Services/Auth/Itura.Auth.Infrastructure/"]
COPY ["src/Services/Auth/Itura.Auth.API/Itura.Auth.API.csproj", "src/Services/Auth/Itura.Auth.API/"]

RUN dotnet restore "src/Services/Auth/Itura.Auth.API/Itura.Auth.API.csproj"

COPY . .

RUN dotnet build "src/Services/Auth/Itura.Auth.API/Itura.Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Services/Auth/Itura.Auth.API/Itura.Auth.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Create non-root user
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Itura.Auth.API.dll"]
