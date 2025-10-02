using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.Sprints;
using ScrumOps.Application.Services.SprintManagement;

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

    public async Task<ScrumOps.Shared.Contracts.Sprints.GetSprintsResponse> GetSprintsAsync(Guid teamId)
    {
        try
        {
            _logger.LogInformation("Fetching sprints for team {TeamId} from API", teamId);
            var apiResponse = await _httpClient.GetFromJsonAsync<ScrumOps.Application.Services.SprintManagement.GetSprintsResponse>($"/api/teams/{teamId}/sprints");
            if (apiResponse == null)
            {
                return new ScrumOps.Shared.Contracts.Sprints.GetSprintsResponse { Sprints = [], TotalCount = 0 };
            }
            
            // Map from Application layer to Web layer contracts
            return new ScrumOps.Shared.Contracts.Sprints.GetSprintsResponse
            {
                Sprints = apiResponse.Sprints?.Select(MapToSprintSummary).ToList() ?? [],
                TotalCount = apiResponse.TotalCount
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sprints for team {TeamId}", teamId);
            return new ScrumOps.Shared.Contracts.Sprints.GetSprintsResponse { Sprints = [], TotalCount = 0 };
        }
    }

    public async Task<SprintDetailsResponse> GetSprintAsync(Guid teamId, Guid sprintId)
    {
        try
        {
            _logger.LogInformation("Fetching sprint {SprintId} for team {TeamId} from API", sprintId, teamId);
            var apiResponse = await _httpClient.GetFromJsonAsync<SprintDetailDto>($"/api/teams/{teamId}/sprints/{sprintId}");
            if (apiResponse == null)
                throw new InvalidOperationException($"Sprint {sprintId} not found for team {teamId}");

            // Map API response to Web contract
            return MapToSprintDetailsResponse(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sprint {SprintId} for team {TeamId}", sprintId, teamId);
            throw;
        }
    }

    public async Task<SprintDetailsResponse> CreateSprintAsync(Guid teamId, CreateSprintRequest request)
    {
        try
        {
            _logger.LogInformation("Creating sprint {Goal} for team {TeamId}", request.Goal, teamId);
            
            var apiRequest = new
            {
                Name = $"Sprint {DateTime.Now:yyyy-MM-dd}",
                Goal = request.Goal,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Capacity = request.Capacity
            };

            var response = await _httpClient.PostAsJsonAsync($"/api/teams/{teamId}/sprints", apiRequest);
            response.EnsureSuccessStatusCode();
            
            var apiResult = await response.Content.ReadFromJsonAsync<SprintDetailDto>();
            if (apiResult == null)
                throw new InvalidOperationException("Failed to create sprint");

            return MapToSprintDetailsResponse(apiResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating sprint {Goal} for team {TeamId}", request.Goal, teamId);
            throw;
        }
    }

    public async Task<SprintDetailsResponse> UpdateSprintAsync(Guid teamId, Guid sprintId, UpdateSprintRequest request)
    {
        try
        {
            _logger.LogInformation("Updating sprint {SprintId} for team {TeamId}", sprintId, teamId);
            
            var apiRequest = new
            {
                Name = $"Sprint {DateTime.Now:yyyy-MM-dd}",
                Goal = request.Goal,
                Capacity = request.Capacity,
                Notes = ""
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/teams/{teamId}/sprints/{sprintId}", apiRequest);
            response.EnsureSuccessStatusCode();
            
            var apiResult = await response.Content.ReadFromJsonAsync<SprintDetailDto>();
            if (apiResult == null)
                throw new InvalidOperationException($"Failed to update sprint {sprintId}");

            return MapToSprintDetailsResponse(apiResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating sprint {SprintId} for team {TeamId}", sprintId, teamId);
            throw;
        }
    }

    public async Task<bool> DeleteSprintAsync(Guid teamId, Guid sprintId)
    {
        try
        {
            _logger.LogInformation("Deleting sprint {SprintId} for team {TeamId}", sprintId, teamId);
            var response = await _httpClient.DeleteAsync($"/api/teams/{teamId}/sprints/{sprintId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return false;
        }
    }

    public async Task<bool> StartSprintAsync(Guid teamId, Guid sprintId)
    {
        try
        {
            _logger.LogInformation("Starting sprint {SprintId} for team {TeamId}", sprintId, teamId);
            var response = await _httpClient.PostAsync($"/api/teams/{teamId}/sprints/{sprintId}/start", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return false;
        }
    }

    public async Task<bool> CompleteSprintAsync(Guid teamId, Guid sprintId)
    {
        try
        {
            _logger.LogInformation("Completing sprint {SprintId} for team {TeamId}", sprintId, teamId);
            var response = await _httpClient.PostAsync($"/api/teams/{teamId}/sprints/{sprintId}/complete", null);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return false;
        }
    }

    public async Task<SprintBurndownResponse> GetSprintBurndownAsync(Guid teamId, Guid sprintId)
    {
        try
        {
            _logger.LogInformation("Fetching sprint burndown for {SprintId} for team {TeamId}", sprintId, teamId);
            var response = await _httpClient.GetFromJsonAsync<SprintBurndownResponse>($"/api/teams/{teamId}/sprints/{sprintId}/burndown");
            return response ?? throw new InvalidOperationException($"Burndown data for sprint {sprintId} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching sprint burndown for {SprintId} for team {TeamId}", sprintId, teamId);
            throw;
        }
    }

    public async Task<bool> AddSprintTaskAsync(Guid teamId, Guid sprintId, Guid itemId, AddSprintTaskRequest request)
    {
        try
        {
            _logger.LogInformation("Adding task {Title} to sprint {SprintId} for team {TeamId}", request.Title, sprintId, teamId);
            var response = await _httpClient.PostAsJsonAsync($"/api/teams/{teamId}/sprints/{sprintId}/items/{itemId}/tasks", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding task {Title} to sprint {SprintId} for team {TeamId}", request.Title, sprintId, teamId);
            return false;
        }
    }

    public async Task<bool> UpdateSprintTaskAsync(Guid teamId, Guid sprintId, Guid taskId, UpdateSprintTaskRequest request)
    {
        try
        {
            _logger.LogInformation("Updating task {TaskId} in sprint {SprintId} for team {TeamId}", taskId, sprintId, teamId);
            var response = await _httpClient.PutAsJsonAsync($"/api/teams/{teamId}/sprints/{sprintId}/tasks/{taskId}", request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating task {TaskId} in sprint {SprintId} for team {TeamId}", taskId, sprintId, teamId);
            return false;
        }
    }

    private static SprintSummary MapToSprintSummary(ScrumOps.Application.Services.SprintManagement.SprintDto dto)
    {
        return new SprintSummary
        {
            Id = dto.Id, // Keep as Guid
            TeamId = dto.TeamId, // Keep as Guid
            TeamName = dto.TeamName,
            Goal = dto.Goal,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status,
            CommittedStoryPoints = dto.Capacity, // Use capacity as committed points
            CompletedStoryPoints = dto.CompletedItemCount,
            TaskCount = dto.BacklogItemCount,
            CompletedTaskCount = dto.CompletedItemCount,
            ProgressPercentage = dto.Capacity > 0 ? (decimal)(dto.CompletedItemCount * 100.0 / dto.Capacity) : 0
        };
    }

    private static SprintDetailsResponse MapToSprintDetailsResponse(SprintDetailDto dto)
    {
        return new SprintDetailsResponse
        {
            Id = dto.Id.ToString(),
            TeamId = dto.Id.GetHashCode(), // Use sprint ID hash as team ID for now
            TeamName = "Team", // Need to get team name from another source
            Goal = dto.Goal,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ActualStartDate = dto.StartDate, // Use start date as actual start
            ActualEndDate = dto.Status == "Completed" ? dto.EndDate : null,
            Capacity = dto.Capacity,
            Status = dto.Status,
            CommittedStoryPoints = dto.Capacity,
            CompletedStoryPoints = dto.BacklogItems?.Count(bi => bi.Status == "Done") ?? 0,
            ActualVelocity = dto.ActualVelocity ?? 0,
            BacklogItems = dto.BacklogItems?.Select(MapToSprintBacklogItem).ToList() ?? [],
            Tasks = new List<SprintTask>() // Tasks are nested in backlog items
        };
    }

    private static SprintBacklogItem MapToSprintBacklogItem(SprintBacklogItemDto dto)
    {
        return new SprintBacklogItem
        {
            Id = dto.Id.ToString(),
            BacklogItemId = dto.ProductBacklogItemId.ToString(),
            Title = dto.Title,
            Description = "", // Not available in this DTO
            Priority = 1, // Default priority
            StoryPoints = dto.OriginalEstimate,
            Status = dto.Status,
            Type = "UserStory", // Default type
            Tasks = new List<SprintTask>() // Would need separate call to get tasks
        };
    }

    private static SprintTask MapToSprintTask(ScrumOps.Application.Services.SprintManagement.TaskDto dto)
    {
        return new SprintTask
        {
            Id = dto.Id.ToString(),
            Title = dto.Title,
            Description = dto.Description,
            EstimatedHours = dto.RemainingHours, // Use remaining hours as estimate
            ActualHours = 0, // Not available in this DTO
            Status = dto.Status,
            AssigneeId = dto.AssignedTo,
            AssigneeName = dto.AssignedTo,
            CreatedDate = DateTime.Now, // Not available in this DTO
            CompletedDate = dto.Status == "Done" ? DateTime.Now : null
        };
    }
}