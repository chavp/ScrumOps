using System;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Application.Services.SprintManagement;

/// <summary>
/// Service interface for sprint management operations.
/// </summary>
public interface ISprintManagementService
{
    // Query operations
    Task<GetSprintsResponse?> GetSprintsAsync(
        TeamId teamId,
        string? status = null,
        int limit = 20,
        int offset = 0,
        CancellationToken cancellationToken = default);

    Task<SprintDetailDto?> GetSprintByIdAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default);

    Task<GetSprintBacklogResponse?> GetSprintBacklogAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default);

    Task<SprintBurndownDto?> GetSprintBurndownAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default);

    Task<SprintVelocityDto?> GetSprintVelocityAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default);

    // Command operations
    Task<SprintId> CreateSprintAsync(
        TeamId teamId,
        string name,
        string goal,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        CancellationToken cancellationToken = default);

    Task StartSprintAsync(
        SprintId sprintId,
        DateTime? actualStartDate = null,
        CancellationToken cancellationToken = default);

    Task UpdateSprintAsync(
        SprintId sprintId,
        string name,
        string goal,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string? notes = null,
        CancellationToken cancellationToken = default);

    Task CompleteSprintAsync(
        SprintId sprintId,
        DateTime? actualEndDate = null,
        string? notes = null,
        CancellationToken cancellationToken = default);
}