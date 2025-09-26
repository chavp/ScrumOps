using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.Sprints;

namespace ScrumOps.Web.Services;

/// <summary>
/// HTTP client service for sprint management operations.
/// </summary>
public class SprintService : ISprintService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SprintService> _logger;

    public SprintService(HttpClient httpClient, ILogger<SprintService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GetSprintsResponse> GetSprintsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching sprints from API");
            var response = await _httpClient.GetFromJsonAsync<GetSprintsResponse>("/api/sprints");
            return response ?? new GetSprintsResponse { Sprints = [], TotalCount = 0 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sprints");
            return new GetSprintsResponse { Sprints = [], TotalCount = 0 };
        }
    }

    public async Task<SprintDetailsResponse> GetSprintAsync(string id)
    {
        try
        {
            _logger.LogInformation("Fetching sprint {SprintId} from API", id);
            var response = await _httpClient.GetFromJsonAsync<SprintDetailsResponse>($"/api/sprints/{id}");
            return response ?? throw new InvalidOperationException($"Sprint {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sprint {SprintId}", id);
            throw;
        }
    }

    public async Task<SprintDetailsResponse> CreateSprintAsync(CreateSprintRequest request)
    {
        try
        {
            _logger.LogInformation("Creating sprint {Goal} for team {TeamId}", request.Goal, request.TeamId);
            var response = await _httpClient.PostAsJsonAsync("/api/sprints", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<SprintDetailsResponse>();
            return result ?? throw new InvalidOperationException("Failed to create sprint");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sprint {Goal} for team {TeamId}", request.Goal, request.TeamId);
            throw;
        }
    }

    public async Task<SprintDetailsResponse> UpdateSprintAsync(string id, UpdateSprintRequest request)
    {
        try
        {
            _logger.LogInformation("Updating sprint {SprintId}", id);
            var response = await _httpClient.PutAsJsonAsync($"/api/sprints/{id}", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<SprintDetailsResponse>();
            return result ?? throw new InvalidOperationException($"Failed to update sprint {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sprint {SprintId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteSprintAsync(string id)
    {
        try
        {
            _logger.LogInformation("Deleting sprint {SprintId}", id);
            var response = await _httpClient.DeleteAsync($"/api/sprints/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sprint {SprintId}", id);
            return false;
        }
    }

    public async Task<bool> StartSprintAsync(string id)
    {
        try
        {
            _logger.LogInformation("Starting sprint {SprintId}", id);
            var response = await _httpClient.PostAsync($"/api/sprints/{id}/start", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting sprint {SprintId}", id);
            return false;
        }
    }

    public async Task<bool> CompleteSprintAsync(string id)
    {
        try
        {
            _logger.LogInformation("Completing sprint {SprintId}", id);
            var response = await _httpClient.PostAsync($"/api/sprints/{id}/complete", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing sprint {SprintId}", id);
            return false;
        }
    }

    public async Task<SprintBurndownResponse> GetSprintBurndownAsync(string id)
    {
        try
        {
            _logger.LogInformation("Fetching sprint burndown for {SprintId}", id);
            var response = await _httpClient.GetFromJsonAsync<SprintBurndownResponse>($"/api/sprints/{id}/burndown");
            return response ?? throw new InvalidOperationException($"Burndown data for sprint {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sprint burndown for {SprintId}", id);
            throw;
        }
    }

    public async Task<bool> AddSprintTaskAsync(string sprintId, string itemId, AddSprintTaskRequest request)
    {
        try
        {
            _logger.LogInformation("Adding task {Title} to sprint {SprintId}", request.Title, sprintId);
            var response = await _httpClient.PostAsJsonAsync($"/api/sprints/{sprintId}/items/{itemId}/tasks", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task {Title} to sprint {SprintId}", request.Title, sprintId);
            return false;
        }
    }

    public async Task<bool> UpdateSprintTaskAsync(string sprintId, string taskId, UpdateSprintTaskRequest request)
    {
        try
        {
            _logger.LogInformation("Updating task {TaskId} in sprint {SprintId}", taskId, sprintId);
            var response = await _httpClient.PutAsJsonAsync($"/api/sprints/{sprintId}/tasks/{taskId}", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId} in sprint {SprintId}", taskId, sprintId);
            return false;
        }
    }
}