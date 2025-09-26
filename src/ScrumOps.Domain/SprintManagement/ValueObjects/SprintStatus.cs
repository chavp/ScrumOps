namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Enumeration representing the different states of a sprint.
/// Follows the standard Scrum sprint lifecycle.
/// </summary>
public enum SprintStatus
{
    /// <summary>
    /// Sprint is being planned - items can be added/removed.
    /// </summary>
    Planning = 1,

    /// <summary>
    /// Sprint is currently active - work is in progress.
    /// </summary>
    Active = 2,

    /// <summary>
    /// Sprint is in review phase - demonstrating completed work.
    /// </summary>
    Review = 3,

    /// <summary>
    /// Sprint is in retrospective phase - reflecting on the process.
    /// </summary>
    Retrospective = 4,

    /// <summary>
    /// Sprint has been completed successfully.
    /// </summary>
    Completed = 5,

    /// <summary>
    /// Sprint has been cancelled before completion.
    /// </summary>
    Cancelled = 6
}