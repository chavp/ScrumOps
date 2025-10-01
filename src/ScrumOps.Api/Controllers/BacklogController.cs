using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.ProductBacklog.Commands;
using ScrumOps.Application.ProductBacklog.Queries;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// Product Backlog API controller for managing team backlogs and items.
/// </summary>
[ApiController]
[Route("api/teams/{teamId:int}/backlog")]
[Produces("application/json")]
public class BacklogController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BacklogController> _logger;

    public BacklogController(IMediator mediator, ILogger<BacklogController> logger)
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
    /// Helper method to convert int ID to ProductBacklogItemId (temporary solution).
    /// </summary>
    private static ProductBacklogItemId ConvertToProductBacklogItemId(int id)
    {
        var guidFromInt = new Guid($"22222222-2222-2222-2222-{id:000000000000}");
        return ProductBacklogItemId.From(guidFromInt);
    }

    /// <summary>
    /// Get team's product backlog with optional filtering.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="status">Filter by item status (optional)</param>
    /// <param name="type">Filter by item type (optional)</param>
    /// <param name="limit">Number of results (default 20, max 100)</param>
    /// <param name="offset">Pagination offset (default 0)</param>
    /// <returns>Product backlog with items</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetBacklogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetBacklogResponse>> GetBacklog(
        int teamId,
        [FromQuery] string? status = null,
        [FromQuery] string? type = null,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        try
        {
            if (limit > 100) limit = 100;
            if (limit < 1) limit = 20;
            if (offset < 0) offset = 0;

            var teamIdValue = ConvertToTeamId(teamId);
            var query = new GetProductBacklogQuery(teamIdValue, status, type, limit, offset);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {teamId} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting backlog for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get specific backlog item with detailed information.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="itemId">Backlog item ID</param>
    /// <returns>Backlog item details with history</returns>
    [HttpGet("items/{itemId:int}")]
    [ProducesResponseType(typeof(BacklogItemDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BacklogItemDetailDto>> GetBacklogItem(int teamId, int itemId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var itemIdValue = ConvertToProductBacklogItemId(itemId);
            var query = new GetBacklogItemByIdQuery(teamIdValue, itemIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Backlog item with ID {itemId} not found for team {teamId}" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting backlog item {ItemId} for team {TeamId}", itemId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Create a new backlog item.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="request">Backlog item creation request</param>
    /// <returns>Created backlog item</returns>
    [HttpPost("items")]
    [ProducesResponseType(typeof(BacklogItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BacklogItemDto>> CreateBacklogItem(int teamId, [FromBody] ScrumOps.Api.DTOs.CreateBacklogItemRequest request)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var command = new CreateBacklogItemCommand(
                teamIdValue,
                request.Title,
                request.Description,
                request.AcceptanceCriteria,
                request.Type,
                request.Priority
            );

            var itemId = await _mediator.Send(command);
            
            // Get the created item to return full details
            var query = new GetBacklogItemByIdQuery(teamIdValue, itemId);
            var createdItem = await _mediator.Send(query);

            return CreatedAtAction(
                nameof(GetBacklogItem), 
                new { teamId, itemId = itemId.Value }, 
                createdItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating backlog item for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Update backlog item details.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="itemId">Backlog item ID</param>
    /// <param name="request">Backlog item update request</param>
    /// <returns>Updated backlog item</returns>
    [HttpPut("items/{itemId:int}")]
    [ProducesResponseType(typeof(BacklogItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BacklogItemDto>> UpdateBacklogItem(int teamId, int itemId, [FromBody] ScrumOps.Api.DTOs.UpdateBacklogItemRequest request)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var itemIdValue = ConvertToProductBacklogItemId(itemId);
            var command = new UpdateBacklogItemCommand(
                itemIdValue,
                request.Title,
                request.Description,
                request.AcceptanceCriteria ?? string.Empty,
                request.Priority,
                request.StoryPoints,
                request.BacklogItemType
            );

            await _mediator.Send(command);
            
            // Get the updated item to return full details
            var query = new GetBacklogItemByIdQuery(teamIdValue, itemIdValue);
            var updatedItem = await _mediator.Send(query);

            if (updatedItem == null)
                return NotFound(new { detail = $"Backlog item with ID {itemId} not found for team {teamId}" });

            return Ok(updatedItem);
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
            _logger.LogError(ex, "Error occurred while updating backlog item {ItemId} for team {TeamId}", itemId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Remove backlog item.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="itemId">Backlog item ID</param>
    /// <returns>No content on success</returns>
    [HttpDelete("items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteBacklogItem(int teamId, int itemId)
    {
        try
        {
            var itemIdValue = ConvertToProductBacklogItemId(itemId);
            var command = new DeleteBacklogItemCommand(itemIdValue);

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
            _logger.LogError(ex, "Error occurred while deleting backlog item {ItemId} for team {TeamId}", itemId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Reorder backlog items by priority.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="request">Reorder request with item priorities</param>
    /// <returns>Updated item orders</returns>
    [HttpPut("reorder")]
    [ProducesResponseType(typeof(ReorderBacklogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ReorderBacklogResponse>> ReorderBacklog(int teamId, [FromBody] ScrumOps.Api.DTOs.ReorderBacklogRequest request)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var command = new ReorderBacklogCommand(teamIdValue, request.ItemOrders);

            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { detail = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while reordering backlog for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get items ready for sprint planning.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <returns>Items ready for sprint with recommendations</returns>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(ReadyItemsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReadyItemsResponse>> GetReadyItems(int teamId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var query = new GetReadyItemsQuery(teamIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {teamId} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting ready items for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get backlog health metrics.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <returns>Backlog metrics and health indicators</returns>
    [HttpGet("metrics")]
    [ProducesResponseType(typeof(BacklogMetricsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BacklogMetricsDto>> GetBacklogMetrics(int teamId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var query = new GetBacklogMetricsQuery(teamIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {teamId} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting backlog metrics for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Get backlog flow metrics (cumulative flow).
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <returns>Flow metrics including lead time and throughput</returns>
    [HttpGet("flow")]
    [ProducesResponseType(typeof(BacklogFlowDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BacklogFlowDto>> GetBacklogFlow(int teamId)
    {
        try
        {
            var teamIdValue = ConvertToTeamId(teamId);
            var query = new GetBacklogFlowQuery(teamIdValue);
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound(new { detail = $"Team with ID {teamId} not found" });

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while getting backlog flow for team {TeamId}", teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }

    /// <summary>
    /// Add or update story point estimate.
    /// </summary>
    /// <param name="teamId">Team ID</param>
    /// <param name="itemId">Backlog item ID</param>
    /// <param name="request">Estimation request</param>
    /// <returns>Updated backlog item</returns>
    [HttpPut("items/{itemId:int}/estimate")]
    [ProducesResponseType(typeof(BacklogItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BacklogItemDto>> EstimateBacklogItem(int teamId, int itemId, [FromBody] ScrumOps.Api.DTOs.EstimateItemRequest request)
    {
        try
        {
            var itemIdValue = ConvertToProductBacklogItemId(itemId);
            var command = new EstimateBacklogItemCommand(
                itemIdValue,
                request.StoryPoints,
                request.EstimatedBy,
                request.EstimationMethod,
                request.Confidence,
                request.Notes
            );

            await _mediator.Send(command);
            
            // Get the updated item to return full details
            var teamIdValue = ConvertToTeamId(teamId);
            var query = new GetBacklogItemByIdQuery(teamIdValue, itemIdValue);
            var updatedItem = await _mediator.Send(query);

            if (updatedItem == null)
                return NotFound(new { detail = $"Backlog item with ID {itemId} not found for team {teamId}" });

            return Ok(updatedItem);
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
            _logger.LogError(ex, "Error occurred while estimating backlog item {ItemId} for team {TeamId}", itemId, teamId);
            return StatusCode(500, new { error = "An error occurred while processing your request" });
        }
    }
}

// Note: DTOs are now defined in ScrumOps.Api.DTOs.ApiDtos
