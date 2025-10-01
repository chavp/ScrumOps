using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Queries;

/// <summary>
/// Query to get sprint backlog items.
/// </summary>
public record GetSprintBacklogQuery(TeamId TeamId, SprintId SprintId) : IRequest<GetSprintBacklogResponse?>;

/// <summary>
/// Response containing sprint backlog items.
/// </summary>
public class GetSprintBacklogResponse
{
    public List<SprintBacklogDetailDto> Items { get; set; } = new();
}

/// <summary>
/// DTO for detailed sprint backlog item with tasks.
/// </summary>
public class SprintBacklogDetailDto
{
    public int Id { get; set; }
    public int ProductBacklogItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int OriginalEstimate { get; set; }
    public int RemainingWork { get; set; }
    public List<TaskDto> Tasks { get; set; } = new();
}

/// <summary>
/// DTO for task information.
/// </summary>
public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AssignedTo { get; set; } = string.Empty;
    public int RemainingHours { get; set; }
    public string Description { get; set; } = string.Empty;
}