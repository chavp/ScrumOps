using MediatR;
using Microsoft.OpenApi.Models;
using Serilog;
using ScrumOps.Application;
using ScrumOps.Infrastructure;

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

// TODO: Apply database migrations when EF configurations are fixed
/*
try
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ScrumOpsDbContext>();
    
    Log.Information("Applying database migrations...");
    await context.Database.MigrateAsync();
    Log.Information("Database migrations applied successfully");
}
catch (Exception ex)
{
    Log.Error(ex, "An error occurred while applying database migrations");
    throw;
}
*/

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

app.UseHttpsRedirection();

app.UseCors("AllowBlazorOrigin");

app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

try
{
    Log.Information("Starting ScrumOps API with PostgreSQL support");
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
