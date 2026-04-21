# Etapa 1: Construcción (build)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copiar la solución y proyectos para restaurar
COPY ["*.sln", "./"]
COPY ["WalletService.Api/WalletService.Api.csproj", "WalletService.Api/"]
COPY ["WalletService.Application/WalletService.Application.csproj", "WalletService.Application/"]
COPY ["WalletService.Domain/WalletService.Domain.csproj", "WalletService.Domain/"]
COPY ["WalletService.Infrastructure/WalletService.Infrastructure.csproj", "WalletService.Infrastructure/"]

# Restaurar dependencias
RUN dotnet restore "WalletService.Api/WalletService.Api.csproj"

# Copiar el resto del código
COPY . .

# Compilar
WORKDIR "/src/WalletService.Api"
RUN dotnet build "WalletService.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

# Publicar la aplicación
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WalletService.Api.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false


# Etapa 2: final - runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "WalletService.Api.dll"]
