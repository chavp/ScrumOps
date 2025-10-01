using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using ScrumOps.Application;
using ScrumOps.Infrastructure;
using ScrumOps.Infrastructure.Persistence;
using ScrumOps.Api.Middleware;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/scrumops-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.Host.UseSerilog();

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

// Configure OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ScrumOps API",
        Version = "v1",
        Description = "Scrum Framework Management System API",
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

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowBlazorOrigin");

// Enable status code pages with Problem Details
app.UseStatusCodePages();

app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

try
{
    Log.Information("Starting ScrumOps API with PostgreSQL and Entity Framework Core");
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

// Make Program class accessible for testing
public partial class Program { }
