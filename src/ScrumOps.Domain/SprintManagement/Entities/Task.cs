using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Entities;

/// <summary>
/// Task entity representing individual work items within a sprint backlog item.
/// Tasks are the smallest unit of work in sprint management.
/// </summary>
public class Task : Entity<TaskId>
{
    /// <summary>
    /// Gets the ID of the sprint backlog item this task belongs to.
    /// </summary>
    public SprintBacklogItemId SprintBacklogItemId { get; private set; }

    /// <summary>
    /// Gets the task title.
    /// </summary>
    public TaskTitle Title { get; private set; }

    /// <summary>
    /// Gets the task description.
    /// </summary>
    public TaskDescription Description { get; private set; }

    /// <summary>
    /// Gets the current status of the task.
    /// </summary>
    public ValueObjects.TaskStatus Status { get; private set; }

    /// <summary>
    /// Gets the original estimate in hours.
    /// </summary>
    public int OriginalEstimateHours { get; private set; }

    /// <summary>
    /// Gets the remaining hours to complete the task.
    /// </summary>
    public int RemainingHours { get; private set; }

    /// <summary>
    /// Gets the date when the task was created.
    /// </summary>
    public DateTime CreatedDate { get; private set; }

    /// <summary>
    /// Gets the date when work on the task was started.
    /// </summary>
    public DateTime? StartedDate { get; private set; }

    /// <summary>
    /// Gets the date when the task was completed.
    /// </summary>
    public DateTime? CompletedDate { get; private set; }

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private Task() : base()
    {
        SprintBacklogItemId = default!;
        Title = default!;
        Description = default!;
    }

    /// <summary>
    /// Initializes a new instance of the Task class.
    /// </summary>
    /// <param name="id">The unique identifier for the task</param>
    /// <param name="sprintBacklogItemId">The ID of the sprint backlog item this task belongs to</param>
    /// <param name="title">The task title</param>
    /// <param name="description">The task description</param>
    /// <param name="estimateHours">The original estimate in hours</param>
    public Task(TaskId id, SprintBacklogItemId sprintBacklogItemId, TaskTitle title, 
                TaskDescription description, int estimateHours)
        : base(id)
    {
        if (estimateHours < 0)
        {
            throw new DomainException("Task estimate cannot be negative");
        }

        if (estimateHours > 40)
        {
            throw new DomainException("Task estimate cannot exceed 40 hours (consider breaking it down)");
        }

        SprintBacklogItemId = sprintBacklogItemId;
        Title = title;
        Description = description;
        Status = ValueObjects.TaskStatus.ToDo;
        OriginalEstimateHours = estimateHours;
        RemainingHours = estimateHours;
        CreatedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Starts work on the task.
    /// </summary>
    public void Start()
    {
        if (Status != ValueObjects.TaskStatus.ToDo)
        {
            throw new DomainException("Only tasks in ToDo status can be started");
        }

        Status = ValueObjects.TaskStatus.InProgress;
        StartedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Completes the task.
    /// </summary>
    public void Complete()
    {
        if (Status == ValueObjects.TaskStatus.Done)
        {
            return; // Already completed
        }

        if (Status == ValueObjects.TaskStatus.Blocked)
        {
            throw new DomainException("Cannot complete a blocked task");
        }

        Status = ValueObjects.TaskStatus.Done;
        RemainingHours = 0;
        CompletedDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Blocks the task with a reason.
    /// </summary>
    public void Block()
    {
        if (Status == ValueObjects.TaskStatus.Done)
        {
            throw new DomainException("Cannot block a completed task");
        }

        Status = ValueObjects.TaskStatus.Blocked;
    }

    /// <summary>
    /// Unblocks the task, returning it to its previous state.
    /// </summary>
    public void Unblock()
    {
        if (Status != ValueObjects.TaskStatus.Blocked)
        {
            return; // Not blocked
        }

        // Return to InProgress if it was started, otherwise ToDo
        Status = StartedDate.HasValue ? ValueObjects.TaskStatus.InProgress : ValueObjects.TaskStatus.ToDo;
    }

    /// <summary>
    /// Updates the remaining hours for the task.
    /// </summary>
    /// <param name="remainingHours">The new remaining hours estimate</param>
    public void UpdateRemainingHours(int remainingHours)
    {
        if (remainingHours < 0)
        {
            throw new DomainException("Remaining hours cannot be negative");
        }

        if (Status == ValueObjects.TaskStatus.Done && remainingHours > 0)
        {
            throw new DomainException("Completed tasks should have zero remaining hours");
        }

        RemainingHours = remainingHours;

        // Auto-complete if remaining hours reaches zero
        if (remainingHours == 0 && Status != ValueObjects.TaskStatus.Done)
        {
            Complete();
        }
    }

    /// <summary>
    /// Updates the task details.
    /// </summary>
    /// <param name="title">The new task title</param>
    /// <param name="description">The new task description</param>
    public void UpdateDetails(TaskTitle title, TaskDescription description)
    {
        Title = title;
        Description = description;
    }
}