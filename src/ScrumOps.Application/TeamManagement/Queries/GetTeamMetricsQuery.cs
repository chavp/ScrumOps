using System;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Queries;

/// <summary>
/// Query to get comprehensive team metrics and KPIs.
/// </summary>
public record GetTeamMetricsQuery(TeamId TeamId) : IRequest<TeamMetricsDto?>;

/// <summary>
/// DTO for comprehensive team metrics.
/// </summary>
public class TeamMetricsDto
{
    public int TeamId { get; set; }
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