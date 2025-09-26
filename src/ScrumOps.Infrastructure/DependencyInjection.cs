using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Domain.TeamManagement.Repositories;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Infrastructure.Persistence;

namespace ScrumOps.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // For demo purposes, use in-memory implementations
        // TODO: Replace with EF Core implementations when EF mappings are fixed
        
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

        // Register repositories
        services.AddScoped<ITeamRepository, InMemoryTeamRepository>();
        services.AddScoped<IProductBacklogRepository, InMemoryProductBacklogRepository>();

        return services;
    }
}