# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY FPTTrackingSystem.sln .

# Copy project files
COPY FPTTrackingSystem/FPTTrackingSystem.csproj FPTTrackingSystem/FPTTrackingSystem.csproj
COPY Repositories/Repositories.csproj Repositories/Repositories.csproj
COPY Entities/Entities.csproj Entities/Entities.csproj
COPY Dtos/DataTranferObjects.csproj Dtos/DataTranferObjects.csproj

# Restore dependencies
RUN dotnet restore FPTTrackingSystem.sln

# Copy all source files
COPY . .

# Build the application
WORKDIR /src/FPTTrackingSystem
RUN dotnet build -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Copy published files
COPY --from=publish /app/publish .

# Copy production appsettings
COPY appsettings.Production.json appsettings.Production.json

# Create wwwroot directory if needed
RUN mkdir -p wwwroot/uploads && chown -R appuser:appuser /app

# Switch to non-root user
USER appuser

# Expose port 5000
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "FPTTrackingSystem.dll"]

