using ScrumOps.Domain.SharedKernel;
using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Domain.SprintManagement.Entities;

/// <summary>
/// Sprint Backlog Item entity representing a product backlog item selected for a sprint.
/// Contains sprint-specific information and tracks progress during the sprint.
/// </summary>
public class SprintBacklogItem : Entity<SprintBacklogItemId>
{
    private readonly List<Task> _tasks = new();

    /// <summary>
    /// Gets the ID of the sprint this item belongs to.
    /// </summary>
    public SprintId SprintId { get; private set; }

    /// <summary>
    /// Gets the reference to the product backlog item.
    /// </summary>
    public ProductBacklogItemId ProductBacklogItemId { get; private set; }

    /// <summary>
    /// Gets the story points for this item.
    /// </summary>
    public StoryPoints StoryPoints { get; private set; }

    /// <summary>
    /// Gets the original estimate of work remaining.
    /// </summary>
    public int OriginalEstimate { get; private set; }

    /// <summary>
    /// Gets the current remaining work estimate.
    /// </summary>
    public int RemainingWork { get; private set; }

    /// <summary>
    /// Gets the date when the item was added to the sprint.
    /// </summary>
    public DateTime AddedToSprintDate { get; private set; }

    /// <summary>
    /// Gets the date when the item was completed.
    /// </summary>
    public DateTime? CompletedDate { get; private set; }

    /// <summary>
    /// Gets whether the item is completed.
    /// </summary>
    public bool IsCompleted => CompletedDate.HasValue;

    /// <summary>
    /// Gets the read-only list of tasks for this backlog item.
    /// </summary>
    public IReadOnlyList<Task> Tasks => _tasks.AsReadOnly();

    /// <summary>
    /// Private constructor for Entity Framework Core.
    /// </summary>
    private SprintBacklogItem() : base()
    {
        SprintId = default!;
        ProductBacklogItemId = default!;
        StoryPoints = default!;
    }

    /// <summary>
    /// Initializes a new instance of the SprintBacklogItem class.
    /// </summary>
    /// <param name="id">The unique identifier for the sprint backlog item</param>
    /// <param name="sprintId">The ID of the sprint this item belongs to</param>
    /// <param name="productBacklogItemId">The reference to the product backlog item</param>
    /// <param name="storyPoints">The story points for this item</param>
    /// <param name="originalEstimate">The original work estimate</param>
    public SprintBacklogItem(SprintBacklogItemId id, SprintId sprintId, 
                           ProductBacklogItemId productBacklogItemId, 
                           StoryPoints storyPoints, int originalEstimate)
        : base(id)
    {
        if (originalEstimate < 0)
        {
            throw new DomainException("Original estimate cannot be negative");
        }

        SprintId = sprintId;
        ProductBacklogItemId = productBacklogItemId;
        StoryPoints = storyPoints;
        OriginalEstimate = originalEstimate;
        RemainingWork = originalEstimate;
        AddedToSprintDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a task to this backlog item.
    /// </summary>
    /// <param name="task">The task to add</param>
    public void AddTask(Task task)
    {
        if (IsCompleted)
        {
            throw new DomainException("Cannot add tasks to completed backlog items");
        }

        if (_tasks.Any(t => t.Title.Value.Equals(task.Title.Value, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DomainException("A task with this title already exists");
        }

        _tasks.Add(task);
    }

    /// <summary>
    /// Removes a task from this backlog item.
    /// </summary>
    /// <param name="taskId">The ID of the task to remove</param>
    public void RemoveTask(TaskId taskId)
    {
        var task = _tasks.FirstOrDefault(t => t.Id.Equals(taskId));
        if (task == null)
        {
            throw new DomainException("Task not found in this backlog item");
        }

        if (task.Status == ValueObjects.TaskStatus.Done)
        {
            throw new DomainException("Cannot remove completed tasks");
        }

        _tasks.Remove(task);
    }

    /// <summary>
    /// Updates the remaining work estimate.
    /// </summary>
    /// <param name="remainingWork">The new remaining work estimate</param>
    public void UpdateRemainingWork(int remainingWork)
    {
        if (remainingWork < 0)
        {
            throw new DomainException("Remaining work cannot be negative");
        }

        RemainingWork = remainingWork;

        // Auto-complete if remaining work reaches zero
        if (remainingWork == 0 && !IsCompleted)
        {
            Complete();
        }
    }

    /// <summary>
    /// Marks the backlog item as completed.
    /// </summary>
    public void Complete()
    {
        if (IsCompleted)
        {
            return; // Already completed
        }

        // Check if all tasks are completed
        var incompleteTasks = _tasks.Where(t => t.Status != ValueObjects.TaskStatus.Done).ToList();
        if (incompleteTasks.Count > 0)
        {
            throw new DomainException($"Cannot complete backlog item with {incompleteTasks.Count} incomplete tasks");
        }

        CompletedDate = DateTime.UtcNow;
        RemainingWork = 0;
    }

    /// <summary>
    /// Calculates the total remaining hours from all tasks.
    /// </summary>
    /// <returns>The sum of remaining hours from all tasks</returns>
    public int CalculateRemainingHours()
    {
        return _tasks.Sum(t => t.RemainingHours);
    }

    /// <summary>
    /// Gets the completion percentage based on completed tasks.
    /// </summary>
    /// <returns>Completion percentage (0-100)</returns>
    public decimal GetCompletionPercentage()
    {
        if (_tasks.Count == 0)
        {
            return IsCompleted ? 100m : 0m;
        }

        var completedTasks = _tasks.Count(t => t.Status == ValueObjects.TaskStatus.Done);
        return (decimal)completedTasks / _tasks.Count * 100;
    }
}