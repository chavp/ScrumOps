using System;
using System.Collections.Generic;
using MediatR;

namespace ScrumOps.Application.TeamManagement.Queries;

/// <summary>
/// Query to get all teams.
/// </summary>
public record GetTeamsQuery() : IRequest<GetTeamsResponse>;

/// <summary>
/// Response containing list of teams with summary information.
/// </summary>
public class GetTeamsResponse
{
    public List<TeamDto> Teams { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// DTO for team summary information.
/// </summary>
public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SprintLengthWeeks { get; set; }
    public decimal Velocity { get; set; }
    public int MemberCount { get; set; }
    public int? CurrentSprintId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}