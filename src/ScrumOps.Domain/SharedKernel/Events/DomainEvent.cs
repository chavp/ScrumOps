namespace ScrumOps.Domain.SharedKernel.Events;

/// <summary>
/// Base class for domain events.
/// Provides the OccurredOn timestamp that all domain events must have.
/// </summary>
public abstract record DomainEvent(DateTime OccurredOn) : IDomainEvent
{
    /// <summary>
    /// Creates a domain event with the current UTC timestamp.
    /// </summary>
    protected DomainEvent() : this(DateTime.UtcNow)
    {
    }
}