namespace ScrumOps.Domain.SharedKernel;

/// <summary>
/// Base class for value objects in the domain.
/// Value objects are objects that are distinguished by their attributes rather than their identity.
/// They are immutable and their equality is based on their attribute values.
/// </summary>
public abstract class ValueObject
{
    /// <summary>
    /// Gets the atomic values that make up this value object.
    /// These values are used for equality comparison and hash code generation.
    /// </summary>
    /// <returns>An enumerable of the atomic values</returns>
    protected abstract IEnumerable<object?> GetAtomicValues();

    /// <summary>
    /// Determines whether two value objects are equal based on their atomic values.
    /// </summary>
    /// <param name="obj">The object to compare with</param>
    /// <returns>True if the value objects have the same atomic values, false otherwise</returns>
    public override bool Equals(object? obj)
    {
        return obj is ValueObject other && GetAtomicValues().SequenceEqual(other.GetAtomicValues());
    }

    /// <summary>
    /// Returns the hash code for this value object based on its atomic values.
    /// </summary>
    /// <returns>A hash code for this value object</returns>
    public override int GetHashCode()
    {
        return GetAtomicValues()
            .Aggregate(1, (current, obj) => 
                current * 23 + (obj?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Determines whether two value objects are equal.
    /// </summary>
    /// <param name="left">The first value object to compare</param>
    /// <param name="right">The second value object to compare</param>
    /// <returns>True if the value objects are equal, false otherwise</returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two value objects are not equal.
    /// </summary>
    /// <param name="left">The first value object to compare</param>
    /// <param name="right">The second value object to compare</param>
    /// <returns>True if the value objects are not equal, false otherwise</returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !Equals(left, right);
    }
}