using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ScrumOps.Application.Metrics.Commands;
using ScrumOps.Application.Metrics.DTOs;
using ScrumOps.Application.Metrics.Queries;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.Services;

/// <summary>
/// Implementation of metrics application service.
/// </summary>
public class MetricsService : IMetricsService
{
    private readonly IMediator _mediator;
    private readonly ILogger<MetricsService> _logger;

    public MetricsService(IMediator mediator, ILogger<MetricsService> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MetricsSummaryDto> GetTeamMetricsSummaryAsync(
        Guid teamId, 
        ReportingPeriodType periodType = ReportingPeriodType.Sprint, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting metrics summary for team {TeamId} with period type {PeriodType}", 
            teamId, periodType);

        var query = new GetTeamMetricsSummaryQuery(teamId, periodType);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<MetricSnapshotDto>> GetTeamMetricsAsync(
        Guid teamId, 
        DateTime startDate, 
        DateTime endDate, 
        IEnumerable<MetricType>? metricTypes = null, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting team metrics for team {TeamId} from {StartDate} to {EndDate}", 
            teamId, startDate, endDate);

        var query = new GetTeamMetricsQuery(teamId, startDate, endDate, metricTypes);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<MetricTrendDto>> GetMetricTrendsAsync(
        Guid teamId, 
        IEnumerable<MetricType> metricTypes, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting metric trends for team {TeamId} from {StartDate} to {EndDate}", 
            teamId, startDate, endDate);

        var query = new GetMetricTrendsQuery(teamId, metricTypes, startDate, endDate);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<IEnumerable<MetricSnapshotDto>> GetSprintMetricsAsync(
        Guid sprintId, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting sprint metrics for sprint {SprintId}", sprintId);

        var query = new GetSprintMetricsQuery(sprintId);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<MetricSnapshotDto> CalculateMetricAsync(
        Guid teamId, 
        MetricType metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating metric {MetricType} for team {TeamId} from {StartDate} to {EndDate}", 
            metricType, teamId, startDate, endDate);

        var command = new CalculateMetricCommand(teamId, metricType, startDate, endDate);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<IEnumerable<MetricSnapshotDto>> CalculateAllMetricsAsync(
        Guid teamId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Calculating all metrics for team {TeamId} from {StartDate} to {EndDate}", 
            teamId, startDate, endDate);

        var command = new CalculateAllMetricsCommand(teamId, startDate, endDate);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<IEnumerable<MetricSnapshotDto>> GetHistoricalMetricsAsync(
        Guid teamId, 
        MetricType metricType, 
        int numberOfPeriods, 
        ReportingPeriodType periodType = ReportingPeriodType.Sprint, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting historical metrics {MetricType} for team {TeamId}, {NumberOfPeriods} periods", 
            metricType, teamId, numberOfPeriods);

        var query = new GetHistoricalMetricsQuery(teamId, metricType, numberOfPeriods, periodType);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<Dictionary<Guid, MetricSnapshotDto>> CompareTeamMetricsAsync(
        IEnumerable<Guid> teamIds, 
        MetricType metricType, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Comparing metric {MetricType} across {TeamCount} teams from {StartDate} to {EndDate}", 
            metricType, teamIds.Count(), startDate, endDate);

        var query = new CompareTeamMetricsQuery(teamIds, metricType, startDate, endDate);
        return await _mediator.Send(query, cancellationToken);
    }

    public async Task<MetricBenchmarkDto> GetMetricBenchmarksAsync(
        MetricType metricType, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Getting benchmarks for metric {MetricType}", metricType);

        var query = new GetMetricBenchmarksQuery(metricType);
        return await _mediator.Send(query, cancellationToken);
    }
}