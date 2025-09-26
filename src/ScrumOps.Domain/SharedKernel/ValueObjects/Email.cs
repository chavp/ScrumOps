using System.ComponentModel.DataAnnotations;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Value object representing an email address.
/// Ensures that email addresses are valid and normalized.
/// </summary>
public class Email : ValueObject
{
    /// <summary>
    /// Gets the email address value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The email address value</param>
    private Email(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Email value object with validation.
    /// </summary>
    /// <param name="email">The email address to validate and create</param>
    /// <returns>A new Email value object</returns>
    /// <exception cref="DomainException">Thrown when the email is invalid</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new DomainException("Email cannot be empty");
        }

        if (!IsValidEmail(email))
        {
            throw new DomainException("Invalid email format");
        }

        return new Email(email.ToLowerInvariant());
    }

    /// <summary>
    /// Validates the email format using .NET's EmailAddressAttribute.
    /// </summary>
    /// <param name="email">The email to validate</param>
    /// <returns>True if the email is valid, false otherwise</returns>
    private static bool IsValidEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The email value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the email.
    /// </summary>
    /// <returns>The email address value</returns>
    public override string ToString() => Value;
}