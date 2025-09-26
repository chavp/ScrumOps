using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Events;

/// <summary>
/// Domain event raised when a task is completed.
/// </summary>
/// <param name="TaskId">The unique identifier of the completed task</param>
/// <param name="SprintBacklogItemId">The sprint backlog item this task belongs to</param>
/// <param name="CompletedDate">The date when the task was completed</param>
/// <param name="CompletedHours">The actual hours spent on the task</param>
public record TaskCompletedEvent(
    TaskId TaskId,
    SprintBacklogItemId SprintBacklogItemId,
    DateTime CompletedDate,
    int CompletedHours) : DomainEvent(CompletedDate);