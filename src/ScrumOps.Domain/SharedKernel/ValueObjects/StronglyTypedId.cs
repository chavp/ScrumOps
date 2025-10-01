namespace ScrumOps.Domain.SharedKernel.ValueObjects;

/// <summary>
/// Base class for strongly typed identifiers.
/// Provides type safety and helps prevent mixing up different types of IDs.
/// </summary>
/// <typeparam name="T">The underlying type of the identifier (typically Guid)</typeparam>
public abstract class StronglyTypedId<T> : ValueObject, IEquatable<StronglyTypedId<T>>
    where T : IEquatable<T>
{
    /// <summary>
    /// Gets the underlying value of the identifier.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance of the StronglyTypedId class.
    /// </summary>
    /// <param name="value">The underlying value of the identifier</param>
    protected StronglyTypedId(T value)
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));
        
        Value = value;
    }

    /// <summary>
    /// Gets the atomic values that make up this value object.
    /// </summary>
    /// <returns>An enumerable of the atomic values</returns>
    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    /// <summary>
    /// Determines whether this instance is equal to another strongly typed ID.
    /// </summary>
    /// <param name="other">The other strongly typed ID to compare with</param>
    /// <returns>True if the IDs are equal, false otherwise</returns>
    public bool Equals(StronglyTypedId<T>? other)
    {
        return other is not null && Value.Equals(other.Value);
    }

    /// <summary>
    /// Determines whether this instance is equal to another object.
    /// </summary>
    /// <param name="obj">The other object to compare with</param>
    /// <returns>True if the objects are equal, false otherwise</returns>
    public override bool Equals(object? obj)
    {
        return obj is StronglyTypedId<T> other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code of this instance.
    /// </summary>
    /// <returns>The hash code</returns>
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    /// <summary>
    /// Returns a string representation of the identifier.
    /// </summary>
    /// <returns>A string representation of the identifier</returns>
    public override string ToString()
    {
        return Value.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Determines whether two strongly typed IDs are equal.
    /// </summary>
    /// <param name="left">The first ID to compare</param>
    /// <param name="right">The second ID to compare</param>
    /// <returns>True if the IDs are equal, false otherwise</returns>
    public static bool operator ==(StronglyTypedId<T>? left, StronglyTypedId<T>? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Determines whether two strongly typed IDs are not equal.
    /// </summary>
    /// <param name="left">The first ID to compare</param>
    /// <param name="right">The second ID to compare</param>
    /// <returns>True if the IDs are not equal, false otherwise</returns>
    public static bool operator !=(StronglyTypedId<T>? left, StronglyTypedId<T>? right)
    {
        return !Equals(left, right);
    }
}