using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrumOps.Application.TeamManagement.Commands;
using ScrumOps.Shared.Contracts.Teams;

namespace ScrumOps.Api.Controllers;

/// <summary>
/// API controller for team management operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Tags("Teams")]
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
    /// Creates a new team.
    /// </summary>
    /// <param name="request">The team creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created team's ID</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TeamDetailsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTeam(
        [FromBody] CreateTeamRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating team with name: {TeamName}", request.Name);

            var command = new CreateTeamCommand(
                request.Name,
                request.Description,
                request.SprintLengthWeeks,
                "po@example.com", // Default for demo
                "sm@example.com"  // Default for demo
            );

            var teamId = await _mediator.Send(command, cancellationToken);

            var response = new TeamDetailsResponse
            {
                Id = int.Parse(teamId.Value.ToString().Replace("-", "")[..8], System.Globalization.NumberStyles.HexNumber),
                Name = request.Name,
                Description = request.Description ?? "",
                SprintLengthWeeks = request.SprintLengthWeeks,
                Velocity = 0,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                Members = new[]
                {
                    new TeamMember 
                    { 
                        Id = 1,
                        Name = "Product Owner", 
                        Email = "po@example.com", // Default for demo
                        Role = "ProductOwner",
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    },
                    new TeamMember 
                    { 
                        Id = 2,
                        Name = "Scrum Master", 
                        Email = "sm@example.com", // Default for demo
                        Role = "ScrumMaster",
                        CreatedDate = DateTime.UtcNow,
                        IsActive = true
                    }
                }
            };

            return CreatedAtAction(nameof(GetTeam), new { id = response.Id }, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating team with name: {TeamName}", request.Name);
            return BadRequest($"Error creating team: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets a team by ID.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The team details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TeamDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTeam(
        int id,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting team with ID: {TeamId}", id);
            
            // TODO: Implement GetTeamQuery and handler
            // For now, return a placeholder response
            var response = new TeamDetailsResponse
            {
                Id = id,
                Name = "Sample Team",
                Description = "Sample team description",
                SprintLengthWeeks = 2,
                Velocity = 15.5m,
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                Members = new[]
                {
                    new TeamMember 
                    { 
                        Id = 1,
                        Name = "Product Owner", 
                        Email = "po@example.com", 
                        Role = "ProductOwner",
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        IsActive = true
                    },
                    new TeamMember 
                    { 
                        Id = 2,
                        Name = "Scrum Master", 
                        Email = "sm@example.com", 
                        Role = "ScrumMaster",
                        CreatedDate = DateTime.UtcNow.AddDays(-1),
                        IsActive = true
                    }
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting team with ID: {TeamId}", id);
            return NotFound($"Team with ID {id} not found");
        }
    }

    /// <summary>
    /// Updates an existing team.
    /// </summary>
    /// <param name="id">The team ID</param>
    /// <param name="request">The team update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated team details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TeamDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTeam(
        int id,
        [FromBody] UpdateTeamRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating team with ID: {TeamId}", id);

            // For now, simulate that team with ID 1 exists, others don't
            if (id != 1)
            {
                return NotFound($"Team with ID {id} not found");
            }

            // TODO: Implement UpdateTeamCommand and handler
            // For now, return a placeholder response
            var response = new TeamDetailsResponse
            {
                Id = id,
                Name = request.Name,
                Description = request.Description ?? "",
                SprintLengthWeeks = request.SprintLengthWeeks,
                Velocity = 20.0m,
                Members = new List<TeamMember>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Product Owner",
                        Email = "po@example.com",
                        Role = "ProductOwner"
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Scrum Master",
                        Email = "sm@example.com",
                        Role = "ScrumMaster"
                    }
                },
                IsActive = true,
                CreatedDate = DateTime.UtcNow.AddDays(-2)
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating team with ID: {TeamId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    /// <summary>
    /// Gets all teams.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of teams</returns>
    [HttpGet]
    [ProducesResponseType(typeof(GetTeamsResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTeams(CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Getting all teams");
            
            // TODO: Implement GetTeamsQuery and handler
            // For now, return a placeholder response
            var response = new GetTeamsResponse
            {
                Teams = new List<TeamSummary>
                {
                    new()
                    {
                        Id = 1,
                        Name = "Sample Team 1",
                        Description = "First sample team",
                        SprintLengthWeeks = 2,
                        Velocity = 18.0m,
                        MemberCount = 5,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow.AddDays(-2)
                    },
                    new()
                    {
                        Id = 2,
                        Name = "Sample Team 2", 
                        Description = "Second sample team",
                        SprintLengthWeeks = 3,
                        Velocity = 22.5m,
                        MemberCount = 7,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow.AddDays(-1)
                    }
                },
                TotalCount = 2
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting teams");
            return StatusCode(500, "Internal server error");
        }
    }
}