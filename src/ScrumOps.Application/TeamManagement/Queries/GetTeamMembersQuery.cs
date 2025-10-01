using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Queries;

/// <summary>
/// Query to get team members with their roles.
/// </summary>
public record GetTeamMembersQuery(TeamId TeamId) : IRequest<List<TeamMemberDto>?>;