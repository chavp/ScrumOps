using Microsoft.EntityFrameworkCore;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.Repositories;

namespace ScrumOps.Infrastructure.Persistence.Repositories;

/// <summary>
/// Repository implementation for Team aggregate.
/// </summary>
public class TeamRepository : ITeamRepository
{
    private readonly ScrumOpsDbContext _context;

    public TeamRepository(ScrumOpsDbContext context)
    {
        _context = context;
    }

    public async Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == teamId, cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetActiveTeamsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .Where(t => t.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetAllTeamsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Team>> GetTeamsByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .Where(t => t.Name.Value.Contains(namePattern))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(string teamName, TeamId? excludeTeamId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Teams.Where(t => t.Name.Value == teamName);
        
        if (excludeTeamId != null)
        {
            query = query.Where(t => t.Id != excludeTeamId);
        }
        
        return await query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        await _context.Teams.AddAsync(team, cancellationToken);
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Update(team);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Team team, CancellationToken cancellationToken = default)
    {
        _context.Teams.Remove(team);
        await Task.CompletedTask;
    }

    public async Task<int> GetCountAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        if (activeOnly)
        {
            return await _context.Teams.CountAsync(t => t.IsActive, cancellationToken);
        }
        
        return await _context.Teams.CountAsync(cancellationToken);
    }

    // Additional methods from old implementation
    public async Task<IReadOnlyList<Team>> GetByMemberEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .Include(t => t.Members)
            .Where(t => t.Members.Any(m => m.Email.Value == email))
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Teams
            .AnyAsync(t => t.Name.Value == name, cancellationToken);
    }
}