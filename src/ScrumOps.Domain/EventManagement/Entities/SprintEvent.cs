using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Interfaces;
using ScrumOps.Domain.SharedKernel.Events;
using ScrumOps.Domain.EventManagement.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.EventManagement.Entities;

/// <summary>
/// Aggregate root representing a sprint event in the Scrum framework.
/// Sprint events are time-boxed activities that occur during a sprint.
/// </summary>
public class SprintEvent : Entity<SprintEventId>, IAggregateRoot
{
    private readonly List<EventParticipant> _participants = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    public SprintId SprintId { get; private set; }
    public EventType EventType { get; private set; }
    public TimeBox TimeBox { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public UserId? FacilitatorId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public bool IsCancelled { get; private set; }
    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<EventParticipant> Participants => _participants.AsReadOnly();

    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private SprintEvent(
        SprintEventId id,
        SprintId sprintId,
        EventType eventType,
        TimeBox timeBox,
        string title) : base(id)
    {
        SprintId = sprintId;
        EventType = eventType;
        TimeBox = timeBox;
        Title = title;
        CreatedAt = DateTime.UtcNow;
        IsCancelled = false;
    }

    // Parameterless constructor for EF Core
    private SprintEvent() : base(SprintEventId.New()) { }

    public static SprintEvent Create(
        SprintId sprintId,
        EventType eventType,
        TimeBox timeBox,
        string title,
        string? description = null,
        string? location = null,
        UserId? facilitatorId = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Event title cannot be null or empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Event title cannot exceed 200 characters.", nameof(title));

        var sprintEvent = new SprintEvent(SprintEventId.New(), sprintId, eventType, timeBox, title.Trim())
        {
            Description = description?.Trim(),
            Location = location?.Trim(),
            FacilitatorId = facilitatorId
        };

        return sprintEvent;
    }

    public void UpdateDetails(string title, string? description = null, string? location = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Event title cannot be null or empty.", nameof(title));

        if (title.Length > 200)
            throw new ArgumentException("Event title cannot exceed 200 characters.", nameof(title));

        if (IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled event.");

        Title = title.Trim();
        Description = description?.Trim();
        Location = location?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimeBox(TimeBox newTimeBox)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot update time box of a cancelled event.");

        TimeBox = newTimeBox;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignFacilitator(UserId facilitatorId)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot assign facilitator to a cancelled event.");

        FacilitatorId = facilitatorId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFacilitator()
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot remove facilitator from a cancelled event.");

        FacilitatorId = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddParticipant(UserId userId, bool isRequired = true)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot add participants to a cancelled event.");

        if (_participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("User is already a participant in this event.");

        var participant = EventParticipant.Create(Id, userId, isRequired);
        _participants.Add(participant);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveParticipant(UserId userId)
    {
        if (IsCancelled)
            throw new InvalidOperationException("Cannot remove participants from a cancelled event.");

        var participant = _participants.FirstOrDefault(p => p.UserId == userId);
        if (participant == null)
            throw new InvalidOperationException("User is not a participant in this event.");

        _participants.Remove(participant);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason cannot be null or empty.", nameof(reason));

        if (IsCancelled)
            throw new InvalidOperationException("Event is already cancelled.");

        IsCancelled = true;
        CancellationReason = reason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        if (!IsCancelled)
            throw new InvalidOperationException("Event is not cancelled.");

        IsCancelled = false;
        CancellationReason = null;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsActive(DateTime currentTime) => TimeBox.IsActive(currentTime) && !IsCancelled;
    public bool HasStarted(DateTime currentTime) => TimeBox.HasStarted(currentTime);
    public bool HasEnded(DateTime currentTime) => TimeBox.HasEnded(currentTime);

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}