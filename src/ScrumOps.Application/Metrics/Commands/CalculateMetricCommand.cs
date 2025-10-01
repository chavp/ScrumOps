using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Application.Metrics.DTOs;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.Commands;

/// <summary>
/// Command to calculate a specific metric for a team.
/// </summary>
public record CalculateMetricCommand(
    Guid TeamId,
    MetricType MetricType,
    DateTime StartDate,
    DateTime EndDate,
    string? Notes = null,
    Dictionary<string, object>? Metadata = null) : IRequest<MetricSnapshotDto>;

/// <summary>
/// Command to calculate all standard metrics for a team.
/// </summary>
public record CalculateAllMetricsCommand(
    Guid TeamId,
    DateTime StartDate,
    DateTime EndDate) : IRequest<IEnumerable<MetricSnapshotDto>>;

/// <summary>
/// Command to generate a comprehensive report.
/// </summary>
public record GenerateReportCommand(
    GenerateReportRequestDto Request) : IRequest<GenerateReportResponseDto>;

/// <summary>
/// Command to generate a sprint summary report.
/// </summary>
public record GenerateSprintSummaryCommand(
    Guid SprintId) : IRequest<GenerateReportResponseDto>;

/// <summary>
/// Command to update metric benchmarks.
/// </summary>
public record UpdateMetricBenchmarksCommand(
    MetricType MetricType,
    decimal? IndustryAverage,
    decimal? TopPerformerThreshold,
    decimal? MinimumThreshold,
    string Source) : IRequest<MetricBenchmarkDto>;

/// <summary>
/// Command to cleanup old reports.
/// </summary>
public record CleanupOldReportsCommand(
    int RetentionDays = 90) : IRequest<int>;