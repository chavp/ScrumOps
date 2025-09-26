using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Events;

/// <summary>
/// Domain event raised when a product backlog is refined.
/// </summary>
/// <param name="ProductBacklogId">The ID of the product backlog</param>
/// <param name="RefinedDate">The date when refinement occurred</param>
/// <param name="Notes">The refinement notes</param>
public record BacklogRefinedEvent(
    ProductBacklogId ProductBacklogId,
    DateTime RefinedDate,
    string Notes) : DomainEvent;