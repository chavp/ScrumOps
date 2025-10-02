using System.Diagnostics.Metrics;

namespace ScrumOps.Api.Services;

/// <summary>
/// Service for collecting business metrics specific to ScrumOps application.
/// </summary>
public class BusinessMetricsService
{
    private readonly Meter _meter;
    private readonly Counter<long> _teamsCreatedCounter;
    private readonly Counter<long> _sprintsStartedCounter;
    private readonly Counter<long> _sprintsCompletedCounter;
    private readonly Counter<long> _backlogItemsCreatedCounter;
    private readonly Counter<long> _backlogItemsCompletedCounter;
    private readonly Histogram<double> _sprintDurationHistogram;
    private readonly Histogram<double> _backlogItemCompletionTimeHistogram;
    private readonly UpDownCounter<long> _activeSprintsGauge;
    private readonly UpDownCounter<long> _totalBacklogItemsGauge;
    private readonly UpDownCounter<long> _activeTeamsGauge;

    public BusinessMetricsService(Meter meter)
    {
        _meter = meter;

        // Counters for business events
        _teamsCreatedCounter = _meter.CreateCounter<long>(
            "scrumops_teams_created_total",
            description: "Total number of teams created");

        _sprintsStartedCounter = _meter.CreateCounter<long>(
            "scrumops_sprints_started_total",
            description: "Total number of sprints started");

        _sprintsCompletedCounter = _meter.CreateCounter<long>(
            "scrumops_sprints_completed_total",
            description: "Total number of sprints completed");

        _backlogItemsCreatedCounter = _meter.CreateCounter<long>(
            "scrumops_backlog_items_created_total",
            description: "Total number of backlog items created");

        _backlogItemsCompletedCounter = _meter.CreateCounter<long>(
            "scrumops_backlog_items_completed_total",
            description: "Total number of backlog items completed");

        // Histograms for distributions
        _sprintDurationHistogram = _meter.CreateHistogram<double>(
            "scrumops_sprint_duration_days",
            unit: "days",
            description: "Distribution of sprint durations in days");

        _backlogItemCompletionTimeHistogram = _meter.CreateHistogram<double>(
            "scrumops_backlog_item_completion_time_hours",
            unit: "hours",
            description: "Distribution of backlog item completion time in hours");

        // Gauges for current state
        _activeSprintsGauge = _meter.CreateUpDownCounter<long>(
            "scrumops_active_sprints_current",
            description: "Current number of active sprints");

        _totalBacklogItemsGauge = _meter.CreateUpDownCounter<long>(
            "scrumops_backlog_items_current",
            description: "Current total number of backlog items");

        _activeTeamsGauge = _meter.CreateUpDownCounter<long>(
            "scrumops_active_teams_current",
            description: "Current number of active teams");
    }

    /// <summary>
    /// Records that a new team was created.
    /// </summary>
    public void RecordTeamCreated(string teamName)
    {
        _teamsCreatedCounter.Add(1, 
            new KeyValuePair<string, object?>("team_name", teamName));
        _activeTeamsGauge.Add(1);
    }

    /// <summary>
    /// Records that a team was deleted.
    /// </summary>
    public void RecordTeamDeleted()
    {
        _activeTeamsGauge.Add(-1);
    }

    /// <summary>
    /// Records that a sprint was started.
    /// </summary>
    public void RecordSprintStarted(string teamName, string sprintName, int durationDays)
    {
        _sprintsStartedCounter.Add(1, 
            new KeyValuePair<string, object?>("team_name", teamName),
            new KeyValuePair<string, object?>("sprint_name", sprintName));
        _activeSprintsGauge.Add(1);
    }

    /// <summary>
    /// Records that a sprint was completed.
    /// </summary>
    public void RecordSprintCompleted(string teamName, string sprintName, double actualDurationDays, int itemsCompleted, int totalItems)
    {
        _sprintsCompletedCounter.Add(1,
            new KeyValuePair<string, object?>("team_name", teamName),
            new KeyValuePair<string, object?>("sprint_name", sprintName));
        
        _sprintDurationHistogram.Record(actualDurationDays,
            new KeyValuePair<string, object?>("team_name", teamName));
        
        _activeSprintsGauge.Add(-1);

        // Record completion rate
        var completionRate = totalItems > 0 ? (double)itemsCompleted / totalItems : 0;
        RecordSprintCompletionRate(teamName, completionRate);
    }

    /// <summary>
    /// Records that a backlog item was created.
    /// </summary>
    public void RecordBacklogItemCreated(string teamName, string itemType, int storyPoints)
    {
        _backlogItemsCreatedCounter.Add(1,
            new KeyValuePair<string, object?>("team_name", teamName),
            new KeyValuePair<string, object?>("item_type", itemType),
            new KeyValuePair<string, object?>("story_points", storyPoints));
        
        _totalBacklogItemsGauge.Add(1);
    }

    /// <summary>
    /// Records that a backlog item was completed.
    /// </summary>
    public void RecordBacklogItemCompleted(string teamName, string itemType, int storyPoints, double completionTimeHours)
    {
        _backlogItemsCompletedCounter.Add(1,
            new KeyValuePair<string, object?>("team_name", teamName),
            new KeyValuePair<string, object?>("item_type", itemType),
            new KeyValuePair<string, object?>("story_points", storyPoints));

        _backlogItemCompletionTimeHistogram.Record(completionTimeHours,
            new KeyValuePair<string, object?>("team_name", teamName),
            new KeyValuePair<string, object?>("item_type", itemType));
    }

    /// <summary>
    /// Records that a backlog item was deleted.
    /// </summary>
    public void RecordBacklogItemDeleted()
    {
        _totalBacklogItemsGauge.Add(-1);
    }

    /// <summary>
    /// Records sprint completion rate for velocity tracking.
    /// </summary>
    private void RecordSprintCompletionRate(string teamName, double completionRate)
    {
        var completionRateHistogram = _meter.CreateHistogram<double>(
            "scrumops_sprint_completion_rate",
            description: "Distribution of sprint completion rates (0.0 to 1.0)");

        completionRateHistogram.Record(completionRate,
            new KeyValuePair<string, object?>("team_name", teamName));
    }

    /// <summary>
    /// Updates the current count of active sprints (for initialization).
    /// </summary>
    public void UpdateActiveSprintsCount(long count)
    {
        // This should be called during application startup to set initial gauge values
        _activeSprintsGauge.Add(count);
    }

    /// <summary>
    /// Updates the current count of total backlog items (for initialization).
    /// </summary>
    public void UpdateTotalBacklogItemsCount(long count)
    {
        // This should be called during application startup to set initial gauge values
        _totalBacklogItemsGauge.Add(count);
    }

    /// <summary>
    /// Updates the current count of active teams (for initialization).
    /// </summary>
    public void UpdateActiveTeamsCount(long count)
    {
        // This should be called during application startup to set initial gauge values
        _activeTeamsGauge.Add(count);
    }
}