using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.Repositories;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of the Sprint repository.
/// </summary>
public class SprintRepository : ISprintRepository
{
    private readonly ScrumOpsDbContext _context;

    public SprintRepository(ScrumOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Sprint?> GetByIdAsync(SprintId sprintId, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .FirstOrDefaultAsync(s => s.Id == sprintId, cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .Where(s => s.TeamId == teamId)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Sprint?> GetActiveSprintByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .FirstOrDefaultAsync(s => s.TeamId == teamId && s.Status == SprintStatus.Active, cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> GetByStatusAsync(SprintStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .Where(s => s.StartDate >= startDate && s.EndDate <= endDate)
            .OrderBy(s => s.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> GetRecentCompletedSprintsAsync(TeamId teamId, int count = 5, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .Where(s => s.TeamId == teamId && s.Status == SprintStatus.Completed)
            .OrderByDescending(s => s.EndDate)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Sprint>> GetSprintsEndingSoonAsync(int withinDays = 7, CancellationToken cancellationToken = default)
    {
        var endDate = DateTime.UtcNow.AddDays(withinDays);
        return await _context.Sprints
            .Where(s => s.Status == SprintStatus.Active && s.EndDate <= endDate)
            .OrderBy(s => s.EndDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasActiveSprintAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .AnyAsync(s => s.TeamId == teamId && s.Status == SprintStatus.Active, cancellationToken);
    }

    public async System.Threading.Tasks.Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default)
    {
        await _context.Sprints.AddAsync(sprint, cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateAsync(Sprint sprint, CancellationToken cancellationToken = default)
    {
        _context.Sprints.Update(sprint);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async System.Threading.Tasks.Task RemoveAsync(Sprint sprint, CancellationToken cancellationToken = default)
    {
        _context.Sprints.Remove(sprint);
        await System.Threading.Tasks.Task.CompletedTask;
    }

    public async Task<int> GetCountAsync(TeamId? teamId = null, SprintStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sprints.AsQueryable();
        
        if (teamId is not null)
            query = query.Where(s => s.TeamId == teamId);
            
        if (status is not null)
            query = query.Where(s => s.Status == status);
            
        return await query.CountAsync(cancellationToken);
    }

    // Keep these methods for backward compatibility but they're not part of the interface
    public void Update(Sprint sprint)
    {
        _context.Sprints.Update(sprint);
    }

    public void Remove(Sprint sprint)
    {
        _context.Sprints.Remove(sprint);
    }

    public async Task<bool> ExistsAsync(SprintId id, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<int> CountByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Sprints
            .CountAsync(s => s.TeamId == teamId, cancellationToken);
    }
}