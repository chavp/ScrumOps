using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Repositories;

/// <summary>
/// Repository interface for Sprint aggregate root.
/// Defines the contract for persisting and retrieving sprints.
/// </summary>
public interface ISprintRepository
{
    /// <summary>
    /// Gets a sprint by its unique identifier.
    /// </summary>
    /// <param name="sprintId">The sprint identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sprint if found, null otherwise</returns>
    Task<Sprint?> GetByIdAsync(SprintId sprintId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all sprints for a specific team.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sprints for the team</returns>
    Task<IEnumerable<Sprint>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current active sprint for a team.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The active sprint if found, null otherwise</returns>
    Task<Sprint?> GetActiveSprintByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sprints by status.
    /// </summary>
    /// <param name="status">The sprint status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sprints with the specified status</returns>
    Task<IEnumerable<Sprint>> GetByStatusAsync(SprintStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sprints within a date range.
    /// </summary>
    /// <param name="startDate">The start date of the range</param>
    /// <param name="endDate">The end date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sprints within the date range</returns>
    Task<IEnumerable<Sprint>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets completed sprints for a team for velocity calculation.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="count">The number of recent sprints to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of recent completed sprints</returns>
    Task<IEnumerable<Sprint>> GetRecentCompletedSprintsAsync(TeamId teamId, int count = 5, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets sprints that are ending soon (within specified days).
    /// </summary>
    /// <param name="withinDays">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of sprints ending soon</returns>
    Task<IEnumerable<Sprint>> GetSprintsEndingSoonAsync(int withinDays = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a team has an active sprint.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the team has an active sprint, false otherwise</returns>
    Task<bool> HasActiveSprintAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new sprint to the repository.
    /// </summary>
    /// <param name="sprint">The sprint to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    System.Threading.Tasks.Task AddAsync(Sprint sprint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sprint in the repository.
    /// </summary>
    /// <param name="sprint">The sprint to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    System.Threading.Tasks.Task UpdateAsync(Sprint sprint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a sprint from the repository.
    /// </summary>
    /// <param name="sprint">The sprint to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    System.Threading.Tasks.Task RemoveAsync(Sprint sprint, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of sprints.
    /// </summary>
    /// <param name="teamId">Optional team ID to filter by</param>
    /// <param name="status">Optional status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total count of sprints</returns>
    Task<int> GetCountAsync(TeamId? teamId = null, SprintStatus? status = null, CancellationToken cancellationToken = default);
}