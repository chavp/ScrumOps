using Microsoft.EntityFrameworkCore;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of IProductBacklogRepository.
/// </summary>
public class ProductBacklogRepository : IProductBacklogRepository
{
    private readonly ScrumOpsDbContext _context;

    public ProductBacklogRepository(ScrumOpsDbContext context)
    {
        _context = context;
    }

    public async Task<ProductBacklog?> GetByIdAsync(ProductBacklogId backlogId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogs
            .Include(pb => pb.Items)
            .FirstOrDefaultAsync(pb => pb.Id == backlogId, cancellationToken);
    }

    public async Task<ProductBacklog?> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogs
            .Include(pb => pb.Items)
            .FirstOrDefaultAsync(pb => pb.TeamId == teamId, cancellationToken);
    }

    public async Task<IEnumerable<ProductBacklog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogs
            .Include(pb => pb.Items)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductBacklogItem>> GetItemsByStatusAsync(BacklogItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogItems
            .Where(item => item.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductBacklogItem>> GetTeamItemsByStatusAsync(TeamId teamId, BacklogItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogItems
            .Include(item => item.ProductBacklog)
            .Where(item => item.ProductBacklog.TeamId == teamId && item.Status == status)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductBacklogItem>> GetReadyItemsForSprintPlanningAsync(TeamId teamId, int maxItems = 50, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogItems
            .Include(item => item.ProductBacklog)
            .Where(item => item.ProductBacklog.TeamId == teamId && item.Status == BacklogItemStatus.Ready)
            .OrderBy(item => item.Priority)
            .Take(maxItems)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProductBacklogItem>> SearchItemsAsync(string searchTerm, TeamId? teamId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.ProductBacklogItems
            .Include(item => item.ProductBacklog)
            .Where(item => item.Title.Value.Contains(searchTerm) || 
                          (item.Description != null && item.Description.Value.Contains(searchTerm)));

        if (teamId.HasValue)
        {
            query = query.Where(item => item.ProductBacklog.TeamId == teamId.Value);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ProductBacklogItem?> GetItemByIdAsync(ProductBacklogItemId itemId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogItems
            .FirstOrDefaultAsync(item => item.Id == itemId, cancellationToken);
    }

    public async Task<bool> ExistsForTeamAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogs
            .AnyAsync(pb => pb.TeamId == teamId, cancellationToken);
    }

    public async Task AddAsync(ProductBacklog productBacklog, CancellationToken cancellationToken = default)
    {
        await _context.ProductBacklogs.AddAsync(productBacklog, cancellationToken);
    }

    public async Task UpdateAsync(ProductBacklog productBacklog, CancellationToken cancellationToken = default)
    {
        _context.ProductBacklogs.Update(productBacklog);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(ProductBacklog productBacklog, CancellationToken cancellationToken = default)
    {
        _context.ProductBacklogs.Remove(productBacklog);
        await Task.CompletedTask;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogs.CountAsync(cancellationToken);
    }

    public async Task<int> GetItemCountByStatusAsync(BacklogItemStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogItems
            .CountAsync(item => item.Status == status, cancellationToken);
    }

    // Legacy methods for backward compatibility
    public void Update(ProductBacklog productBacklog)
    {
        _context.ProductBacklogs.Update(productBacklog);
    }

    public void Remove(ProductBacklog productBacklog)
    {
        _context.ProductBacklogs.Remove(productBacklog);
    }

    public async Task<bool> ExistsAsync(ProductBacklogId id, CancellationToken cancellationToken = default)
    {
        return await _context.ProductBacklogs
            .AnyAsync(pb => pb.Id == id, cancellationToken);
    }
}