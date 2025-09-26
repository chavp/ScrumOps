using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing a user name (for backlog item creator/owner).
/// </summary>
public sealed class UserName : ValueObject
{
    public const int MaxLength = 100;
    public const int MinLength = 2;

    public string Value { get; }

    private UserName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a UserName from a string value.
    /// </summary>
    public static UserName Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new DomainException("User name cannot be empty");
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < MinLength)
        {
            throw new DomainException($"User name must be at least {MinLength} characters long");
        }

        if (trimmedValue.Length > MaxLength)
        {
            throw new DomainException($"User name cannot exceed {MaxLength} characters");
        }

        return new UserName(trimmedValue);
    }

    /// <summary>
    /// Gets the length of the user name.
    /// </summary>
    public int Length => Value.Length;

    public override string ToString() => Value;

    public static implicit operator string(UserName userName) => userName.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}