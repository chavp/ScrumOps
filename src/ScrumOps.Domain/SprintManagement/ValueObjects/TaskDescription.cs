using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a task description.
/// Ensures task descriptions are within acceptable length limits.
/// </summary>
public class TaskDescription : ValueObject
{
    /// <summary>
    /// Gets the task description value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets an empty task description.
    /// </summary>
    public static readonly TaskDescription Empty = new(string.Empty);

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The task description value</param>
    private TaskDescription(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Creates a new TaskDescription value object with validation.
    /// </summary>
    /// <param name="description">The task description to validate and create</param>
    /// <returns>A new TaskDescription value object</returns>
    /// <exception cref="DomainException">Thrown when the task description is invalid</exception>
    public static TaskDescription Create(string description)
    {
        var trimmedDescription = description?.Trim() ?? string.Empty;
        
        if (trimmedDescription.Length > 1000)
        {
            throw new DomainException("Task description cannot exceed 1000 characters");
        }

        return new TaskDescription(trimmedDescription);
    }

    /// <summary>
    /// Gets whether this description is empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The task description value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the task description.
    /// </summary>
    /// <returns>The task description value</returns>
    public override string ToString() => Value;
}