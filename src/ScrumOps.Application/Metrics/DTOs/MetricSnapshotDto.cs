using System;
using System.Collections.Generic;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.DTOs;

/// <summary>
/// Data transfer object for metric snapshots.
/// </summary>
public class MetricSnapshotDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public MetricType MetricType { get; set; }
    public string MetricDisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string FormattedValue { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Notes { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();

    // Period information
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ReportingPeriodType PeriodType { get; set; }
    public string PeriodDisplayName { get; set; } = string.Empty;

    // Status flags
    public bool IsCurrentPeriod { get; set; }
    public bool IsHistorical { get; set; }
}

/// <summary>
/// Data transfer object for metric trends over time.
/// </summary>
public class MetricTrendDto
{
    public MetricType MetricType { get; set; }
    public string MetricDisplayName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public List<MetricDataPointDto> DataPoints { get; set; } = new();
    public TrendDirection Trend { get; set; }
    public decimal TrendPercentage { get; set; }
    public string TrendDescription { get; set; } = string.Empty;
}

/// <summary>
/// Individual data point in a metric trend.
/// </summary>
public class MetricDataPointDto
{
    public decimal Value { get; set; }
    public string FormattedValue { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string PeriodLabel { get; set; } = string.Empty;
}

/// <summary>
/// Direction of metric trend.
/// </summary>
public enum TrendDirection
{
    Improving,
    Declining,
    Stable,
    Volatile
}

/// <summary>
/// Summary of metrics for dashboard display.
/// </summary>
public class MetricsSummaryDto
{
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string PeriodDisplayName { get; set; } = string.Empty;

    // Key Performance Indicators
    public decimal? TeamVelocity { get; set; }
    public string? TeamVelocityFormatted { get; set; }
    public TrendDirection VelocityTrend { get; set; }

    public decimal? SprintCompletion { get; set; }
    public string? SprintCompletionFormatted { get; set; }

    public decimal? CapacityUtilization { get; set; }
    public string? CapacityUtilizationFormatted { get; set; }

    public decimal? CycleTime { get; set; }
    public string? CycleTimeFormatted { get; set; }

    // Health indicators
    public HealthStatus VelocityHealth { get; set; }
    public HealthStatus QualityHealth { get; set; }
    public HealthStatus ProcessHealth { get; set; }
    public HealthStatus OverallHealth { get; set; }

    // Counts
    public int TotalMetrics { get; set; }
    public int CriticalInsights { get; set; }
    public int Recommendations { get; set; }
}

/// <summary>
/// Health status indicators.
/// </summary>
public enum HealthStatus
{
    Excellent,
    Good,
    Warning,
    Critical,
    Unknown
}