using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.SprintManagement.ValueObjects;

/// <summary>
/// Value object representing story points for sprint backlog items.
/// Follows Fibonacci sequence for estimation consistency.
/// </summary>
public class StoryPoints : ValueObject
{
    /// <summary>
    /// Gets the story points value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Valid story point values following Fibonacci sequence.
    /// </summary>
    private static readonly int[] ValidPoints = { 1, 2, 3, 5, 8, 13, 21, 34 };

    /// <summary>
    /// Private constructor to enforce creation through factory method.
    /// </summary>
    /// <param name="value">The story points value</param>
    private StoryPoints(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new StoryPoints value object with validation.
    /// </summary>
    /// <param name="points">The story points value</param>
    /// <returns>A new StoryPoints value object</returns>
    /// <exception cref="DomainException">Thrown when the story points are invalid</exception>
    public static StoryPoints Create(int points)
    {
        if (!ValidPoints.Contains(points))
        {
            throw new DomainException($"Story points must be one of: {string.Join(", ", ValidPoints)}");
        }

        return new StoryPoints(points);
    }

    /// <summary>
    /// Gets the atomic values for equality comparison.
    /// </summary>
    /// <returns>The story points value</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Returns the string representation of the story points.
    /// </summary>
    /// <returns>The story points value</returns>
    public override string ToString() => $"{Value} story points";
}