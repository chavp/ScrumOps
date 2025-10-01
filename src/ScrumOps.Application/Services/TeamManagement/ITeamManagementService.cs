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
    Task<GetTeamsResponse> GetTeamsAsync(CancellationToken cancellationToken = default);
    
    Task<TeamDetailDto?> GetTeamByIdAsync(TeamId teamId, CancellationToken cancellationToken = default);
    
    Task<List<TeamMemberDto>?> GetTeamMembersAsync(TeamId teamId, CancellationToken cancellationToken = default);
    
    Task<TeamVelocityDto?> GetTeamVelocityAsync(TeamId teamId, CancellationToken cancellationToken = default);
    
    Task<TeamMetricsDto?> GetTeamMetricsAsync(TeamId teamId, CancellationToken cancellationToken = default);

    // Command operations
    Task<TeamId> CreateTeamAsync(
        string name,
        string? description,
        int sprintLengthWeeks,
        string productOwnerEmail,
        string scrumMasterEmail,
        CancellationToken cancellationToken = default);

    Task UpdateTeamAsync(
        TeamId teamId,
        string name,
        string? description,
        int sprintLengthWeeks,
        string productOwnerEmail,
        string scrumMasterEmail,
        CancellationToken cancellationToken = default);

    Task DeactivateTeamAsync(TeamId teamId, CancellationToken cancellationToken = default);
}