using System;
using ScrumOps.Domain.Metrics.ValueObjects;

namespace ScrumOps.Application.Metrics.DTOs;

/// <summary>
/// Benchmark data for a specific metric.
/// </summary>
public class MetricBenchmarkDto
{
    public MetricType MetricType { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public decimal? IndustryAverage { get; set; }
    public decimal? TopPerformerThreshold { get; set; }
    public decimal? MinimumThreshold { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public string Source { get; set; } = string.Empty;
}