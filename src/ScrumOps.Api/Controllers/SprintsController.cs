using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.SprintManagement.Commands;
using ScrumOps.Application.SprintManagement.Queries;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// Sprints API controller for managing team sprints.
/// </summary>
[ApiController]
[Route("api/teams/{teamId:int}/sprints")]
[Produces("application/json")]
public class SprintsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<SprintsController> _logger;

    public SprintsController(IMediator mediator, ILogger<SprintsController> logger)
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
    /// Helper method to convert int ID to SprintId (temporary solution).
    /// </summary>
    private static SprintId ConvertToSprintId(int id)
    {
        var guidFromInt = new Guid($"11111111-1111-1111-1111-{id:000000000000}");
        return SprintId.From(guidFromInt);
    }

    /// <summary>
    /// Get team's sprints with optional filtering.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="status">Filter by sprint status (optional)</param>
    /// <param name="limit">Number of results (default 10, max 50)</param>
    /// <param name="offset">Pagination offset (default 0)</param>
    /// <returns>List of sprints</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetSprintsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetSprintsResponse>> GetSprints(
        int teamId,
        [FromQuery] string? status = null,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        try
        {
            if (limit > 50) limit = 50;
            if (limit < 1) limit = 10;
            if (offset < 0) offset = 0;

            var teamIdValue = ConvertToTeamId(teamId);
            var query = new GetSprintsQuery(teamIdValue, status, limit, offset);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {teamId} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting sprints for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get specific sprint with full details.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <returns>Sprint details with backlog items and impediments</returns>
    [HttpGet("{sprintId:int}")]
    [ProducesResponseType(typeof(SprintDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintDetailDto>> GetSprint(int teamId, int sprintId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var sprintIdValue = ConvertToSprintId(sprintId);
            var query = new GetSprintByIdQuery(teamIdValue, sprintIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Sprint with ID {sprintId} not found for team {teamId}" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Create a new sprint.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="request">Sprint creation request</param>
    /// <returns>Created sprint information</returns>
    [HttpPost]
    [ProducesResponseType(typeof(SprintDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SprintDto>> CreateSprint(int teamId, [FromBody] ScrumOps.Api.DTOs.CreateSprintRequest request)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var command = new CreateSprintCommand(
                teamIdValue,
                request.Name,
                request.Goal,
                request.StartDate,
                request.EndDate,
                request.Capacity
            );

            var sprintId = await _mediator.Send(command);
            
            // Get the created sprint to return full details
            var query = new GetSprintByIdQuery(teamIdValue, sprintId);
            var createdSprint = await _mediator.Send(query);

            return CreatedAtAction(
                nameof(GetSprint), 
                new { teamId, sprintId = sprintId.Value }, 
                createdSprint);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating sprint for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Update sprint details.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <param name="request">Sprint update request</param>
    /// <returns>Updated sprint information</returns>
    [HttpPut("{sprintId:int}")]
    [ProducesResponseType(typeof(SprintDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintDto>> UpdateSprint(int teamId, int sprintId, [FromBody] ScrumOps.Api.DTOs.UpdateSprintRequest request)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var sprintIdValue = ConvertToSprintId(sprintId);
            var command = new UpdateSprintCommand(
                sprintIdValue,
                request.Name,
                request.Goal,
                request.Capacity,
                request.Notes
            );

            await _mediator.Send(command);
            
            // Get the updated sprint to return full details
            var query = new GetSprintByIdQuery(teamIdValue, sprintIdValue);
            var updatedSprint = await _mediator.Send(query);

            if (updatedSprint == null)
                return NotFound(new { detail = $"Sprint with ID {sprintId} not found for team {teamId}" });

            return Ok(updatedSprint);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while updating sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Start a sprint (change status to Active).
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <returns>Sprint with updated status</returns>
    [HttpPost("{sprintId:int}/start")]
    [ProducesResponseType(typeof(ScrumOps.Api.DTOs.SprintStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScrumOps.Api.DTOs.SprintStatusDto>> StartSprint(int teamId, int sprintId)
    {
        try
        {
            var sprintIdValue = ConvertToSprintId(sprintId);
            var command = new StartSprintCommand(sprintIdValue);

            await _mediator.Send(command);
            
            var result = new ScrumOps.Api.DTOs.SprintStatusDto
            {
                Id = sprintId,
                Status = "Active",
                StartedAt = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while starting sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Complete a sprint.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <param name="request">Sprint completion request</param>
    /// <returns>Sprint with updated status</returns>
    [HttpPost("{sprintId:int}/complete")]
    [ProducesResponseType(typeof(ScrumOps.Api.DTOs.SprintStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScrumOps.Api.DTOs.SprintStatusDto>> CompleteSprint(int teamId, int sprintId, [FromBody] ScrumOps.Api.DTOs.CompleteSprintRequest request)
    {
        try
        {
            var sprintIdValue = ConvertToSprintId(sprintId);
            var command = new CompleteSprintCommand(
                sprintIdValue,
                request.ActualVelocity,
                request.Notes
            );

            await _mediator.Send(command);
            
            var result = new ScrumOps.Api.DTOs.SprintStatusDto
            {
                Id = sprintId,
                Status = "Completed",
                CompletedAt = DateTime.UtcNow
            };

            return Ok(result);
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("not found"))
        {
            return NotFound(new { detail = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while completing sprint {SprintId} for team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get sprint backlog items.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <returns>Sprint backlog items with tasks</returns>
    [HttpGet("{sprintId:int}/backlog")]
    [ProducesResponseType(typeof(GetSprintBacklogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetSprintBacklogResponse>> GetSprintBacklog(int teamId, int sprintId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var sprintIdValue = ConvertToSprintId(sprintId);
            var query = new GetSprintBacklogQuery(teamIdValue, sprintIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Sprint with ID {sprintId} not found for team {teamId}" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting sprint backlog for sprint {SprintId} team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get sprint burndown chart data.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <returns>Burndown chart data</returns>
    [HttpGet("{sprintId:int}/burndown")]
    [ProducesResponseType(typeof(SprintBurndownDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintBurndownDto>> GetSprintBurndown(int teamId, int sprintId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var sprintIdValue = ConvertToSprintId(sprintId);
            var query = new GetSprintBurndownQuery(teamIdValue, sprintIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Sprint with ID {sprintId} not found for team {teamId}" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting sprint burndown for sprint {SprintId} team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get sprint velocity calculation.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <returns>Sprint velocity metrics</returns>
    [HttpGet("{sprintId:int}/velocity")]
    [ProducesResponseType(typeof(SprintVelocityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintVelocityDto>> GetSprintVelocity(int teamId, int sprintId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var sprintIdValue = ConvertToSprintId(sprintId);
            var query = new GetSprintVelocityQuery(teamIdValue, sprintIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Sprint with ID {sprintId} not found for team {teamId}" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting sprint velocity for sprint {SprintId} team {TeamId}", sprintId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
}

// Note: DTOs are now defined in ScrumOps.Api.DTOs.ApiDtos
