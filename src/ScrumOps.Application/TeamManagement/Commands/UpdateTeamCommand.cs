using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Commands;

/// <summary>
/// Command to update team details.
/// </summary>
public record UpdateTeamCommand(
    TeamId TeamId,
    string Name,
    string? Description,
    int SprintLengthWeeks
) : IRequest;