# Domain Events Catalog

**Created**: 2025-01-27  
**Purpose**: Comprehensive catalog of domain events in the ScrumOps system  
**Architecture**: Domain Driven Design with event-driven architecture

## Domain Event Framework

### Base Domain Event Types
```csharp
public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}

public abstract record DomainEvent(DateTime OccurredOn) : IDomainEvent
{
    protected DomainEvent() : this(DateTime.UtcNow) { }
}

public interface IDomainEventHandler<in TDomainEvent> where TDomainEvent : IDomainEvent
{
    Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
```

## Team Management Context Events

### Team Lifecycle Events
```csharp
public record TeamCreatedEvent(
    TeamId TeamId,
    string TeamName,
    DateTime CreatedDate) : DomainEvent;

public record TeamDeactivatedEvent(
    TeamId TeamId,
    string Reason,
    DateTime DeactivatedDate) : DomainEvent;

public record TeamVelocityUpdatedEvent(
    TeamId TeamId,
    Velocity PreviousVelocity,
    Velocity NewVelocity) : DomainEvent;
```

### Team Membership Events
```csharp
public record MemberAddedToTeamEvent(
    TeamId TeamId,
    UserId UserId,
    ScrumRole Role) : DomainEvent;

public record MemberRemovedFromTeamEvent(
    TeamId TeamId,
    UserId UserId,
    string Reason) : DomainEvent;

public record MemberRoleChangedEvent(
    TeamId TeamId,
    UserId UserId,
    ScrumRole PreviousRole,
    ScrumRole NewRole) : DomainEvent;
```

## Product Backlog Context Events

### Backlog Management Events
```csharp
public record BacklogItemAddedEvent(
    ProductBacklogId BacklogId,
    ProductBacklogItemId ItemId,
    string Title) : DomainEvent;

public record BacklogItemRemovedEvent(
    ProductBacklogId BacklogId,
    ProductBacklogItemId ItemId,
    string Reason) : DomainEvent;

public record BacklogReorderedEvent(
    ProductBacklogId BacklogId,
    DateTime ReorderedDate) : DomainEvent;

public record BacklogRefinedEvent(
    ProductBacklogId BacklogId,
    DateTime RefinedDate) : DomainEvent;
```

### Backlog Item Lifecycle Events
```csharp
public record BacklogItemEstimatedEvent(
    ProductBacklogItemId ItemId,
    StoryPoints? PreviousEstimate,
    StoryPoints NewEstimate,
    UserId EstimatedBy) : DomainEvent;

public record BacklogItemUpdatedEvent(
    ProductBacklogItemId ItemId,
    string FieldName,
    string? PreviousValue,
    string NewValue,
    UserId UpdatedBy) : DomainEvent;

public record BacklogItemStatusChangedEvent(
    ProductBacklogItemId ItemId,
    BacklogItemStatus PreviousStatus,
    BacklogItemStatus NewStatus) : DomainEvent;
```

## Sprint Management Context Events

### Sprint Lifecycle Events
```csharp
public record SprintCreatedEvent(
    SprintId SprintId,
    TeamId TeamId,
    string SprintName,
    SprintGoal Goal,
    DateRange Duration) : DomainEvent;

public record SprintStartedEvent(
    SprintId SprintId,
    DateTime StartedAt) : DomainEvent;

public record SprintCompletedEvent(
    SprintId SprintId,
    DateTime CompletedAt,
    Velocity ActualVelocity,
    int CompletedStoryPoints,
    int TotalStoryPoints) : DomainEvent;

public record SprintCancelledEvent(
    SprintId SprintId,
    string Reason,
    DateTime CancelledAt) : DomainEvent;
```

### Sprint Backlog Events
```csharp
public record ItemAddedToSprintEvent(
    SprintId SprintId,
    ProductBacklogItemId ItemId) : DomainEvent;

public record ItemRemovedFromSprintEvent(
    SprintId SprintId,
    ProductBacklogItemId ItemId,
    string Reason) : DomainEvent;

public record SprintBacklogItemStatusChangedEvent(
    SprintId SprintId,
    SprintBacklogItemId ItemId,
    SprintBacklogStatus PreviousStatus,
    SprintBacklogStatus NewStatus) : DomainEvent;
```

### Task Management Events
```csharp
public record TaskCreatedEvent(
    TaskId TaskId,
    SprintBacklogItemId ParentItemId,
    string TaskTitle,
    int EstimatedHours) : DomainEvent;

public record TaskAssignedEvent(
    TaskId TaskId,
    UserId? PreviousAssignee,
    UserId NewAssignee) : DomainEvent;

public record TaskStatusChangedEvent(
    TaskId TaskId,
    TaskStatus PreviousStatus,
    TaskStatus NewStatus,
    UserId? ChangedBy) : DomainEvent;

public record TaskCompletedEvent(
    TaskId TaskId,
    UserId CompletedBy,
    DateTime CompletedAt,
    int ActualHours) : DomainEvent;
```

