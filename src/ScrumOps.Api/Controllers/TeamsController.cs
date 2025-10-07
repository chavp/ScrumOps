using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.Services.TeamManagement;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Api.Extensions;

using ScrumOps.Domain.SharedKernel.Extensions;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// Teams API controller for managing scrum teams.
/// </summary>
[ApiController]
[Route("api/teams")]
[Produces("application/json")]
public class TeamsController : ApiBase
{
    private readonly ITeamManagementService _teamManagementService;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(ITeamManagementService teamManagementService, ILogger<TeamsController> logger)
    {
        _teamManagementService = teamManagementService;
        _logger = logger;
    }

    /// <summary>
    /// Get all teams.
    /// </summary>
    /// <returns>List of teams with summary information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetTeamsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTeams()
    {
        var result = await _teamManagementService.GetTeamsAsync();
        return result.Match(Ok, BadRequest);
    }

    /// <summary>
    /// Get a specific team by ID with detailed information.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team details including members and current sprint</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeamDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeam(Guid id)
        => await TeamId.Create(id)
            .ToMaybe()
            .BindAsync(req => _teamManagementService.GetTeamByIdAsync(req))
            .MatchAsync(Ok, NotFound);

    /// <summary>
    /// Create a new team.
    /// </summary>
    /// <param name="request">Team creation request</param>
    /// <returns>Created team information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] ScrumOps.Api.DTOs.CreateTeamRequest request)
    {
        var teamId = await _teamManagementService.CreateTeamAsync(
            request.Name,
            request.Description,
            request.SprintLengthWeeks
        );
        
        // Get the created team to return full details
        var createdTeam = await _teamManagementService.GetTeamByIdAsync(teamId);

        // Return created team directly with proper Location header
        Response.Headers.Location = $"/api/teams/{teamId.Value}";
        return StatusCode(201, createdTeam);
    }

    /// <summary>
    /// Update team details.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="request">Team update request</param>
    /// <returns>Updated team information</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTeam(Guid id, [FromBody] ScrumOps.Api.DTOs.UpdateTeamRequest request)
    {
        // Explicit approach with clear error handling for each step
        //var teamIdResult = TeamId.Create(id);
        //if (teamIdResult.IsFailure)
        //    return BadRequest(teamIdResult.Error);

        //var updateResult = await _teamManagementService.UpdateTeamAsync(
        //    teamIdResult.Value,
        //    request.Name,
        //    request.Description,
        //    request.SprintLengthWeeks,
        //    string.Empty, // TODO: Add these fields to request DTO
        //    string.Empty  // TODO: Add these fields to request DTO
        //);

        //if (updateResult.IsFailure)
        //    return BadRequest(updateResult.Error);

        //var updatedTeam = await _teamManagementService.GetTeamByIdAsync(updateResult.Value);

        //return updatedTeam.Match(
        //    team => Ok(team),
        //    () => NotFound(this.NotFoundProblem("team", id))
        //);

        // Alternative functional approach using extension methods:
        return await TeamId.Create(id)
            .BindAsync(teamId => _teamManagementService.UpdateTeamAsync(
                teamId,
                request.Name,
                request.Description,
                request.SprintLengthWeeks,
                string.Empty,
                string.Empty))
            .MatchAsync(
                async updatedTeamId =>
                {
                    var team = await _teamManagementService.GetTeamByIdAsync(updatedTeamId);
                    return team.Match(Ok, NotFound);
                },
                error => Task.FromResult<IActionResult>(BadRequest(error))
            );
    }

    /// <summary>
    /// Deactivate a team (soft delete).
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteTeam(Guid id)
    {
        var teamId = TeamId.From(id);
        await _teamManagementService.DeactivateTeamAsync(teamId);
        return NoContent();
    }

    /// <summary>
    /// Get team members with their roles.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>List of team members</returns>
    [HttpGet("{id:guid}/members")]
    [ProducesResponseType(typeof(List<TeamMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTeamMembers(Guid id)
    {
        var teamId = TeamId.From(id);
        var result = await _teamManagementService.GetTeamMembersAsync(teamId);

        if (!result.Any())
            return NotFound(this.NotFoundProblem("team", id));

        return Ok(result);
    }

    /// <summary>
    /// Get team velocity metrics.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team velocity data</returns>
    [HttpGet("{id:guid}/velocity")]
    [ProducesResponseType(typeof(TeamVelocityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTeamVelocity(Guid id)
        => await Maybe<TeamId>
            .Some(TeamId.From(id))
            .BindAsync(req => _teamManagementService.GetTeamVelocityAsync(req))
            .MatchAsync(Ok, NotFound);

    /// <summary>
    /// Get team performance metrics.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team metrics and KPIs</returns>
    [HttpGet("{id:guid}/metrics")]
    [ProducesResponseType(typeof(TeamMetricsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTeamMetrics(Guid id)
    => await Maybe<TeamId>
            .Some(TeamId.From(id))
            .BindAsync(req => _teamManagementService.GetTeamMetricsAsync(req))
            .MatchAsync(Ok, NotFound);

    /// <summary>
    /// Add a member to a team.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="request">Add member request</param>
    /// <returns>Added team member</returns>
    [HttpPost("{id:guid}/members")]
    [ProducesResponseType(typeof(TeamMemberDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamMemberDto>> AddTeamMember(Guid id, [FromBody] ScrumOps.Api.DTOs.AddTeamMemberRequest request)
    {
        var teamId = TeamId.From(id);
        var memberId = await _teamManagementService.AddTeamMemberAsync(teamId, request.Name, request.Email, request.Role);
        
        // Get the added member to return
        var teamMembers = await _teamManagementService.GetTeamMembersAsync(teamId);
        var addedMember = teamMembers?.FirstOrDefault(m => m.Id == memberId.Value);

        if (addedMember == null)
            return NotFound(this.NotFoundProblem("team", id));

        Response.Headers.Location = $"/api/teams/{id}/members/{memberId.Value}";
        return StatusCode(201, addedMember);
    }

    /// <summary>
    /// Remove a member from a team.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="memberId">Member ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:guid}/members/{memberId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> RemoveTeamMember(Guid id, Guid memberId)
    {
        var teamId = TeamId.From(id);
        var userIdObj = UserId.From(memberId);
        await _teamManagementService.RemoveTeamMemberAsync(teamId, userIdObj);
        return NoContent();
    }
}
