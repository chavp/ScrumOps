using ScrumOps.Domain.SharedKernel;

namespace ScrumOps.Domain.EventManagement.ValueObjects;

/// <summary>
/// Value object representing the type of a sprint event.
/// </summary>
public class EventType : ValueObject
{
    public string Value { get; }

    private EventType(string value)
    {
        Value = value;
    }

    // Predefined event types according to Scrum framework
    public static EventType SprintPlanning => new("Sprint Planning");
    public static EventType DailyScrum => new("Daily Scrum");
    public static EventType SprintReview => new("Sprint Review");
    public static EventType SprintRetrospective => new("Sprint Retrospective");
    public static EventType BacklogRefinement => new("Backlog Refinement");

    public static EventType Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Event type cannot be null or empty.", nameof(value));

        if (value.Length > 100)
            throw new ArgumentException("Event type cannot exceed 100 characters.", nameof(value));

        return new EventType(value.Trim());
    }

    protected override IEnumerable<object?> GetAtomicValues()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}