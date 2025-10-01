using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Queries;

/// <summary>
/// Query to get a product backlog by team ID with filtering.
/// </summary>
public record GetProductBacklogQuery(
    TeamId TeamId,
    string? Status = null,
    string? Type = null,
    int Limit = 20,
    int Offset = 0
) : IRequest<GetBacklogResponse?>;

/// <summary>
/// Query to get a product backlog by its ID.
/// </summary>
public record GetProductBacklogByIdQuery(ProductBacklogId BacklogId) : IRequest<ProductBacklogDto?>;

/// <summary>
/// Response for getting product backlog with items.
/// </summary>
public class GetBacklogResponse
{
    public ProductBacklogDto Backlog { get; set; } = new();
    public List<BacklogItemDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
}

/// <summary>
/// DTO for product backlog with items.
/// </summary>
public class ProductBacklogDto
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public DateTime? LastRefinedDate { get; set; }
}

/// <summary>
/// DTO for product backlog item.
/// </summary>
public class BacklogItemDto
{
    public int Id { get; set; }
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
    public int? SprintId { get; set; }
}

/// <summary>
/// DTO for detailed backlog item with history.
/// </summary>
public class BacklogItemDetailDto : BacklogItemDto
{
    public DateTime? LastModifiedDate { get; set; }
    public SprintDetailsDto? SprintDetails { get; set; }
    public List<BacklogItemHistoryDto> History { get; set; } = new();
}

/// <summary>
/// DTO for sprint details in backlog item.
/// </summary>
public class SprintDetailsDto
{
    public int SprintId { get; set; }
    public string SprintName { get; set; } = string.Empty;
    public DateTime AddedToSprintDate { get; set; }
}

/// <summary>
/// DTO for backlog item history.
/// </summary>
public class BacklogItemHistoryDto
{
    public DateTime Date { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string ModifiedBy { get; set; } = string.Empty;
}