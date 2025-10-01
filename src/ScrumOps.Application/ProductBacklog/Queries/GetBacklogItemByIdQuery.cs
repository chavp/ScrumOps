using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Queries;

/// <summary>
/// Query to get a specific backlog item by ID with detailed information.
/// </summary>
public record GetBacklogItemByIdQuery(TeamId TeamId, ProductBacklogItemId ItemId) : IRequest<BacklogItemDetailDto?>;