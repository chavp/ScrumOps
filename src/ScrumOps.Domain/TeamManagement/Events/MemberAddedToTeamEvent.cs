using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Domain.TeamManagement.Events;

/// <summary>
/// Domain event raised when a member is added to a team.
/// </summary>
/// <param name="TeamId">The unique identifier of the team</param>
/// <param name="UserId">The unique identifier of the user added to the team</param>
/// <param name="Role">The role assigned to the user in the team</param>
public record MemberAddedToTeamEvent(
    TeamId TeamId,
    UserId UserId,
    ScrumRole Role) : DomainEvent();