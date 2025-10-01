using System;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Queries;

/// <summary>
/// Query to get backlog health metrics.
/// </summary>
public record GetBacklogMetricsQuery(TeamId TeamId) : IRequest<BacklogMetricsDto?>;

/// <summary>
/// DTO for backlog health metrics.
/// </summary>
public class BacklogMetricsDto
{
    public int TotalItems { get; set; }
    public int ReadyItems { get; set; }
    public int EstimatedItems { get; set; }
    public decimal AverageStoryPoints { get; set; }
    public decimal VelocityTrend { get; set; }
    public RefinementHealthDto RefinementHealth { get; set; } = new();
    public PriorityDistributionDto PriorityDistribution { get; set; } = new();
}

/// <summary>
/// DTO for refinement health metrics.
/// </summary>
public class RefinementHealthDto
{
    public int Score { get; set; }
    public DateTime? LastRefinedDate { get; set; }
    public int ItemsNeedingRefinement { get; set; }
}

/// <summary>
/// DTO for priority distribution metrics.
/// </summary>
public class PriorityDistributionDto
{
    public int UserStories { get; set; }
    public int Bugs { get; set; }
    public int TechnicalTasks { get; set; }
}