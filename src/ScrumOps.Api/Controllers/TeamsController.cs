using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.Services.TeamManagement;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Api.Extensions;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// Teams API controller for managing scrum teams.
/// </summary>
[ApiController]
[Route("api/teams")]
[Produces("application/json")]
public class TeamsController : ControllerBase
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
    public async Task<ActionResult<GetTeamsResponse>> GetTeams()
    {
        var result = await _teamManagementService.GetTeamsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Get a specific team by ID with detailed information.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team details including members and current sprint</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TeamDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamDetailDto>> GetTeam(Guid id)
    {
        var teamId = TeamId.From(id);
        var result = await _teamManagementService.GetTeamByIdAsync(teamId);

        if (result == null)
            return NotFound(this.NotFoundProblem("team", id));

        return Ok(result);
    }

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
            request.SprintLengthWeeks,
            request.ProductOwnerEmail ?? string.Empty,
            request.ScrumMasterEmail ?? string.Empty
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
    public async Task<ActionResult<TeamDto>> UpdateTeam(Guid id, [FromBody] ScrumOps.Api.DTOs.UpdateTeamRequest request)
    {
        var teamId = TeamId.From(id);
        await _teamManagementService.UpdateTeamAsync(
            teamId,
            request.Name,
            request.Description,
            request.SprintLengthWeeks,
            string.Empty, // TODO: Add these fields to request DTO
            string.Empty  // TODO: Add these fields to request DTO
        );
        
        // Get the updated team to return full details
        var updatedTeam = await _teamManagementService.GetTeamByIdAsync(teamId);

        if (updatedTeam == null)
            return NotFound(this.NotFoundProblem("team", id));

        return Ok(updatedTeam);
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
    public async Task<ActionResult<List<TeamMemberDto>>> GetTeamMembers(Guid id)
    {
        var teamId = TeamId.From(id);
        var result = await _teamManagementService.GetTeamMembersAsync(teamId);

        if (result == null)
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamVelocityDto>> GetTeamVelocity(Guid id)
    {
        var teamId = TeamId.From(id);
        var result = await _teamManagementService.GetTeamVelocityAsync(teamId);

        if (result == null)
            return NotFound(this.NotFoundProblem("team", id));

        return Ok(result);
    }

    /// <summary>
    /// Get team performance metrics.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team metrics and KPIs</returns>
    [HttpGet("{id:guid}/metrics")]
    [ProducesResponseType(typeof(TeamMetricsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamMetricsDto>> GetTeamMetrics(Guid id)
    {
        var teamId = TeamId.From(id);
        var result = await _teamManagementService.GetTeamMetricsAsync(teamId);

        if (result == null)
            return NotFound(this.NotFoundProblem("team", id));

        return Ok(result);
    }
}
