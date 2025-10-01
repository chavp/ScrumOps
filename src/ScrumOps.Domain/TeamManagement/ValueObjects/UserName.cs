using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.TeamManagement.ValueObjects;

/// <summary>
/// Value object representing a user's name.
/// Ensures user names are valid and within acceptable length limits.
/// </summary>
public class UserName : ValueObject
{
    /// <summary>
    /// Maximum allowed length for user names.
    /// </summary>
    public const int MaxLength = 100;

    /// <summary>
    /// Minimum allowed length for user names.
    /// </summary>
    public const int MinLength = 2;

    /// <summary>
    /// Gets the user name value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The user name value</param>
    private UserName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new UserName value object with validation.
    /// </summary>
    /// <param name="name">The user name to validate and create</param>
    /// <returns>A new UserName value object</returns>
    /// <exception cref="DomainException">Thrown when the user name is invalid</exception>
    public static UserName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("User name cannot be empty");
        }

        var trimmedName = name.Trim();
        
        if (trimmedName.Length < MinLength || trimmedName.Length > MaxLength)
        {
            throw new DomainException($"User name must be between {MinLength} and {MaxLength} characters");
        }

        return new UserName(trimmedName);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The user name value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the user name.
    /// </summary>
    /// <returns>The user name value</returns>
    public override string ToString() => Value;
}