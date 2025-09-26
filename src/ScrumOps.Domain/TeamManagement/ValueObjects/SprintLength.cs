using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.TeamManagement.ValueObjects;

/// <summary>
/// Value object representing the length of a sprint in weeks.
/// Ensures sprint lengths are within the acceptable range for Scrum practices.
/// </summary>
public class SprintLength : ValueObject
{
    /// <summary>
    /// Gets the sprint length in weeks.
    /// </summary>
    public int Weeks { get; }

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="weeks">The sprint length in weeks</param>
    private SprintLength(int weeks)
    {
        Weeks = weeks;
    }

    /// <summary>
    /// Creates a new SprintLength value object with validation.
    /// </summary>
    /// <param name="weeks">The sprint length in weeks</param>
    /// <returns>A new SprintLength value object</returns>
    /// <exception cref="DomainException">Thrown when the sprint length is invalid</exception>
    public static SprintLength Create(int weeks)
    {
        if (weeks < 1 || weeks > 4)
        {
            throw new DomainException("Sprint length must be between 1 and 4 weeks");
        }

        return new SprintLength(weeks);
    }

    /// <summary>
    /// Gets the duration of the sprint as a TimeSpan.
    /// </summary>
    public TimeSpan Duration => TimeSpan.FromDays(Weeks * 7);

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The sprint length in weeks</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Weeks;
    }

    /// <summary>
    /// Returns the string representation of the sprint length.
    /// </summary>
    /// <returns>The sprint length description</returns>
    public override string ToString() => $"{Weeks} week{(Weeks == 1 ? "" : "s")}";
}