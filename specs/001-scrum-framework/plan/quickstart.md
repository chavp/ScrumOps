# ScrumOps Development Quickstart Guide

**Target Audience**: Developers setting up local development environment  
**Prerequisites**: .NET 8.0 SDK, Visual Studio 2022 or VS Code  
**Estimated Setup Time**: 15-20 minutes

## Quick Setup (TL;DR)

```bash
# Clone and setup
git clone <repository-url>
cd ScrumOps
dotnet restore

# Database setup
dotnet ef database update --project src/ScrumOps.Api

# Run applications
dotnet run --project src/ScrumOps.Api  # API (port 5000/5001)
dotnet run --project src/ScrumOps.Web  # Blazor UI (port 5002/5003)

# Run tests
dotnet test
```

**Verify Setup**: Navigate to https://localhost:5003 to see Blazor UI, https://localhost:5001/swagger for API docs

## Detailed Setup Instructions

### 1. Prerequisites Installation

#### .NET 8.0 SDK
```bash
# Verify installation
dotnet --version  # Should show 8.0.x

# If not installed, download from: https://dotnet.microsoft.com/download/dotnet/8.0
```

#### IDE Setup
**Visual Studio 2022 (Recommended)**:
- Version 17.8 or later
- ASP.NET and web development workload
- .NET desktop development workload

**VS Code Alternative**:
- C# Dev Kit extension
- C# extension
- REST Client extension (for API testing)

#### Database Tools (Optional)
```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Install SQLite command-line tools (optional)
# Windows: Download from https://sqlite.org/download.html
# macOS: brew install sqlite
# Linux: apt-get install sqlite3
```

### 2. Project Setup

#### Clone Repository
```bash
git clone <repository-url>
cd ScrumOps

# Verify project structure
ls -la src/
# Should see: ScrumOps.Api/, ScrumOps.Domain/, ScrumOps.Web/, ScrumOps.Shared/
```

#### Restore Dependencies
```bash
# Restore all project dependencies
dotnet restore

# Verify restore succeeded
dotnet build
```

### 3. Database Configuration

#### Connection String Setup
**Development**: Uses SQLite database in `App_Data/scrumops.db`

**appsettings.Development.json**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=App_Data/scrumops.db;Cache=Shared"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

#### Database Creation and Migration
```bash
# Create database and apply migrations
cd src/ScrumOps.Api
dotnet ef database update

# Verify database created
ls -la App_Data/
# Should see: scrumops.db
```

#### Seed Development Data (Optional)
```bash
# Run with seed data flag
dotnet run --project src/ScrumOps.Api --seed-data

# This creates:
# - Sample team "Demo Team"
# - Users with all three Scrum roles
# - Sample product backlog items
# - Active sprint with tasks
```

### 4. Running the Application

#### Start API Server
```bash
# Terminal 1: Start API
cd src/ScrumOps.Api
dotnet run

# API will be available at:
# - HTTP: http://localhost:5000
# - HTTPS: https://localhost:5001
# - Swagger UI: https://localhost:5001/swagger
```

#### Start Blazor Web Application
```bash
# Terminal 2: Start Blazor UI
cd src/ScrumOps.Web
dotnet run

# Web app will be available at:
# - HTTP: http://localhost:5002
# - HTTPS: https://localhost:5003
```

#### Alternative: Use Visual Studio
1. Set multiple startup projects:
   - Right-click solution → Properties
   - Select "Multiple startup projects"
   - Set ScrumOps.Api and ScrumOps.Web to "Start"
2. Press F5 to run both projects

### 5. Development Workflow

#### Hot Reload Configuration
Both projects are configured for hot reload:
- **API**: Changes to controllers, services automatically reload
- **Blazor**: Component changes reflect immediately in browser

#### Database Development
```bash
# Add new migration after model changes
dotnet ef migrations add MigrationName --project src/ScrumOps.Api

# Apply migrations
dotnet ef database update --project src/ScrumOps.Api

# Reset database (development only)
dotnet ef database drop --project src/ScrumOps.Api
dotnet ef database update --project src/ScrumOps.Api
```

#### Testing Workflow
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test tests/ScrumOps.Api.Tests/

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Watch mode for TDD
dotnet watch test --project tests/ScrumOps.Api.Tests/
```

### 6. API Development and Testing

#### Using Swagger UI
1. Navigate to https://localhost:5001/swagger
2. Explore all available endpoints
3. Test endpoints directly from the UI
4. View request/response schemas

#### Using REST Client (VS Code)
Create `api-tests.http` file:
```http
### Get all teams
GET https://localhost:5001/api/teams
Accept: application/json

