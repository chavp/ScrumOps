using System.Net.Http.Json;
using ScrumOps.Shared.Contracts.ProductBacklog;

namespace ScrumOps.Web.Services;

/// <summary>
/// HTTP client service for product backlog management operations.
/// </summary>
public class ProductBacklogService : IProductBacklogService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductBacklogService> _logger;

    public ProductBacklogService(HttpClient httpClient, ILogger<ProductBacklogService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<GetProductBacklogsResponse> GetProductBacklogsAsync()
    {
        try
        {
            _logger.LogInformation("Fetching product backlogs from API");
            var response = await _httpClient.GetFromJsonAsync<GetProductBacklogsResponse>("/api/backlog");
            return response ?? new GetProductBacklogsResponse { Backlogs = [], TotalCount = 0 };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product backlogs");
            return new GetProductBacklogsResponse { Backlogs = [], TotalCount = 0 };
        }
    }

    public async Task<ProductBacklogResponse> GetProductBacklogAsync(string id)
    {
        try
        {
            _logger.LogInformation("Fetching product backlog {BacklogId} from API", id);
            var response = await _httpClient.GetFromJsonAsync<ProductBacklogResponse>($"/api/backlog/{id}");
            return response ?? throw new InvalidOperationException($"Product backlog {id} not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching product backlog {BacklogId}", id);
            throw;
        }
    }

    public async Task<ProductBacklogResponse> CreateProductBacklogAsync(CreateProductBacklogRequest request)
    {
        try
        {
            _logger.LogInformation("Creating product backlog for team {TeamId}", request.TeamId);
            var response = await _httpClient.PostAsJsonAsync("/api/backlog", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<ProductBacklogResponse>();
            return result ?? throw new InvalidOperationException("Failed to create product backlog");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product backlog for team {TeamId}", request.TeamId);
            throw;
        }
    }

    public async Task<BacklogItemResponse> AddBacklogItemAsync(string backlogId, AddBacklogItemRequest request)
    {
        try
        {
            _logger.LogInformation("Adding backlog item {Title} to backlog {BacklogId}", request.Title, backlogId);
            var response = await _httpClient.PostAsJsonAsync($"/api/backlog/{backlogId}/items", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<BacklogItemResponse>();
            return result ?? throw new InvalidOperationException("Failed to add backlog item");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding backlog item {Title} to backlog {BacklogId}", request.Title, backlogId);
            throw;
        }
    }

    public async Task<BacklogItemResponse> UpdateBacklogItemAsync(string backlogId, string itemId, UpdateBacklogItemRequest request)
    {
        try
        {
            _logger.LogInformation("Updating backlog item {ItemId} in backlog {BacklogId}", itemId, backlogId);
            var response = await _httpClient.PutAsJsonAsync($"/api/backlog/{backlogId}/items/{itemId}", request);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadFromJsonAsync<BacklogItemResponse>();
            return result ?? throw new InvalidOperationException($"Failed to update backlog item {itemId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating backlog item {ItemId} in backlog {BacklogId}", itemId, backlogId);
            throw;
        }
    }

    public async Task<bool> DeleteBacklogItemAsync(string backlogId, string itemId)
    {
        try
        {
            _logger.LogInformation("Deleting backlog item {ItemId} from backlog {BacklogId}", itemId, backlogId);
            var response = await _httpClient.DeleteAsync($"/api/backlog/{backlogId}/items/{itemId}");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting backlog item {ItemId} from backlog {BacklogId}", itemId, backlogId);
            return false;
        }
    }
}