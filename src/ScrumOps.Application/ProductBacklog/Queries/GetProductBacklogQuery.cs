using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Queries;

/// <summary>
/// Query to get a product backlog by team ID.
/// </summary>
public record GetProductBacklogQuery(TeamId TeamId) : IRequest<ProductBacklogDto?>;

/// <summary>
/// Query to get a product backlog by its ID.
/// </summary>
public record GetProductBacklogByIdQuery(ProductBacklogId BacklogId) : IRequest<ProductBacklogDto?>;

/// <summary>
/// DTO for product backlog with items.
/// </summary>
public class ProductBacklogDto
{
    public string Id { get; set; } = string.Empty;
    public string TeamId { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime? LastRefinedDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<ProductBacklogItemDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for product backlog item.
/// </summary>
public class ProductBacklogItemDto
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