using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Domain.ProductBacklog.Repositories;

/// <summary>
/// Repository interface for ProductBacklog aggregate root.
/// Defines the contract for persisting and retrieving product backlogs.
/// </summary>
public interface IProductBacklogRepository
{
    /// <summary>
    /// Gets a product backlog by its unique identifier.
    /// </summary>
    /// <param name="backlogId">The product backlog identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product backlog if found, null otherwise</returns>
    Task<Entities.ProductBacklog?> GetByIdAsync(ProductBacklogId backlogId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the product backlog for a specific team.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product backlog for the team if found, null otherwise</returns>
    Task<Entities.ProductBacklog?> GetByTeamIdAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all product backlogs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of all product backlogs</returns>
    Task<IEnumerable<Entities.ProductBacklog>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product backlog items by status across all backlogs.
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of product backlog items with the specified status</returns>
    Task<IEnumerable<ProductBacklogItem>> GetItemsByStatusAsync(BacklogItemStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets product backlog items for a specific team by status.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="status">The status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of product backlog items with the specified status for the team</returns>
    Task<IEnumerable<ProductBacklogItem>> GetTeamItemsByStatusAsync(TeamId teamId, BacklogItemStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets ready items for sprint planning for a specific team.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="maxItems">Maximum number of items to retrieve</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of ready items ordered by priority</returns>
    Task<IEnumerable<ProductBacklogItem>> GetReadyItemsForSprintPlanningAsync(TeamId teamId, int maxItems = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for product backlog items by title or description.
    /// </summary>
    /// <param name="searchTerm">The search term</param>
    /// <param name="teamId">Optional team ID to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Collection of matching product backlog items</returns>
    Task<IEnumerable<ProductBacklogItem>> SearchItemsAsync(string searchTerm, TeamId? teamId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a product backlog item by its unique identifier.
    /// </summary>
    /// <param name="itemId">The product backlog item identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product backlog item if found, null otherwise</returns>
    Task<ProductBacklogItem?> GetItemByIdAsync(ProductBacklogItemId itemId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a team already has a product backlog.
    /// </summary>
    /// <param name="teamId">The team identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the team has a product backlog, false otherwise</returns>
    Task<bool> ExistsForTeamAsync(TeamId teamId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new product backlog to the repository.
    /// </summary>
    /// <param name="productBacklog">The product backlog to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task AddAsync(Entities.ProductBacklog productBacklog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing product backlog in the repository.
    /// </summary>
    /// <param name="productBacklog">The product backlog to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task UpdateAsync(Entities.ProductBacklog productBacklog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a product backlog from the repository.
    /// </summary>
    /// <param name="productBacklog">The product backlog to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task RemoveAsync(Entities.ProductBacklog productBacklog, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of product backlogs.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The total count of product backlogs</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of items across all backlogs by status.
    /// </summary>
    /// <param name="status">The status to filter by</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The count of items with the specified status</returns>
    Task<int> GetItemCountByStatusAsync(BacklogItemStatus status, CancellationToken cancellationToken = default);
}