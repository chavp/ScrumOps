namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing a Sprint identifier.
/// Provides type safety and prevents mixing up different types of IDs.
/// </summary>
public record SprintId(Guid Value)
{
    /// <summary>
    /// Creates a new unique SprintId.
    /// </summary>
    /// <returns>A new SprintId with a unique GUID value</returns>
    public static SprintId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a SprintId from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new SprintId with the specified value</returns>
    public static SprintId From(Guid value) => new(value);

    /// <summary>
    /// Implicitly converts SprintId to GUID for convenience.
    /// </summary>
    /// <param name="sprintId">The SprintId to convert</param>
    /// <returns>The GUID value of the SprintId</returns>
    public static implicit operator Guid(SprintId sprintId) => sprintId.Value;

    /// <summary>
    /// Converts the SprintId to its string representation.
    /// </summary>
    /// <returns>String representation of the GUID value</returns>
    public override string ToString() => Value.ToString();
}