using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Commands;

/// <summary>
/// Command to deactivate a team (soft delete).
/// </summary>
public record DeactivateTeamCommand(TeamId TeamId) : IRequest;