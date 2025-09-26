using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Events;

/// <summary>
/// Domain event raised when an item is added to a product backlog.
/// </summary>
/// <param name="ProductBacklogId">The ID of the product backlog</param>
/// <param name="ItemId">The ID of the added item</param>
/// <param name="ItemTitle">The title of the added item</param>
/// <param name="Priority">The priority assigned to the item</param>
public record BacklogItemAddedEvent(
    ProductBacklogId ProductBacklogId,
    ProductBacklogItemId ItemId,
    string ItemTitle,
    int Priority) : DomainEvent;