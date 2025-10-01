using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Queries;

/// <summary>
/// Query to get sprint velocity calculation.
/// </summary>
public record GetSprintVelocityQuery(TeamId TeamId, SprintId SprintId) : IRequest<SprintVelocityDto?>;

/// <summary>
/// DTO for sprint velocity metrics.
/// </summary>
public class SprintVelocityDto
{
    public int SprintId { get; set; }
    public decimal PlannedVelocity { get; set; }
    public decimal ActualVelocity { get; set; }
    public int CompletedStoryPoints { get; set; }
    public int TotalStoryPoints { get; set; }
    public decimal CompletionPercentage { get; set; }
}