using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.ProductBacklog.Commands;
using ScrumOps.Application.ProductBacklog.Queries;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Shared.Contracts.ProductBacklog;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// API controller for product backlog management operations.
/// </summary>
[ApiController]
[Route("api/backlogs")]
[Tags("Product Backlogs")]
public class ProductBacklogController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductBacklogController> _logger;

    public ProductBacklogController(IMediator mediator, ILogger<ProductBacklogController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new product backlog for a team.
    /// </summary>
    /// <param name="request">The backlog creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created backlog details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProductBacklogResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProductBacklog(
        [FromBody] CreateProductBacklogRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating product backlog for team: {TeamId}", request.TeamId);

            var teamId = new TeamId(new Guid(request.TeamId.ToString("X8").PadLeft(32, '0').Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-")));
            var command = new CreateProductBacklogCommand(teamId, request.Notes);

            var backlogId = await _mediator.Send(command, cancellationToken);

            // Get the created backlog to return full details
            var query = new GetProductBacklogByIdQuery(backlogId);
            var backlogDto = await _mediator.Send(query, cancellationToken);

            if (backlogDto == null)
            {
                return BadRequest("Failed to create product backlog");
            }

            var response = new ProductBacklogResponse
            {
                Id = backlogDto.Id,
                TeamId = request.TeamId,
                TeamName = $"Team {request.TeamId}", // TODO: Get actual team name
                CreatedDate = backlogDto.CreatedDate,
                LastRefinedDate = backlogDto.LastRefinedDate,
                Notes = backlogDto.Notes,
                TotalItems = backlogDto.Items.Count,
                Items = backlogDto.Items.Select(item => new BacklogItemSummary
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Priority = item.Priority,
                    StoryPoints = item.StoryPoints,
                    Status = item.Status,
                    Type = item.Type,
                    CreatedDate = item.CreatedDate
                }).ToList()
            };

            return CreatedAtAction(nameof(GetProductBacklog), new { id = backlogDto.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product backlog for team: {TeamId}", request.TeamId);
            return BadRequest($"Error creating product backlog: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a product backlog by ID.
    /// </summary>
    /// <param name="id">The backlog ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The backlog details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductBacklogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductBacklog(
        string id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting product backlog with ID: {BacklogId}", id);

            var backlogId = ProductBacklogId.From(id);
            var query = new GetProductBacklogByIdQuery(backlogId);
            var backlogDto = await _mediator.Send(query, cancellationToken);

            if (backlogDto == null)
            {
                return NotFound($"Product backlog with ID {id} not found");
            }

            var response = new ProductBacklogResponse
            {
                Id = backlogDto.Id,
                TeamId = int.Parse(backlogDto.TeamId.Replace("-", "")[..8], System.Globalization.NumberStyles.HexNumber),
                TeamName = $"Team {backlogDto.TeamId[..8]}", // TODO: Get actual team name
                CreatedDate = backlogDto.CreatedDate,
                LastRefinedDate = backlogDto.LastRefinedDate,
                Notes = backlogDto.Notes,
                TotalItems = backlogDto.Items.Count,
                Items = backlogDto.Items.Select(item => new BacklogItemSummary
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Priority = item.Priority,
                    StoryPoints = item.StoryPoints,
                    Status = item.Status,
                    Type = item.Type,
                    CreatedDate = item.CreatedDate
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product backlog with ID: {BacklogId}", id);
            return NotFound($"Product backlog with ID {id} not found");
        }
    }

    /// <summary>
    /// Gets a product backlog by team ID.
    /// </summary>
    /// <param name="teamId">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The backlog details</returns>
    [HttpGet("team/{teamId}")]
    [ProducesResponseType(typeof(ProductBacklogResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProductBacklogByTeam(
        int teamId,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting product backlog for team: {TeamId}", teamId);

            var teamIdObj = new TeamId(new Guid(teamId.ToString("X8").PadLeft(32, '0').Insert(8, "-").Insert(13, "-").Insert(18, "-").Insert(23, "-")));
            var query = new GetProductBacklogQuery(teamIdObj);
            var backlogDto = await _mediator.Send(query, cancellationToken);

            if (backlogDto == null)
            {
                return NotFound($"No product backlog found for team {teamId}");
            }

            var response = new ProductBacklogResponse
            {
                Id = backlogDto.Id,
                TeamId = teamId,
                TeamName = $"Team {teamId}", // TODO: Get actual team name
                CreatedDate = backlogDto.CreatedDate,
                LastRefinedDate = backlogDto.LastRefinedDate,
                Notes = backlogDto.Notes,
                TotalItems = backlogDto.Items.Count,
                Items = backlogDto.Items.Select(item => new BacklogItemSummary
                {
                    Id = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    Priority = item.Priority,
                    StoryPoints = item.StoryPoints,
                    Status = item.Status,
                    Type = item.Type,
                    CreatedDate = item.CreatedDate
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting product backlog for team: {TeamId}", teamId);
            return NotFound($"No product backlog found for team {teamId}");
        }
    }

    /// <summary>
    /// Adds a new item to a product backlog.
    /// </summary>
    /// <param name="id">The backlog ID</param>
    /// <param name="request">The item creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created item details</returns>
    [HttpPost("{id}/items")]
    [ProducesResponseType(typeof(BacklogItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddBacklogItem(
        string id,
        [FromBody] AddBacklogItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Adding item to backlog {BacklogId}: {ItemTitle}", id, request.Title);

            var backlogId = ProductBacklogId.From(Guid.Parse(id));
            var command = new AddBacklogItemCommand(
                backlogId,
                request.Title,
                request.Description,
                request.AcceptanceCriteria,
                request.Priority,
                request.StoryPoints,
                request.Type
            );

            var itemId = await _mediator.Send(command, cancellationToken);

            // Get the updated backlog to find the created item
            var query = new GetProductBacklogByIdQuery(backlogId);
            var backlogDto = await _mediator.Send(query, cancellationToken);

            var createdItem = backlogDto?.Items.FirstOrDefault(i => i.Id == itemId.Value.ToString());
            if (createdItem == null)
            {
                return BadRequest("Failed to create backlog item");
            }

            var response = new BacklogItemResponse
            {
                Id = createdItem.Id,
                Title = createdItem.Title,
                Description = createdItem.Description,
                AcceptanceCriteria = createdItem.AcceptanceCriteria,
                Priority = createdItem.Priority,
                StoryPoints = createdItem.StoryPoints,
                Status = createdItem.Status,
                Type = createdItem.Type,
                CreatedBy = createdItem.CreatedBy,
                CreatedDate = createdItem.CreatedDate
            };

            return CreatedAtAction(nameof(GetProductBacklog), new { id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding item to backlog {BacklogId}: {ItemTitle}", id, request.Title);
            return BadRequest($"Error adding backlog item: {ex.Message}");
        }
    }
}