using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.Entities;
using ScrumOps.Domain.SprintManagement.Repositories;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using TaskEntity = ScrumOps.Domain.SprintManagement.Entities.Task;
using SprintVelocity = ScrumOps.Domain.SprintManagement.ValueObjects.Velocity;

namespace ScrumOps.Application.Services.SprintManagement;

/// <summary>
/// Service implementation for sprint management operations.
/// </summary>
public class SprintManagementService : ISprintManagementService
{
    private readonly ISprintRepository _sprintRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SprintManagementService(ISprintRepository sprintRepository, IUnitOfWork unitOfWork)
    {
        _sprintRepository = sprintRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetSprintsResponse?> GetSprintsAsync(
        TeamId teamId,
        string? status = null,
        int limit = 20,
        int offset = 0,
        CancellationToken cancellationToken = default)
    {
        IEnumerable<Sprint> sprints;

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SprintStatus>(status, true, out var statusEnum))
        {
            sprints = await _sprintRepository.GetByStatusAsync(statusEnum, cancellationToken);
            sprints = sprints.Where(s => s.TeamId == teamId);
        }
        else
        {
            sprints = await _sprintRepository.GetByTeamIdAsync(teamId, cancellationToken);
        }

        var totalCount = sprints.Count();
        var paginatedSprints = sprints.Skip(offset).Take(limit);

        var sprintDtos = paginatedSprints.Select(sprint => new SprintDto
        {
            Id = sprint.Id.Value,
            TeamId = sprint.TeamId.Value,
            Name = sprint.Goal?.Value ?? $"Sprint {sprint.Id.Value}",
            Goal = sprint.Goal?.Value,
            StartDate = sprint.StartDate,
            EndDate = sprint.EndDate,
            Status = sprint.Status.ToString(),
            Capacity = sprint.Capacity.Hours
        }).ToList();

        return new GetSprintsResponse
        {
            Sprints = sprintDtos,
            TotalCount = totalCount,
            HasNext = offset + limit < totalCount
        };
    }

    public async Task<SprintDetailDto?> GetSprintByIdAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, cancellationToken);
        if (sprint == null || sprint.TeamId != teamId) return null;

        var backlogItems = sprint.BacklogItems.Select(item => new SprintBacklogItemDto
        {
            Id = item.Id.Value,
            ProductBacklogItemId = item.ProductBacklogItemId.Value,
            Title = "TODO: Load from ProductBacklogItem", // TODO: Load title from ProductBacklogItem
            Status = item.IsCompleted ? "Completed" : "In Progress",
            OriginalEstimate = item.OriginalEstimate,
            RemainingWork = item.RemainingWork,
            TaskCount = item.Tasks.Count,
            CompletedTaskCount = item.Tasks.Count(t => t.CompletedDate.HasValue)
        }).ToList();

        return new SprintDetailDto
        {
            Id = sprint.Id.Value,
            Name = sprint.Goal?.Value ?? $"Sprint {sprint.Id.Value}",
            Goal = sprint.Goal?.Value,
            StartDate = sprint.StartDate,
            EndDate = sprint.EndDate,
            Status = sprint.Status.ToString(),
            Capacity = sprint.Capacity.Hours,
            BacklogItems = backlogItems,
            Impediments = new List<ImpedimentDto>() // TODO: Implement impediment tracking
        };
    }

    public async Task<GetSprintBacklogResponse?> GetSprintBacklogAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new GetSprintBacklogResponse
        {
            Items = new List<SprintBacklogDetailDto>()
        };
    }

    public async Task<SprintBurndownDto?> GetSprintBurndownAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new SprintBurndownDto
        {
            SprintId = sprintId.Value,
            SprintDays = 10,
            TotalCapacity = 40,
            BurndownData = new List<BurndownDataPoint>()
        };
    }

    public async Task<SprintVelocityDto?> GetSprintVelocityAsync(
        TeamId teamId,
        SprintId sprintId,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new SprintVelocityDto
        {
            SprintId = sprintId.Value,
            PlannedVelocity = 20,
            ActualVelocity = 18,
            CompletedStoryPoints = 18,
            TotalStoryPoints = 20,
            CompletionPercentage = 90
        };
    }

    public async Task<SprintId> CreateSprintAsync(
        TeamId teamId,
        string name,
        string goal,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        CancellationToken cancellationToken = default)
    {
        // Check if team already has an active sprint
        var hasActiveSprint = await _sprintRepository.HasActiveSprintAsync(teamId, cancellationToken);
        if (hasActiveSprint)
        {
            throw new InvalidOperationException($"Team {teamId.Value} already has an active sprint.");
        }

        var sprintId = SprintId.New();
        var sprintGoal = SprintGoal.Create(goal);
        var sprintCapacity = Capacity.Create(capacity);

        var sprint = new Sprint(sprintId, teamId, sprintGoal, startDate, endDate, sprintCapacity);

        await _sprintRepository.AddAsync(sprint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return sprint.Id;
    }

    public async System.Threading.Tasks.Task StartSprintAsync(
        SprintId sprintId,
        DateTime? actualStartDate = null,
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, cancellationToken);
        if (sprint == null)
        {
            throw new InvalidOperationException($"Sprint with ID {sprintId.Value} not found.");
        }

        sprint.Start();

        await _sprintRepository.UpdateAsync(sprint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task UpdateSprintAsync(
        SprintId sprintId,
        string name,
        string goal,
        DateTime startDate,
        DateTime endDate,
        int capacity,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, cancellationToken);
        if (sprint == null)
        {
            throw new InvalidOperationException($"Sprint with ID {sprintId.Value} not found.");
        }

        // Note: Sprint entity currently only supports updating the goal
        // For start/end dates and capacity, the sprint would need to be recreated
        var newGoal = SprintGoal.Create(goal);
        sprint.UpdateGoal(newGoal);

        await _sprintRepository.UpdateAsync(sprint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task CompleteSprintAsync(
        SprintId sprintId,
        DateTime? actualEndDate = null,
        string? notes = null,
        CancellationToken cancellationToken = default)
    {
        var sprint = await _sprintRepository.GetByIdAsync(sprintId, cancellationToken);
        if (sprint == null)
        {
            throw new InvalidOperationException($"Sprint with ID {sprintId.Value} not found.");
        }

        // Calculate actual velocity based on completed items
        var completedStoryPoints = sprint.BacklogItems
            .Where(item => item.IsCompleted)
            .Sum(item => item.StoryPoints?.Value ?? 0);
        
        var actualVelocity = SprintVelocity.Create(completedStoryPoints);
        sprint.Complete(actualVelocity);

        await _sprintRepository.UpdateAsync(sprint, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
