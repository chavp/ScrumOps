using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.TeamManagement.ValueObjects;

/// <summary>
/// Value object representing a team description.
/// Ensures team descriptions are within acceptable length limits.
/// </summary>
public class TeamDescription : ValueObject
{
    /// <summary>
    /// Maximum allowed length for team descriptions.
    /// </summary>
    public const int MaxLength = 500;

    /// <summary>
    /// Gets the team description value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets an empty team description.
    /// </summary>
    public static readonly TeamDescription Empty = new(string.Empty);

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The team description value</param>
    private TeamDescription(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Creates a new TeamDescription value object with validation.
    /// </summary>
    /// <param name="description">The team description to validate and create</param>
    /// <returns>A new TeamDescription value object</returns>
    /// <exception cref="DomainException">Thrown when the team description is invalid</exception>
    public static TeamDescription Create(string description)
    {
        var trimmedDescription = description?.Trim() ?? string.Empty;
        
        if (trimmedDescription.Length > MaxLength)
        {
            throw new DomainException($"Team description cannot exceed {MaxLength} characters");
        }

        return new TeamDescription(trimmedDescription);
    }

    /// <summary>
    /// Gets whether this description is empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The team description value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the team description.
    /// </summary>
    /// <returns>The team description value</returns>
    public override string ToString() => Value;
}