using ScrumOps.Shared.Contracts.Teams;

namespace ScrumOps.Web.Services;

/// <summary>
/// Service interface for team management operations.
/// </summary>
public interface ITeamService
{
    Task<ScrumOps.Shared.Contracts.Teams.GetTeamsResponse> GetTeamsAsync();
    Task<TeamDetailsResponse> GetTeamAsync(Guid id);
    Task<TeamDetailsResponse> CreateTeamAsync(CreateTeamRequest request);
    Task<TeamDetailsResponse> UpdateTeamAsync(Guid id, UpdateTeamRequest request);
    Task<bool> DeleteTeamAsync(Guid id);
    
    // Member management operations
    Task<TeamMember> AddTeamMemberAsync(Guid teamId, AddTeamMemberRequest request);
    Task<bool> RemoveTeamMemberAsync(Guid teamId, Guid memberId);
    Task<IEnumerable<TeamMember>> GetTeamMembersAsync(Guid teamId);
    
    // Additional operations
    Task<TeamVelocityResponse> GetTeamVelocityAsync(Guid teamId);
}