using System.ComponentModel.DataAnnotations;

namespace ScrumOps.Shared.Contracts.ProductBacklog;

/// <summary>
/// Request model for POST /api/backlogs endpoint
/// </summary>
public class CreateProductBacklogRequest
{
    [Required]
    public int TeamId { get; set; }

    [StringLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// Request model for POST /api/backlogs/{id}/items endpoint
/// </summary>
public class AddBacklogItemRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string AcceptanceCriteria { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public int Priority { get; set; }

    [Range(1, 100)]
    public int? StoryPoints { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;
}

/// <summary>
/// Request model for PUT /api/backlogs/{backlogId}/items/{itemId} endpoint
/// </summary>
public class UpdateBacklogItemRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(1000, MinimumLength = 5)]
    public string AcceptanceCriteria { get; set; } = string.Empty;

    [Required]
    [Range(1, 1000)]
    public int Priority { get; set; }

    [Range(1, 100)]
    public int? StoryPoints { get; set; }

    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;
}