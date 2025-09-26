# GitHub Copilot Development Instructions: ScrumOps

**Project**: Scrum Framework Management System  
**Technology Stack**: C# .NET 8, ASP.NET Core, Blazor, Entity Framework Core, SQLite  
**Architecture**: Domain Driven Design (DDD) with Clean Architecture  
**Last Updated**: 2025-01-27 (DDD Implementation)

## Project Context

You are helping develop **ScrumOps**, a comprehensive Scrum framework management system using **Domain Driven Design** principles that implements:
- Complete Scrum process management organized in bounded contexts
- Rich domain model with aggregates, entities, and value objects
- Domain events for cross-context communication
- CQRS pattern with MediatR for application layer
- Clean architecture with proper separation of concerns

## DDD Architecture Overview

### Bounded Contexts
1. **Team Management**: Teams, users, roles, permissions, team velocity
2. **Product Backlog**: Backlog items, prioritization, refinement, estimation
3. **Sprint Management**: Sprint planning, execution, tracking, velocity calculation
4. **Event Management**: Scrum ceremonies, scheduling, participation, time-boxing

### Architecture Layers
- **Domain Layer**: Rich domain model, business rules, domain events
- **Application Layer**: Use cases (CQRS), application services, DTOs
- **Infrastructure Layer**: EF Core, repositories, external services
- **Presentation Layer**: API controllers, Blazor components

## Constitutional Requirements (NON-NEGOTIABLE)

### Code Quality Standards
- **Functions**: Keep under 20 lines, classes under 300 lines
- **Naming**: Use ubiquitous language from domain model
- **Documentation**: Every public API documented with XML comments and examples
- **Static Analysis**: Code must pass all .NET analyzers and linting rules
- **Domain Purity**: Domain layer must have no external dependencies

### Test-First Development (TDD MANDATORY)
- **Always write tests BEFORE implementation** - this is non-negotiable
- **Minimum 80% code coverage**, 100% for domain logic
- **Test Types**: Domain unit tests, application tests, integration tests, E2E tests
- **Test Quality**: Tests must be readable, maintainable, and test behavior

### DDD Design Principles
- **Rich Domain Model**: Entities contain business logic, not just data
- **Aggregate Boundaries**: Consistency boundaries around related entities
- **Value Objects**: Immutable objects for domain concepts
- **Domain Events**: Communication between bounded contexts
- **Ubiquitous Language**: Consistent terminology throughout codebase

## Project Structure

```
src/
├── ScrumOps.Api/              # Presentation Layer - API Controllers
├── ScrumOps.Application/      # Application Layer - Use Cases (CQRS)
│   ├── TeamManagement/        # Team bounded context use cases
│   │   ├── Commands/          # CreateTeam, UpdateTeam commands
│   │   ├── Queries/           # GetTeam, ListTeams queries
│   │   ├── Handlers/          # Command and query handlers
│   │   └── DTOs/              # Team-related DTOs
│   ├── ProductBacklog/        # Backlog bounded context use cases
│   ├── SprintManagement/      # Sprint bounded context use cases
│   ├── EventManagement/       # Event bounded context use cases
│   └── Common/                # Shared application concerns
├── ScrumOps.Domain/           # Domain Layer - Rich Domain Model
│   ├── TeamManagement/        # Team Management Bounded Context
│   │   ├── Entities/          # Team (Aggregate Root), User
│   │   ├── ValueObjects/      # TeamName, ScrumRole, SprintLength
│   │   ├── Services/          # Team domain services
│   │   ├── Repositories/      # ITeamRepository interface
│   │   └── Events/            # TeamCreated, MemberAdded events
│   ├── ProductBacklog/        # Product Backlog Bounded Context
│   ├── SprintManagement/      # Sprint Management Bounded Context
│   ├── EventManagement/       # Event Management Bounded Context
│   └── SharedKernel/          # Shared domain concepts
├── ScrumOps.Infrastructure/   # Infrastructure Layer
│   ├── Persistence/           # EF Core implementation
│   └── EventHandlers/         # Domain event handlers
├── ScrumOps.Web/             # Presentation Layer - Blazor UI
└── ScrumOps.Shared/          # Shared contracts and DTOs

tests/
├── ScrumOps.Domain.Tests/     # Domain logic unit tests (by bounded context)
├── ScrumOps.Application.Tests/ # Application use case tests
├── ScrumOps.Infrastructure.Tests/ # Infrastructure tests
├── ScrumOps.Api.Tests/        # API integration tests
└── ScrumOps.ArchitectureTests/ # Architecture compliance tests
```

