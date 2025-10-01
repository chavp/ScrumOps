using ScrumOps.Domain.Metrics.ValueObjects;
using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.Metrics.Entities;

/// <summary>
/// Represents a point-in-time snapshot of a metric value.
/// </summary>
public class MetricSnapshot : Entity<MetricId>
{
    public TeamId TeamId { get; private set; }
    public MetricType MetricType { get; private set; }
    public MetricValue Value { get; private set; }
    public ReportingPeriod Period { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? Notes { get; private set; }
    public Dictionary<string, object> Metadata { get; private set; }

    // For EF Core
    private MetricSnapshot() : base(MetricId.New()) 
    {
        Metadata = new Dictionary<string, object>();
    }

    public MetricSnapshot(
        MetricId id,
        TeamId teamId,
        MetricType metricType,
        MetricValue value,
        ReportingPeriod period,
        string? notes = null,
        Dictionary<string, object>? metadata = null) : base(id)
    {
        TeamId = teamId ?? throw new ArgumentNullException(nameof(teamId));
        MetricType = metricType;
        Value = value ?? throw new ArgumentNullException(nameof(value));
        Period = period ?? throw new ArgumentNullException(nameof(period));
        CreatedAt = DateTime.UtcNow;
        Notes = notes;
        Metadata = metadata ?? new Dictionary<string, object>();
    }

    public static MetricSnapshot Create(
        TeamId teamId,
        MetricType metricType,
        decimal value,
        ReportingPeriod period,
        string? notes = null,
        Dictionary<string, object>? metadata = null)
    {
        var metricValue = MetricValue.Create(value, metricType);
        return new MetricSnapshot(
            MetricId.New(),
            teamId,
            metricType,
            metricValue,
            period,
            notes,
            metadata);
    }

    public void UpdateValue(decimal newValue, string? notes = null)
    {
        Value = MetricValue.Create(newValue, MetricType, DateTime.UtcNow);
        Notes = notes;
    }

    public void AddMetadata(string key, object value)
    {
        Metadata[key] = value;
    }

    public T? GetMetadata<T>(string key)
    {
        if (Metadata.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return default;
    }

    public bool IsCurrentPeriod()
    {
        return Period.Contains(DateTime.UtcNow);
    }

    public bool IsHistorical()
    {
        return !IsCurrentPeriod();
    }

    public string GetFormattedValue()
    {
        return Value.FormatValue();
    }
}