using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.TeamManagement.Queries;

/// <summary>
/// Query to get team velocity metrics.
/// </summary>
public record GetTeamVelocityQuery(TeamId TeamId) : IRequest<TeamVelocityDto?>;

/// <summary>
/// DTO for team velocity information.
/// </summary>
public class TeamVelocityDto
{
    public int TeamId { get; set; }
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