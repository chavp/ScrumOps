using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Domain.TeamManagement.Repositories;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Domain.SprintManagement.Repositories;
using ScrumOps.Infrastructure.Persistence;
using ScrumOps.Infrastructure.Persistence.Repositories;

namespace ScrumOps.Infrastructure;

/// <summary>
/// Dependency injection configuration for the Infrastructure layer.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Add PostgreSQL Database Context
        services.AddDbContext<ScrumOpsDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Host=localhost;Database=scrumops;Username=scrumops;Password=scrumops123";
            options.UseNpgsql(connectionString);
        });

        // Register Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register Entity Framework repositories
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IProductBacklogRepository, ProductBacklogRepository>();
        services.AddScoped<ISprintRepository, SprintRepository>();
        // TODO: Add ISprintEventRepository when properly implemented

        return services;
    }
}