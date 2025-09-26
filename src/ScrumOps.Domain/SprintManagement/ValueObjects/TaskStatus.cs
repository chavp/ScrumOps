namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Enumeration representing the different states of a task.
/// Follows standard Kanban/Scrum task workflow.
/// </summary>
public enum TaskStatus
{
    /// <summary>
    /// Task has been created but work has not started.
    /// </summary>
    ToDo = 1,

    /// <summary>
    /// Task is currently being worked on.
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Task has been completed.
    /// </summary>
    Done = 3,

    /// <summary>
    /// Task is blocked by an impediment or dependency.
    /// </summary>
    Blocked = 4
}