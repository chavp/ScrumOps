using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.EventManagement.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Domain.EventManagement.Entities;

/// <summary>
/// Entity representing a participant in a sprint event.
/// </summary>
public class EventParticipant : Entity<Guid>
{
    public SprintEventId SprintEventId { get; private set; }
    public UserId UserId { get; private set; }
    public bool IsRequired { get; private set; }
    public bool HasAccepted { get; private set; }
    public DateTime? AcceptedAt { get; private set; }
    public DateTime? DeclinedAt { get; private set; }
    public string? DeclineReason { get; private set; }
    public DateTime AddedAt { get; private set; }

    private EventParticipant(
        Guid id,
        SprintEventId sprintEventId,
        UserId userId,
        bool isRequired) : base(id)
    {
        SprintEventId = sprintEventId;
        UserId = userId;
        IsRequired = isRequired;
        HasAccepted = false;
        AddedAt = DateTime.UtcNow;
    }

    // Parameterless constructor for EF Core
    private EventParticipant() : base(Guid.NewGuid()) { }

    public static EventParticipant Create(
        SprintEventId sprintEventId,
        UserId userId,
        bool isRequired = true)
    {
        return new EventParticipant(Guid.NewGuid(), sprintEventId, userId, isRequired);
    }

    public void Accept()
    {
        if (HasAccepted)
            throw new InvalidOperationException("Participant has already accepted the event.");

        if (DeclinedAt.HasValue)
            throw new InvalidOperationException("Cannot accept an event that was previously declined. Remove and re-add the participant.");

        HasAccepted = true;
        AcceptedAt = DateTime.UtcNow;
    }

    public void Decline(string? reason = null)
    {
        if (DeclinedAt.HasValue)
            throw new InvalidOperationException("Participant has already declined the event.");

        HasAccepted = false;
        AcceptedAt = null;
        DeclinedAt = DateTime.UtcNow;
        DeclineReason = reason?.Trim();
    }

    public void UpdateRequirement(bool isRequired)
    {
        IsRequired = isRequired;
    }

    public ParticipationStatus GetStatus()
    {
        if (DeclinedAt.HasValue)
            return ParticipationStatus.Declined;
        
        if (HasAccepted)
            return ParticipationStatus.Accepted;
        
        return ParticipationStatus.Pending;
    }
}

/// <summary>
/// Enumeration representing the participation status of an event participant.
/// </summary>
public enum ParticipationStatus
{
    Pending,
    Accepted,
    Declined
}