using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Application.Metrics.DTOs;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.Services;

/// <summary>
/// Application service for metrics operations.
/// </summary>
public interface IMetricsService
{
    /// <summary>
    /// Gets metrics summary for a team dashboard.
    /// </summary>
    Task<MetricsSummaryDto> GetTeamMetricsSummaryAsync(
        Guid teamId, 
        ReportingPeriodType periodType = ReportingPeriodType.Sprint, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets detailed metrics for a specific team and period.
    /// </summary>
    Task<IEnumerable<MetricSnapshotDto>> GetTeamMetricsAsync(
        Guid teamId, 
        DateTime startDate, 
        DateTime endDate, 
        IEnumerable<MetricType>? metricTypes = null, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metric trends over time for visualization.
    /// </summary>
    Task<IEnumerable<MetricTrendDto>> GetMetricTrendsAsync(
        Guid teamId, 
        IEnumerable<MetricType> metricTypes, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets metrics for a specific sprint.
    /// </summary>
    Task<IEnumerable<MetricSnapshotDto>> GetSprintMetricsAsync(
        Guid sprintId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates and stores a specific metric for a team.
    /// </summary>
    Task<MetricSnapshotDto> CalculateMetricAsync(
        Guid teamId, 
        MetricType metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates and stores all standard metrics for a team.
    /// </summary>
    Task<IEnumerable<MetricSnapshotDto>> CalculateAllMetricsAsync(
        Guid teamId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets historical metrics for comparison and analysis.
    /// </summary>
    Task<IEnumerable<MetricSnapshotDto>> GetHistoricalMetricsAsync(
        Guid teamId, 
        MetricType metricType, 
        int numberOfPeriods, 
        ReportingPeriodType periodType = ReportingPeriodType.Sprint, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares metrics between teams.
    /// </summary>
    Task<Dictionary<Guid, MetricSnapshotDto>> CompareTeamMetricsAsync(
        IEnumerable<Guid> teamIds, 
        MetricType metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets performance benchmarks for a metric type.
    /// </summary>
    Task<MetricBenchmarkDto> GetMetricBenchmarksAsync(
        MetricType metricType, 
        CancellationToken cancellationToken = default);
}