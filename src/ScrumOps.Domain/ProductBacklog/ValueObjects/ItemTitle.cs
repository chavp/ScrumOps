using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing the title of a Product Backlog Item.
/// </summary>
public sealed class ItemTitle : ValueObject
{
    public const int MaxLength = 200;
    public const int MinLength = 3;

    public string Value { get; }

    private ItemTitle(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates an ItemTitle from a string value.
    /// </summary>
    public static ItemTitle Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("Item title cannot be empty");
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < MinLength)
        {
            throw new DomainException($"Item title must be at least {MinLength} characters long");
        }

        if (trimmedValue.Length > MaxLength)
        {
            throw new DomainException($"Item title cannot exceed {MaxLength} characters");
        }

        return new ItemTitle(trimmedValue);
    }

    /// <summary>
    /// Gets the length of the title.
    /// </summary>
    public int Length => Value.Length;

    public override string ToString() => Value;

    public static implicit operator string(ItemTitle title) => title.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}