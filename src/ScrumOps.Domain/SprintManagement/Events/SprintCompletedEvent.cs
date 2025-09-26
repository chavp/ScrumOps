using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using SprintVelocity = ScrumOps.Domain.SprintManagement.ValueObjects.Velocity;

namespace ScrumOps.Domain.SprintManagement.Events;

/// <summary>
/// Domain event raised when a sprint is completed.
/// </summary>
/// <param name="SprintId">The unique identifier of the completed sprint</param>
/// <param name="CompletedDate">The actual date when the sprint was completed</param>
/// <param name="ActualVelocity">The actual velocity achieved during the sprint</param>
/// <param name="CompletedItems">The number of backlog items completed</param>
public record SprintCompletedEvent(
    SprintId SprintId,
    DateTime CompletedDate,
    SprintVelocity ActualVelocity,
    int CompletedItems) : DomainEvent(CompletedDate);