### Create new team
POST https://localhost:5001/api/teams
Content-Type: application/json

{
  "name": "Test Team",
  "description": "API testing team",
  "sprintLengthWeeks": 2
}

### Get team details
GET https://localhost:5001/api/teams/1
Accept: application/json
```

#### Using curl
```bash
# Test API health
curl -k https://localhost:5001/health

# Get teams
curl -k https://localhost:5001/api/teams

# Create team
curl -k -X POST https://localhost:5001/api/teams \
  -H "Content-Type: application/json" \
  -d '{"name":"API Team","sprintLengthWeeks":2}'
```

### 7. Frontend Development

#### Blazor Component Development
- Components in `src/ScrumOps.Web/Components/`
- Pages in `src/ScrumOps.Web/Pages/`
- Shared layouts in `src/ScrumOps.Web/Shared/`

#### CSS and Styling
- Bootstrap 5 included by default
- Custom styles in `wwwroot/css/app.css`
- Component-specific styles using `<style>` blocks

#### JavaScript Interop (if needed)
- Custom JS in `wwwroot/js/app.js`
- Use `IJSRuntime` for interop in components

### 8. Debugging and Troubleshooting

#### Common Issues

**"Database is locked" Error**:
```bash
# Close all connections and restart
dotnet ef database drop --project src/ScrumOps.Api
dotnet ef database update --project src/ScrumOps.Api
```

**Port Already in Use**:
```bash
# Kill processes using ports 5000-5003
# Windows:
netstat -ano | findstr :5001
taskkill /F /PID <process-id>

# macOS/Linux:
lsof -ti:5001 | xargs kill -9
```

**SSL Certificate Issues**:
```bash
# Trust development certificates
dotnet dev-certs https --trust
```

#### Debugging Configuration
**Visual Studio**:
- Set breakpoints in both API and Blazor projects
- Use "Attach to Process" for running applications
- Enable SQL logging in Entity Framework

**VS Code**:
- Use `.vscode/launch.json` for debugging configuration
- Install C# extension for IntelliSense and debugging

#### Logging
- Structured logging with Serilog
- Logs written to console and file
- Different log levels for different environments
- EF Core query logging enabled in development

### 9. Testing the Complete Flow

#### Manual Testing Checklist
1. **API Health**: https://localhost:5001/health returns 200 OK
2. **Swagger UI**: https://localhost:5001/swagger loads documentation
3. **Blazor UI**: https://localhost:5003 loads homepage
4. **Database**: Can create team via API and see in Blazor UI
5. **Hot Reload**: Changes to components reflect immediately

#### Sample Test Scenario
```http
# 1. Create team
POST https://localhost:5001/api/teams
Content-Type: application/json

{
  "name": "Quick Test Team",
  "description": "Testing the quickstart guide",
  "sprintLengthWeeks": 2
}

# 2. Verify team exists
GET https://localhost:5001/api/teams

# 3. Check in Blazor UI at https://localhost:5003/teams
```

### 10. Next Steps

After successful setup:
1. **Read the Architecture Guide**: Understanding the code organization
2. **Review API Contracts**: Familiarize with all available endpoints  
3. **Explore Test Suite**: Understand testing patterns and practices
4. **Constitutional Compliance**: Review code quality and testing standards

#### Development Best Practices
- Follow TDD: Write tests before implementation
- Use meaningful commit messages
- Run tests before pushing changes
- Keep functions small and focused
- Document public APIs

#### Performance Considerations
- Enable SQL query logging to identify N+1 problems
- Use `Include()` carefully to avoid over-fetching
- Implement pagination for large result sets
- Monitor SQLite database size and performance

---

## Troubleshooting Guide

### Database Issues
- **Connection Issues**: Check connection string and file permissions
- **Migration Errors**: Drop database and recreate for development
- **Seed Data Problems**: Clear database and re-run with `--seed-data`

### Build Issues
- **Dependency Conflicts**: Run `dotnet clean` then `dotnet restore`
- **Version Mismatches**: Ensure .NET 8.0 SDK is installed
- **Missing References**: Verify all project references are correct

### Runtime Issues
- **API Not Found**: Verify API is running on correct port
- **CORS Errors**: Check CORS configuration in API startup
- **Authentication Issues**: Review role-based authorization setup

**Need Help?** Check the project README, review existing tests for examples, or consult the architecture documentation.

---
*Quickstart guide follows constitutional principles for development quality and testing*