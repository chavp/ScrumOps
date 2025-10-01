using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.Services.SprintManagement;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.SprintManagement.ValueObjects;
using ScrumOps.Api.Extensions;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// Sprints API controller for managing team sprints.
/// </summary>
[ApiController]
[Route("api/teams/{teamId:guid}/sprints")]
[Produces("application/json")]
public class SprintsController : ControllerBase
{
    private readonly ISprintManagementService _sprintManagementService;
    private readonly ILogger<SprintsController> _logger;

    public SprintsController(ISprintManagementService sprintManagementService, ILogger<SprintsController> logger)
    {
        _sprintManagementService = sprintManagementService;
        _logger = logger;
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
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetSprintsResponse>> GetSprints(
        Guid teamId,
        [FromQuery] string? status = null,
        [FromQuery] int limit = 10,
        [FromQuery] int offset = 0)
    {
        if (limit > 50) limit = 50;
        if (limit < 1) limit = 10;
        if (offset < 0) offset = 0;

        var teamIdValue = TeamId.From(teamId);
        var result = await _sprintManagementService.GetSprintsAsync(teamIdValue, status, limit, offset);

        if (result == null)
            return NotFound(this.NotFoundProblem("team", teamId));

        return Ok(result);
    }

    /// <summary>
    /// Get specific sprint with full details.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="sprintId">Sprint ID</param>
    /// <returns>Sprint details with backlog items and impediments</returns>
    [HttpGet("{sprintId:guid}")]
    [ProducesResponseType(typeof(SprintDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintDetailDto>> GetSprint(Guid teamId, Guid sprintId)
    {
        try
        {
            var teamIdValue = TeamId.From(teamId);
            var sprintIdValue = SprintId.From(sprintId);
            var result = await _sprintManagementService.GetSprintByIdAsync(teamIdValue, sprintIdValue);

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
    public async Task<ActionResult<SprintDto>> CreateSprint(Guid teamId, [FromBody] ScrumOps.Api.DTOs.CreateSprintRequest request)
    {
        try
        {
            var teamIdValue = TeamId.From(teamId);
            var sprintId = await _sprintManagementService.CreateSprintAsync(
                teamIdValue,
                request.Name,
                request.Goal,
                request.StartDate,
                request.EndDate,
                request.Capacity
            );
            
            // Get the created sprint to return full details
            var createdSprint = await _sprintManagementService.GetSprintByIdAsync(teamIdValue, sprintId);

            // Return created sprint directly with proper Location header
            Response.Headers.Location = $"/api/teams/{teamId}/sprints/{sprintId.Value}";
            return StatusCode(201, createdSprint);
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
    [HttpPut("{sprintId:guid}")]
    [ProducesResponseType(typeof(SprintDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintDto>> UpdateSprint(Guid teamId, Guid sprintId, [FromBody] ScrumOps.Api.DTOs.UpdateSprintRequest request)
    {
        try
        {
            var teamIdValue = TeamId.From(teamId);
            var sprintIdValue = SprintId.From(sprintId);
            await _sprintManagementService.UpdateSprintAsync(
                sprintIdValue,
                request.Name,
                request.Goal,
                DateTime.MinValue, // TODO: Get dates from request
                DateTime.MinValue, // TODO: Get dates from request
                request.Capacity,
                request.Notes
            );
            
            // Get the updated sprint to return full details
            var updatedSprint = await _sprintManagementService.GetSprintByIdAsync(teamIdValue, sprintIdValue);

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
    [HttpPost("{sprintId:guid}/start")]
    [ProducesResponseType(typeof(ScrumOps.Api.DTOs.SprintStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScrumOps.Api.DTOs.SprintStatusDto>> StartSprint(Guid teamId, Guid sprintId)
    {
        try
        {
            var sprintIdValue = SprintId.From(sprintId);
            await _sprintManagementService.StartSprintAsync(sprintIdValue);
            
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
    [HttpPost("{sprintId:guid}/complete")]
    [ProducesResponseType(typeof(ScrumOps.Api.DTOs.SprintStatusDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScrumOps.Api.DTOs.SprintStatusDto>> CompleteSprint(Guid teamId, Guid sprintId, [FromBody] ScrumOps.Api.DTOs.CompleteSprintRequest request)
    {
        try
        {
            var sprintIdValue = SprintId.From(sprintId);
            await _sprintManagementService.CompleteSprintAsync(
                sprintIdValue,
                null, // ActualEndDate not available in DTO
                request.Notes
            );
            
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
    [HttpGet("{sprintId:guid}/backlog")]
    [ProducesResponseType(typeof(GetSprintBacklogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetSprintBacklogResponse>> GetSprintBacklog(Guid teamId, Guid sprintId)
    {
        try
        {
            var teamIdValue = TeamId.From(teamId);
            var sprintIdValue = SprintId.From(sprintId);
            var result = await _sprintManagementService.GetSprintBacklogAsync(teamIdValue, sprintIdValue);

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
    [HttpGet("{sprintId:guid}/burndown")]
    [ProducesResponseType(typeof(SprintBurndownDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintBurndownDto>> GetSprintBurndown(Guid teamId, Guid sprintId)
    {
        try
        {
            var teamIdValue = TeamId.From(teamId);
            var sprintIdValue = SprintId.From(sprintId);
            var result = await _sprintManagementService.GetSprintBurndownAsync(teamIdValue, sprintIdValue);

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
    [HttpGet("{sprintId:guid}/velocity")]
    [ProducesResponseType(typeof(SprintVelocityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SprintVelocityDto>> GetSprintVelocity(Guid teamId, Guid sprintId)
    {
        try
        {
            var teamIdValue = TeamId.From(teamId);
            var sprintIdValue = SprintId.From(sprintId);
            var result = await _sprintManagementService.GetSprintVelocityAsync(teamIdValue, sprintIdValue);

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
