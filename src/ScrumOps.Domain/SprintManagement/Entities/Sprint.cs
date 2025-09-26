using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.Interfaces;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.Events;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using SprintVelocity = ScrumOps.Domain.SprintManagement.ValueObjects.Velocity;

namespace ScrumOps.Domain.SprintManagement.Entities;

/// <summary>
/// Sprint aggregate root representing a time-boxed iteration in Scrum.
/// Manages sprint lifecycle, backlog items, and sprint execution.
/// </summary>
public class Sprint : Entity<SprintId>, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new();
    private readonly List<SprintBacklogItem> _backlogItems = new();

    /// <summary>
    /// Gets the ID of the team executing this sprint.
    /// </summary>
    public TeamId TeamId { get; private set; }

    /// <summary>
    /// Gets the sprint goal.
    /// </summary>
    public SprintGoal Goal { get; private set; }

    /// <summary>
    /// Gets the planned start date of the sprint.
    /// </summary>
    public DateTime StartDate { get; private set; }

    /// <summary>
    /// Gets the planned end date of the sprint.
    /// </summary>
    public DateTime EndDate { get; private set; }

    /// <summary>
    /// Gets the current status of the sprint.
    /// </summary>
    public SprintStatus Status { get; private set; }

    /// <summary>
    /// Gets the team's capacity for this sprint.
    /// </summary>
    public Capacity Capacity { get; private set; }

    /// <summary>
    /// Gets the actual velocity achieved (set when sprint completes).
    /// </summary>
    public SprintVelocity? ActualVelocity { get; private set; }

    /// <summary>
    /// Gets the date when the sprint was created.
    /// </summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets the date when the sprint was actually started.
    /// </summary>
    public DateTime? ActualStartDate { get; private set; }

    /// <summary>
    /// Gets the date when the sprint was actually completed.
    /// </summary>
    public DateTime? ActualEndDate { get; private set; }

    /// <summary>
    /// Gets the read-only list of backlog items in this sprint.
    /// </summary>
    public IReadOnlyList<SprintBacklogItem> BacklogItems => _backlogItems.AsReadOnly();

    /// <summary>
    /// Gets the read-only list of domain events.
    /// </summary>
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Gets the sprint length in days.
    /// </summary>
    public int LengthInDays => (EndDate - StartDate).Days + 1;

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private Sprint() : base()
    {
        TeamId = default!;
        Goal = default!;
        Capacity = default!;
    }

    /// <summary>
    /// Initializes a new instance of the Sprint class.
    /// </summary>
    /// <param name="id">The unique identifier for the sprint</param>
    /// <param name="teamId">The ID of the team executing this sprint</param>
    /// <param name="goal">The sprint goal</param>
    /// <param name="startDate">The planned start date</param>
    /// <param name="endDate">The planned end date</param>
    /// <param name="capacity">The team's capacity for this sprint</param>
    public Sprint(SprintId id, TeamId teamId, SprintGoal goal, 
                  DateTime startDate, DateTime endDate, Capacity capacity)
        : base(id)
    {
        if (endDate <= startDate)
        {
            throw new DomainException("Sprint end date must be after start date");
        }

        var lengthInDays = (endDate - startDate).Days + 1;
        if (lengthInDays < 7 || lengthInDays > 28)
        {
            throw new DomainException("Sprint length must be between 1 and 4 weeks");
        }

        TeamId = teamId;
        Goal = goal;
        StartDate = startDate;
        EndDate = endDate;
        Capacity = capacity;
        Status = SprintStatus.Planning;
        CreatedDate = DateTime.UtcNow;

        _domainEvents.Add(new SprintCreatedEvent(Id, TeamId, Goal.Value, StartDate, EndDate));
    }

    /// <summary>
    /// Starts the sprint, moving it from Planning to Active status.
    /// </summary>
    public void Start()
    {
        if (Status != SprintStatus.Planning)
        {
            throw new DomainException("Sprint has already started or is completed");
        }

        // Allow starting empty sprints (though not recommended in practice)
        Status = SprintStatus.Active;
        ActualStartDate = DateTime.UtcNow;

        _domainEvents.Add(new SprintStartedEvent(Id, ActualStartDate.Value));
    }

    /// <summary>
    /// Completes the sprint with the actual velocity achieved.
    /// </summary>
    /// <param name="actualVelocity">The velocity actually achieved during the sprint</param>
    public void Complete(SprintVelocity actualVelocity)
    {
        if (Status != SprintStatus.Active)
        {
            throw new DomainException("Sprint is not active and cannot be completed");
        }

        Status = SprintStatus.Completed;
        ActualVelocity = actualVelocity;
        ActualEndDate = DateTime.UtcNow;

        var completedItems = _backlogItems.Count(item => item.IsCompleted);

        _domainEvents.Add(new SprintCompletedEvent(Id, ActualEndDate.Value, actualVelocity, completedItems));
    }

    /// <summary>
    /// Cancels the sprint before completion.
    /// </summary>
    /// <param name="reason">The reason for cancellation</param>
    public void Cancel(string reason = "")
    {
        if (Status == SprintStatus.Completed || Status == SprintStatus.Cancelled)
        {
            throw new DomainException("Cannot cancel a completed or already cancelled sprint");
        }

        Status = SprintStatus.Cancelled;
        ActualEndDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a backlog item to the sprint.
    /// </summary>
    /// <param name="backlogItem">The backlog item to add</param>
    public void AddBacklogItem(SprintBacklogItem backlogItem)
    {
        if (Status != SprintStatus.Planning)
        {
            throw new DomainException("Sprint cannot add items when active or completed");
        }

        if (_backlogItems.Any(item => item.ProductBacklogItemId.Equals(backlogItem.ProductBacklogItemId)))
        {
            throw new DomainException("This product backlog item is already in the sprint");
        }

        _backlogItems.Add(backlogItem);

        _domainEvents.Add(new BacklogItemAddedToSprintEvent(
            Id, backlogItem.Id, backlogItem.ProductBacklogItemId, backlogItem.StoryPoints));
    }

    /// <summary>
    /// Removes a backlog item from the sprint.
    /// </summary>
    /// <param name="backlogItemId">The ID of the backlog item to remove</param>
    public void RemoveBacklogItem(SprintBacklogItemId backlogItemId)
    {
        if (Status != SprintStatus.Planning)
        {
            throw new DomainException("Can only remove backlog items during sprint planning");
        }

        var item = _backlogItems.FirstOrDefault(i => i.Id.Equals(backlogItemId));
        if (item == null)
        {
            throw new DomainException("Backlog item not found in this sprint");
        }

        _backlogItems.Remove(item);
    }

    /// <summary>
    /// Calculates the total remaining work across all backlog items.
    /// </summary>
    /// <returns>The sum of remaining work from all backlog items</returns>
    public int CalculateRemainingWork()
    {
        return _backlogItems.Sum(item => item.RemainingWork);
    }

    /// <summary>
    /// Calculates the total story points committed in this sprint.
    /// </summary>
    /// <returns>The sum of story points from all backlog items</returns>
    public int CalculateCommittedStoryPoints()
    {
        return _backlogItems.Sum(item => item.StoryPoints.Value);
    }

    /// <summary>
    /// Calculates the completed story points.
    /// </summary>
    /// <returns>The sum of story points from completed backlog items</returns>
    public int CalculateCompletedStoryPoints()
    {
        return _backlogItems.Where(item => item.IsCompleted).Sum(item => item.StoryPoints.Value);
    }

    /// <summary>
    /// Gets the sprint progress as a percentage.
    /// </summary>
    /// <returns>Progress percentage (0-100)</returns>
    public decimal GetProgressPercentage()
    {
        var totalStoryPoints = CalculateCommittedStoryPoints();
        if (totalStoryPoints == 0)
        {
            return 0;
        }

        var completedStoryPoints = CalculateCompletedStoryPoints();
        return (decimal)completedStoryPoints / totalStoryPoints * 100;
    }

    /// <summary>
    /// Updates the sprint goal.
    /// </summary>
    /// <param name="newGoal">The new sprint goal</param>
    public void UpdateGoal(SprintGoal newGoal)
    {
        if (Status == SprintStatus.Completed || Status == SprintStatus.Cancelled)
        {
            throw new DomainException("Cannot update goal of completed or cancelled sprint");
        }

        Goal = newGoal;
    }

    /// <summary>
    /// Moves the sprint to review phase.
    /// </summary>
    public void StartReview()
    {
        if (Status != SprintStatus.Active)
        {
            throw new DomainException("Only active sprints can move to review");
        }

        Status = SprintStatus.Review;
    }

    /// <summary>
    /// Moves the sprint to retrospective phase.
    /// </summary>
    public void StartRetrospective()
    {
        if (Status != SprintStatus.Review)
        {
            throw new DomainException("Sprint must be in review to start retrospective");
        }

        Status = SprintStatus.Retrospective;
    }

    /// <summary>
    /// Clears all domain events from this aggregate.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}