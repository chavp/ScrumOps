namespace ScrumOps.Shared.Contracts.Sprints;

/// <summary>
/// Response model for GET /api/sprints/{id} endpoint
/// </summary>
public class SprintDetailsResponse
{
    public string Id { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }
    public int Capacity { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CommittedStoryPoints { get; set; }
    public int CompletedStoryPoints { get; set; }
    public decimal ActualVelocity { get; set; }
    public List<SprintBacklogItem> BacklogItems { get; set; } = new();
    public List<SprintTask> Tasks { get; set; } = new();
}

/// <summary>
/// Response model for GET /api/sprints endpoint
/// </summary>
public class GetSprintsResponse
{
    public List<SprintSummary> Sprints { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// Summary information about a sprint
/// </summary>
public class SprintSummary
{
    public string Id { get; set; } = string.Empty;
    public int TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int CommittedStoryPoints { get; set; }
    public int CompletedStoryPoints { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
    public decimal ProgressPercentage { get; set; }
}

/// <summary>
/// Sprint backlog item information
/// </summary>
public class SprintBacklogItem
{
    public string Id { get; set; } = string.Empty;
    public string BacklogItemId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int? StoryPoints { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public List<SprintTask> Tasks { get; set; } = new();
}

/// <summary>
/// Sprint task information
/// </summary>
public class SprintTask
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AssigneeId { get; set; } = string.Empty;
    public string AssigneeName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? CompletedDate { get; set; }
}

/// <summary>
/// Sprint burndown chart data
/// </summary>
public class SprintBurndownResponse
{
    public string SprintId { get; set; } = string.Empty;
    public List<BurndownDataPoint> DataPoints { get; set; } = new();
    public decimal TotalEstimatedHours { get; set; }
    public decimal RemainingHours { get; set; }
    public decimal CompletedHours { get; set; }
    public decimal ProgressPercentage { get; set; }
}

/// <summary>
/// Burndown chart data point
/// </summary>
public class BurndownDataPoint
{
    public DateTime Date { get; set; }
    public decimal RemainingHours { get; set; }
    public decimal IdealRemainingHours { get; set; }
    public decimal CompletedHours { get; set; }
}