using System;
using System.Collections.Generic;
using ScrumOps.Domain.Metrics.Entities;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.DTOs;

/// <summary>
/// Data transfer object for comprehensive reports.
/// </summary>
public class ReportDto
{
    public Guid Id { get; set; }
    public Guid TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public string TypeDisplayName { get; set; } = string.Empty;
    public ReportStatus Status { get; set; }
    public DateTime GeneratedAt { get; set; }
    public Guid GeneratedBy { get; set; }
    public string GeneratedByName { get; set; } = string.Empty;

    // Period information
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public ReportingPeriodType PeriodType { get; set; }
    public string PeriodDisplayName { get; set; } = string.Empty;
    public int PeriodDurationDays { get; set; }

    // Content
    public List<MetricSnapshotDto> Metrics { get; set; } = new();
    public List<ReportInsightDto> Insights { get; set; } = new();
    public ReportSummaryDto Summary { get; set; } = new();

    // Metadata
    public int TotalMetrics { get; set; }
    public int TotalInsights { get; set; }
    public int CriticalInsights { get; set; }
    public bool HasCriticalInsights { get; set; }
}

/// <summary>
/// Data transfer object for report insights.
/// </summary>
public class ReportInsightDto
{
    public InsightType Type { get; set; }
    public string TypeDisplayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public InsightSeverity Severity { get; set; }
    public string SeverityDisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
    public string IconClass { get; set; } = string.Empty;
    public string ColorClass { get; set; } = string.Empty;
}

/// <summary>
/// Summary information for a report.
/// </summary>
public class ReportSummaryDto
{
    public HealthStatus OverallHealth { get; set; }
    public string HealthDescription { get; set; } = string.Empty;
    public List<KeyMetricDto> KeyMetrics { get; set; } = new();
    public List<string> HighlightedInsights { get; set; } = new();
    public Dictionary<string, int> MetricsByCategory { get; set; } = new();
    public Dictionary<InsightSeverity, int> InsightsBySeverity { get; set; } = new();
}

/// <summary>
/// Key metric for report summary.
/// </summary>
public class KeyMetricDto
{
    public MetricType MetricType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string FormattedValue { get; set; } = string.Empty;
    public TrendDirection Trend { get; set; }
    public decimal? PreviousValue { get; set; }
    public string? PreviousFormattedValue { get; set; }
    public decimal? ChangePercentage { get; set; }
    public bool IsImprovement { get; set; }
}

/// <summary>
/// Request for generating a custom report.
/// </summary>
public class GenerateReportRequestDto
{
    public Guid TeamId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ReportType Type { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<MetricType> IncludeMetrics { get; set; } = new();
    public bool IncludeInsights { get; set; } = true;
    public bool IncludeTrends { get; set; } = true;
    public bool IncludeComparisons { get; set; } = false;
}

/// <summary>
/// Response for report generation request.
/// </summary>
public class GenerateReportResponseDto
{
    public Guid ReportId { get; set; }
    public ReportStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public ReportDto? Report { get; set; }
}