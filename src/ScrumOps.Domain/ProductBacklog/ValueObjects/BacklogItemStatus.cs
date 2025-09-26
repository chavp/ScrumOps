namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Enumeration representing the status of a Product Backlog Item.
/// </summary>
public enum BacklogItemStatus
{
    /// <summary>
    /// Newly created item that hasn't been refined or estimated yet.
    /// </summary>
    New = 1,

    /// <summary>
    /// Item has been refined and estimated, ready for sprint planning.
    /// </summary>
    Ready = 2,

    /// <summary>
    /// Item is currently being worked on in a sprint.
    /// </summary>
    InProgress = 3,

    /// <summary>
    /// Item has been completed and meets the definition of done.
    /// </summary>
    Done = 4
}