using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.Services.TeamManagement;

/// <summary>
/// Service interface for team management operations.
/// </summary>
public interface ITeamManagementService
{
    // Query operations
    Task<Result<GetTeamsResponse>> GetTeamsAsync(CancellationToken cancellationToken = default);
    
    Task<Maybe<TeamDetailDto>> GetTeamByIdAsync(TeamId teamId, CancellationToken cancellationToken = default);
    
    Task<List<TeamMemberDto>> GetTeamMembersAsync(TeamId teamId, CancellationToken cancellationToken = default);
    
    Task<Maybe<TeamVelocityDto>> GetTeamVelocityAsync(TeamId teamId, CancellationToken cancellationToken = default);
    
    Task<Maybe<TeamMetricsDto>> GetTeamMetricsAsync(TeamId teamId, CancellationToken cancellationToken = default);

    // Command operations
    Task<TeamId> CreateTeamAsync(
        string name,
        string? description,
        int sprintLengthWeeks,
        CancellationToken cancellationToken = default);

    Task<Result<TeamId>> UpdateTeamAsync(
        TeamId teamId,
        string name,
        string? description,
        int sprintLengthWeeks,
        string productOwnerEmail,
        string scrumMasterEmail,
        CancellationToken cancellationToken = default);

    Task<Result<int>> DeactivateTeamAsync(TeamId teamId, CancellationToken cancellationToken = default);

    // Member operations
    Task<UserId> AddTeamMemberAsync(
        TeamId teamId,
        string name,
        string email,
        string role,
        CancellationToken cancellationToken = default);

    Task RemoveTeamMemberAsync(
        TeamId teamId,
        UserId memberId,
        CancellationToken cancellationToken = default);
}
