using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Application.Services.ProductBacklog;

/// <summary>
/// Service implementation for product backlog operations.
/// </summary>
public class ProductBacklogService : IProductBacklogService
{
    private readonly IProductBacklogRepository _backlogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProductBacklogService(IProductBacklogRepository backlogRepository, IUnitOfWork unitOfWork)
    {
        _backlogRepository = backlogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<GetBacklogResponse?> GetProductBacklogAsync(
        TeamId teamId, 
        string? status = null, 
        string? type = null, 
        int limit = 20, 
        int offset = 0, 
        CancellationToken cancellationToken = default)
    {
        var backlog = await _backlogRepository.GetByTeamIdAsync(teamId, cancellationToken);
        if (backlog == null) return null;

        // Get items based on status filter
        IEnumerable<ProductBacklogItem> items = backlog.Items;
        
        if (!string.IsNullOrEmpty(status) && Enum.TryParse<BacklogItemStatus>(status, true, out var statusEnum))
        {
            items = items.Where(item => item.Status == statusEnum);
        }

        // Apply pagination
        var totalCount = items.Count();
        var paginatedItems = items.Skip(offset).Take(limit);

        var itemDtos = paginatedItems.Select(item => new BacklogItemDto
        {
            Id = item.Id.Value,
            Title = item.Title.Value,
            Description = item.Description?.Value,
            Status = item.Status.ToString(),
            Priority = item.Priority.Value,
            StoryPoints = item.StoryPoints?.Value,
            Type = item.Type.ToString(),
            CreatedDate = item.CreatedDate
        }).ToList();

        return new GetBacklogResponse
        {
            Backlog = new ProductBacklogDto 
            { 
                Id = backlog.Id.Value, 
                TeamId = backlog.TeamId.Value 
            },
            Items = itemDtos,
            TotalCount = totalCount,
            HasNext = offset + limit < totalCount
        };
    }

    public async Task<ProductBacklogDto?> GetProductBacklogByIdAsync(
        ProductBacklogId backlogId, 
        CancellationToken cancellationToken = default)
    {
        var backlog = await _backlogRepository.GetByIdAsync(backlogId, cancellationToken);
        if (backlog == null) return null;

        return new ProductBacklogDto
        {
            Id = backlog.Id.Value,
            TeamId = backlog.TeamId.Value
        };
    }

    public async Task<BacklogItemDetailDto?> GetBacklogItemByIdAsync(
        TeamId teamId, 
        ProductBacklogItemId itemId, 
        CancellationToken cancellationToken = default)
    {
        var item = await _backlogRepository.GetItemByIdAsync(itemId, cancellationToken);
        if (item == null) return null;

        return new BacklogItemDetailDto 
        { 
            Id = item.Id.Value,
            Title = item.Title.Value,
            Description = item.Description?.Value,
            AcceptanceCriteria = item.AcceptanceCriteria?.Value,
            Status = item.Status.ToString(),
            Priority = item.Priority.Value,
            StoryPoints = item.StoryPoints?.Value,
            Type = item.Type.ToString(),
            CreatedDate = item.CreatedDate,
            History = new List<BacklogItemHistoryDto>() // TODO: Implement history tracking
        };
    }

    public async Task<ReadyItemsResponse?> GetReadyItemsAsync(
        TeamId teamId, 
        CancellationToken cancellationToken = default)
    {
        var readyItems = await _backlogRepository.GetReadyItemsForSprintPlanningAsync(teamId, cancellationToken: cancellationToken);
        
        var readyItemDtos = readyItems.Select(item => new ReadyItemDto
        {
            Id = item.Id.Value,
            Title = item.Title.Value,
            StoryPoints = item.StoryPoints?.Value ?? 0,
            Priority = item.Priority.Value,
            HasAcceptanceCriteria = item.AcceptanceCriteria != null,
            IsEstimated = item.StoryPoints != null
        }).ToList();

        var totalReadyPoints = readyItemDtos.Sum(item => item.StoryPoints);

        return new ReadyItemsResponse
        {
            ReadyItems = readyItemDtos,
            TotalReadyPoints = totalReadyPoints,
            RecommendedForNextSprint = readyItemDtos.Take(10).Select(item => item.Id).ToList()
        };
    }

    public async Task<BacklogMetricsDto?> GetBacklogMetricsAsync(
        TeamId teamId, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new BacklogMetricsDto
        {
            TotalItems = 0,
            ReadyItems = 0,
            EstimatedItems = 0,
            AverageStoryPoints = 0,
            VelocityTrend = 0,
            RefinementHealth = new RefinementHealthDto { Score = 0 },
            PriorityDistribution = new PriorityDistributionDto()
        };
    }

    public async Task<BacklogFlowDto?> GetBacklogFlowAsync(
        TeamId teamId, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new BacklogFlowDto
        {
            FlowData = new List<FlowDataPoint>(),
            LeadTime = new LeadTimeDto(),
            Throughput = new ThroughputDto()
        };
    }

    public async Task<ProductBacklogId> CreateProductBacklogAsync(
        TeamId teamId,
        string? notes,
        CancellationToken cancellationToken = default)
    {
        // Check if team already has a backlog
        //var existsForTeam = await _backlogRepository.ExistsForTeamAsync(teamId, cancellationToken);
        //if (existsForTeam)
        //{
        //    throw new InvalidOperationException($"Team {teamId.Value} already has a product backlog.");
        //}

        var backlog = new Domain.ProductBacklog.Entities
            .ProductBacklog(ProductBacklogId.New(), teamId, BacklogNotes.Create(notes));


        await _backlogRepository.AddAsync(backlog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return backlog.Id;
    }

    public async Task<ProductBacklogItemId> AddBacklogItemAsync(
        ProductBacklogId backlogId, 
        string title, 
        string description, 
        string acceptanceCriteria, 
        int priority, 
        int? storyPoints, 
        string backlogItemType, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return ProductBacklogItemId.New();
    }

    public async Task<ProductBacklogItemId> CreateBacklogItemAsync(
        TeamId teamId, 
        string title, 
        string description, 
        string acceptanceCriteria, 
        string type, 
        int priority, 
        CancellationToken cancellationToken = default)
    {
        var backlog = await _backlogRepository.GetByTeamIdAsync(teamId, cancellationToken);
        if (backlog == null)
        {
            throw new InvalidOperationException($"No product backlog found for team {teamId.Value}.");
        }

        if (!Enum.TryParse<BacklogItemType>(type, true, out var itemType))
        {
            throw new ArgumentException($"Invalid backlog item type: {type}");
        }

        // Create the backlog item
        var itemId = ProductBacklogItemId.New();
        var itemTitle = ItemTitle.Create(title);
        var itemDescription = ItemDescription.Create(description);
        var createdBy = UserName.Create("System"); // TODO: Get from current user context

        var item = new ProductBacklogItem(itemId, backlog.Id, itemTitle, itemDescription, itemType, createdBy);
        
        // Set acceptance criteria if provided
        if (!string.IsNullOrEmpty(acceptanceCriteria))
        {
            item.SetAcceptanceCriteria(AcceptanceCriteria.Create(acceptanceCriteria));
        }
        
        backlog.AddItem(item);
        
        // Note: Priority is managed by the ProductBacklog aggregate

        await _backlogRepository.UpdateAsync(backlog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return item.Id;
    }

    public async Task UpdateBacklogItemAsync(
        ProductBacklogItemId itemId, 
        string title, 
        string description, 
        string acceptanceCriteria, 
        int priority, 
        int? storyPoints, 
        string backlogItemType, 
        CancellationToken cancellationToken = default)
    {
        var productBacklogItem = await _backlogRepository.GetItemByIdAsync(itemId, cancellationToken);
        if (productBacklogItem == null)
        {
            throw new InvalidOperationException($"Backlog item {itemId.Value} not found.");
        }

        var productBacklog = await _backlogRepository.GetByIdAsync(productBacklogItem.ProductBacklogId, cancellationToken);
        if (productBacklog == null)
        {
            throw new InvalidOperationException($"Product backlog {productBacklogItem.ProductBacklogId.Value} not found.");
        }

        // Update fields
        productBacklogItem
            .UpdateTitle(ItemTitle.Create(title))
            .UpdateDescription(ItemDescription.Create(description))
            .SetAcceptanceCriteria(AcceptanceCriteria.Create(acceptanceCriteria));

        productBacklog.ApplyItemPriority(productBacklogItem, priority);

        
        if (storyPoints.HasValue)
        {
            productBacklogItem.EstimateStoryPoints(StoryPoints.Create(storyPoints.Value));
        }

        if (Enum.TryParse<BacklogItemType>(backlogItemType, true, out var itemType))
        {
            productBacklog.ApplyItemType(productBacklogItem, itemType);
        }
        else
        {
            throw new ArgumentException($"Invalid backlog item type: {backlogItemType}");
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await Task.CompletedTask;
    }

    public async Task DeleteBacklogItemAsync(
        ProductBacklogItemId itemId, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        await Task.CompletedTask;
    }

    public async Task<ReorderBacklogResponse> ReorderBacklogAsync(
        TeamId teamId, 
        List<ItemOrder> itemOrders, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        return new ReorderBacklogResponse
        {
            UpdatedItems = itemOrders
        };
    }

    public async Task EstimateBacklogItemAsync(
        ProductBacklogItemId itemId, 
        int storyPoints, 
        string estimatedBy, 
        string estimationMethod, 
        int confidence, 
        string? notes, 
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual logic
        await Task.CompletedTask;
    }
}
