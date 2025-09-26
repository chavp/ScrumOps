using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.Repositories;

namespace ScrumOps.Infrastructure.Persistence;

/// <summary>
/// In-memory implementation of ITeamRepository for demonstration purposes.
/// </summary>
public class InMemoryTeamRepository : ITeamRepository
{
    private static readonly ConcurrentDictionary<TeamId, Team> _teams = new();

    public async Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        _teams.TryGetValue(teamId, out var team);
        return await Task.FromResult(team);
    }

    public async Task<IEnumerable<Team>> GetActiveTeamsAsync(CancellationToken cancellationToken = default)
    {
        var activeTeams = _teams.Values.Where(t => t.IsActive);
        return await Task.FromResult(activeTeams);
    }

    public async Task<IEnumerable<Team>> GetAllTeamsAsync(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_teams.Values);
    }

    public async Task<IEnumerable<Team>> GetTeamsByNameAsync(string namePattern, CancellationToken cancellationToken = default)
    {
        var matchingTeams = _teams.Values.Where(t => t.Name.Value.Contains(namePattern, StringComparison.OrdinalIgnoreCase));
        return await Task.FromResult(matchingTeams);
    }

    public async Task<bool> ExistsWithNameAsync(string teamName, TeamId? excludeTeamId = null, CancellationToken cancellationToken = default)
    {
        var exists = _teams.Values.Any(t => 
            t.Name.Value.Equals(teamName, StringComparison.OrdinalIgnoreCase) && 
            (excludeTeamId == null || !t.Id.Equals(excludeTeamId)));
        return await Task.FromResult(exists);
    }

    public async Task AddAsync(Team team, CancellationToken cancellationToken = default)
    {
        _teams.TryAdd(team.Id, team);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(Team team, CancellationToken cancellationToken = default)
    {
        _teams.TryUpdate(team.Id, team, _teams[team.Id]);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Team team, CancellationToken cancellationToken = default)
    {
        _teams.TryRemove(team.Id, out _);
        await Task.CompletedTask;
    }

    public async Task<int> GetCountAsync(bool activeOnly = true, CancellationToken cancellationToken = default)
    {
        var count = activeOnly 
            ? _teams.Values.Count(t => t.IsActive) 
            : _teams.Values.Count;
        return await Task.FromResult(count);
    }
}