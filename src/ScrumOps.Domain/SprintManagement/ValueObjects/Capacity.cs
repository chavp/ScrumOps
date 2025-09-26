using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing sprint capacity (available hours for the team).
/// Ensures capacity values are within reasonable bounds.
/// </summary>
public class Capacity : ValueObject
{
    /// <summary>
    /// Gets the capacity value in hours.
    /// </summary>
    public int Hours { get; }

    /// <summary>
    /// Gets a zero capacity.
    /// </summary>
    public static readonly Capacity Zero = new(0);

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="hours">The capacity value in hours</param>
    private Capacity(int hours)
    {
        Hours = hours;
    }

    /// <summary>
    /// Creates a new Capacity value object with validation.
    /// </summary>
    /// <param name="hours">The capacity value in hours</param>
    /// <returns>A new Capacity value object</returns>
    /// <exception cref="DomainException">Thrown when the capacity is invalid</exception>
    public static Capacity Create(int hours)
    {
        if (hours < 0)
        {
            throw new DomainException("Capacity cannot be negative");
        }

        if (hours > 1000)
        {
            throw new DomainException("Capacity cannot exceed 1000 hours per sprint");
        }

        return new Capacity(hours);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The capacity value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Hours;
    }

    /// <summary>
    /// Returns the string representation of the capacity.
    /// </summary>
    /// <returns>The capacity value with units</returns>
    public override string ToString() => $"{Hours} hours";
}