## DDD Development Patterns and Guidelines

### Aggregate Root Pattern
```csharp
// ✅ Good: Rich aggregate root with business logic
public class Sprint : Entity<SprintId>, IAggregateRoot
{
    private readonly List<SprintBacklogItem> _backlogItems = new();
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public TeamId TeamId { get; private set; }
    public SprintGoal Goal { get; private set; }
    public DateRange Duration { get; private set; }
    public SprintStatus Status { get; private set; }
    public Capacity Capacity { get; private set; }
    
    public IReadOnlyList<SprintBacklogItem> BacklogItems => _backlogItems.AsReadOnly();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    
    // Business logic methods, not just properties
    public void StartSprint()
    {
        if (Status != SprintStatus.Planning)
            throw new DomainException("Only planning sprints can be started");
            
        if (!_backlogItems.Any())
            throw new DomainException("Cannot start sprint without backlog items");
            
        Status = SprintStatus.Active;
        _domainEvents.Add(new SprintStartedEvent(Id, DateTime.UtcNow));
    }
    
    public void AddBacklogItem(ProductBacklogItemId itemId, StoryPoints estimate)
    {
        if (Status != SprintStatus.Planning)
            throw new DomainException("Can only add items during sprint planning");
            
        if (WouldExceedCapacity(estimate))
            throw new DomainException("Adding item would exceed sprint capacity");
            
        var sprintBacklogItem = new SprintBacklogItem(itemId, estimate);
        _backlogItems.Add(sprintBacklogItem);
        
        _domainEvents.Add(new ItemAddedToSprintEvent(Id, itemId));
    }
    
    private bool WouldExceedCapacity(StoryPoints estimate)
    {
        var currentPoints = _backlogItems.Sum(bi => bi.Estimate.Value);
        return currentPoints + estimate.Value > Capacity.MaxStoryPoints;
    }
    
    public void ClearDomainEvents() => _domainEvents.Clear();
}

// ❌ Avoid: Anemic domain model (just data, no behavior)
public class Sprint
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
    // No business logic - this is just a data container
}
```

### Value Object Pattern
```csharp
// ✅ Good: Immutable value object with validation
public class StoryPoints : ValueObject
{
    public int Value { get; }
    
    private static readonly int[] ValidPoints = { 1, 2, 3, 5, 8, 13, 21, 34 };
    
    private StoryPoints(int value) => Value = value;
    
    public static StoryPoints Create(int points)
    {
        if (!ValidPoints.Contains(points))
            throw new DomainException($"Story points must be one of: {string.Join(", ", ValidPoints)}");
            
        return new StoryPoints(points);
    }
    
    public static StoryPoints operator +(StoryPoints left, StoryPoints right) =>
        Create(left.Value + right.Value);
    
    protected override IEnumerable<object?> GetAtomicValues() 
    { 
        yield return Value; 
    }
}
```

## Development Patterns and Guidelines

### Entity Framework Patterns
```csharp
// ✅ Good: Use proper navigation properties
public class Sprint
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual Team Team { get; set; } = null!;
    public virtual ICollection<SprintBacklogItem> BacklogItems { get; set; } = new List<SprintBacklogItem>();
}

// ✅ Good: Use explicit loading for performance
var sprint = await context.Sprints
    .Include(s => s.Team)
    .Include(s => s.BacklogItems)
    .ThenInclude(bi => bi.ProductBacklogItem)
    .FirstOrDefaultAsync(s => s.Id == sprintId);

// ❌ Avoid: Lazy loading without explicit includes
var sprint = await context.Sprints.FirstAsync(s => s.Id == sprintId);
var team = sprint.Team; // This causes N+1 query
```

