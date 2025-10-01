using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.Interfaces;

namespace ScrumOps.Domain.SharedKernel;

/// <summary>
/// Base class for aggregate roots in the domain.
/// Aggregate roots are the entry points to aggregates and are responsible for maintaining business invariants.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root identifier</typeparam>
public abstract class AggregateRoot<TId> : Entity<TId>, IAggregateRoot
    where TId : class
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <summary>
    /// Gets the read-only list of domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the AggregateRoot class.
    /// </summary>
    /// <param name="id">The unique identifier for the aggregate root</param>
    protected AggregateRoot(TId id) : base(id)
    {
    }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    protected AggregateRoot() : base()
    {
    }

    /// <summary>
    /// Adds a domain event to the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event to add</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}