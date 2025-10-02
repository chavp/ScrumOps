using ScrumOps.Shared.Contracts.Sprints;

namespace ScrumOps.Web.Services;

/// <summary>
/// Service interface for sprint management operations.
/// </summary>
public interface ISprintService
{
    Task<ScrumOps.Shared.Contracts.Sprints.GetSprintsResponse> GetSprintsAsync(Guid teamId);
    Task<SprintDetailsResponse> GetSprintAsync(Guid teamId, Guid sprintId);
    Task<SprintDetailsResponse> CreateSprintAsync(Guid teamId, CreateSprintRequest request);
    Task<SprintDetailsResponse> UpdateSprintAsync(Guid teamId, Guid sprintId, UpdateSprintRequest request);
    Task<bool> DeleteSprintAsync(Guid teamId, Guid sprintId);
    Task<bool> StartSprintAsync(Guid teamId, Guid sprintId);
    Task<bool> CompleteSprintAsync(Guid teamId, Guid sprintId);
    Task<SprintBurndownResponse> GetSprintBurndownAsync(Guid teamId, Guid sprintId);
    Task<bool> AddSprintTaskAsync(Guid teamId, Guid sprintId, Guid itemId, AddSprintTaskRequest request);
    Task<bool> UpdateSprintTaskAsync(Guid teamId, Guid sprintId, Guid taskId, UpdateSprintTaskRequest request);
    
    // Additional methods for comprehensive sprint management
    Task<bool> AddBacklogItemToSprintAsync(Guid teamId, Guid sprintId, Guid backlogItemId);
    Task<bool> RemoveBacklogItemFromSprintAsync(Guid teamId, Guid sprintId, Guid backlogItemId);
    Task<bool> UpdateBacklogItemStatusAsync(Guid teamId, Guid sprintId, Guid itemId, string status);
    Task<bool> SaveSprintRetrospectiveAsync(Guid teamId, Guid sprintId, object retrospectiveData);
}