### API Controller Patterns
```csharp
// ✅ Good: Thin controllers with proper error handling
[ApiController]
[Route("api/teams/{teamId}/sprints")]
public class SprintsController : ControllerBase
{
    private readonly ISprintService _sprintService;
    
    [HttpGet]
    public async Task<ActionResult<SprintListResponse>> GetSprints(
        int teamId,
        [FromQuery] SprintStatus? status = null,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        try
        {
            var result = await _sprintService.GetSprintsAsync(teamId, status, limit, offset);
            return Ok(result);
        }
        catch (TeamNotFoundException)
        {
            return NotFound($"Team with ID {teamId} not found");
        }
    }
}

// ❌ Avoid: Business logic in controllers
[HttpPost]
public async Task<ActionResult> CreateSprint(CreateSprintRequest request)
{
    // Don't put business logic here - delegate to service
}
```

### Blazor Component Patterns
```razor
@* ✅ Good: Component with proper lifecycle and error handling *@
<div class="sprint-card @CssClass">
    @if (IsLoading)
    {
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
    }
    else if (Sprint != null)
    {
        <h5>@Sprint.Name</h5>
        <p>@Sprint.Goal</p>
        <div class="progress">
            <div class="progress-bar" style="width: @ProgressPercentage%"></div>
        </div>
    }
    else
    {
        <div class="alert alert-warning">Sprint data not available</div>
    }
</div>

@code {
    [Parameter] public Sprint? Sprint { get; set; }
    [Parameter] public string CssClass { get; set; } = string.Empty;
    
    public bool IsLoading { get; set; }
    public double ProgressPercentage => Sprint?.CalculateProgress() ?? 0;
    
    protected override async Task OnParametersSetAsync()
    {
        if (Sprint?.Id > 0)
        {
            IsLoading = true;
            try
            {
                // Load additional data if needed
                await LoadSprintDetailsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Failed to load sprint details for sprint {SprintId}", Sprint.Id);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
```

### Testing Patterns (WRITE TESTS FIRST!)
```csharp
// ✅ Good: Test behavior, not implementation
public class SprintServiceTests
{
    [Fact]
    public async Task CreateSprint_WithValidData_ShouldReturnCreatedSprint()
    {
        // Arrange
        var team = TestDataBuilder.CreateTeam().Build();
        var request = new CreateSprintRequest
        {
            Name = "Sprint 1",
            Goal = "Complete user authentication",
            StartDate = DateTime.Today.AddDays(1),
            EndDate = DateTime.Today.AddDays(15)
        };
        
        // Act
        var result = await _sprintService.CreateSprintAsync(team.Id, request);
        
        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Goal.Should().Be(request.Goal);
        result.Status.Should().Be(SprintStatus.Planning);
    }
    
    [Fact]
    public async Task CreateSprint_WithOverlappingDates_ShouldThrowValidationException()
    {
        // Arrange
        var team = TestDataBuilder.CreateTeam().WithActiveSprint().Build();
        var request = new CreateSprintRequest
        {
            StartDate = DateTime.Today, // Overlaps with existing sprint
            EndDate = DateTime.Today.AddDays(14)
        };
        
        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _sprintService.CreateSprintAsync(team.Id, request));
    }
}

// ✅ Good: Integration test for API endpoints
public class SprintsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetSprints_WithValidTeamId_ShouldReturnSprints()
    {
        // Arrange
        var client = _factory.CreateClient();
        var team = await SeedTeamWithSprints();
        
        // Act
        var response = await client.GetAsync($"/api/teams/{team.Id}/sprints");
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SprintListResponse>(content);
        result.Sprints.Should().HaveCount(2);
    }
}
```

