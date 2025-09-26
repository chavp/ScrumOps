using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.TeamManagement.Events;

/// <summary>
/// Domain event raised when a new team is created.
/// </summary>
/// <param name="TeamId">The unique identifier of the created team</param>
/// <param name="TeamName">The name of the created team</param>
/// <param name="CreatedDate">The date and time when the team was created</param>
public record TeamCreatedEvent(
    TeamId TeamId,
    string TeamName,
    DateTime CreatedDate) : DomainEvent(CreatedDate);