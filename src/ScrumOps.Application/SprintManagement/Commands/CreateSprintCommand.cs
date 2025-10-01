using System;
using MediatR;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.SprintManagement.Commands;

/// <summary>
/// Command to create a new sprint for a team.
/// </summary>
public record CreateSprintCommand(
    TeamId TeamId,
    string Name,
    string Goal,
    DateTime StartDate,
    DateTime EndDate,
    int Capacity
) : IRequest<SprintId>;