### Service Layer Patterns
```csharp
// ✅ Good: Service with proper error handling and business logic
public class SprintService : ISprintService
{
    private readonly IScrumOpsRepository _repository;
    private readonly ILogger<SprintService> _logger;
    
    public async Task<Sprint> CreateSprintAsync(int teamId, CreateSprintRequest request)
    {
        // Validate business rules
        await ValidateSprintDatesAsync(teamId, request.StartDate, request.EndDate);
        
        var team = await _repository.GetTeamAsync(teamId) 
            ?? throw new TeamNotFoundException($"Team {teamId} not found");
            
        var sprint = new Sprint
        {
            TeamId = teamId,
            Name = request.Name,
            Goal = request.Goal,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = SprintStatus.Planning,
            Capacity = CalculateSprintCapacity(team)
        };
        
        await _repository.AddAsync(sprint);
        await _repository.SaveChangesAsync();
        
        _logger.LogInformation("Created sprint {SprintName} for team {TeamId}", 
            sprint.Name, teamId);
            
        return sprint;
    }
    
    private async Task ValidateSprintDatesAsync(int teamId, DateTime startDate, DateTime endDate)
    {
        if (startDate >= endDate)
            throw new ValidationException("Sprint start date must be before end date");
            
        var overlappingSprints = await _repository.GetOverlappingSprintsAsync(teamId, startDate, endDate);
        if (overlappingSprints.Any())
            throw new ValidationException("Sprint dates overlap with existing sprint");
    }
}
```

## Scrum Domain Knowledge

### Key Entities and Relationships
- **Team** has many **Users** (Product Owner, Scrum Master, Developers)
- **Team** owns one **ProductBacklog** with many **ProductBacklogItems**
- **Team** executes **Sprints** containing **SprintBacklogItems**
- **SprintBacklogItems** are broken down into **Tasks** assigned to **Users**
- **Sprints** track **Impediments** and include **SprintEvents** (ceremonies)

### Business Rules to Enforce
- Only Product Owner can prioritize product backlog
- Sprint length cannot change during active sprint
- Daily scrums are time-boxed to 15 minutes
- Sprint capacity should not exceed team velocity by >20%
- Tasks cannot be completed if parent story is not done

### Scrum Events (Time-boxed)
- **Sprint Planning**: 2-8 hours depending on sprint length
- **Daily Scrum**: 15 minutes maximum
- **Sprint Review**: 1-4 hours depending on sprint length  
- **Sprint Retrospective**: 1.5-3 hours depending on sprint length

## Common Code Generation Requests

### When generating controllers:
- Include proper error handling and status codes
- Use ActionResult<T> return types
- Implement proper request validation
- Include OpenAPI/Swagger documentation attributes
- Follow RESTful naming conventions

### When generating Blazor components:
- Use proper component lifecycle methods
- Include loading states and error handling
- Make components responsive with Bootstrap classes
- Follow accessibility guidelines (alt text, ARIA labels)
- Use dependency injection for services

### When generating Entity Framework entities:
- Include proper navigation properties
- Use appropriate column types and constraints
- Configure relationships in OnModelCreating
- Include audit fields (CreatedDate, ModifiedDate)
- Add proper indexes for performance

### When generating tests:
- Always write tests BEFORE implementation
- Use Arrange-Act-Assert pattern
- Test both happy path and error conditions
- Use descriptive test method names
- Include integration tests for API endpoints

## Performance Considerations

- Use `async/await` throughout the stack
- Implement proper database indexing
- Use `Include()` for related data loading
- Implement pagination for large result sets
- Cache frequently accessed read-only data
- Use connection pooling for database access
- Optimize Blazor component rendering with `@key` attributes

## Security Considerations

- Validate all input at API boundaries
- Use parameterized queries (EF Core handles this)
- Implement proper authorization checks
- Log security-related events
- Don't expose sensitive data in error messages
- Use HTTPS for all communications

Remember: **Always write tests first, keep functions small, and follow the constitutional principles for code quality, testing, UX consistency, and performance!**