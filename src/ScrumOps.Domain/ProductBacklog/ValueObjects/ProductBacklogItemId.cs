using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing a unique identifier for a Product Backlog Item.
/// </summary>
public sealed class ProductBacklogItemId : ValueObject
{
    public Guid Value { get; }

    private ProductBacklogItemId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("ProductBacklogItemId cannot be empty", nameof(value));
        }
        Value = value;
    }

    /// <summary>
    /// Creates a new ProductBacklogItemId with a unique identifier.
    /// </summary>
    public static ProductBacklogItemId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ProductBacklogItemId from an existing Guid value.
    /// </summary>
    public static ProductBacklogItemId From(Guid value) => new(value);

    /// <summary>
    /// Creates a ProductBacklogItemId from a string representation.
    /// </summary>
    public static ProductBacklogItemId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException("Invalid ProductBacklogItemId format", nameof(value));
        }
        return From(guid);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ProductBacklogItemId id) => id.Value;
    
    public static implicit operator string(ProductBacklogItemId id) => id.Value.ToString();

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}