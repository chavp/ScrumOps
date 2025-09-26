using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.EventManagement.Entities;
using ScrumOps.Domain.EventManagement.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.EventManagement.Repositories;

/// <summary>
/// Repository interface for SprintEvent aggregate root.
/// Defines the contract for persisting and retrieving sprint events.
/// </summary>
public interface ISprintEventRepository
{
    /// <summary>
    /// Gets a sprint event by its unique identifier.
    /// </summary>
    /// <param name="eventId">The sprint event identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sprint event if found, null otherwise</returns>
    Task<SprintEvent?> GetByIdAsync(SprintEventId eventId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all events for a specific sprint.
    /// </summary>
    /// <param name="sprintId">The sprint identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of events for the sprint</returns>
    Task<IEnumerable<SprintEvent>> GetBySprintIdAsync(SprintId sprintId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all events for a specific team.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of events for the team</returns>
    Task<IEnumerable<SprintEvent>> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events by type.
    /// </summary>
    /// <param name="eventType">The event type to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of events with the specified type</returns>
    Task<IEnumerable<SprintEvent>> GetByEventTypeAsync(EventType eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events within a date range.
    /// </summary>
    /// <param name="startDate">The start date of the range</param>
    /// <param name="endDate">The end date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of events within the date range</returns>
    Task<IEnumerable<SprintEvent>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets upcoming events for a team.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="withinDays">Number of days to look ahead</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of upcoming events</returns>
    Task<IEnumerable<SprintEvent>> GetUpcomingEventsAsync(TeamId teamId, int withinDays = 7, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets events that are currently in progress.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of events currently in progress</returns>
    Task<IEnumerable<SprintEvent>> GetEventsInProgressAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the most recent event of a specific type for a sprint.
    /// </summary>
    /// <param name="sprintId">The sprint identifier</param>
    /// <param name="eventType">The event type</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The most recent event of the specified type</returns>
    Task<SprintEvent?> GetMostRecentEventAsync(SprintId sprintId, EventType eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets event statistics for a team over a date range.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="startDate">The start date of the range</param>
    /// <param name="endDate">The end date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Dictionary of event type counts</returns>
    Task<Dictionary<EventType, int>> GetEventStatisticsAsync(TeamId teamId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if an event conflicts with existing events (time overlap).
    /// </summary>
    /// <param name="eventDate">The event date</param>
    /// <param name="duration">The event duration</param>
    /// <param name="teamId">The team identifier</param>
    /// <param name="excludeEventId">Optional event ID to exclude from conflict check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if there's a conflict, false otherwise</returns>
    Task<bool> HasConflictAsync(DateTime eventDate, TimeBox duration, TeamId teamId, SprintEventId? excludeEventId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new sprint event to the repository.
    /// </summary>
    /// <param name="sprintEvent">The sprint event to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(SprintEvent sprintEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sprint event in the repository.
    /// </summary>
    /// <param name="sprintEvent">The sprint event to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(SprintEvent sprintEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a sprint event from the repository.
    /// </summary>
    /// <param name="sprintEvent">The sprint event to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(SprintEvent sprintEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of events.
    /// </summary>
    /// <param name="teamId">Optional team ID to filter by</param>
    /// <param name="eventType">Optional event type to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total count of events</returns>
    Task<int> GetCountAsync(TeamId? teamId = null, EventType? eventType = null, CancellationToken cancellationToken = default);
}