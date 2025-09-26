namespace ScrumOps.Domain.SharedKernel.Events;

/// <summary>
/// Marker interface for domain events.
/// Domain events represent something significant that happened in the domain.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when this domain event occurred.
    /// </summary>
    DateTime OccurredOn { get; }
}