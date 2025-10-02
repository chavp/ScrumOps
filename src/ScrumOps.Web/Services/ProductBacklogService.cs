using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.ProductBacklog;
using ScrumOps.Shared.Contracts.Teams;
using ScrumOps.Application.Services.ProductBacklog;

namespace ScrumOps.Web.Services;

/// <summary>
/// HTTP client service for product backlog management operations.
/// </summary>
public class ProductBacklogService : IProductBacklogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductBacklogService> _logger;
    private readonly ITeamService _teamService;

    public ProductBacklogService(HttpClient httpClient, ILogger<ProductBacklogService> logger,
        ITeamService teamService)
    {
        _httpClient = httpClient;
        _logger = logger;
        _teamService = teamService;
    }

    public async Task<GetProductBacklogsResponse> GetProductBacklogsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all product backlogs from API");
            
            // Get all teams first with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var teamsResponse = await _teamService.GetTeamsAsync();
            var backlogs = new List<ProductBacklogSummary>();

            foreach (var team in teamsResponse.Teams)
            {
                try
                {
                    using var teamCts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                    var backlog = await GetProductBacklogAsync(team.Id);
                    if (backlog != null)
                    {
                        backlogs.Add(new ProductBacklogSummary
                        {
                            Id = backlog.Id,
                            TeamId = team.Id, // Use actual team Guid
                            TeamName = backlog.TeamName,
                            CreatedDate = backlog.CreatedDate,
                            LastRefinedDate = backlog.LastRefinedDate,
                            TotalItems = backlog.TotalItems,
                            CompletedItems = backlog.Items.Count(i => i.Status == "Done"),
                            IsActive = true
                        });
                    }
                }
                catch (HttpRequestException ex) when (ex.Message.Contains("404"))
                {
                    _logger.LogInformation("No backlog found for team {TeamId} - this is normal for teams without backlogs", team.Id);
                    // Skip teams without backlogs - this is expected behavior
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Timeout loading backlog for team {TeamId}", team.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not load backlog for team {TeamId}", team.Id);
                }
            }
            
            return new GetProductBacklogsResponse { Backlogs = backlogs, TotalCount = backlogs.Count };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetProductBacklogsAsync operation timed out");
            return new GetProductBacklogsResponse { Backlogs = [], TotalCount = 0 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product backlogs");
            return new GetProductBacklogsResponse { Backlogs = [], TotalCount = 0 };
        }
    }

    public async Task<ProductBacklogResponse?> GetProductBacklogAsync(Guid teamId)
    {
        try
        {
            _logger.LogInformation("Fetching product backlog for team {TeamId} from API", teamId);

            // Get the API response directly
            var apiResponse = await _httpClient.GetFromJsonAsync<GetBacklogResponse>($"/api/teams/{teamId}/backlog");
            if (apiResponse == null) return null;

            // Get team information for the response
            var team = await _teamService.GetTeamAsync(teamId);

            // Map API response to Web contract
            var webResponse = new ProductBacklogResponse
            {
                Id = apiResponse.Backlog?.Id,
                TeamId = teamId,
                TeamName = team?.Name ?? "Unknown Team",
                CreatedDate = DateTime.UtcNow,
                LastRefinedDate = apiResponse.Backlog?.LastRefinedDate,
                Notes = "",
                TotalItems = apiResponse.TotalCount,
                Items = apiResponse.Items.Select(MapToBacklogItemSummary).ToList()
            };

            return webResponse;
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            _logger.LogInformation("No backlog found for team {TeamId} - team may not have a backlog yet", teamId);
            return null; // Return null instead of throwing for 404
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product backlog for team {TeamId}", teamId);
            throw;
        }
    }

    public async Task<ProductBacklogResponse> CreateProductBacklogAsync(CreateProductBacklogRequest request)
    {
        try
        {
            _logger.LogInformation("Creating product backlog for team {TeamId}", request.TeamId);
            
            if (!request.TeamId.HasValue)
            {
                throw new InvalidOperationException("TeamId is required to create a backlog");
            }

            // Get team information to validate it exists
            var team = await _teamService.GetTeamAsync(request.TeamId.Value);
            if (team == null)
            {
                throw new InvalidOperationException($"Team with ID {request.TeamId} not found");
            }

            // Call the API to create the backlog (the API POST endpoint doesn't need a body)
            var response = await _httpClient.PostAsync($"/api/teams/{request.TeamId.Value}/backlog", null);
            response.EnsureSuccessStatusCode();
            
            var apiResult = await response.Content.ReadFromJsonAsync<GetBacklogResponse>();
            if (apiResult == null)
            {
                throw new InvalidOperationException("Failed to create product backlog");
            }

            // Map API response to Web contract
            var webResponse = new ProductBacklogResponse
            {
                Id = apiResult.Backlog?.Id,
                TeamId = request.TeamId,
                TeamName = team.Name,
                CreatedDate = DateTime.UtcNow,
                LastRefinedDate = apiResult.Backlog?.LastRefinedDate,
                Notes = request.Notes ?? "",
                TotalItems = apiResult.TotalCount,
                Items = apiResult.Items.Select(MapToBacklogItemSummary).ToList()
            };

            return webResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product backlog for team {TeamId}", request.TeamId);
            throw;
        }
    }

    public async Task<BacklogItemResponse> AddBacklogItemAsync(Guid teamId, AddBacklogItemRequest request)
    {
        try
        {
            _logger.LogInformation("Adding backlog item {Title} to team {TeamId} backlog", request.Title, teamId);
            
            // Map web request to API request
            var apiRequest = new
            {
                Title = request.Title,
                Description = request.Description,
                AcceptanceCriteria = request.AcceptanceCriteria,
                Priority = request.Priority,
                StoryPoints = request.StoryPoints,
                Type = request.Type
            };

            var response = await _httpClient.PostAsJsonAsync($"/api/teams/{teamId}/backlog/items", apiRequest);
            response.EnsureSuccessStatusCode();
            
            var apiResult = await response.Content.ReadFromJsonAsync<BacklogItemDto>();
            if (apiResult == null)
                throw new InvalidOperationException("Failed to add backlog item");

            // Map API response to Web contract
            return MapToBacklogItemResponse(apiResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding backlog item {Title} to team {TeamId} backlog", request.Title, teamId);
            throw;
        }
    }

    public async Task<BacklogItemResponse> UpdateBacklogItemAsync(Guid teamId, Guid itemId, UpdateBacklogItemRequest request)
    {
        try
        {
            _logger.LogInformation("Updating backlog item {ItemId} in team {TeamId} backlog", itemId, teamId);
            
            var apiRequest = new
            {
                Title = request.Title,
                Description = request.Description,
                AcceptanceCriteria = request.AcceptanceCriteria,
                Priority = request.Priority,
                StoryPoints = request.StoryPoints,
                BacklogItemType = request.Type
            };

            var response = await _httpClient.PutAsJsonAsync($"/api/teams/{teamId}/backlog/items/{itemId}", apiRequest);
            response.EnsureSuccessStatusCode();
            
            var apiResult = await response.Content.ReadFromJsonAsync<BacklogItemDto>();
            if (apiResult == null)
                throw new InvalidOperationException($"Failed to update backlog item {itemId}");

            return MapToBacklogItemResponse(apiResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating backlog item {ItemId} in team {TeamId} backlog", itemId, teamId);
            throw;
        }
    }

    public async Task<bool> DeleteBacklogItemAsync(Guid teamId, Guid itemId)
    {
        try
        {
            _logger.LogInformation("Deleting backlog item {ItemId} from team {TeamId} backlog", itemId, teamId);
            var response = await _httpClient.DeleteAsync($"/api/teams/{teamId}/backlog/items/{itemId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backlog item {ItemId} from team {TeamId} backlog", itemId, teamId);
            return false;
        }
    }

    public async Task<BacklogItemResponse> GetBacklogItemAsync(Guid teamId, Guid itemId)
    {
        try
        {
            _logger.LogInformation("Fetching backlog item {ItemId} from team {TeamId} backlog", itemId, teamId);
            var apiResponse = await _httpClient.GetFromJsonAsync<BacklogItemDto>($"/api/teams/{teamId}/backlog/items/{itemId}");
            
            if (apiResponse == null)
                throw new InvalidOperationException($"Backlog item {itemId} not found");

            return MapToBacklogItemResponse(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching backlog item {ItemId} from team {TeamId} backlog", itemId, teamId);
            throw;
        }
    }

    private static BacklogItemSummary MapToBacklogItemSummary(BacklogItemDto dto)
    {
        return new BacklogItemSummary
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            StoryPoints = dto.StoryPoints,
            Status = dto.Status,
            Type = dto.Type,
            CreatedDate = dto.CreatedDate
        };
    }

    private static BacklogItemResponse MapToBacklogItemResponse(BacklogItemDto dto)
    {
        return new BacklogItemResponse
        {
            Id = dto.Id.ToString(),
            Title = dto.Title,
            Description = dto.Description,
            AcceptanceCriteria = dto.AcceptanceCriteria,
            Priority = dto.Priority,
            StoryPoints = dto.StoryPoints,
            Status = dto.Status,
            Type = dto.Type,
            CreatedBy = dto.CreatedBy,
            CreatedDate = dto.CreatedDate
        };
    }
}
