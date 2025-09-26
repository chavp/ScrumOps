using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Events;

/// <summary>
/// Domain event raised when a new sprint is created.
/// </summary>
/// <param name="SprintId">The unique identifier of the created sprint</param>
/// <param name="TeamId">The team identifier for the sprint</param>
/// <param name="Goal">The sprint goal</param>
/// <param name="StartDate">The planned start date of the sprint</param>
/// <param name="EndDate">The planned end date of the sprint</param>
public record SprintCreatedEvent(
    SprintId SprintId,
    TeamId TeamId,
    string Goal,
    DateTime StartDate,
    DateTime EndDate) : DomainEvent();