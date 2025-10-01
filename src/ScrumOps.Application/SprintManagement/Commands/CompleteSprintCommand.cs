using MediatR;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Commands;

/// <summary>
/// Command to complete a sprint.
/// </summary>
public record CompleteSprintCommand(
    SprintId SprintId,
    decimal ActualVelocity,
    string? Notes
) : IRequest;