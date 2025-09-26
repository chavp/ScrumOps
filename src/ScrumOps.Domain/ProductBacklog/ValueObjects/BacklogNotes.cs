using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.ProductBacklog.ValueObjects;

/// <summary>
/// Value object representing notes or description for a Product Backlog.
/// </summary>
public sealed class BacklogNotes : ValueObject
{
    public string Value { get; }

    private BacklogNotes(string value)
    {
        Value = value ?? string.Empty;
    }

    /// <summary>
    /// Creates BacklogNotes from a string value.
    /// </summary>
    public static BacklogNotes From(string value) => new(value?.Trim() ?? string.Empty);

    /// <summary>
    /// Creates BacklogNotes from a string value (alias for From).
    /// </summary>
    public static BacklogNotes Create(string value) => From(value);

    /// <summary>
    /// Gets an empty BacklogNotes instance.
    /// </summary>
    public static BacklogNotes Empty => new(string.Empty);

    /// <summary>
    /// Checks if the notes are empty.
    /// </summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>
    /// Gets the length of the notes.
    /// </summary>
    public int Length => Value.Length;

    public override string ToString() => Value;

    public static implicit operator string(BacklogNotes notes) => notes.Value;

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }
}