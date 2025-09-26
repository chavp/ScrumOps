using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Domain.TeamManagement.Events;

/// <summary>
/// Domain event raised when a team's velocity is updated.
/// </summary>
/// <param name="TeamId">The unique identifier of the team</param>
/// <param name="PreviousVelocity">The team's previous velocity</param>
/// <param name="NewVelocity">The team's new velocity</param>
public record TeamVelocityUpdatedEvent(
    TeamId TeamId,
    Velocity PreviousVelocity,
    Velocity NewVelocity) : DomainEvent();