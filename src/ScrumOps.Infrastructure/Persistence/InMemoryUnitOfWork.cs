using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Application.Common.Interfaces;

namespace ScrumOps.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of IUnitOfWork for demonstration purposes.
/// </summary>
public class InMemoryUnitOfWork : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // In-memory repositories handle persistence automatically
        return await Task.FromResult(1);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No-op for in-memory implementation
        await Task.CompletedTask;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No-op for in-memory implementation
        await Task.CompletedTask;
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        // No-op for in-memory implementation
        await Task.CompletedTask;
    }
}