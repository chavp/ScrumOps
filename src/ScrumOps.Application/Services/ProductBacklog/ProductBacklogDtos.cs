using System;
using System.Collections.Generic;

namespace ScrumOps.Application.Services.ProductBacklog;

/// <summary>
/// Response containing items ready for sprint planning.
/// </summary>
public class ReadyItemsResponse
{
    public List<ReadyItemDto> ReadyItems { get; set; } = new();
    public int TotalReadyPoints { get; set; }
    public List<Guid> RecommendedForNextSprint { get; set; } = new();
}

/// <summary>
/// DTO for ready backlog item.
/// </summary>
public class ReadyItemDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int StoryPoints { get; set; }
    public int Priority { get; set; }
    public bool HasAcceptanceCriteria { get; set; }
    public bool IsEstimated { get; set; }
    public List<string> Dependencies { get; set; } = new();
}

/// <summary>
/// DTO for backlog health metrics.
/// </summary>
public class BacklogMetricsDto
{
    public int TotalItems { get; set; }
    public int ReadyItems { get; set; }
    public int EstimatedItems { get; set; }
    public decimal AverageStoryPoints { get; set; }
    public decimal VelocityTrend { get; set; }
    public RefinementHealthDto RefinementHealth { get; set; } = new();
    public PriorityDistributionDto PriorityDistribution { get; set; } = new();
}

/// <summary>
/// DTO for refinement health metrics.
/// </summary>
public class RefinementHealthDto
{
    public int Score { get; set; }
    public DateTime? LastRefinedDate { get; set; }
    public int ItemsNeedingRefinement { get; set; }
}

/// <summary>
/// DTO for priority distribution metrics.
/// </summary>
public class PriorityDistributionDto
{
    public int UserStories { get; set; }
    public int Bugs { get; set; }
    public int TechnicalTasks { get; set; }
}

/// <summary>
/// DTO for backlog flow metrics.
/// </summary>
public class BacklogFlowDto
{
    public List<FlowDataPoint> FlowData { get; set; } = new();
    public LeadTimeDto LeadTime { get; set; } = new();
    public ThroughputDto Throughput { get; set; } = new();
}

/// <summary>
/// Flow data point for cumulative flow diagram.
/// </summary>
public class FlowDataPoint
{
    public DateTime Date { get; set; }
    public int New { get; set; }
    public int Ready { get; set; }
    public int InProgress { get; set; }
    public int Done { get; set; }
}

/// <summary>
/// Lead time metrics.
/// </summary>
public class LeadTimeDto
{
    public decimal Average { get; set; }
    public decimal Median { get; set; }
    public decimal P95 { get; set; }
}

/// <summary>
/// Throughput metrics.
/// </summary>
public class ThroughputDto
{
    public decimal WeeklyAverage { get; set; }
    public int MonthlyTotal { get; set; }
}

/// <summary>
/// Item order specification.
/// </summary>
public record ItemOrder(
    Guid ItemId,
    int Priority);

/// <summary>
/// Response for reorder backlog operation.
/// </summary>
public class ReorderBacklogResponse
{
    public List<ItemOrder> UpdatedItems { get; set; } = new();
}

// Basic DTOs from removed queries
public class GetBacklogResponse 
{ 
    public ProductBacklogDto Backlog { get; set; } = new(); 
    public List<BacklogItemDto> Items { get; set; } = new(); 
    public int TotalCount { get; set; } 
    public bool HasNext { get; set; } 
}

public class ProductBacklogDto 
{ 
    public Guid Id { get; set; } 
    public Guid TeamId { get; set; } 
    public DateTime? LastRefinedDate { get; set; } 
}

public class BacklogItemDto 
{ 
    public Guid Id { get; set; } 
    public string Title { get; set; } = string.Empty; 
    public string Description { get; set; } = string.Empty; 
    public string AcceptanceCriteria { get; set; } = string.Empty; 
    public int Priority { get; set; } 
    public int? StoryPoints { get; set; } 
    public string Status { get; set; } = string.Empty; 
    public string Type { get; set; } = string.Empty; 
    public string CreatedBy { get; set; } = string.Empty; 
    public DateTime CreatedDate { get; set; } 
    public bool IsInCurrentSprint { get; set; } 
    public Guid? SprintId { get; set; } 
}

public class BacklogItemDetailDto : BacklogItemDto 
{ 
    public DateTime? LastModifiedDate { get; set; } 
    public SprintDetailsDto? SprintDetails { get; set; } 
    public List<BacklogItemHistoryDto> History { get; set; } = new(); 
}

public class SprintDetailsDto 
{ 
    public Guid SprintId { get; set; } 
    public string SprintName { get; set; } = string.Empty; 
    public DateTime AddedToSprintDate { get; set; } 
}

public class BacklogItemHistoryDto 
{ 
    public DateTime Date { get; set; } 
    public string Action { get; set; } = string.Empty; 
    public string Field { get; set; } = string.Empty; 
    public string OldValue { get; set; } = string.Empty; 
    public string NewValue { get; set; } = string.Empty; 
    public string ModifiedBy { get; set; } = string.Empty; 
}
