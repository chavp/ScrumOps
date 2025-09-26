using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing a unique identifier for a Product Backlog.
/// </summary>
public sealed class ProductBacklogId : ValueObject
{
    public Guid Value { get; }

    private ProductBacklogId(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("ProductBacklogId cannot be empty", nameof(value));
        }
        Value = value;
    }

    /// <summary>
    /// Creates a new ProductBacklogId with a unique identifier.
    /// </summary>
    public static ProductBacklogId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a ProductBacklogId from an existing Guid value.
    /// </summary>
    public static ProductBacklogId From(Guid value) => new(value);

    /// <summary>
    /// Creates a ProductBacklogId from a string representation.
    /// </summary>
    public static ProductBacklogId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException("Invalid ProductBacklogId format", nameof(value));
        }
        return From(guid);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(ProductBacklogId id) => id.Value;
    
    public static implicit operator string(ProductBacklogId id) => id.Value.ToString();

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}