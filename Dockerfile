# ============================================
# Dockerfile for FPTTrackingSystem
# Multi-stage build for .NET 8.0 Application
# ============================================

# Stage 1: Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file and project files first for better layer caching
COPY ["FPTTrackingSystem.sln", "."]
COPY ["FPTTrackingSystem/FPTTrackingSystem.csproj", "FPTTrackingSystem/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Dtos/DataTranferObjects.csproj", "Dtos/"]

# Restore dependencies
RUN dotnet restore "FPTTrackingSystem.sln"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/FPTTrackingSystem"
RUN dotnet build "FPTTrackingSystem.csproj" -c Release -o /app/build

# Stage 2: Publish stage
FROM build AS publish
RUN dotnet publish "FPTTrackingSystem.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create a non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published files from build stage
COPY --from=publish /app/publish .

# Expose port (default ASP.NET Core port)
EXPOSE 8080
EXPOSE 8081

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER=true

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Entry point
ENTRYPOINT ["dotnet", "FPTTrackingSystem.dll"]
