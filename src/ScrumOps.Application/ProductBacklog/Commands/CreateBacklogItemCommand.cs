using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to create a new backlog item.
/// </summary>
public record CreateBacklogItemCommand(
    TeamId TeamId,
    string Title,
    string Description,
    string? AcceptanceCriteria,
    string Type,
    int? Priority
) : IRequest<ProductBacklogItemId>;