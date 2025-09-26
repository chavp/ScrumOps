using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing the description of a Product Backlog Item.
/// </summary>
public sealed class ItemDescription : ValueObject
{
    public const int MaxLength = 2000;

    public string Value { get; }

    private ItemDescription(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Creates an ItemDescription from a string value.
    /// </summary>
    public static ItemDescription Create(string value)
    {
        var trimmedValue = value?.Trim() ?? string.Empty;

        if (trimmedValue.Length > MaxLength)
        {
            throw new ArgumentException($"Item description cannot exceed {MaxLength} characters", nameof(value));
        }

        return new ItemDescription(trimmedValue);
    }

    /// <summary>
    /// Gets an empty ItemDescription instance.
    /// </summary>
    public static ItemDescription Empty => new(string.Empty);

    /// <summary>
    /// Checks if the description is empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the length of the description.
    /// </summary>
    public int Length => Value.Length;

    public override string ToString() => Value;

    public static implicit operator string(ItemDescription description) => description.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}