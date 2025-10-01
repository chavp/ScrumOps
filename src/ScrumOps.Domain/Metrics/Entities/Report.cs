using ScrumOps.Domain.Metrics.ValueObjects;
using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.Metrics.Entities;

/// <summary>
/// Represents a comprehensive report containing multiple metrics and analysis.
/// </summary>
public class Report : AggregateRoot<MetricId>
{
    private readonly List<MetricSnapshot> _metrics = new();
    private readonly List<ReportInsight> _insights = new();

    public TeamId TeamId { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public ReportingPeriod Period { get; private set; }
    public ReportType Type { get; private set; }
    public DateTime GeneratedAt { get; private set; }
    public UserId GeneratedBy { get; private set; }
    public ReportStatus Status { get; private set; }

    public IReadOnlyList<MetricSnapshot> Metrics => _metrics.AsReadOnly();
    public IReadOnlyList<ReportInsight> Insights => _insights.AsReadOnly();

    // For EF Core
    private Report() : base(MetricId.New()) { }

    public Report(
        MetricId id,
        TeamId teamId,
        string title,
        string description,
        ReportingPeriod period,
        ReportType type,
        UserId generatedBy) : base(id)
    {
        TeamId = teamId ?? throw new ArgumentNullException(nameof(teamId));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Period = period ?? throw new ArgumentNullException(nameof(period));
        Type = type;
        GeneratedBy = generatedBy ?? throw new ArgumentNullException(nameof(generatedBy));
        GeneratedAt = DateTime.UtcNow;
        Status = ReportStatus.Generating;
    }

    public static Report Create(
        TeamId teamId,
        string title,
        string description,
        ReportingPeriod period,
        ReportType type,
        UserId generatedBy)
    {
        return new Report(
            MetricId.New(),
            teamId,
            title,
            description,
            period,
            type,
            generatedBy);
    }

    public void AddMetric(MetricSnapshot metric)
    {
        if (metric.TeamId != TeamId)
            throw new InvalidOperationException("Metric must belong to the same team as the report");

        if (!Period.Contains(metric.Value.Timestamp))
            throw new InvalidOperationException("Metric timestamp must be within the reporting period");

        _metrics.Add(metric);
    }

    public void AddInsight(ReportInsight insight)
    {
        _insights.Add(insight);
    }

    public void MarkAsCompleted()
    {
        Status = ReportStatus.Completed;
    }

    public void MarkAsFailed(string reason)
    {
        Status = ReportStatus.Failed;
        AddInsight(ReportInsight.Create(
            InsightType.Error,
            "Report Generation Failed",
            reason,
            InsightSeverity.High));
    }

    public MetricSnapshot? GetMetric(MetricType metricType)
    {
        return _metrics.FirstOrDefault(m => m.MetricType == metricType);
    }

    public IEnumerable<MetricSnapshot> GetMetricsByCategory(string category)
    {
        return _metrics.Where(m => m.MetricType.GetCategory() == category);
    }

    public bool HasMetric(MetricType metricType)
    {
        return _metrics.Any(m => m.MetricType == metricType);
    }

    public decimal? GetMetricValue(MetricType metricType)
    {
        return GetMetric(metricType)?.Value.Value;
    }

    public IEnumerable<ReportInsight> GetInsightsBySeverity(InsightSeverity severity)
    {
        return _insights.Where(i => i.Severity == severity);
    }

    public bool HasCriticalInsights()
    {
        return _insights.Any(i => i.Severity == InsightSeverity.High);
    }
}

/// <summary>
/// Types of reports that can be generated.
/// </summary>
public enum ReportType
{
    TeamPerformance,
    SprintSummary,
    ProductBacklogHealth,
    QualityMetrics,
    ProcessEfficiency,
    Custom
}

/// <summary>
/// Status of report generation.
/// </summary>
public enum ReportStatus
{
    Generating,
    Completed,
    Failed
}

/// <summary>
/// Represents an insight or recommendation derived from metrics analysis.
/// </summary>
public class ReportInsight : ValueObject
{
    public InsightType Type { get; }
    public string Title { get; }
    public string Description { get; }
    public InsightSeverity Severity { get; }
    public DateTime CreatedAt { get; }
    public Dictionary<string, object> Data { get; }

    private ReportInsight(
        InsightType type,
        string title,
        string description,
        InsightSeverity severity,
        Dictionary<string, object>? data = null)
    {
        Type = type;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Severity = severity;
        CreatedAt = DateTime.UtcNow;
        Data = data ?? new Dictionary<string, object>();
    }

    public static ReportInsight Create(
        InsightType type,
        string title,
        string description,
        InsightSeverity severity,
        Dictionary<string, object>? data = null)
    {
        return new ReportInsight(type, title, description, severity, data);
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Type;
        yield return Title;
        yield return Description;
        yield return Severity;
    }
}

/// <summary>
/// Types of insights that can be generated.
/// </summary>
public enum InsightType
{
    Recommendation,
    Warning,
    Achievement,
    Trend,
    Anomaly,
    Error
}

/// <summary>
/// Severity levels for insights.
/// </summary>
public enum InsightSeverity
{
    Low,
    Medium,
    High
}