using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Events;

/// <summary>
/// Domain event raised when a product backlog is created.
/// </summary>
/// <param name="ProductBacklogId">The ID of the created product backlog</param>
/// <param name="TeamId">The ID of the team that owns the backlog</param>
/// <param name="CreatedDate">The date when the backlog was created</param>
public record ProductBacklogCreatedEvent(
    ProductBacklogId ProductBacklogId,
    TeamId TeamId,
    DateTime CreatedDate) : DomainEvent;