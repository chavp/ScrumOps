using ScrumOps.Shared.Contracts.Sprints;

namespace ScrumOps.Web.Services;

/// <summary>
/// Service interface for sprint management operations.
/// </summary>
public interface ISprintService
{
    Task<GetSprintsResponse> GetSprintsAsync();
    Task<SprintDetailsResponse> GetSprintAsync(string id);
    Task<SprintDetailsResponse> CreateSprintAsync(CreateSprintRequest request);
    Task<SprintDetailsResponse> UpdateSprintAsync(string id, UpdateSprintRequest request);
    Task<bool> DeleteSprintAsync(string id);
    Task<bool> StartSprintAsync(string id);
    Task<bool> CompleteSprintAsync(string id);
    Task<SprintBurndownResponse> GetSprintBurndownAsync(string id);
    Task<bool> AddSprintTaskAsync(string sprintId, string itemId, AddSprintTaskRequest request);
    Task<bool> UpdateSprintTaskAsync(string sprintId, string taskId, UpdateSprintTaskRequest request);
}