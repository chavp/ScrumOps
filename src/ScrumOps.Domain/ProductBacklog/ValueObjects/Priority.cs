using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing the priority of a Product Backlog Item.
/// Lower numbers indicate higher priority.
/// </summary>
public sealed class Priority : ValueObject
{
    public const int MinValue = 0;  // 0 = Unassigned
    public const int MaxValue = 1000;

    public int Value { get; }

    private Priority(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a Priority from an integer value.
    /// </summary>
    public static Priority Create(int value)
    {
        if (value < MinValue)
        {
            throw new DomainException($"Priority cannot be negative");
        }

        if (value > MaxValue)
        {
            throw new DomainException($"Priority cannot exceed {MaxValue}");
        }

        return new Priority(value);
    }

    /// <summary>
    /// Gets the highest priority (value = 1).
    /// </summary>
    public static Priority Highest => new(1);

    /// <summary>
    /// Gets the lowest priority (value = MaxValue).
    /// </summary>
    public static Priority Lowest => new(MaxValue);

    /// <summary>
    /// Gets an unassigned priority (value = 0 for unassigned items).
    /// </summary>
    public static Priority Unassigned => new(0);

    /// <summary>
    /// Checks if this priority is higher than another priority.
    /// </summary>
    public bool IsHigherThan(Priority other) => Value < other.Value;

    /// <summary>
    /// Checks if this priority is lower than another priority.
    /// </summary>
    public bool IsLowerThan(Priority other) => Value > other.Value;

    public override string ToString() => Value.ToString();

    public static implicit operator int(Priority priority) => priority.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}