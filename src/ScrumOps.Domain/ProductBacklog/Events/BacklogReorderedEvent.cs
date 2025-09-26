using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Events;

/// <summary>
/// Represents a priority change for a backlog item during reordering.
/// </summary>
/// <param name="ItemId">The ID of the item being reordered</param>
/// <param name="NewPriority">The new priority value</param>
public record BacklogItemPriorityChange(ProductBacklogItemId ItemId, int NewPriority);

/// <summary>
/// Domain event raised when items in a product backlog are reordered.
/// </summary>
/// <param name="ProductBacklogId">The ID of the product backlog</param>
/// <param name="PriorityChanges">The list of priority changes made</param>
public record BacklogReorderedEvent(
    ProductBacklogId ProductBacklogId,
    List<BacklogItemPriorityChange> PriorityChanges) : DomainEvent;