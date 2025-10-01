# Test script for ScrumOps Docker setup
Write-Host "🚀 Testing ScrumOps PostgreSQL Docker Setup" -ForegroundColor Cyan

# Check if Docker is running
Write-Host "`n📋 Checking Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "✅ Docker is available: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not available or not running" -ForegroundColor Red
    exit 1
}

# Validate docker-compose configuration
Write-Host "`n📋 Validating docker-compose configuration..." -ForegroundColor Yellow
try {
    docker-compose config --quiet
    Write-Host "✅ Docker Compose configuration is valid" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker Compose configuration is invalid" -ForegroundColor Red
    exit 1
}

# Check if project builds
Write-Host "`n📋 Building .NET project..." -ForegroundColor Yellow
try {
    $buildResult = dotnet build --no-restore --verbosity quiet
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ .NET project builds successfully" -ForegroundColor Green
    } else {
        Write-Host "❌ .NET project build failed" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "❌ Error building .NET project" -ForegroundColor Red
    exit 1
}

# Display service information
Write-Host "`n📋 Service Information:" -ForegroundColor Yellow
Write-Host "   🌐 API: http://localhost:8080" -ForegroundColor Cyan
Write-Host "   🏥 Health Check: http://localhost:8080/health" -ForegroundColor Cyan
Write-Host "   📖 Swagger: http://localhost:8080/swagger" -ForegroundColor Cyan
Write-Host "   🗄️  pgAdmin: http://localhost:8081 (admin@scrumops.com / admin123)" -ForegroundColor Cyan
Write-Host "   🐘 PostgreSQL: localhost:5432 (scrumops / scrumops123)" -ForegroundColor Cyan

Write-Host "`n🎯 Next Steps:" -ForegroundColor Yellow
Write-Host "   1. Run: docker-compose up -d" -ForegroundColor White
Write-Host "   2. Wait for services to start (check: docker-compose logs -f)" -ForegroundColor White
Write-Host "   3. Test API: curl http://localhost:8080/health" -ForegroundColor White
Write-Host "   4. Access pgAdmin: http://localhost:8081" -ForegroundColor White

Write-Host "`n✅ All checks passed! Ready to run with Docker." -ForegroundColor Green