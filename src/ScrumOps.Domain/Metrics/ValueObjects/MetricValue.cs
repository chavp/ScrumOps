using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.Metrics.ValueObjects;

/// <summary>
/// Represents a metric value with validation and formatting capabilities.
/// </summary>
public sealed class MetricValue : ValueObject
{
    public decimal Value { get; }
    public string Unit { get; }
    public DateTime Timestamp { get; }

    private MetricValue(decimal value, string unit, DateTime timestamp)
    {
        Value = value;
        Unit = unit ?? throw new ArgumentNullException(nameof(unit));
        Timestamp = timestamp;
    }

    public static MetricValue Create(decimal value, string unit, DateTime? timestamp = null)
    {
        if (value < 0)
            throw new ArgumentException("Metric value cannot be negative", nameof(value));
        
        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        return new MetricValue(value, unit.Trim(), timestamp ?? DateTime.UtcNow);
    }

    public static MetricValue Create(decimal value, MetricType metricType, DateTime? timestamp = null)
    {
        return Create(value, metricType.GetUnit(), timestamp);
    }

    public string FormatValue()
    {
        return Unit switch
        {
            "Percentage" => $"{Value:F1}%",
            "Days" => Value == 1 ? "1 day" : $"{Value:F1} days",
            "Hours" => Value == 1 ? "1 hour" : $"{Value:F1} hours",
            var unit when unit.Contains("Score") => $"{Value:F1}/10",
            _ => $"{Value:F2} {Unit}"
        };
    }

    public bool IsPercentage => Unit == "Percentage";
    public bool IsTimeBased => Unit == "Days" || Unit == "Hours";
    public bool IsScore => Unit.Contains("Score");

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
        yield return Unit;
        yield return Timestamp;
    }
}