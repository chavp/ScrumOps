# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution file
COPY ScrumOps.sln ./

# Copy project files
COPY src/ScrumOps.Api/ScrumOps.Api.csproj src/ScrumOps.Api/
COPY src/ScrumOps.Domain/ScrumOps.Domain.csproj src/ScrumOps.Domain/
COPY src/ScrumOps.Infrastructure/ScrumOps.Infrastructure.csproj src/ScrumOps.Infrastructure/
COPY src/ScrumOps.Shared/ScrumOps.Shared.csproj src/ScrumOps.Shared/

# Restore dependencies
RUN dotnet restore src/ScrumOps.Api/ScrumOps.Api.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/src/ScrumOps.Api
RUN dotnet build ScrumOps.Api.csproj -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish ScrumOps.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy the published application
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose port
EXPOSE 8080

# Set the entry point
ENTRYPOINT ["dotnet", "ScrumOps.Api.dll"]