### Impediment Events
```csharp
public record ImpedimentReportedEvent(
    ImpedimentId ImpedimentId,
    SprintId SprintId,
    string Title,
    ImpedimentSeverity Severity,
    UserId ReportedBy) : DomainEvent;

public record ImpedimentResolvedEvent(
    ImpedimentId ImpedimentId,
    string Resolution,
    UserId ResolvedBy,
    DateTime ResolvedAt) : DomainEvent;

public record ImpedimentEscalatedEvent(
    ImpedimentId ImpedimentId,
    ImpedimentSeverity PreviousSeverity,
    ImpedimentSeverity NewSeverity,
    string Reason) : DomainEvent;
```

## Event Management Context Events

### Sprint Event Lifecycle
```csharp
public record SprintEventScheduledEvent(
    SprintEventId EventId,
    SprintId SprintId,
    SprintEventType EventType,
    DateTime ScheduledDate,
    TimeBox Duration) : DomainEvent;

public record SprintEventStartedEvent(
    SprintEventId EventId,
    DateTime StartedAt,
    int ParticipantCount) : DomainEvent;

public record SprintEventCompletedEvent(
    SprintEventId EventId,
    DateTime CompletedAt,
    TimeSpan ActualDuration,
    string Outcomes) : DomainEvent;

public record SprintEventCancelledEvent(
    SprintEventId EventId,
    string Reason,
    DateTime CancelledAt) : DomainEvent;
```

### Event Participation
```csharp
public record ParticipantJoinedEventEvent(
    SprintEventId EventId,
    UserId ParticipantId,
    DateTime JoinedAt) : DomainEvent;

public record ParticipantLeftEventEvent(
    SprintEventId EventId,
    UserId ParticipantId,
    DateTime LeftAt,
    TimeSpan Duration) : DomainEvent;
```

## Cross-Context Integration Events

### Business Process Events
```csharp
public record SprintPlanningCompletedEvent(
    SprintId SprintId,
    int SelectedItemCount,
    int TotalStoryPoints,
    Capacity PlannedCapacity) : DomainEvent;

public record DailyStandupCompletedEvent(
    SprintId SprintId,
    DateTime Date,
    int ParticipantCount,
    int ReportedImpediments) : DomainEvent;

public record SprintReviewCompletedEvent(
    SprintId SprintId,
    int DemoedItems,
    int AcceptedItems,
    string StakeholderFeedback) : DomainEvent;

public record SprintRetrospectiveCompletedEvent(
    SprintId SprintId,
    int ActionItems,
    string KeyInsights) : DomainEvent;
```

## Event Handler Examples

### Team Velocity Calculation Handler
```csharp
public class TeamVelocityCalculationHandler : IDomainEventHandler<SprintCompletedEvent>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IVelocityCalculationService _velocityService;
    
    public async Task Handle(SprintCompletedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(domainEvent.TeamId);
        if (team == null) return;
        
        var newVelocity = await _velocityService.CalculateVelocityAsync(
            domainEvent.TeamId, 
            domainEvent.ActualVelocity);
            
        team.UpdateVelocity(newVelocity);
        
        await _teamRepository.SaveAsync(team);
    }
}
```

### Notification Handler
```csharp
public class ImpedimentNotificationHandler : IDomainEventHandler<ImpedimentReportedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IUserRepository _userRepository;
    
    public async Task Handle(ImpedimentReportedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Notify Scrum Master
        var scrumMasters = await _userRepository.GetScrumMastersBySprintAsync(domainEvent.SprintId);
        
        foreach (var scrumMaster in scrumMasters)
        {
            await _notificationService.SendImpedimentNotificationAsync(
                scrumMaster.Email,
                domainEvent.Title,
                domainEvent.Severity);
        }
    }
}
```

## Event Sourcing Considerations

### Event Store Schema
```csharp
public class StoredEvent
{
    public Guid Id { get; set; }
    public string AggregateId { get; set; } = string.Empty;
    public string AggregateType { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string EventData { get; set; } = string.Empty;
    public DateTime OccurredOn { get; set; }
    public int Version { get; set; }
}
```

### Event Dispatcher
```csharp
public interface IEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

public class MediatREventDispatcher : IEventDispatcher
{
    private readonly IMediator _mediator;
    
    public MediatREventDispatcher(IMediator mediator) => _mediator = mediator;
    
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _mediator.Publish(domainEvent, cancellationToken);
    }
    
    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
```

## Event Processing Patterns

### Immediate Processing
- Domain events processed immediately after aggregate persistence
- Used for critical business rules and data consistency
- Examples: Velocity calculation, capacity validation

### Eventual Consistency
- Domain events processed asynchronously via message queues
- Used for cross-context integration and notifications
- Examples: Email notifications, reporting updates

### Event Replay
- Events stored for audit trail and potential replay
- Enables debugging and business intelligence
- Examples: Performance metrics, historical analysis

## Testing Domain Events

### Event Testing Strategy
```csharp
[Fact]
public void Sprint_WhenStarted_ShouldRaiseSprintStartedEvent()
{
    // Arrange
    var sprint = TestDataBuilder.CreateSprint()
        .InPlanningStatus()
        .WithBacklogItems()
        .Build();
    
    // Act
    sprint.StartSprint();
    
    // Assert
    var domainEvent = sprint.DomainEvents.OfType<SprintStartedEvent>().Single();
    domainEvent.SprintId.Should().Be(sprint.Id);
    domainEvent.StartedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
}
```

---
*Domain events provide loose coupling between bounded contexts and enable rich business behavior*