using ScrumOps.Domain.Metrics.Entities;
using ScrumOps.Domain.Metrics.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.ProductBacklog.Entities;

namespace ScrumOps.Domain.Metrics.Services;

/// <summary>
/// Domain service for calculating various Scrum metrics.
/// </summary>
public interface IMetricsCalculationService
{
    /// <summary>
    /// Calculates team velocity based on completed story points over multiple sprints.
    /// </summary>
    Task<MetricSnapshot> CalculateTeamVelocityAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates current sprint burndown progress.
    /// </summary>
    Task<MetricSnapshot> CalculateSprintBurndownAsync(
        SprintId sprintId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates team capacity utilization for a given period.
    /// </summary>
    Task<MetricSnapshot> CalculateCapacityUtilizationAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates sprint completion percentage.
    /// </summary>
    Task<MetricSnapshot> CalculateSprintCompletionAsync(
        SprintId sprintId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates backlog item cycle time (from in-progress to done).
    /// </summary>
    Task<MetricSnapshot> CalculateBacklogItemCycleTimeAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates backlog item lead time (from creation to done).
    /// </summary>
    Task<MetricSnapshot> CalculateBacklogItemLeadTimeAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates scope change percentage during sprint.
    /// </summary>
    Task<MetricSnapshot> CalculateSprintScopeChangeAsync(
        SprintId sprintId, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates individual team member contribution.
    /// </summary>
    Task<MetricSnapshot> CalculateIndividualContributionAsync(
        UserId userId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates planning accuracy by comparing planned vs actual velocity.
    /// </summary>
    Task<MetricSnapshot> CalculatePlanningAccuracyAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates estimation accuracy by comparing estimated vs actual effort.
    /// </summary>
    Task<MetricSnapshot> CalculateEstimationAccuracyAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates backlog refinement rate (items refined per sprint).
    /// </summary>
    Task<MetricSnapshot> CalculateBacklogRefinementRateAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates team productivity (story points per hour).
    /// </summary>
    Task<MetricSnapshot> CalculateTeamProductivityAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Calculates task completion rate within sprints.
    /// </summary>
    Task<MetricSnapshot> CalculateTaskCompletionRateAsync(
        TeamId teamId, 
        ReportingPeriod period, 
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Domain service for generating insights and recommendations from metrics.
/// </summary>
public interface IMetricsAnalysisService
{
    /// <summary>
    /// Analyzes team performance trends and generates insights.
    /// </summary>
    Task<IEnumerable<ReportInsight>> AnalyzeTeamPerformanceAsync(
        TeamId teamId, 
        IEnumerable<MetricSnapshot> metrics, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes sprint health and identifies potential issues.
    /// </summary>
    Task<IEnumerable<ReportInsight>> AnalyzeSprintHealthAsync(
        SprintId sprintId, 
        IEnumerable<MetricSnapshot> metrics, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Analyzes velocity trends and provides recommendations.
    /// </summary>
    Task<IEnumerable<ReportInsight>> AnalyzeVelocityTrendsAsync(
        TeamId teamId, 
        IEnumerable<MetricSnapshot> velocityHistory, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Detects anomalies in metrics data.
    /// </summary>
    Task<IEnumerable<ReportInsight>> DetectAnomaliesAsync(
        IEnumerable<MetricSnapshot> metrics, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates predictive insights based on historical data.
    /// </summary>
    Task<IEnumerable<ReportInsight>> GeneratePredictiveInsightsAsync(
        TeamId teamId, 
        IEnumerable<MetricSnapshot> historicalMetrics, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Compares team performance against benchmarks or other teams.
    /// </summary>
    Task<IEnumerable<ReportInsight>> ComparativeAnalysisAsync(
        TeamId teamId, 
        IEnumerable<MetricSnapshot> teamMetrics, 
        IEnumerable<MetricSnapshot> benchmarkMetrics, 
        CancellationToken cancellationToken = default);
}