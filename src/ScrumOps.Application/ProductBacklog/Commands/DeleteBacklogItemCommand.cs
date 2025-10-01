using MediatR;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to delete a backlog item.
/// </summary>
public record DeleteBacklogItemCommand(ProductBacklogItemId ItemId) : IRequest;