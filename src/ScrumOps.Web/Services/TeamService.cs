using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.Teams;
using ScrumOps.Application.Services.TeamManagement;

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

    public async Task<ScrumOps.Shared.Contracts.Teams.GetTeamsResponse> GetTeamsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching teams from API");
            var response = await _httpClient.GetFromJsonAsync<ScrumOps.Application.Services.TeamManagement.GetTeamsResponse>("/api/teams");
            
            if (response == null)
                return new ScrumOps.Shared.Contracts.Teams.GetTeamsResponse { Teams = [], TotalCount = 0 };

            // Map from Application DTOs to Web Contracts
            var teams = response.Teams.Select(MapToTeamSummary).ToArray();
            return new ScrumOps.Shared.Contracts.Teams.GetTeamsResponse 
            { 
                Teams = teams, 
                TotalCount = response.TotalCount 
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching teams");
            return new ScrumOps.Shared.Contracts.Teams.GetTeamsResponse { Teams = [], TotalCount = 0 };
        }
    }

    public async Task<TeamDetailsResponse> GetTeamAsync(Guid id)
    {
        try
        {
            _logger.LogInformation("Fetching team {TeamId} from API", id);
            var response = await _httpClient.GetFromJsonAsync<TeamDetailDto>($"/api/teams/{id}");
            
            if (response == null)
                throw new InvalidOperationException($"Team {id} not found");

            return MapToTeamDetailsResponse(response);
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
            
            // Create the request object to match API expectations
            var apiRequest = new
            {
                Name = request.Name,
                Description = request.Description,
                SprintLengthWeeks = request.SprintLengthWeeks,
                ProductOwnerEmail = (string?)null,
                ScrumMasterEmail = (string?)null,
            };

            var response = await _httpClient.PostAsJsonAsync("/api/teams", apiRequest);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<TeamDetailDto>();
            if (result == null)
                throw new InvalidOperationException("Failed to create team");

            return MapToTeamDetailsResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team {TeamName}", request.Name);
            throw;
        }
    }

    public async Task<TeamDetailsResponse> UpdateTeamAsync(Guid id, UpdateTeamRequest request)
    {
        try
        {
            _logger.LogInformation("Updating team {TeamId}", id);
            
            // Create the request object to match API expectations
            var apiRequest = new
            {
                Name = request.Name,
                Description = request.Description,
                SprintLengthWeeks = request.SprintLengthWeeks
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/teams/{id}", apiRequest);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<TeamDetailDto>();
            if (result == null)
                throw new InvalidOperationException($"Failed to update team {id}");

            return MapToTeamDetailsResponse(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team {TeamId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteTeamAsync(Guid id)
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

    private static TeamSummary MapToTeamSummary(TeamDto dto)
    {
        return new TeamSummary
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            SprintLengthWeeks = dto.SprintLengthWeeks,
            Velocity = dto.Velocity,
            MemberCount = dto.MemberCount,
            CurrentSprintId = dto.CurrentSprintId,
            IsActive = dto.IsActive,
            CreatedDate = dto.CreatedDate
        };
    }

    private static TeamDetailsResponse MapToTeamDetailsResponse(TeamDetailDto dto)
    {
        return new TeamDetailsResponse
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            SprintLengthWeeks = dto.SprintLengthWeeks,
            Velocity = dto.Velocity,
            IsActive = dto.IsActive,
            CreatedDate = dto.CreatedDate,
            CurrentSprintId = dto.CurrentSprint?.Id,
            Members = dto.Members.Select(MapToTeamMember).ToArray()
        };
    }

    private static TeamMember MapToTeamMember(TeamMemberDto dto)
    {
        return new TeamMember
        {
            Id = dto.Id,
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role,
            IsActive = dto.IsActive,
            CreatedDate = DateTime.UtcNow, // Default value - the Application DTO doesn't have this
            LastLoginDate = null // Default value - the Application DTO doesn't have this
        };
    }

    public async Task<TeamMember> AddTeamMemberAsync(Guid teamId, AddTeamMemberRequest request)
    {
        try
        {
            _logger.LogInformation("Adding member {Name} to team {TeamId}", request.Name, teamId);
            
            var response = await _httpClient.PostAsJsonAsync($"/api/teams/{teamId}/members", new
            {
                Name = request.Name,
                Email = request.Email,
                Role = request.Role
            });
            
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<TeamMemberDto>();
            if (result == null)
                throw new InvalidOperationException("Failed to add team member");

            return MapToTeamMember(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding member to team {TeamId}", teamId);
            throw;
        }
    }

    public async Task<bool> RemoveTeamMemberAsync(Guid teamId, Guid memberId)
    {
        try
        {
            _logger.LogInformation("Removing member {MemberId} from team {TeamId}", memberId, teamId);
            var response = await _httpClient.DeleteAsync($"/api/teams/{teamId}/members/{memberId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing member {MemberId} from team {TeamId}", memberId, teamId);
            return false;
        }
    }

    public async Task<IEnumerable<TeamMember>> GetTeamMembersAsync(Guid teamId)
    {
        try
        {
            _logger.LogInformation("Fetching members for team {TeamId}", teamId);
            var response = await _httpClient.GetFromJsonAsync<List<TeamMemberDto>>($"/api/teams/{teamId}/members");
            
            if (response == null)
                return Array.Empty<TeamMember>();

            return response.Select(MapToTeamMember).ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team members for team {TeamId}", teamId);
            return Array.Empty<TeamMember>();
        }
    }

    public async Task<TeamVelocityResponse> GetTeamVelocityAsync(Guid teamId)
    {
        try
        {
            _logger.LogInformation("Fetching velocity for team {TeamId}", teamId);
            var response = await _httpClient.GetFromJsonAsync<TeamVelocityResponse>($"/api/teams/{teamId}/velocity");
            
            return response ?? new TeamVelocityResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching team velocity for team {TeamId}", teamId);
            return new TeamVelocityResponse();
        }
    }
}
