using Microsoft.EntityFrameworkCore;
using ScrumOps.Infrastructure.Persistence;

namespace ScrumOps.Infrastructure.Tests;

/// <summary>
/// Base class for infrastructure tests providing common setup and utilities.
/// </summary>
public abstract class TestBase : IDisposable
{
    protected ScrumOpsDbContext Context { get; private set; }
    private bool _disposed = false;

    protected TestBase()
    {
        Context = CreateInMemoryContext();
    }

    /// <summary>
    /// Creates an in-memory Entity Framework context for testing.
    /// </summary>
    protected static ScrumOpsDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ScrumOpsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var context = new ScrumOpsDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    /// <summary>
    /// Creates a fresh in-memory context (useful for testing across contexts).
    /// </summary>
    protected ScrumOpsDbContext CreateFreshContext()
    {
        return CreateInMemoryContext();
    }

    /// <summary>
    /// Saves changes and detaches all entities to simulate a fresh context.
    /// </summary>
    protected async Task SaveAndDetachAsync()
    {
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            Context?.Dispose();
            _disposed = true;
        }
    }
}