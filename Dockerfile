# -------------------------------------------------------------
# STAGE 1: BUILD (Compiles application using full .NET 10 SDK)
# -------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src

# 1. Copy project file and restore NuGet packages (Cached layer)
COPY ["Ecommerce.csproj", "./"]
RUN dotnet restore "Ecommerce.csproj"

# 2. Copy source code and publish optimized release binaries
COPY . .
RUN dotnet publish "Ecommerce.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------------------------------------------------------------
# STAGE 2: FINAL RUNTIME (Ultra-lightweight, secure Alpine image)
# -------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS final
WORKDIR /app

# Run as non-root user for enterprise container security
USER $APP_UID

# Copy compiled binaries from the build stage
COPY --from=build /app/publish .

# Expose standard container port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Ecommerce.dll"]
