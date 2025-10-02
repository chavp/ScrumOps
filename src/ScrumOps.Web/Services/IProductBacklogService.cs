using ScrumOps.Shared.Contracts.ProductBacklog;

namespace ScrumOps.Web.Services;

/// <summary>
/// Service interface for product backlog management operations.
/// </summary>
public interface IProductBacklogService
{
    Task<GetProductBacklogsResponse> GetProductBacklogsAsync();
    Task<ProductBacklogResponse?> GetProductBacklogAsync(Guid id);
    Task<ProductBacklogResponse> CreateProductBacklogAsync(CreateProductBacklogRequest request);
    Task<BacklogItemResponse> AddBacklogItemAsync(Guid teamId, AddBacklogItemRequest request);
    Task<BacklogItemResponse> UpdateBacklogItemAsync(Guid teamId, Guid itemId, UpdateBacklogItemRequest request);
    Task<bool> DeleteBacklogItemAsync(Guid teamId, Guid itemId);
    Task<BacklogItemResponse> GetBacklogItemAsync(Guid teamId, Guid itemId);
    Task<GetBacklogItemsResponse> GetBacklogItemsAsync();
}
