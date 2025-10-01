using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Application.Metrics.DTOs;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.Queries;

/// <summary>
/// Query to get metrics for a specific team and time period.
/// </summary>
public record GetTeamMetricsQuery(
    Guid TeamId,
    DateTime StartDate,
    DateTime EndDate,
    IEnumerable<MetricType>? MetricTypes = null) : IRequest<IEnumerable<MetricSnapshotDto>>;

/// <summary>
/// Query to get metrics summary for team dashboard.
/// </summary>
public record GetTeamMetricsSummaryQuery(
    Guid TeamId,
    ReportingPeriodType PeriodType = ReportingPeriodType.Sprint) : IRequest<MetricsSummaryDto>;

/// <summary>
/// Query to get metric trends over time.
/// </summary>
public record GetMetricTrendsQuery(
    Guid TeamId,
    IEnumerable<MetricType> MetricTypes,
    DateTime StartDate,
    DateTime EndDate) : IRequest<IEnumerable<MetricTrendDto>>;

/// <summary>
/// Query to get metrics for a specific sprint.
/// </summary>
public record GetSprintMetricsQuery(
    Guid SprintId) : IRequest<IEnumerable<MetricSnapshotDto>>;

/// <summary>
/// Query to get historical metrics for analysis.
/// </summary>
public record GetHistoricalMetricsQuery(
    Guid TeamId,
    MetricType MetricType,
    int NumberOfPeriods,
    ReportingPeriodType PeriodType = ReportingPeriodType.Sprint) : IRequest<IEnumerable<MetricSnapshotDto>>;

/// <summary>
/// Query to compare metrics between teams.
/// </summary>
public record CompareTeamMetricsQuery(
    IEnumerable<Guid> TeamIds,
    MetricType MetricType,
    DateTime StartDate,
    DateTime EndDate) : IRequest<Dictionary<Guid, MetricSnapshotDto>>;

/// <summary>
/// Query to get performance benchmarks.
/// </summary>
public record GetMetricBenchmarksQuery(
    MetricType MetricType) : IRequest<MetricBenchmarkDto>;