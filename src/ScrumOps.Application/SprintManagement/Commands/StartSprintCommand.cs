using MediatR;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Commands;

/// <summary>
/// Command to start a sprint (change status to Active).
/// </summary>
public record StartSprintCommand(SprintId SprintId) : IRequest;