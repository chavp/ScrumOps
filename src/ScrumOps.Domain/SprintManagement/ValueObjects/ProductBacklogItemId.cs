namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a Product Backlog Item identifier in Sprint Management context.
/// This is a reference to items from the Product Backlog bounded context.
/// </summary>
public record ProductBacklogItemId(Guid Value)
{
    /// <summary>
    /// Creates a new unique ProductBacklogItemId.
    /// </summary>
    /// <returns>A new ProductBacklogItemId with a unique GUID value</returns>
    public static ProductBacklogItemId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts ProductBacklogItemId to GUID for convenience.
    /// </summary>
    /// <param name="itemId">The ProductBacklogItemId to convert</param>
    /// <returns>The GUID value of the ProductBacklogItemId</returns>
    public static implicit operator Guid(ProductBacklogItemId itemId) => itemId.Value;

    /// <summary>
    /// Converts the ProductBacklogItemId to its string representation.
    /// </summary>
    /// <returns>String representation of the GUID value</returns>
    public override string ToString() => Value.ToString();
}