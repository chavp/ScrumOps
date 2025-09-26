using System.ComponentModel.DataAnnotations;

namespace ScrumOps.Shared.Contracts.Teams;

/// <summary>
/// Request model for POST /api/teams endpoint
/// </summary>
public class CreateTeamRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 4)]
    public int SprintLengthWeeks { get; set; }
}

/// <summary>
/// Request model for PUT /api/teams/{id} endpoint
/// </summary>
public class UpdateTeamRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 4)]
    public int SprintLengthWeeks { get; set; }
}

/// <summary>
/// Request model for POST /api/teams/{id}/members endpoint
/// </summary>
public class AddTeamMemberRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// Response model for team velocity endpoint
/// </summary>
public class TeamVelocityResponse
{
    public decimal CurrentVelocity { get; set; }
    public IEnumerable<VelocityDataPoint> VelocityHistory { get; set; } = Array.Empty<VelocityDataPoint>();
}

/// <summary>
/// Velocity data point for historical tracking
/// </summary>
public class VelocityDataPoint
{
    public int SprintNumber { get; set; }
    public decimal Velocity { get; set; }
    public DateTime SprintEndDate { get; set; }
}