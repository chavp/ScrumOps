using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.Teams;

namespace ScrumOps.Web.Services;

/// <summary>
/// HTTP client service for team management operations.
/// </summary>
public class TeamService : ITeamService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TeamService> _logger;

    public TeamService(HttpClient httpClient, ILogger<TeamService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GetTeamsResponse> GetTeamsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching teams from API");
            var response = await _httpClient.GetFromJsonAsync<GetTeamsResponse>("/api/teams");
            return response ?? new GetTeamsResponse { Teams = [], TotalCount = 0 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teams");
            return new GetTeamsResponse { Teams = [], TotalCount = 0 };
        }
    }

    public async Task<TeamDetailsResponse> GetTeamAsync(int id)
    {
        try
        {
            _logger.LogInformation("Fetching team {TeamId} from API", id);
            var response = await _httpClient.GetFromJsonAsync<TeamDetailsResponse>($"/api/teams/{id}");
            return response ?? throw new InvalidOperationException($"Team {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team {TeamId}", id);
            throw;
        }
    }

    public async Task<TeamDetailsResponse> CreateTeamAsync(CreateTeamRequest request)
    {
        try
        {
            _logger.LogInformation("Creating team {TeamName}", request.Name);
            var response = await _httpClient.PostAsJsonAsync("/api/teams", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<TeamDetailsResponse>();
            return result ?? throw new InvalidOperationException("Failed to create team");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team {TeamName}", request.Name);
            throw;
        }
    }

    public async Task<TeamDetailsResponse> UpdateTeamAsync(int id, UpdateTeamRequest request)
    {
        try
        {
            _logger.LogInformation("Updating team {TeamId}", id);
            var response = await _httpClient.PutAsJsonAsync($"/api/teams/{id}", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<TeamDetailsResponse>();
            return result ?? throw new InvalidOperationException($"Failed to update team {id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team {TeamId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTeamAsync(int id)
    {
        try
        {
            _logger.LogInformation("Deleting team {TeamId}", id);
            var response = await _httpClient.DeleteAsync($"/api/teams/{id}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting team {TeamId}", id);
            return false;
        }
    }
}