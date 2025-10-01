namespace ScrumOps.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Value object representing a User identifier.
/// Provides type safety and prevents mixing up different types of IDs.
/// </summary>
public record UserId(Guid Value)
{
    /// <summary>
    /// Creates a new unique UserId.
    /// </summary>
    /// <returns>A new UserId with a unique GUID value</returns>
    public static UserId New() => new(Guid.NewGuid());

    /// <summary>
    /// Creates a UserId from an existing GUID value.
    /// </summary>
    /// <param name="value">The GUID value</param>
    /// <returns>A new UserId with the specified value</returns>
    public static UserId From(Guid value) => new(value);

    /// <summary>
    /// Implicitly converts UserId to GUID for convenience.
    /// </summary>
    /// <param name="userId">The UserId to convert</param>
    /// <returns>The GUID value of the UserId</returns>
    public static implicit operator Guid(UserId userId) => userId.Value;

    /// <summary>
    /// Converts the UserId to its string representation.
    /// </summary>
    /// <returns>String representation of the GUID value</returns>
    public override string ToString() => Value.ToString();
}