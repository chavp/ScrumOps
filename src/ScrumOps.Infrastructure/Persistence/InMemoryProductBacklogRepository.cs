using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of IProductBacklogRepository for demonstration purposes.
/// </summary>
public class InMemoryProductBacklogRepository : IProductBacklogRepository
{
    private static readonly ConcurrentDictionary<ProductBacklogId, ProductBacklog> _backlogs = new();

    public async Task<ProductBacklog?> GetByIdAsync(ProductBacklogId backlogId, CancellationToken cancellationToken = default)
    {
        _backlogs.TryGetValue(backlogId, out var backlog);
        return await Task.FromResult(backlog);
    }

    public async Task<ProductBacklog?> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        var backlog = _backlogs.Values.FirstOrDefault(b => b.TeamId.Equals(teamId));
        return await Task.FromResult(backlog);
    }

    public async Task<IEnumerable<ProductBacklog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_backlogs.Values);
    }

    public async Task<IEnumerable<ProductBacklogItem>> GetItemsByStatusAsync(BacklogItemStatus status, CancellationToken cancellationToken = default)
    {
        var items = _backlogs.Values.SelectMany(b => b.Items.Where(i => i.Status == status));
        return await Task.FromResult(items);
    }

    public async Task<IEnumerable<ProductBacklogItem>> GetTeamItemsByStatusAsync(TeamId teamId, BacklogItemStatus status, CancellationToken cancellationToken = default)
    {
        var backlog = _backlogs.Values.FirstOrDefault(b => b.TeamId.Equals(teamId));
        var items = backlog?.Items.Where(i => i.Status == status) ?? Enumerable.Empty<ProductBacklogItem>();
        return await Task.FromResult(items);
    }

    public async Task<IEnumerable<ProductBacklogItem>> GetReadyItemsForSprintPlanningAsync(TeamId teamId, int maxItems = 50, CancellationToken cancellationToken = default)
    {
        var backlog = _backlogs.Values.FirstOrDefault(b => b.TeamId.Equals(teamId));
        var items = backlog?.Items
            .Where(i => i.Status == BacklogItemStatus.Ready)
            .OrderBy(i => i.Priority.Value)
            .Take(maxItems) ?? Enumerable.Empty<ProductBacklogItem>();
        return await Task.FromResult(items);
    }

    public async Task<IEnumerable<ProductBacklogItem>> SearchItemsAsync(string searchTerm, TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var backlogs = teamId != null 
            ? _backlogs.Values.Where(b => b.TeamId.Equals(teamId)) 
            : _backlogs.Values;
            
        var items = backlogs.SelectMany(b => b.Items)
            .Where(i => i.Title.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                       i.Description.Value.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(items);
    }

    public async Task<ProductBacklogItem?> GetItemByIdAsync(ProductBacklogItemId itemId, CancellationToken cancellationToken = default)
    {
        var item = _backlogs.Values.SelectMany(b => b.Items).FirstOrDefault(i => i.Id.Equals(itemId));
        return await Task.FromResult(item);
    }

    public async Task<bool> ExistsForTeamAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        var exists = _backlogs.Values.Any(b => b.TeamId.Equals(teamId));
        return await Task.FromResult(exists);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_backlogs.Count);
    }

    public async Task<int> GetItemCountByStatusAsync(BacklogItemStatus status, CancellationToken cancellationToken = default)
    {
        var count = _backlogs.Values.SelectMany(b => b.Items).Count(i => i.Status == status);
        return await Task.FromResult(count);
    }

    public async Task AddAsync(ProductBacklog backlog, CancellationToken cancellationToken = default)
    {
        _backlogs.TryAdd(backlog.Id, backlog);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(ProductBacklog backlog, CancellationToken cancellationToken = default)
    {
        _backlogs.TryUpdate(backlog.Id, backlog, _backlogs[backlog.Id]);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(ProductBacklog backlog, CancellationToken cancellationToken = default)
    {
        _backlogs.TryRemove(backlog.Id, out _);
        await Task.CompletedTask;
    }
}