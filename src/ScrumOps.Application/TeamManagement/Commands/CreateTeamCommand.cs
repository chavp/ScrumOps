using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Commands;

/// <summary>
/// Command to create a new team.
/// </summary>
public record CreateTeamCommand(
    string Name,
    string? Description,
    int SprintLengthWeeks,
    string ProductOwnerEmail,
    string ScrumMasterEmail
) : IRequest<TeamId>;