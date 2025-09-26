using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Events;

/// <summary>
/// Domain event raised when a backlog item is added to a sprint.
/// </summary>
/// <param name="SprintId">The unique identifier of the sprint</param>
/// <param name="SprintBacklogItemId">The unique identifier of the sprint backlog item</param>
/// <param name="ProductBacklogItemId">The reference to the product backlog item</param>
/// <param name="StoryPoints">The story points of the added item</param>
public record BacklogItemAddedToSprintEvent(
    SprintId SprintId,
    SprintBacklogItemId SprintBacklogItemId,
    ProductBacklogItemId ProductBacklogItemId,
    StoryPoints StoryPoints) : DomainEvent();