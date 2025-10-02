using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.Repositories;
using ScrumOps.Domain.TeamManagement.ValueObjects;

namespace ScrumOps.Application.Services.TeamManagement;

/// <summary>
/// Service implementation for team management operations.
/// </summary>
public class TeamManagementService : ITeamManagementService
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    public TeamManagementService(ITeamRepository teamRepository, IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetTeamsResponse> GetTeamsAsync(CancellationToken cancellationToken = default)
    {
        var teams = await _teamRepository.GetActiveTeamsAsync(cancellationToken);
        var teamDtos = teams.Select(team => new TeamDto
        {
            Id = team.Id.Value,
            Name = team.Name.Value,
            Description = team.Description?.Value,
            SprintLengthWeeks = team.SprintLength.Weeks,
            Velocity = team.CurrentVelocity?.Value ?? 0,
            IsActive = team.IsActive,
            CreatedDate = team.CreatedDate,
            MemberCount = team.Members.Count
        }).ToList();

        return new GetTeamsResponse
        {
            Teams = teamDtos,
            TotalCount = teamDtos.Count
        };
    }

    public async Task<TeamDetailDto?> GetTeamByIdAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null) return null;

        var members = team.Members.Select(member => new TeamMemberDto
        {
            Id = member.Id.Value,
            Name = member.Name.Value,
            Role = member.Role.ToString(),
            Email = member.Email.Value,
            IsActive = member.IsActive
        }).ToList();

        return new TeamDetailDto
        {
            Id = team.Id.Value,
            Name = team.Name.Value,
            Description = team.Description?.Value,
            SprintLengthWeeks = team.SprintLength.Weeks,
            Velocity = team.CurrentVelocity?.Value ?? 0,
            IsActive = team.IsActive,
            CreatedDate = team.CreatedDate,
            Members = members,
            CurrentSprint = null // TODO: Add current sprint lookup if needed
        };
    }

    public async Task<List<TeamMemberDto>?> GetTeamMembersAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null) return null;

        return team.Members.Select(member => new TeamMemberDto
        {
            Id = member.Id.Value,
            Name = member.Name.Value,
            Role = member.Role.ToString(),
            Email = member.Email.Value,
            IsActive = member.IsActive
        }).ToList();
    }

    public async Task<TeamVelocityDto?> GetTeamVelocityAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new TeamVelocityDto
        {
            TeamId = teamId.Value,
            CurrentVelocity = 20,
            AverageVelocity = 18,
            VelocityTrend = new List<VelocityDataPoint>(),
            TotalSprints = 5,
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task<TeamMetricsDto?> GetTeamMetricsAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new TeamMetricsDto
        {
            TeamId = teamId.Value,
            TeamName = "Sample Team",
            Velocity = new VelocityMetrics(),
            Quality = new QualityMetrics(),
            Productivity = new ProductivityMetrics(),
            Engagement = new EngagementMetrics(),
            LastUpdated = DateTime.UtcNow
        };
    }

    public async Task<TeamId> CreateTeamAsync(
        string name,
        string? description,
        int sprintLengthWeeks,
        CancellationToken cancellationToken = default)
    {
        // Check if team with same name already exists
        var existingTeam = await _teamRepository.ExistsWithNameAsync(name, cancellationToken: cancellationToken);
        if (existingTeam)
        {
            throw new InvalidOperationException($"A team with the name '{name}' already exists.");
        }

        // Create new team
        var teamId = TeamId.New();
        var teamName = TeamName.Create(name);
        var teamDescription = description != null ? TeamDescription.Create(description) : null;
        var sprintLength = SprintLength.Create(sprintLengthWeeks);

        var team = new Team(teamId, teamName, teamDescription!, sprintLength);

        await _teamRepository.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return team.Id;
    }

    public async Task UpdateTeamAsync(
        TeamId teamId,
        string name,
        string? description,
        int sprintLengthWeeks,
        string productOwnerEmail,
        string scrumMasterEmail,
        CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException($"Team with ID {teamId.Value} not found.");
        }

        // Check if name conflicts with another team
        var existingTeam = await _teamRepository.ExistsWithNameAsync(name, teamId, cancellationToken);
        if (existingTeam)
        {
            throw new InvalidOperationException($"A team with the name '{name}' already exists.");
        }

        team.UpdateTeamInfo(
            TeamName.Create(name), 
            description != null ? TeamDescription.Create(description) : null, 
            SprintLength.Create(sprintLengthWeeks)
        );

        await _teamRepository.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateTeamAsync(TeamId teamId, CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException($"Team with ID {teamId.Value} not found.");
        }

        team.Deactivate();

        await _teamRepository.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserId> AddTeamMemberAsync(
        TeamId teamId,
        string name,
        string email,
        string role,
        CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException($"Team with ID {teamId.Value} not found.");
        }

        // Create new user ID for the member
        var memberId = UserId.From(Guid.NewGuid());
        
        // Create User entity with proper value objects
        var user = new User(
            memberId, 
            teamId, 
            UserName.Create(name), 
            Email.Create(email), 
            ScrumRole.FromString(role)
        );
        
        // Add member to team
        team.AddMember(user);

        await _teamRepository.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return memberId;
    }

    public async Task RemoveTeamMemberAsync(
        TeamId teamId,
        UserId memberId,
        CancellationToken cancellationToken = default)
    {
        var team = await _teamRepository.GetByIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new InvalidOperationException($"Team with ID {teamId.Value} not found.");
        }

        team.RemoveMember(memberId);

        await _teamRepository.UpdateAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
