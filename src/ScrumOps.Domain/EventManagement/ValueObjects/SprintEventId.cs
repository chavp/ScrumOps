using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.EventManagement.ValueObjects;

/// <summary>
/// Value object representing a unique identifier for a sprint event.
/// </summary>
public class SprintEventId : ValueObject
{
    public Guid Value { get; }

    public SprintEventId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Sprint event ID cannot be empty.", nameof(value));
        
        Value = value;
    }

    public static SprintEventId New() => new(Guid.NewGuid());

    public static implicit operator Guid(SprintEventId id) => id.Value;
    public static implicit operator SprintEventId(Guid id) => new(id);

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}