using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.Metrics.ValueObjects;

/// <summary>
/// Unique identifier for metrics records.
/// </summary>
public sealed class MetricId : StronglyTypedId<Guid>
{
    private MetricId(Guid value) : base(value) { }

    public static MetricId New() => new(Guid.NewGuid());
    public static MetricId From(Guid value) => new(value);
    public static MetricId From(string value) => new(Guid.Parse(value));
}