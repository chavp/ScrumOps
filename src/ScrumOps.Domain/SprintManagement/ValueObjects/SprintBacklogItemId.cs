namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a Sprint Backlog Item identifier.
/// Provides type safety and prevents mixing up different types of IDs.
/// </summary>
public record SprintBacklogItemId(Guid Value)
{
    /// <summary>
    /// Creates a new unique SprintBacklogItemId.
    /// </summary>
    /// <returns>A new SprintBacklogItemId with a unique GUID value</returns>
    public static SprintBacklogItemId New() => new(Guid.NewGuid());

    /// <summary>
    /// Implicitly converts SprintBacklogItemId to GUID for convenience.
    /// </summary>
    /// <param name="itemId">The SprintBacklogItemId to convert</param>
    /// <returns>The GUID value of the SprintBacklogItemId</returns>
    public static implicit operator Guid(SprintBacklogItemId itemId) => itemId.Value;

    /// <summary>
    /// Converts the SprintBacklogItemId to its string representation.
    /// </summary>
    /// <returns>String representation of the GUID value</returns>
    public override string ToString() => Value.ToString();
}