using MediatR;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to update an existing backlog item.
/// </summary>
public record UpdateBacklogItemCommand(
    ProductBacklogItemId ItemId,
    string Title,
    string Description,
    string AcceptanceCriteria,
    int Priority,
    int? StoryPoints,
    string BacklogItemType
) : IRequest<Unit>;