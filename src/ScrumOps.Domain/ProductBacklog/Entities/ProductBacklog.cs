using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.Interfaces;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.ProductBacklog.Events;

namespace ScrumOps.Domain.ProductBacklog.Entities;

/// <summary>
/// Product Backlog aggregate root.
/// Manages the prioritized list of features, requirements, and enhancements for a product.
/// </summary>
public class ProductBacklog : Entity<ProductBacklogId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];
    private readonly List<ProductBacklogItem> _items = [];

    /// <summary>
    /// Gets the team that owns this product backlog.
    /// </summary>
    public TeamId TeamId { get; private set; }

    /// <summary>
    /// Gets the date and time when the product backlog was created.
    /// </summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets the date and time when the product backlog was last refined.
    /// </summary>
    public DateTime? LastRefinedDate { get; private set; }

    /// <summary>
    /// Gets the refinement notes for the product backlog.
    /// </summary>
    public BacklogNotes Notes { get; private set; }

    /// <summary>
    /// Gets the read-only collection of product backlog items.
    /// </summary>
    public IReadOnlyList<ProductBacklogItem> Items => _items.AsReadOnly();

    /// <summary>
    /// Gets the domain events that have been raised by this aggregate.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the ProductBacklog class.
    /// </summary>
    /// <param name="id">The unique identifier for the product backlog</param>
    /// <param name="teamId">The team identifier that owns this backlog</param>
    public ProductBacklog(ProductBacklogId id, TeamId teamId, BacklogNotes? notes = null) : base(id)
    {
        TeamId = teamId ?? throw new ArgumentNullException(nameof(teamId));
        CreatedDate = DateTime.UtcNow;
        Notes = BacklogNotes.Empty;
        if(notes != null)
            Notes = notes;
        RaiseDomainEvent(new ProductBacklogCreatedEvent(Id, TeamId, CreatedDate));
    }

    /// <summary>
    /// Parameterless constructor for Entity Framework Core.
    /// </summary>
    private ProductBacklog() : base()
    {
        Notes = BacklogNotes.Empty;
    }

    /// <summary>
    /// Adds a new item to the product backlog.
    /// </summary>
    /// <param name="item">The product backlog item to add</param>
    /// <exception cref="DomainException">Thrown when an item with the same title already exists</exception>
    public void AddItem(ProductBacklogItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item));

        if (_items.Any(i => i.Title.Value.Equals(item.Title.Value, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"An item with title '{item.Title}' already exists in the backlog");

        var nextPriority = _items.Count == 0 ? 1 : _items.Max(i => i.Priority.Value) + 1;
        item.SetPriority(Priority.Create(nextPriority));
        
        _items.Add(item);
        
        RaiseDomainEvent(new BacklogItemAddedEvent(Id, item.Id, item.Title.Value, nextPriority));
    }

    public ProductBacklog ApplyItemPriority(ProductBacklogItem item, int priority)
    {
        item.SetPriority(Priority.Create(priority));
        return this;
    }

    public ProductBacklog ApplyItemType(ProductBacklogItem item, BacklogItemType type)
    {
        item.SetType(type);
        return this;
    }

    /// <summary>
    /// Reorders items in the product backlog by updating their priorities.
    /// </summary>
    /// <param name="reorderData">Array of tuples containing item ID and new priority</param>
    /// <exception cref="DomainException">Thrown when trying to reorder non-existent items</exception>
    public void ReorderItems(IEnumerable<(ProductBacklogItemId ItemId, int NewPriority)> reorderData)
    {
        if (reorderData == null)
            throw new ArgumentNullException(nameof(reorderData));

        var reorderArray = reorderData.ToArray();
        
        foreach (var (itemId, newPriority) in reorderArray)
        {
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new DomainException($"Item with ID {itemId} not found in backlog");

            item.SetPriority(Priority.Create(newPriority));
        }

        RaiseDomainEvent(new BacklogReorderedEvent(Id, reorderArray.Select(r => new BacklogItemPriorityChange(r.ItemId, r.NewPriority)).ToList()));
    }

    /// <summary>
    /// Marks the product backlog as refined with updated notes.
    /// </summary>
    /// <param name="refinedDate">The date when refinement occurred</param>
    /// <param name="notes">The refinement notes</param>
    public void MarkAsRefined(DateTime refinedDate, BacklogNotes notes)
    {
        LastRefinedDate = refinedDate;
        Notes = notes ?? throw new ArgumentNullException(nameof(notes));
        
        RaiseDomainEvent(new BacklogRefinedEvent(Id, refinedDate, notes.Value));
    }

    /// <summary>
    /// Removes an item from the product backlog.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove</param>
    /// <exception cref="DomainException">Thrown when the item is not found or cannot be removed</exception>
    public void RemoveItem(ProductBacklogItemId itemId)
    {
        var item = _items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException($"Item with ID {itemId} not found in backlog");

        if (item.Status == BacklogItemStatus.InProgress)
            throw new DomainException("Cannot remove an item that is in progress");

        _items.Remove(item);
        
        RaiseDomainEvent(new BacklogItemRemovedEvent(Id, itemId, item.Title.Value));
    }

    /// <summary>
    /// Gets items by their current status.
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <returns>Collection of items with the specified status</returns>
    public IEnumerable<ProductBacklogItem> GetItemsByStatus(BacklogItemStatus status)
    {
        return _items.Where(i => i.Status == status);
    }

    /// <summary>
    /// Gets the total story points for items with a specific status.
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <returns>Total story points for items with the specified status</returns>
    public int GetTotalStoryPointsByStatus(BacklogItemStatus status)
    {
        return _items
            .Where(i => i.Status == status && i.StoryPoints != null)
            .Sum(i => i.StoryPoints!.Value);
    }

    /// <summary>
    /// Clears all domain events from this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Raises a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to raise</param>
    private void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}
