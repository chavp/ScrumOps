using System.ComponentModel.DataAnnotations;

namespace ScrumOps.Shared.Contracts.Sprints;

/// <summary>
/// Request model for POST /api/sprints endpoint
/// </summary>
public class CreateSprintRequest
{
    [Required]
    public Guid TeamId { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Goal { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(1, 200)]
    public int Capacity { get; set; }

    public List<string> BacklogItemIds { get; set; } = new();
}

/// <summary>
/// Request model for PUT /api/sprints/{id} endpoint
/// </summary>
public class UpdateSprintRequest
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string Goal { get; set; } = string.Empty;

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Range(1, 200)]
    public int Capacity { get; set; }
}

/// <summary>
/// Request model for POST /api/sprints/{id}/items endpoint
/// </summary>
public class AddSprintBacklogItemRequest
{
    [Required]
    public string BacklogItemId { get; set; } = string.Empty;

    [Range(1, 100)]
    public int? StoryPoints { get; set; }
}

/// <summary>
/// Request model for POST /api/sprints/{sprintId}/items/{itemId}/tasks endpoint
/// </summary>
public class AddSprintTaskRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.5, 40)]
    public decimal EstimatedHours { get; set; }

    public string AssigneeId { get; set; } = string.Empty;
}

/// <summary>
/// Request model for PUT /api/sprints/{sprintId}/tasks/{taskId} endpoint
/// </summary>
public class UpdateSprintTaskRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.5, 40)]
    public decimal EstimatedHours { get; set; }

    public decimal ActualHours { get; set; }

    [Required]
    public string Status { get; set; } = string.Empty;

    public string AssigneeId { get; set; } = string.Empty;
}
