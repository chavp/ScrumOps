// Shared DTOs for the ScrumOps API
// This file contains the request DTOs and re-exports from Application layer

namespace ScrumOps.Api.DTOs;

// Request DTOs (these are specific to API layer)
public record CreateTeamRequest(
    string Name,
    string? Description,
    int SprintLengthWeeks,
    string? ProductOwnerEmail,
    string? ScrumMasterEmail);

public record UpdateTeamRequest(
    string Name,
    string? Description,
    int SprintLengthWeeks);

public record CreateSprintRequest(
    string Name,
    string Goal,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity);

public record UpdateSprintRequest(
    string Name,
    string Goal,
    int Capacity,
    string? Notes);

public record CompleteSprintRequest(
    decimal ActualVelocity,
    string? Notes);

public class SprintStatusDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public record CreateBacklogItemRequest(
    string Title,
    string Description,
    string? AcceptanceCriteria,
    string Type,
    int? Priority);

public record UpdateBacklogItemRequest(
    string Title,
    string Description,
    string? AcceptanceCriteria,
    int Priority,
    int? StoryPoints,
    string BacklogItemType);

public record ReorderBacklogRequest(
    List<ScrumOps.Application.Services.ProductBacklog.ItemOrder> ItemOrders);

public record EstimateItemRequest(
    int StoryPoints,
    string EstimatedBy,
    string EstimationMethod,
    string Confidence,
    string? Notes);
