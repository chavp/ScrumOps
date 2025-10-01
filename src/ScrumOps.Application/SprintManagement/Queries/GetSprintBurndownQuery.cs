using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Queries;

/// <summary>
/// Query to get sprint burndown chart data.
/// </summary>
public record GetSprintBurndownQuery(TeamId TeamId, SprintId SprintId) : IRequest<SprintBurndownDto?>;

/// <summary>
/// DTO for sprint burndown chart data.
/// </summary>
public class SprintBurndownDto
{
    public int SprintId { get; set; }
    public int SprintDays { get; set; }
    public int TotalCapacity { get; set; }
    public List<BurndownDataPoint> BurndownData { get; set; } = new();
}

/// <summary>
/// Data point for burndown chart.
/// </summary>
public class BurndownDataPoint
{
    public DateTime Date { get; set; }
    public int RemainingWork { get; set; }
    public decimal IdealRemaining { get; set; }
}