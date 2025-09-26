using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.TeamManagement.Events;

/// <summary>
/// Domain event raised when a team is deactivated.
/// </summary>
/// <param name="TeamId">The unique identifier of the deactivated team</param>
/// <param name="DeactivatedDate">The date and time when the team was deactivated</param>
/// <param name="Reason">The reason for deactivation</param>
public record TeamDeactivatedEvent(
    TeamId TeamId,
    DateTime DeactivatedDate,
    string Reason) : DomainEvent(DeactivatedDate);