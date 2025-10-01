using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Queries;

/// <summary>
/// Query to get a specific sprint with detailed information.
/// </summary>
public record GetSprintByIdQuery(TeamId TeamId, SprintId SprintId) : IRequest<SprintDetailDto?>;

/// <summary>
/// DTO for detailed sprint information.
/// </summary>
public class SprintDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal? ActualVelocity { get; set; }
    public string Notes { get; set; } = string.Empty;
    public List<SprintBacklogItemDto> BacklogItems { get; set; } = new();
    public List<ImpedimentDto> Impediments { get; set; } = new();
}

/// <summary>
/// DTO for sprint backlog item.
/// </summary>
public class SprintBacklogItemDto
{
    public int Id { get; set; }
    public int ProductBacklogItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int OriginalEstimate { get; set; }
    public int RemainingWork { get; set; }
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
}

/// <summary>
/// DTO for impediment information.
/// </summary>
public class ImpedimentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string Description { get; set; } = string.Empty;
}