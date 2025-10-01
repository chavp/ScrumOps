using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Queries;

/// <summary>
/// Query to get a specific team by ID with detailed information.
/// </summary>
public record GetTeamByIdQuery(TeamId TeamId) : IRequest<TeamDetailDto?>;

/// <summary>
/// DTO for detailed team information including members and current sprint.
/// </summary>
public class TeamDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SprintLengthWeeks { get; set; }
    public decimal Velocity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<TeamMemberDto> Members { get; set; } = new();
    public CurrentSprintDto? CurrentSprint { get; set; }
}

/// <summary>
/// DTO for team member information.
/// </summary>
public class TeamMemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for current sprint information.
/// </summary>
public class CurrentSprintDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}