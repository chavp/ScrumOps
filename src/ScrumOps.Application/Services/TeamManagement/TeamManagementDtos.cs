using System;
using System.Collections.Generic;

namespace ScrumOps.Application.Services.TeamManagement;

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
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SprintLengthWeeks { get; set; }
    public decimal Velocity { get; set; }
    public int MemberCount { get; set; }
    public Guid? CurrentSprintId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// DTO for detailed team information including members and current sprint.
/// </summary>
public class TeamDetailDto
{
    public Guid Id { get; set; }
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
    public Guid Id { get; set; }
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
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// DTO for team velocity information.
/// </summary>
public class TeamVelocityDto
{
    public Guid TeamId { get; set; }
    public decimal CurrentVelocity { get; set; }
    public decimal AverageVelocity { get; set; }
    public List<VelocityDataPoint> VelocityTrend { get; set; } = new();
    public int TotalSprints { get; set; }
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Velocity data point for trend analysis.
/// </summary>
public class VelocityDataPoint
{
    public int SprintNumber { get; set; }
    public string SprintName { get; set; } = string.Empty;
    public decimal PlannedVelocity { get; set; }
    public decimal ActualVelocity { get; set; }
    public DateTime SprintEndDate { get; set; }
}

/// <summary>
/// DTO for comprehensive team metrics.
/// </summary>
public class TeamMetricsDto
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public VelocityMetrics Velocity { get; set; } = new();
    public QualityMetrics Quality { get; set; } = new();
    public ProductivityMetrics Productivity { get; set; } = new();
    public EngagementMetrics Engagement { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

/// <summary>
/// Velocity-related metrics.
/// </summary>
public class VelocityMetrics
{
    public decimal CurrentVelocity { get; set; }
    public decimal AverageVelocity { get; set; }
    public decimal VelocityTrend { get; set; }
    public int CompletedSprints { get; set; }
}

/// <summary>
/// Quality-related metrics.
/// </summary>
public class QualityMetrics
{
    public decimal DefectRate { get; set; }
    public decimal CodeCoverage { get; set; }
    public int BugsReported { get; set; }
    public int BugsResolved { get; set; }
}

/// <summary>
/// Productivity-related metrics.
/// </summary>
public class ProductivityMetrics
{
    public decimal Throughput { get; set; }
    public decimal LeadTime { get; set; }
    public decimal CycleTime { get; set; }
    public int StoriesCompleted { get; set; }
}

/// <summary>
/// Team engagement metrics.
/// </summary>
public class EngagementMetrics
{
    public decimal TeamSatisfaction { get; set; }
    public decimal RetroActionItems { get; set; }
    public decimal ImpedimentResolutionTime { get; set; }
    public int TotalImpediments { get; set; }
}