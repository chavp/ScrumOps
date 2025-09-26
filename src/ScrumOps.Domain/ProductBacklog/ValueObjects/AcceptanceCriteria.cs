using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing acceptance criteria for a Product Backlog Item.
/// Acceptance criteria define the conditions that must be met for the item to be considered complete.
/// </summary>
public sealed class AcceptanceCriteria : ValueObject
{
    public const int MaxLength = 5000;
    public const int MinLength = 10;

    public string Value { get; }

    private AcceptanceCriteria(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates AcceptanceCriteria from a string value.
    /// </summary>
    /// <param name="value">The acceptance criteria text</param>
    /// <returns>A new AcceptanceCriteria instance</returns>
    /// <exception cref="DomainException">Thrown when the value is invalid</exception>
    public static AcceptanceCriteria Create(string value)
    {
        // Allow empty acceptance criteria
        if (string.IsNullOrWhiteSpace(value))
        {
            return new AcceptanceCriteria(string.Empty);
        }

        var trimmedValue = value.Trim();

        if (trimmedValue.Length < MinLength)
        {
            throw new DomainException($"Acceptance criteria must be at least {MinLength} characters long");
        }

        if (trimmedValue.Length > MaxLength)
        {
            throw new DomainException($"Acceptance criteria cannot exceed {MaxLength} characters");
        }

        return new AcceptanceCriteria(trimmedValue);
    }

    /// <summary>
    /// Gets an empty AcceptanceCriteria instance.
    /// </summary>
    public static AcceptanceCriteria Empty => new(string.Empty);

    /// <summary>
    /// Checks if the acceptance criteria are empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the number of criteria items (assumes criteria are separated by newlines or bullet points).
    /// </summary>
    /// <returns>Estimated number of criteria items</returns>
    public int GetCriteriaCount()
    {
        var lines = Value.Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries);
        return lines.Count(line => line.Trim().StartsWith("- ") || line.Trim().StartsWith("* ") || !string.IsNullOrWhiteSpace(line));
    }

    /// <summary>
    /// Checks if the acceptance criteria contain specific keywords that suggest testability.
    /// </summary>
    /// <returns>True if the criteria appear to be testable</returns>
    public bool IsTestable()
    {
        var testableKeywords = new[] { "given", "when", "then", "should", "must", "verify", "check", "validate" };
        var lowerValue = Value.ToLowerInvariant();
        return testableKeywords.Any(keyword => lowerValue.Contains(keyword));
    }

    /// <summary>
    /// Gets the length of the acceptance criteria.
    /// </summary>
    public int Length => Value.Length;

    public override string ToString() => Value;

    public static implicit operator string(AcceptanceCriteria criteria) => criteria.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}