using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.ProductBacklog.ValueObjects;

namespace ScrumOps.Application.Services.ProductBacklog;

/// <summary>
/// Service interface for product backlog operations.
/// </summary>
public interface IProductBacklogService
{
    // Query operations
    Task<GetBacklogResponse?> GetProductBacklogAsync(
        TeamId teamId,
        string? status = null,
        string? type = null,
        int limit = 20,
        int offset = 0,
        CancellationToken cancellationToken = default);

    Task<ProductBacklogDto?> GetProductBacklogByIdAsync(
        ProductBacklogId backlogId,
        CancellationToken cancellationToken = default);

    Task<BacklogItemDetailDto?> GetBacklogItemByIdAsync(
        TeamId teamId,
        ProductBacklogItemId itemId,
        CancellationToken cancellationToken = default);

    Task<ReadyItemsResponse?> GetReadyItemsAsync(
        TeamId teamId,
        CancellationToken cancellationToken = default);

    Task<BacklogMetricsDto?> GetBacklogMetricsAsync(
        TeamId teamId,
        CancellationToken cancellationToken = default);

    Task<BacklogFlowDto?> GetBacklogFlowAsync(
        TeamId teamId,
        CancellationToken cancellationToken = default);

    // Command operations
    Task<ProductBacklogId> CreateProductBacklogAsync(
        TeamId teamId,
        CancellationToken cancellationToken = default);

    Task<ProductBacklogItemId> AddBacklogItemAsync(
        ProductBacklogId backlogId,
        string title,
        string description,
        string acceptanceCriteria,
        int priority,
        int? storyPoints,
        string backlogItemType,
        CancellationToken cancellationToken = default);

    Task<ProductBacklogItemId> CreateBacklogItemAsync(
        TeamId teamId,
        string title,
        string description,
        string acceptanceCriteria,
        string type,
        int priority,
        CancellationToken cancellationToken = default);

    Task UpdateBacklogItemAsync(
        ProductBacklogItemId itemId,
        string title,
        string description,
        string acceptanceCriteria,
        int priority,
        int? storyPoints,
        string backlogItemType,
        CancellationToken cancellationToken = default);

    Task DeleteBacklogItemAsync(
        ProductBacklogItemId itemId,
        CancellationToken cancellationToken = default);

    Task<ReorderBacklogResponse> ReorderBacklogAsync(
        TeamId teamId,
        List<ItemOrder> itemOrders,
        CancellationToken cancellationToken = default);

    Task EstimateBacklogItemAsync(
        ProductBacklogItemId itemId,
        int storyPoints,
        string estimatedBy,
        string estimationMethod,
        int confidence,
        string? notes,
        CancellationToken cancellationToken = default);
}