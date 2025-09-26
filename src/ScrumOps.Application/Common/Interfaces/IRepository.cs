using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Interfaces;

namespace ScrumOps.Application.Common.Interfaces;

/// <summary>
/// Base repository interface for domain entities.
/// </summary>
/// <typeparam name="T">The entity type</typeparam>
/// <typeparam name="TId">The entity identifier type</typeparam>
public interface IRepository<T, TId> where T : Entity<TId>, IAggregateRoot where TId : notnull
{
    Task<T?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}