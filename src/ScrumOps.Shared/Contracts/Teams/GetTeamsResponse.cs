namespace ScrumOps.Shared.Contracts.Teams;

/// <summary>
/// Response model for GET /api/teams endpoint
/// </summary>
public class GetTeamsResponse
{
    public IEnumerable<TeamSummary> Teams { get; set; } = Array.Empty<TeamSummary>();
    public int TotalCount { get; set; }
}

/// <summary>
/// Summary information about a team
/// </summary>
public class TeamSummary
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