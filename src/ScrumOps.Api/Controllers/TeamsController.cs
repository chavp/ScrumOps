using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.TeamManagement.Commands;
using ScrumOps.Application.TeamManagement.Queries;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// Teams API controller for managing scrum teams.
/// </summary>
[ApiController]
[Route("api/teams")]
[Produces("application/json")]
public class TeamsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<TeamsController> _logger;

    public TeamsController(IMediator mediator, ILogger<TeamsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Helper method to convert int ID to TeamId (temporary solution).
    /// </summary>
    private static TeamId ConvertToTeamId(int id)
    {
        var guidFromInt = new Guid($"00000000-0000-0000-0000-{id:000000000000}");
        return TeamId.From(guidFromInt);
    }

    /// <summary>
    /// Get all teams.
    /// </summary>
    /// <returns>List of teams with summary information</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetTeamsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetTeamsResponse>> GetTeams()
    {
        try
        {
            var query = new GetTeamsQuery();
            var result = await _mediator.Send(query);
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting teams");
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get a specific team by ID with detailed information.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team details including members and current sprint</returns>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TeamDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamDetailDto>> GetTeam(int id)
    {
        try
        {
            var teamId = ConvertToTeamId(id);
            var query = new GetTeamByIdQuery(teamId);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting team {TeamId}", id);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Create a new team.
    /// </summary>
    /// <param name="request">Team creation request</param>
    /// <returns>Created team information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TeamDto>> CreateTeam([FromBody] ScrumOps.Api.DTOs.CreateTeamRequest request)
    {
        try
        {
            var command = new CreateTeamCommand(
                request.Name,
                request.Description,
                request.SprintLengthWeeks,
                request.ProductOwnerEmail ?? string.Empty,
                request.ScrumMasterEmail ?? string.Empty
            );

            var teamId = await _mediator.Send(command);
            
            // Get the created team to return full details
            var query = new GetTeamByIdQuery(teamId);
            var createdTeam = await _mediator.Send(query);

            return CreatedAtAction(
                nameof(GetTeam), 
                new { id = teamId.Value }, 
                createdTeam);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating team");
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Update team details.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <param name="request">Team update request</param>
    /// <returns>Updated team information</returns>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TeamDto>> UpdateTeam(int id, [FromBody] ScrumOps.Api.DTOs.UpdateTeamRequest request)
    {
        try
        {
            var teamId = ConvertToTeamId(id);
            var command = new UpdateTeamCommand(
                teamId,
                request.Name,
                request.Description,
                request.SprintLengthWeeks
            );

            await _mediator.Send(command);
            
            // Get the updated team to return full details
            var query = new GetTeamByIdQuery(teamId);
            var updatedTeam = await _mediator.Send(query);

            if (updatedTeam == null)
                return NotFound(new { detail = $"Team with ID {id} not found" });

            return Ok(updatedTeam);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating team {TeamId}", id);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Deactivate a team (soft delete).
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteTeam(int id)
    {
        try
        {
            var teamId = ConvertToTeamId(id);
            var command = new DeactivateTeamCommand(teamId);

            await _mediator.Send(command);
            return NoContent();
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting team {TeamId}", id);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get team members with their roles.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>List of team members</returns>
    [HttpGet("{id:int}/members")]
    [ProducesResponseType(typeof(List<TeamMemberDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<TeamMemberDto>>> GetTeamMembers(int id)
    {
        try
        {
            var teamId = ConvertToTeamId(id);
            var query = new GetTeamMembersQuery(teamId);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting team members for team {TeamId}", id);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get team velocity metrics.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team velocity data</returns>
    [HttpGet("{id:int}/velocity")]
    [ProducesResponseType(typeof(TeamVelocityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamVelocityDto>> GetTeamVelocity(int id)
    {
        try
        {
            var teamId = ConvertToTeamId(id);
            var query = new GetTeamVelocityQuery(teamId);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting team velocity for team {TeamId}", id);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get team performance metrics.
    /// </summary>
    /// <param name="id">Team ID</param>
    /// <returns>Team metrics and KPIs</returns>
    [HttpGet("{id:int}/metrics")]
    [ProducesResponseType(typeof(TeamMetricsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TeamMetricsDto>> GetTeamMetrics(int id)
    {
        try
        {
            var teamId = ConvertToTeamId(id);
            var query = new GetTeamMetricsQuery(teamId);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {id} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting team metrics for team {TeamId}", id);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
}
