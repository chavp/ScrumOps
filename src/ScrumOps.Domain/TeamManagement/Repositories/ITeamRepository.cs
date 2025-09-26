using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;

namespace ScrumOps.Domain.TeamManagement.Repositories;

/// <summary>
/// Repository interface for Team aggregate root.
/// Defines the contract for persisting and retrieving teams.
/// </summary>
public interface ITeamRepository
{
    /// <summary>
    /// Gets a team by its unique identifier.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The team if found, null otherwise</returns>
    Task<Team?> GetByIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active teams.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of active teams</returns>
    Task<IEnumerable<Team>> GetActiveTeamsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all teams (active and inactive).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all teams</returns>
    Task<IEnumerable<Team>> GetAllTeamsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets teams by name pattern.
    /// </summary>
    /// <param name="namePattern">The pattern to search for in team names</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of teams matching the name pattern</returns>
    Task<IEnumerable<Team>> GetTeamsByNameAsync(string namePattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a team with the specified name exists.
    /// </summary>
    /// <param name="teamName">The team name to check</param>
    /// <param name="excludeTeamId">Optional team ID to exclude from the check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a team with the name exists, false otherwise</returns>
    Task<bool> ExistsWithNameAsync(string teamName, TeamId? excludeTeamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new team to the repository.
    /// </summary>
    /// <param name="team">The team to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(Team team, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing team in the repository.
    /// </summary>
    /// <param name="team">The team to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(Team team, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a team from the repository.
    /// </summary>
    /// <param name="team">The team to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(Team team, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of teams.
    /// </summary>
    /// <param name="activeOnly">Whether to count only active teams</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total count of teams</returns>
    Task<int> GetCountAsync(bool activeOnly = true, CancellationToken cancellationToken = default);
}