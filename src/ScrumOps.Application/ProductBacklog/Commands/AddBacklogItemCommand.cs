using MediatR;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to add a new item to a product backlog.
/// </summary>
public record AddBacklogItemCommand(
    ProductBacklogId BacklogId,
    string Title,
    string Description,
    string AcceptanceCriteria,
    int Priority,
    int? StoryPoints,
    string BacklogItemType
) : IRequest<ProductBacklogItemId>;