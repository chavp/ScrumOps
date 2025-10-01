using MediatR;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to add or update story point estimate for a backlog item.
/// </summary>
public record EstimateBacklogItemCommand(
    ProductBacklogItemId ItemId,
    int StoryPoints,
    string EstimatedBy,
    string EstimationMethod,
    string Confidence,
    string? Notes
) : IRequest;