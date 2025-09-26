using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to create a new product backlog for a team.
/// </summary>
public record CreateProductBacklogCommand(
    TeamId TeamId,
    string? Notes
) : IRequest<ProductBacklogId>;