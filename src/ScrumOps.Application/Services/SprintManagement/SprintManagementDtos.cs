using System;
using System.Collections.Generic;

namespace ScrumOps.Application.Services.SprintManagement;

/// <summary>
/// Response containing list of sprints.
/// </summary>
public class GetSprintsResponse
{
    public List<SprintDto> Sprints { get; set; } = new();
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
}

/// <summary>
/// DTO for sprint summary information.
/// </summary>
public class SprintDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal? ActualVelocity { get; set; }
    public int BacklogItemCount { get; set; }
    public int CompletedItemCount { get; set; }
    public int ImpedimentCount { get; set; }
    public string Notes { get; set; } = string.Empty;
}

/// <summary>
/// DTO for detailed sprint information.
/// </summary>
public class SprintDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal? ActualVelocity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<SprintBacklogItemDto> BacklogItems { get; set; } = new();
    public List<ImpedimentDto> Impediments { get; set; } = new();
}

/// <summary>
/// DTO for sprint backlog item.
/// </summary>
public class SprintBacklogItemDto
{
    public int Id { get; set; }
    public int ProductBacklogItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int OriginalEstimate { get; set; }
    public int RemainingWork { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}

/// <summary>
/// DTO for impediment information.
/// </summary>
public class ImpedimentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// Response containing sprint backlog items.
/// </summary>
public class GetSprintBacklogResponse
{
    public List<SprintBacklogDetailDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for detailed sprint backlog item with tasks.
/// </summary>
public class SprintBacklogDetailDto
{
    public int Id { get; set; }
    public int ProductBacklogItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int OriginalEstimate { get; set; }
    public int RemainingWork { get; set; }
    public List<TaskDto> Tasks { get; set; } = new();
}

/// <summary>
/// DTO for task information.
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public int RemainingHours { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// DTO for sprint burndown chart data.
/// </summary>
public class SprintBurndownDto
{
    public int SprintId { get; set; }
    public int SprintDays { get; set; }
    public int TotalCapacity { get; set; }
    public List<BurndownDataPoint> BurndownData { get; set; } = new();
}

/// <summary>
/// Data point for burndown chart.
/// </summary>
public class BurndownDataPoint
{
    public DateTime Date { get; set; }
    public int RemainingWork { get; set; }
    public decimal IdealRemaining { get; set; }
}

/// <summary>
/// DTO for sprint velocity metrics.
/// </summary>
public class SprintVelocityDto
{
    public int SprintId { get; set; }
    public decimal PlannedVelocity { get; set; }
    public decimal ActualVelocity { get; set; }
    public int CompletedStoryPoints { get; set; }
    public int TotalStoryPoints { get; set; }
    public decimal CompletionPercentage { get; set; }
}