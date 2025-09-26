using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Events;

/// <summary>
/// Domain event raised when an item is removed from a product backlog.
/// </summary>
/// <param name="ProductBacklogId">The ID of the product backlog</param>
/// <param name="ItemId">The ID of the removed item</param>
/// <param name="ItemTitle">The title of the removed item</param>
public record BacklogItemRemovedEvent(
    ProductBacklogId ProductBacklogId,
    ProductBacklogItemId ItemId,
    string ItemTitle) : DomainEvent;