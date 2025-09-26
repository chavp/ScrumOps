using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing team velocity - the amount of work a team can complete in a sprint.
/// Velocity is typically measured in story points completed per sprint.
/// </summary>
public sealed class Velocity : ValueObject
{
    public const int MinValue = 0;
    public const int MaxValue = 500;

    public int Value { get; }

    private Velocity(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a Velocity from an integer value.
    /// </summary>
    /// <param name="value">The velocity value in story points</param>
    /// <returns>A new Velocity instance</returns>
    /// <exception cref="DomainException">Thrown when the value is invalid</exception>
    public static Velocity Create(int value)
    {
        if (value < MinValue)
        {
            throw new DomainException($"Velocity cannot be negative");
        }

        if (value > MaxValue)
        {
            throw new DomainException($"Velocity cannot exceed {MaxValue} story points");
        }

        return new Velocity(value);
    }

    /// <summary>
    /// Gets a zero velocity instance.
    /// </summary>
    public static Velocity Zero => new(MinValue);

    /// <summary>
    /// Calculates the average velocity from multiple sprint velocities.
    /// </summary>
    /// <param name="velocities">Collection of velocity values</param>
    /// <returns>Average velocity</returns>
    public static Velocity CalculateAverage(IEnumerable<Velocity> velocities)
    {
        if (velocities == null || !velocities.Any())
            return Zero;

        var average = (int)Math.Round(velocities.Average(v => v.Value));
        return Create(average);
    }

    /// <summary>
    /// Checks if this velocity is within a percentage of another velocity.
    /// </summary>
    /// <param name="other">The other velocity to compare</param>
    /// <param name="tolerancePercentage">The tolerance percentage (e.g., 20 for 20%)</param>
    /// <returns>True if the velocities are within tolerance</returns>
    public bool IsWithinTolerance(Velocity other, double tolerancePercentage)
    {
        if (other == null) return false;
        
        var tolerance = other.Value * (tolerancePercentage / 100.0);
        var difference = Math.Abs(Value - other.Value);
        return difference <= tolerance;
    }

    /// <summary>
    /// Gets the performance level based on velocity value.
    /// </summary>
    /// <returns>String representation of performance level</returns>
    public string GetPerformanceLevel()
    {
        return Value switch
        {
            0 => "No Velocity",
            >= 1 and <= 10 => "Low",
            >= 11 and <= 30 => "Moderate",
            >= 31 and <= 60 => "High",
            _ => "Very High"
        };
    }

    public override string ToString() => $"{Value} story points";

    public static implicit operator int(Velocity velocity) => velocity.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}