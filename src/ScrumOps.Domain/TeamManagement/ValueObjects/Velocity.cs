using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.TeamManagement.ValueObjects;

/// <summary>
/// Value object representing team velocity (story points completed per sprint).
/// Ensures velocity values are within reasonable bounds.
/// </summary>
public class Velocity : ValueObject
{
    /// <summary>
    /// Gets the velocity value.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Gets a zero velocity.
    /// </summary>
    public static readonly Velocity Zero = new(0);

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The velocity value</param>
    private Velocity(decimal value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new Velocity value object with validation.
    /// </summary>
    /// <param name="velocity">The velocity value</param>
    /// <returns>A new Velocity value object</returns>
    /// <exception cref="DomainException">Thrown when the velocity is invalid</exception>
    public static Velocity Create(decimal velocity)
    {
        if (velocity < 0)
        {
            throw new DomainException("Velocity cannot be negative");
        }

        if (velocity > 1000)
        {
            throw new DomainException("Velocity cannot exceed 1000 story points");
        }

        return new Velocity(velocity);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The velocity value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the velocity.
    /// </summary>
    /// <returns>The velocity value</returns>
    public override string ToString() => $"{Value:F1} story points";
}