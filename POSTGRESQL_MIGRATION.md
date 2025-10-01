# PostgreSQL Migration Summary

This document outlines the changes made to migrate ScrumOps from SQLite to PostgreSQL and add Docker support.

## Changes Made

### 1. NuGet Package Updates
- **Removed**: `Microsoft.EntityFrameworkCore.Sqlite` (Version 9.0.9)
- **Added**: `Npgsql.EntityFrameworkCore.PostgreSQL` (Version 8.0.10)

### 2. Connection String Configuration
Updated `appsettings.json` and `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=scrumops;Username=scrumops;Password=scrumops123"
  }
}
```

### 3. Database Context Updates
- Updated `ScrumOpsDbContext.cs` to use PostgreSQL instead of SQLite:
  ```csharp
  optionsBuilder.UseNpgsql("Host=localhost;Database=scrumops;Username=scrumops;Password=scrumops123");
  ```

### 4. Docker Configuration
Created complete Docker setup:

#### `docker-compose.yml`
- PostgreSQL 16 Alpine container
- ScrumOps API container
- pgAdmin container for database management
- Proper networking and health checks

#### `src/ScrumOps.Api/Dockerfile`
- Multi-stage build with .NET 8 SDK and runtime
- Optimized for containerized deployment
- Health check endpoint support

#### `.dockerignore`
- Excludes unnecessary files from Docker build context

#### `scripts/init-db.sql`
- Database initialization script
- Creates schemas for bounded contexts
- Sets up proper permissions

### 5. Infrastructure Layer Updates
- Temporarily maintained in-memory repositories until EF configurations are fixed
- Added PostgreSQL connection string configuration
- Prepared for future Entity Framework integration

### 6. Documentation Updates
Updated `README.md` with:
- Docker prerequisites
- Docker Compose setup instructions
- PostgreSQL as the primary database
- Local development options

## Current Status

### ✅ Completed
- [x] PostgreSQL NuGet package integration
- [x] Connection string configuration
- [x] Docker Compose setup with PostgreSQL
- [x] Docker containerization of API
- [x] Database initialization scripts
- [x] Updated documentation
- [x] Build verification

### 🔄 In Progress / Future Work
- [ ] Fix Entity Framework configurations for PostgreSQL
- [ ] Enable automatic database migrations
- [ ] Implement proper EF Core repositories
- [ ] Add database seeding scripts
- [ ] Add integration tests with PostgreSQL

## How to Run

### Option 1: Docker (Recommended)
```bash
# Start all services (PostgreSQL + API + pgAdmin)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### Option 2: Local Development
```bash
# Start PostgreSQL manually or use existing instance
# Update connection string in appsettings.Development.json
dotnet run --project src/ScrumOps.Api
```

## Service URLs
- **API**: http://localhost:8080
- **API Health Check**: http://localhost:8080/health
- **API Swagger**: http://localhost:8080/swagger
- **pgAdmin**: http://localhost:8081 (admin@scrumops.com / admin123)
- **PostgreSQL**: localhost:5432 (scrumops / scrumops123)

## Database Schema
The application uses schema separation for bounded contexts:
- `TeamManagement` - Teams and users
- `ProductBacklog` - Product backlogs and items  
- `SprintManagement` - Sprints, tasks, and sprint items

## Notes
- Currently using in-memory repositories while EF configurations are being fixed
- PostgreSQL connection is configured but not actively used until EF issues are resolved
- All Docker services include proper health checks and restart policies
- Database data is persisted in Docker volumes