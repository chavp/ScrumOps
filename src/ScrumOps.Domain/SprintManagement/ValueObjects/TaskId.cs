namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a Task identifier.
/// Provides type safety and prevents mixing up different types of IDs.
/// </summary>
public record TaskId(Guid Value)
{
    /// <summary>
    /// Creates a new unique TaskId.
    /// </summary>
    /// <returns>A new TaskId with a unique GUID value</returns>
    public static TaskId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a TaskId from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new TaskId with the specified value</returns>
    public static TaskId From(Guid value) => new(value);

    /// <summary>
    /// Implicitly converts TaskId to GUID for convenience.
    /// </summary>
    /// <param name="taskId">The TaskId to convert</param>
    /// <returns>The GUID value of the TaskId</returns>
    public static implicit operator Guid(TaskId taskId) => taskId.Value;

    /// <summary>
    /// Converts the TaskId to its string representation.
    /// </summary>
    /// <returns>String representation of the GUID value</returns>
    public override string ToString() => Value.ToString();
}