using ScrumOps.Shared.Contracts.ProductBacklog;

namespace ScrumOps.Web.Services;

/// <summary>
/// Service interface for product backlog management operations.
/// </summary>
public interface IProductBacklogService
{
    Task<GetProductBacklogsResponse> GetProductBacklogsAsync();
    Task<ProductBacklogResponse> GetProductBacklogAsync(string id);
    Task<ProductBacklogResponse> CreateProductBacklogAsync(CreateProductBacklogRequest request);
    Task<BacklogItemResponse> AddBacklogItemAsync(string backlogId, AddBacklogItemRequest request);
    Task<BacklogItemResponse> UpdateBacklogItemAsync(string backlogId, string itemId, UpdateBacklogItemRequest request);
    Task<bool> DeleteBacklogItemAsync(string backlogId, string itemId);
}