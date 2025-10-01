using System.Collections.Generic;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.ProductBacklog.Commands;

/// <summary>
/// Command to reorder backlog items by priority.
/// </summary>
public record ReorderBacklogCommand(
    TeamId TeamId,
    List<ItemOrder> ItemOrders
) : IRequest<ReorderBacklogResponse>;

/// <summary>
/// Item order specification.
/// </summary>
public record ItemOrder(
    int ItemId,
    int Priority);

/// <summary>
/// Response for reorder backlog operation.
/// </summary>
public class ReorderBacklogResponse
{
    public List<ItemOrder> UpdatedItems { get; set; } = new();
}