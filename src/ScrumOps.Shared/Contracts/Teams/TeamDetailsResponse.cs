namespace ScrumOps.Shared.Contracts.Teams;

/// <summary>
/// Detailed response model for GET /api/teams/{id} endpoint
/// </summary>
public class TeamDetailsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SprintLengthWeeks { get; set; }
    public decimal Velocity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
    public IEnumerable<TeamMember> Members { get; set; } = Array.Empty<TeamMember>();
    public int? CurrentSprintId { get; set; }
}

/// <summary>
/// Team member information
/// </summary>
public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsActive { get; set; }
}