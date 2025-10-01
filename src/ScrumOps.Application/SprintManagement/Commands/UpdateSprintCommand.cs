using MediatR;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Commands;

/// <summary>
/// Command to update sprint details.
/// </summary>
public record UpdateSprintCommand(
    SprintId SprintId,
    string Name,
    string Goal,
    int Capacity,
    string? Notes
) : IRequest;