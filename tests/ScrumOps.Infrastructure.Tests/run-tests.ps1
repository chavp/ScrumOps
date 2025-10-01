# Infrastructure Tests Runner Script
param(
    [string]$TestFilter = "",
    [switch]$SkipIntegration = $false,
    [switch]$Coverage = $false
)

Write-Host "🧪 Running ScrumOps Infrastructure Tests" -ForegroundColor Cyan

# Check if Docker is running (needed for integration tests)
if (-not $SkipIntegration) {
    Write-Host "`n📋 Checking Docker for integration tests..." -ForegroundColor Yellow
    try {
        docker version | Out-Null
        Write-Host "✅ Docker is running" -ForegroundColor Green
    } catch {
        Write-Host "⚠️  Docker not available - skipping integration tests" -ForegroundColor Yellow
        $SkipIntegration = $true
    }
}

# Build test arguments
$testArgs = @("test")

if ($TestFilter) {
    $testArgs += @("--filter", $TestFilter)
    Write-Host "🔍 Test Filter: $TestFilter" -ForegroundColor Cyan
}

if ($SkipIntegration) {
    $testArgs += @("--filter", "Category!=Integration")
    Write-Host "⏭️  Skipping integration tests" -ForegroundColor Yellow
}

if ($Coverage) {
    $testArgs += @("--collect:XPlat Code Coverage")
    Write-Host "📊 Code coverage enabled" -ForegroundColor Cyan
}

$testArgs += @(
    "--logger", "console;verbosity=normal",
    "--configuration", "Debug",
    "--no-build"
)

Write-Host "`n🏗️  Building test project..." -ForegroundColor Yellow
try {
    dotnet build --configuration Debug --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Build failed" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Build successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Build error: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n🧪 Running tests..." -ForegroundColor Yellow
Write-Host "Command: dotnet $($testArgs -join ' ')" -ForegroundColor DarkGray

try {
    $testStart = Get-Date
    & dotnet @testArgs
    $testEnd = Get-Date
    $duration = $testEnd - $testStart
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n✅ All tests passed!" -ForegroundColor Green
        Write-Host "⏱️  Duration: $($duration.TotalSeconds.ToString('F2'))s" -ForegroundColor Cyan
        
        if ($Coverage) {
            Write-Host "`n📊 Coverage report generated in TestResults folder" -ForegroundColor Cyan
        }
    } else {
        Write-Host "`n❌ Some tests failed" -ForegroundColor Red
        exit 1
    }
} catch {
    Write-Host "`n❌ Test execution error: $_" -ForegroundColor Red
    exit 1
}

Write-Host "`n🎯 Test Categories:" -ForegroundColor Yellow
Write-Host "   • Unit Tests: Entity configurations, repositories, builders" -ForegroundColor White
Write-Host "   • Integration Tests: Real PostgreSQL database with Testcontainers" -ForegroundColor White
Write-Host "   • Performance Tests: Index and query performance validation" -ForegroundColor White

Write-Host "`n📋 Usage Examples:" -ForegroundColor Yellow
Write-Host "   .\run-tests.ps1                           # Run all tests" -ForegroundColor White
Write-Host "   .\run-tests.ps1 -SkipIntegration          # Skip integration tests" -ForegroundColor White
Write-Host "   .\run-tests.ps1 -TestFilter ""Repository""  # Run only repository tests" -ForegroundColor White
Write-Host "   .\run-tests.ps1 -Coverage                 # Run with code coverage" -ForegroundColor White