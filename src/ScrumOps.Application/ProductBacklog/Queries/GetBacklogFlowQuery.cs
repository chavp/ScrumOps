using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Queries;

/// <summary>
/// Query to get backlog flow metrics (cumulative flow).
/// </summary>
public record GetBacklogFlowQuery(TeamId TeamId) : IRequest<BacklogFlowDto?>;

/// <summary>
/// DTO for backlog flow metrics.
/// </summary>
public class BacklogFlowDto
{
    public List<FlowDataPoint> FlowData { get; set; } = new();
    public LeadTimeDto LeadTime { get; set; } = new();
    public ThroughputDto Throughput { get; set; } = new();
}

/// <summary>
/// Flow data point for cumulative flow diagram.
/// </summary>
public class FlowDataPoint
{
    public DateTime Date { get; set; }
    public int New { get; set; }
    public int Ready { get; set; }
    public int InProgress { get; set; }
    public int Done { get; set; }
}

/// <summary>
/// Lead time metrics.
/// </summary>
public class LeadTimeDto
{
    public decimal Average { get; set; }
    public decimal Median { get; set; }
    public decimal P95 { get; set; }
}

/// <summary>
/// Throughput metrics.
/// </summary>
public class ThroughputDto
{
    public decimal WeeklyAverage { get; set; }
    public int MonthlyTotal { get; set; }
}