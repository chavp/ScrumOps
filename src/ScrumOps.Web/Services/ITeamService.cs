using ScrumOps.Shared.Contracts.Teams;

namespace ScrumOps.Web.Services;

/// <summary>
/// Service interface for team management operations.
/// </summary>
public interface ITeamService
{
    Task<GetTeamsResponse> GetTeamsAsync();
    Task<TeamDetailsResponse> GetTeamAsync(int id);
    Task<TeamDetailsResponse> CreateTeamAsync(CreateTeamRequest request);
    Task<TeamDetailsResponse> UpdateTeamAsync(int id, UpdateTeamRequest request);
    Task<bool> DeleteTeamAsync(int id);
}