using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.TeamManagement.ValueObjects;

/// <summary>
/// Value object representing a team name.
/// Ensures team names are valid and within acceptable length limits.
/// </summary>
public class TeamName : ValueObject
{
    /// <summary>
    /// Maximum allowed length for team names.
    /// </summary>
    public const int MaxLength = 50;

    /// <summary>
    /// Minimum allowed length for team names.
    /// </summary>
    public const int MinLength = 3;

    /// <summary>
    /// Gets the team name value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The team name value</param>
    private TeamName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new TeamName value object with validation.
    /// </summary>
    /// <param name="name">The team name to validate and create</param>
    /// <returns>A new TeamName value object</returns>
    /// <exception cref="DomainException">Thrown when the team name is invalid</exception>
    public static TeamName Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Team name cannot be empty");
        }

        var trimmedName = name.Trim();
        
        if (trimmedName.Length < MinLength || trimmedName.Length > MaxLength)
        {
            throw new DomainException($"Team name must be between {MinLength} and {MaxLength} characters");
        }

        return new TeamName(trimmedName);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The team name value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the team name.
    /// </summary>
    /// <returns>The team name value</returns>
    public override string ToString() => Value;
}