namespace ScrumOps.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Value object representing a Team identifier.
/// Provides type safety and prevents mixing up different types of IDs.
/// </summary>
public record TeamId(Guid Value)
{
    /// <summary>
    /// Creates a new unique TeamId.
    /// </summary>
    /// <returns>A new TeamId with a unique GUID value</returns>
    public static TeamId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a TeamId from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new TeamId with the specified value</returns>
    public static TeamId From(Guid value) => new(value);

    /// <summary>
    /// Implicitly converts TeamId to GUID for convenience.
    /// </summary>
    /// <param name="teamId">The TeamId to convert</param>
    /// <returns>The GUID value of the TeamId</returns>
    public static implicit operator Guid(TeamId teamId) => teamId.Value;

    /// <summary>
    /// Converts the TeamId to its string representation.
    /// </summary>
    /// <returns>String representation of the GUID value</returns>
    public override string ToString() => Value.ToString();
}