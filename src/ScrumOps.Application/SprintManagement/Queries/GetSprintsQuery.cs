using System;
using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Queries;

/// <summary>
/// Query to get sprints for a team with optional filtering.
/// </summary>
public record GetSprintsQuery(
    TeamId TeamId,
    string? Status,
    int Limit,
    int Offset
) : IRequest<GetSprintsResponse?>;

/// <summary>
/// Response containing list of sprints.
/// </summary>
public class GetSprintsResponse
{
    public List<SprintDto> Sprints { get; set; } = new();
    public int TotalCount { get; set; }
    public bool HasNext { get; set; }
}

/// <summary>
/// DTO for sprint summary information.
/// </summary>
public class SprintDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Goal { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public decimal? ActualVelocity { get; set; }
    public int BacklogItemCount { get; set; }
    public int CompletedItemCount { get; set; }
    public int ImpedimentCount { get; set; }
    public string Notes { get; set; } = string.Empty;
}