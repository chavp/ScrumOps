using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing story points estimation for a Product Backlog Item.
/// Story points are a unit of measure for expressing an estimate of the overall effort
/// that will be required to fully implement a product backlog item.
/// </summary>
public sealed class StoryPoints : ValueObject
{
    /// <summary>
    /// Valid story points values following a modified Fibonacci sequence.
    /// </summary>
    public static readonly int[] ValidValues = { 1, 2, 3, 5, 8, 13, 21, 34, 55, 89 };

    public int Value { get; }

    private StoryPoints(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates StoryPoints from an integer value.
    /// </summary>
    /// <param name="value">The story points value</param>
    /// <returns>A new StoryPoints instance</returns>
    /// <exception cref="DomainException">Thrown when the value is not valid</exception>
    public static StoryPoints Create(int value)
    {
        if (!ValidValues.Contains(value))
        {
            throw new DomainException($"Story points must be one of: {string.Join(", ", ValidValues)}");
        }

        return new StoryPoints(value);
    }

    /// <summary>
    /// Gets all valid story points values.
    /// </summary>
    /// <returns>Array of valid story points values</returns>
    public static int[] GetValidValues() => ValidValues.ToArray();

    /// <summary>
    /// Checks if a value is a valid story points value.
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <returns>True if the value is valid, false otherwise</returns>
    public static bool IsValidValue(int value) => ValidValues.Contains(value);

    /// <summary>
    /// Gets the complexity level based on story points value.
    /// </summary>
    /// <returns>String representation of complexity</returns>
    public string GetComplexityLevel()
    {
        return Value switch
        {
            1 or 2 or 3 => "Low",
            5 or 8 => "Medium",
            13 or 21 => "High",
            _ => "Very High"
        };
    }

    public override string ToString() => Value.ToString();

    public static implicit operator int(StoryPoints storyPoints) => storyPoints.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}