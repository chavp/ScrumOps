using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using ScrumOps.Application;
using ScrumOps.Infrastructure;
using ScrumOps.Infrastructure.Persistence;
using ScrumOps.Api.Middleware;
using ScrumOps.Api.Extensions;
using ScrumOps.Api.Services;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using System.Diagnostics.Metrics;

// Configure initial Serilog logger for startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/startup-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add comprehensive observability (logging, metrics, tracing)
    builder.AddObservability();

    // Add services to the container.
    builder.Services.AddControllers();

    // Configure Problem Details
    builder.Services.AddProblemDetails(options =>
    {
        // Customize the problem details response
        options.CustomizeProblemDetails = (context) =>
        {
            var problemDetails = context.ProblemDetails;
            
            // Add additional information
            problemDetails.Extensions["machine"] = Environment.MachineName;
            problemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier;
            
            // Add timestamp
            problemDetails.Extensions["timestamp"] = DateTimeOffset.UtcNow;
        };
    });

    // Add Application layer services
    builder.Services.AddApplication();

    // Add Infrastructure layer services
    builder.Services.AddInfrastructure(builder.Configuration);

    // Add observability services
    builder.Services.AddScoped<BusinessMetricsService>();

    // Configure OpenAPI/Swagger
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "ScrumOps API",
            Version = "v1",
            Description = "Scrum Framework Management System API with comprehensive observability",
            Contact = new OpenApiContact
            {
                Name = "ScrumOps Team"
            }
        });

        // Add Problem Details schema to Swagger
        c.MapType<Microsoft.AspNetCore.Mvc.ProblemDetails>(() => new Microsoft.OpenApi.Models.OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["type"] = new() { Type = "string", Format = "uri" },
                ["title"] = new() { Type = "string" },
                ["status"] = new() { Type = "integer", Format = "int32" },
                ["detail"] = new() { Type = "string" },
                ["instance"] = new() { Type = "string", Format = "uri" },
                ["traceId"] = new() { Type = "string" },
                ["timestamp"] = new() { Type = "string", Format = "date-time" }
            }
        });
    });

    // Add CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazorOrigin",
            policy => policy
                .WithOrigins("https://localhost:5003", "http://localhost:5002")
                .AllowAnyHeader()
                .AllowAnyMethod());
    });

    var app = builder.Build();

    // Apply database migrations on startup
    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ScrumOpsDbContext>();
        
        Log.Information("Applying database migrations...");
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }
        Log.Information("Database migrations applied successfully");

        // Initialize business metrics with current counts
        var metricsService = scope.ServiceProvider.GetRequiredService<BusinessMetricsService>();
        await InitializeBusinessMetrics(context, metricsService);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while applying database migrations");
        throw;
    }

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "ScrumOps API v1");
            c.RoutePrefix = "swagger";
        });
    }
    else
    {
        // Use built-in problem details handler in production
        app.UseExceptionHandler();
    }

    // Add observability middleware pipeline
    app.UseObservability();

    // Add custom observability middleware for detailed request tracking
    app.UseMiddleware<ObservabilityMiddleware>();

    // Add global exception handling middleware (after observability)
    app.UseMiddleware<GlobalExceptionMiddleware>();

    app.UseHttpsRedirection();

    app.UseCors("AllowBlazorOrigin");

    // Enable status code pages with Problem Details
    app.UseStatusCodePages();

    app.UseAuthorization();

    app.MapControllers();

    // Add health check endpoint with more detailed information
    app.MapGet("/health", (IServiceProvider serviceProvider) =>
    {
        var environment = serviceProvider.GetRequiredService<IWebHostEnvironment>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        
        return Results.Ok(new 
        { 
            Status = "Healthy",
            Timestamp = DateTimeOffset.UtcNow,
            Environment = environment.EnvironmentName,
            Version = "1.0.0",
            Machine = Environment.MachineName,
            Observability = new
            {
                Logging = configuration.GetValue<bool>("Observability:LoggingEnabled", true),
                Metrics = configuration.GetValue<bool>("Observability:MetricsEnabled", true),
                Tracing = configuration.GetValue<bool>("Observability:TracingEnabled", true)
            }
        });
    });

    // Add readiness check endpoint
    app.MapGet("/health/ready", async (ScrumOpsDbContext context) =>
    {
        try
        {
            // Check database connectivity
            await context.Database.CanConnectAsync();
            return Results.Ok(new { Status = "Ready", Timestamp = DateTimeOffset.UtcNow });
        }
        catch (Exception ex)
        {
            Log.Warning(ex, "Readiness check failed");
            return Results.StatusCode(503);
        }
    });

    // Add liveness check endpoint
    app.MapGet("/health/live", () => 
        Results.Ok(new { Status = "Alive", Timestamp = DateTimeOffset.UtcNow }));

    Log.Information("Starting ScrumOps API with comprehensive observability features");
    Log.Information("Observability endpoints: /metrics (Prometheus), /health, /health/ready, /health/live");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ScrumOps API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

/// <summary>
/// Initialize business metrics with current database counts.
/// </summary>
static async Task InitializeBusinessMetrics(ScrumOpsDbContext context, BusinessMetricsService metricsService)
{
    try
    {
        // Get current counts from database
        var teamsCount = await context.Teams.CountAsync();
        var activeSprintsCount = await context.Sprints
            .Where(s => s.Status == SprintStatus.Active)
            .CountAsync();
        var backlogItemsCount = await context.ProductBacklogItems.CountAsync() + 
                               await context.SprintBacklogItems.CountAsync();

        // Initialize metrics
        metricsService.UpdateActiveTeamsCount(teamsCount);
        metricsService.UpdateActiveSprintsCount(activeSprintsCount);
        metricsService.UpdateTotalBacklogItemsCount(backlogItemsCount);

        Log.Information("Business metrics initialized - Teams: {TeamsCount}, Active Sprints: {SprintsCount}, Backlog Items: {BacklogCount}",
            teamsCount, activeSprintsCount, backlogItemsCount);
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to initialize business metrics");
    }
}

// Make Program class accessible for testing
public partial class Program { }
