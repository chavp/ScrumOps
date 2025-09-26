using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a sprint goal.
/// Ensures sprint goals are meaningful and within acceptable length limits.
/// </summary>
public class SprintGoal : ValueObject
{
    /// <summary>
    /// Gets the sprint goal value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets an empty sprint goal.
    /// </summary>
    public static readonly SprintGoal Empty = new(string.Empty);

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The sprint goal value</param>
    private SprintGoal(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Creates a new SprintGoal value object with validation.
    /// </summary>
    /// <param name="goal">The sprint goal to validate and create</param>
    /// <returns>A new SprintGoal value object</returns>
    /// <exception cref="DomainException">Thrown when the sprint goal is invalid</exception>
    public static SprintGoal Create(string goal)
    {
        if (string.IsNullOrWhiteSpace(goal))
        {
            throw new DomainException("Sprint goal cannot be empty");
        }

        var trimmedGoal = goal.Trim();
        
        if (trimmedGoal.Length < 10 || trimmedGoal.Length > 200)
        {
            throw new DomainException("Sprint goal must be between 10 and 200 characters");
        }

        return new SprintGoal(trimmedGoal);
    }

    /// <summary>
    /// Gets whether this goal is empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The sprint goal value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the sprint goal.
    /// </summary>
    /// <returns>The sprint goal value</returns>
    public override string ToString() => Value;
}