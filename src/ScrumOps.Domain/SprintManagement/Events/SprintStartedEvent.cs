using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Events;

/// <summary>
/// Domain event raised when a sprint is started.
/// </summary>
/// <param name="SprintId">The unique identifier of the started sprint</param>
/// <param name="StartedDate">The actual date when the sprint was started</param>
public record SprintStartedEvent(
    SprintId SprintId,
    DateTime StartedDate) : DomainEvent(StartedDate);