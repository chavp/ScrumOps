using Microsoft.EntityFrameworkCore;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Infrastructure.Persistence.Configurations;
using DomainTask = ScrumOps.Domain.SprintManagement.Entities.Task;

namespace ScrumOps.Infrastructure.Persistence;

/// <summary>
/// Entity Framework DbContext for the ScrumOps application.
/// Implements Domain Driven Design patterns with proper aggregate boundary separation.
/// </summary>
public class ScrumOpsDbContext : DbContext
{
    public ScrumOpsDbContext(DbContextOptions<ScrumOpsDbContext> options) : base(options)
    {
    }

    // Team Management Bounded Context
    public DbSet<Team> Teams { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    // Product Backlog Bounded Context
    public DbSet<ProductBacklog> ProductBacklogs { get; set; } = null!;
    public DbSet<ProductBacklogItem> ProductBacklogItems { get; set; } = null!;

    // Sprint Management Bounded Context
    public DbSet<Sprint> Sprints { get; set; } = null!;
    public DbSet<SprintBacklogItem> SprintBacklogItems { get; set; } = null!;
    public DbSet<DomainTask> Tasks { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations for all entities
        // This follows the DDD pattern of keeping aggregate configurations separate
        
        // Team Management Bounded Context
        modelBuilder.ApplyConfiguration(new TeamConfiguration());
        modelBuilder.ApplyConfiguration(new UserConfiguration());

        // Product Backlog Bounded Context
        modelBuilder.ApplyConfiguration(new ProductBacklogConfiguration());
        modelBuilder.ApplyConfiguration(new ProductBacklogItemConfiguration());

        // Sprint Management Bounded Context
        modelBuilder.ApplyConfiguration(new SprintConfiguration());
        modelBuilder.ApplyConfiguration(new SprintBacklogItemConfiguration());
        modelBuilder.ApplyConfiguration(new TaskConfiguration());

        // Configure schema separation for bounded contexts
        ConfigureSchemas(modelBuilder);
        
        // Configure global conventions
        ConfigureConventions(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Default configuration for development
            optionsBuilder.UseNpgsql("Host=localhost;Database=scrumops;Username=scrumops;Password=scrumops123");
        }

        // Enable sensitive data logging in development
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.EnableDetailedErrors();
    }

    /// <summary>
    /// Configures schema separation for bounded contexts.
    /// Each bounded context gets its own schema for better organization.
    /// </summary>
    private static void ConfigureSchemas(ModelBuilder modelBuilder)
    {
        // Team Management schema
        modelBuilder.Entity<Team>().ToTable("Teams", "TeamManagement");
        modelBuilder.Entity<User>().ToTable("Users", "TeamManagement");

        // Product Backlog schema
        modelBuilder.Entity<ProductBacklog>().ToTable("ProductBacklogs", "ProductBacklog");
        modelBuilder.Entity<ProductBacklogItem>().ToTable("ProductBacklogItems", "ProductBacklog");

        // Sprint Management schema
        modelBuilder.Entity<Sprint>().ToTable("Sprints", "SprintManagement");
        modelBuilder.Entity<SprintBacklogItem>().ToTable("SprintBacklogItems", "SprintManagement");
        modelBuilder.Entity<DomainTask>().ToTable("Tasks", "SprintManagement");
    }

    /// <summary>
    /// Configures global conventions for all entities.
    /// </summary>
    private static void ConfigureConventions(ModelBuilder modelBuilder)
    {
        // Configure string properties to have a reasonable default max length
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string) && property.GetMaxLength() == null)
                {
                    property.SetMaxLength(500);
                }
            }
        }

        // Configure DateTime properties to be stored as UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(
                        new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
                            v => v.ToUniversalTime(),
                            v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }
    }

    /// <summary>
    /// Override SaveChanges to handle domain events and audit fields.
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields before saving
        UpdateAuditFields();

        // TODO: Publish domain events here when implementing event handling
        // var domainEvents = ExtractDomainEvents();

        var result = await base.SaveChangesAsync(cancellationToken);

        // TODO: Publish domain events after successful save
        // await PublishDomainEvents(domainEvents);

        return result;
    }

    /// <summary>
    /// Updates audit fields like LastModifiedDate before saving changes.
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Update LastModifiedDate if the entity has this property
            if (entry.Entity.GetType().GetProperty("LastModifiedDate") != null)
            {
                entry.Property("LastModifiedDate").CurrentValue = DateTime.UtcNow;
            }

            // Set CreatedDate for new entities
            if (entry.State == EntityState.Added)
            {
                if (entry.Entity.GetType().GetProperty("CreatedDate") != null)
                {
                    entry.Property("CreatedDate").CurrentValue = DateTime.UtcNow;
                }
            }
        }
    }
}