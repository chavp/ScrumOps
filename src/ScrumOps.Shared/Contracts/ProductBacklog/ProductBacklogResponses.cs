namespace ScrumOps.Shared.Contracts.ProductBacklog;

/// <summary>
/// Response model for GET /api/backlogs/{id} endpoint
/// </summary>
public class ProductBacklogResponse
{
    public Guid? Id { get; set; }
    public Guid? TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastRefinedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public int TotalItems { get; set; }
    public List<BacklogItemSummary> Items { get; set; } = new();
}

/// <summary>
/// Response model for GET /api/backlogs endpoint
/// </summary>
public class GetProductBacklogsResponse
{
    public List<ProductBacklogSummary> Backlogs { get; set; } = new();
    public int TotalCount { get; set; }
}

/// <summary>
/// Summary information about a product backlog
/// </summary>
public class ProductBacklogSummary
{
    public Guid? Id { get; set; }
    public Guid? TeamId { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastRefinedDate { get; set; }
    public int TotalItems { get; set; }
    public int CompletedItems { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Detailed response model for backlog items
/// </summary>
public class BacklogItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AcceptanceCriteria { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int? StoryPoints { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Summary information about a backlog item
/// </summary>
public class BacklogItemSummary
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Priority { get; set; }
    public int? StoryPoints { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
}

/// <summary>
/// Response model for GET /api/backlog/items endpoint
/// </summary>
public class GetBacklogItemsResponse
{
    public List<BacklogItemSummary> Items { get; set; } = new();
    public int TotalCount { get; set; }
}
