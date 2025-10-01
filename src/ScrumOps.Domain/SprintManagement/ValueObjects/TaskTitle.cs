using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a task title.
/// Ensures task titles are meaningful and within acceptable length limits.
/// </summary>
public class TaskTitle : ValueObject
{
    /// <summary>
    /// Maximum allowed length for task titles.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    /// Minimum allowed length for task titles.
    /// </summary>
    public const int MinLength = 3;

    /// <summary>
    /// Gets the task title value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The task title value</param>
    private TaskTitle(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new TaskTitle value object with validation.
    /// </summary>
    /// <param name="title">The task title to validate and create</param>
    /// <returns>A new TaskTitle value object</returns>
    /// <exception cref="DomainException">Thrown when the task title is invalid</exception>
    public static TaskTitle Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new DomainException("Task title cannot be empty");
        }

        var trimmedTitle = title.Trim();
        
        if (trimmedTitle.Length < MinLength || trimmedTitle.Length > MaxLength)
        {
            throw new DomainException($"Task title must be between {MinLength} and {MaxLength} characters");
        }

        return new TaskTitle(trimmedTitle);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The task title value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the task title.
    /// </summary>
    /// <returns>The task title value</returns>
    public override string ToString() => Value;
}