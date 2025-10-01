using Microsoft.Extensions.DependencyInjection;
using ScrumOps.Application.Services.ProductBacklog;
using ScrumOps.Application.Services.TeamManagement;
using ScrumOps.Application.Services.SprintManagement;
using ScrumOps.Application.Metrics.Services;

namespace ScrumOps.Application;

/// <summary>
/// Dependency injection configuration for the Application layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register application services
        services.AddScoped<IProductBacklogService, ProductBacklogService>();
        services.AddScoped<ITeamManagementService, TeamManagementService>();
        services.AddScoped<ISprintManagementService, SprintManagementService>();
        
        // Register metrics services
        services.AddScoped<IMetricsService, MetricsService>();

        return services;
    }
}