using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Domain.SprintManagement.Events;
using DomainTaskStatus = ScrumOps.Domain.SprintManagement.ValueObjects.TaskStatus;

namespace ScrumOps.Domain.Tests.SprintManagement;

/// <summary>
/// Domain tests for Sprint aggregate root.
/// These tests MUST FAIL initially to follow TDD principles.
/// </summary>
public class SprintTests
{
    [Fact]
    public void Sprint_Create_ShouldInitializeCorrectly()
    {
        // Arrange
        var sprintId = SprintId.New();
        var teamId = TeamId.New();
        var goal = SprintGoal.Create("Complete user authentication");
        var startDate = DateTime.UtcNow;
        var endDate = startDate.AddDays(14);
        var capacity = Capacity.Create(40);

        // Act
        var sprint = new Sprint(sprintId, teamId, goal, startDate, endDate, capacity);

        // Assert
        Assert.Equal(sprintId, sprint.Id);
        Assert.Equal(teamId, sprint.TeamId);
        Assert.Equal(goal, sprint.Goal);
        Assert.Equal(startDate, sprint.StartDate);
        Assert.Equal(endDate, sprint.EndDate);
        Assert.Equal(capacity, sprint.Capacity);
        Assert.Equal(SprintStatus.Planning, sprint.Status);
        Assert.Contains(sprint.DomainEvents, e => e is SprintCreatedEvent);
    }

    [Fact]
    public void Sprint_Start_ShouldChangeStatusToActive()
    {
        // Arrange
        var sprint = CreateValidSprint();

        // Act
        sprint.Start();

        // Assert
        Assert.Equal(SprintStatus.Active, sprint.Status);
        Assert.Contains(sprint.DomainEvents, e => e is SprintStartedEvent);
    }

    [Fact]
    public void Sprint_Start_WhenNotInPlanningStatus_ShouldThrowDomainException()
    {
        // Arrange
        var sprint = CreateValidSprint();
        sprint.Start(); // Move to Active status

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => sprint.Start());
        Assert.Contains("already started", exception.Message);
    }

    [Fact]
    public void Sprint_AddBacklogItem_ShouldAddItemSuccessfully()
    {
        // Arrange
        var sprint = CreateValidSprint();
        var backlogItem = CreateValidSprintBacklogItem(sprint.Id);

        // Act
        sprint.AddBacklogItem(backlogItem);

        // Assert
        Assert.Single(sprint.BacklogItems);
        Assert.Contains(backlogItem, sprint.BacklogItems);
        Assert.Contains(sprint.DomainEvents, e => e is BacklogItemAddedToSprintEvent);
    }

    [Fact]
    public void Sprint_AddBacklogItem_WhenSprintActive_ShouldThrowDomainException()
    {
        // Arrange
        var sprint = CreateValidSprint();
        sprint.Start();
        var backlogItem = CreateValidSprintBacklogItem(sprint.Id);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => sprint.AddBacklogItem(backlogItem));
        Assert.Contains("cannot add items", exception.Message);
    }

    [Fact]
    public void Sprint_Complete_ShouldChangeStatusToCompleted()
    {
        // Arrange
        var sprint = CreateValidSprint();
        sprint.Start();
        var actualVelocity = Velocity.Create(25);

        // Act
        sprint.Complete(actualVelocity);

        // Assert
        Assert.Equal(SprintStatus.Completed, sprint.Status);
        Assert.Equal(actualVelocity, sprint.ActualVelocity);
        Assert.Contains(sprint.DomainEvents, e => e is SprintCompletedEvent);
    }

    [Fact]
    public void Sprint_Complete_WhenNotActive_ShouldThrowDomainException()
    {
        // Arrange
        var sprint = CreateValidSprint();
        var actualVelocity = Velocity.Create(25);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => sprint.Complete(actualVelocity));
        Assert.Contains("not active", exception.Message);
    }

    [Fact]
    public void Sprint_CalculateRemainingWork_ShouldReturnCorrectTotal()
    {
        // Arrange
        var sprint = CreateValidSprint();
        var item1 = CreateValidSprintBacklogItem(sprint.Id, remainingWork: 8);
        var item2 = CreateValidSprintBacklogItem(sprint.Id, remainingWork: 5);
        
        sprint.AddBacklogItem(item1);
        sprint.AddBacklogItem(item2);

        // Act
        var remainingWork = sprint.CalculateRemainingWork();

        // Assert
        Assert.Equal(13, remainingWork);
    }

    [Fact]
    public void SprintBacklogItem_CompleteTask_ShouldUpdateRemainingWork()
    {
        // Arrange
        var sprint = CreateValidSprint();
        var backlogItem = CreateValidSprintBacklogItem(sprint.Id, remainingWork: 8);
        var task = CreateValidTask(backlogItem.Id, remainingHours: 3);
        
        backlogItem.AddTask(task);

        // Act
        task.Complete();

        // Assert
        Assert.Equal(DomainTaskStatus.Done, task.Status);
        Assert.Equal(0, task.RemainingHours);
        Assert.NotNull(task.CompletedDate);
    }

    [Fact]
    public void Task_Start_ShouldUpdateStatus()
    {
        // Arrange
        var sprint = CreateValidSprint();
        var backlogItem = CreateValidSprintBacklogItem(sprint.Id);
        var task = CreateValidTask(backlogItem.Id);

        // Act
        task.Start();

        // Assert
        Assert.Equal(DomainTaskStatus.InProgress, task.Status);
        Assert.NotNull(task.StartedDate);
    }

    private static Sprint CreateValidSprint()
    {
        return new Sprint(
            SprintId.New(),
            TeamId.New(),
            SprintGoal.Create("Test Sprint Goal"),
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(14),
            Capacity.Create(40)
        );
    }

    private static SprintBacklogItem CreateValidSprintBacklogItem(SprintId sprintId, int remainingWork = 5)
    {
        return new SprintBacklogItem(
            SprintBacklogItemId.New(),
            sprintId,
            ProductBacklogItemId.New(),
            StoryPoints.Create(remainingWork),
            remainingWork
        );
    }

    private static Domain.SprintManagement.Entities.Task CreateValidTask(SprintBacklogItemId backlogItemId, int remainingHours = 4)
    {
        return new Domain.SprintManagement.Entities.Task(
            TaskId.New(),
            backlogItemId,
            TaskTitle.Create("Test Task"),
            TaskDescription.Create("Test task description"),
            remainingHours
        );
    }
}