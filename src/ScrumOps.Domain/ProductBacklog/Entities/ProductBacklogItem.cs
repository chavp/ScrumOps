using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Entities;

/// <summary>
/// Represents an individual item in the product backlog.
/// Contains features, user stories, enhancements, or bug fixes that need to be implemented.
/// </summary>
public class ProductBacklogItem : Entity<ProductBacklogItemId>
{
    /// <summary>
    /// Gets the product backlog that this item belongs to.
    /// </summary>
    public ProductBacklogId ProductBacklogId { get; private set; }

    /// <summary>
    /// Gets the title of the backlog item.
    /// </summary>
    public ItemTitle Title { get; private set; }

    /// <summary>
    /// Gets the detailed description of the backlog item.
    /// </summary>
    public ItemDescription Description { get; private set; }

    /// <summary>
    /// Gets the type of this backlog item (User Story, Bug, Epic, etc.).
    /// </summary>
    public BacklogItemType Type { get; private set; }

    /// <summary>
    /// Gets the current priority of this item in the backlog.
    /// </summary>
    public Priority Priority { get; private set; }

    /// <summary>
    /// Gets the story points estimate for this item, if estimated.
    /// </summary>
    public StoryPoints? StoryPoints { get; private set; }

    /// <summary>
    /// Gets the current status of this backlog item.
    /// </summary>
    public BacklogItemStatus Status { get; private set; }

    /// <summary>
    /// Gets the acceptance criteria for this item, if defined.
    /// </summary>
    public AcceptanceCriteria? AcceptanceCriteria { get; private set; }

    /// <summary>
    /// Gets the name of the person who created this item.
    /// </summary>
    public UserName CreatedBy { get; private set; }

    /// <summary>
    /// Gets the date and time when this item was created.
    /// </summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets the date and time when this item was last modified.
    /// </summary>
    public DateTime LastModifiedDate { get; private set; }

    /// <summary>
    /// Initializes a new instance of the ProductBacklogItem class.
    /// </summary>
    /// <param name="id">The unique identifier for the backlog item</param>
    /// <param name="productBacklogId">The product backlog this item belongs to</param>
    /// <param name="title">The title of the backlog item</param>
    /// <param name="description">The description of the backlog item</param>
    /// <param name="type">The type of backlog item</param>
    /// <param name="createdBy">The name of the person who created this item</param>
    public ProductBacklogItem(
        ProductBacklogItemId id,
        ProductBacklogId productBacklogId,
        ItemTitle title,
        ItemDescription description,
        BacklogItemType type,
        UserName createdBy) : base(id)
    {
        ProductBacklogId = productBacklogId ?? throw new ArgumentNullException(nameof(productBacklogId));
        Title = title ?? throw new ArgumentNullException(nameof(title));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Type = type;
        CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        CreatedDate = DateTime.UtcNow;
        LastModifiedDate = DateTime.UtcNow;
        Status = BacklogItemStatus.New;
        Priority = Priority.Lowest; // Will be set by the ProductBacklog when added
    }

    /// <summary>
    /// Parameterless constructor for Entity Framework Core.
    /// </summary>
    private ProductBacklogItem() : base()
    {
        Title = default!;
        Description = default!;
        CreatedBy = default!;
        ProductBacklogId = default!;
        Priority = default!;
    }

    /// <summary>
    /// Sets the priority of this backlog item.
    /// This should only be called by the ProductBacklog aggregate.
    /// </summary>
    /// <param name="priority">The new priority</param>
    internal void SetPriority(Priority priority)
    {
        Priority = priority ?? throw new ArgumentNullException(nameof(priority));
        LastModifiedDate = DateTime.UtcNow;
    }

    internal void SetType(BacklogItemType backlogItemType)
    {
        Type = backlogItemType;
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Estimates the story points for this backlog item.
    /// </summary>
    /// <param name="storyPoints">The story points estimate</param>
    public void EstimateStoryPoints(StoryPoints storyPoints)
    {
        StoryPoints = storyPoints ?? throw new ArgumentNullException(nameof(storyPoints));
        
        // Once estimated, the item becomes ready for sprint planning
        if (Status == BacklogItemStatus.New)
        {
            Status = BacklogItemStatus.Ready;
        }
        
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds or updates the acceptance criteria for this backlog item.
    /// </summary>
    /// <param name="acceptanceCriteria">The acceptance criteria</param>
    public void SetAcceptanceCriteria(AcceptanceCriteria acceptanceCriteria)
    {
        AcceptanceCriteria = acceptanceCriteria ?? throw new ArgumentNullException(nameof(acceptanceCriteria));
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the title of the backlog item.
    /// </summary>
    /// <param name="title">The new title</param>
    /// <exception cref="DomainException">Thrown when trying to modify a completed item</exception>
    public void UpdateTitle(ItemTitle title)
    {
        if (Status == BacklogItemStatus.Done)
            throw new DomainException("Cannot modify a completed backlog item");

        Title = title ?? throw new ArgumentNullException(nameof(title));
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the description of the backlog item.
    /// </summary>
    /// <param name="description">The new description</param>
    /// <exception cref="DomainException">Thrown when trying to modify a completed item</exception>
    public void UpdateDescription(ItemDescription description)
    {
        if (Status == BacklogItemStatus.Done)
            throw new DomainException("Cannot modify a completed backlog item");

        Description = description ?? throw new ArgumentNullException(nameof(description));
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks this backlog item as in progress.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the item is not ready to be started</exception>
    public void MarkAsInProgress()
    {
        if (Status != BacklogItemStatus.Ready)
            throw new DomainException("Only ready items can be marked as in progress");

        Status = BacklogItemStatus.InProgress;
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks this backlog item as done.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the item is not in progress</exception>
    public void MarkAsDone()
    {
        if (Status != BacklogItemStatus.InProgress)
            throw new DomainException("Only items in progress can be marked as done");

        Status = BacklogItemStatus.Done;
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Resets the backlog item status back to ready.
    /// </summary>
    /// <exception cref="DomainException">Thrown when the item cannot be reset</exception>
    public void ResetToReady()
    {
        if (Status == BacklogItemStatus.Done)
            throw new DomainException("Cannot reset a completed item");

        if (StoryPoints == null)
            throw new DomainException("Cannot reset an item without story points estimation");

        Status = BacklogItemStatus.Ready;
        LastModifiedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if this item is ready for sprint planning.
    /// </summary>
    /// <returns>True if the item has story points and acceptance criteria defined</returns>
    public bool IsReadyForSprint()
    {
        return Status == BacklogItemStatus.Ready 
               && StoryPoints != null 
               && AcceptanceCriteria != null;
    }

    /// <summary>
    /// Gets the age of this backlog item in days.
    /// </summary>
    /// <returns>Number of days since the item was created</returns>
    public int GetAgeInDays()
    {
        return (DateTime.UtcNow - CreatedDate).Days;
    }
}
