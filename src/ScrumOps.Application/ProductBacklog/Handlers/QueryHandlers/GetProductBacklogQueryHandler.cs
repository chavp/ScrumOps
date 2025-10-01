using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrumOps.Application.ProductBacklog.Queries;
using ScrumOps.Domain.ProductBacklog.Repositories;

namespace ScrumOps.Application.ProductBacklog.Handlers.QueryHandlers;

/// <summary>
/// Handler for getting a product backlog by team ID with filtering.
/// </summary>
public class GetProductBacklogQueryHandler : IRequestHandler<GetProductBacklogQuery, GetBacklogResponse?>
{
    private readonly IProductBacklogRepository _backlogRepository;

    public GetProductBacklogQueryHandler(IProductBacklogRepository backlogRepository)
    {
        _backlogRepository = backlogRepository;
    }

    public async Task<GetBacklogResponse?> Handle(GetProductBacklogQuery request, CancellationToken cancellationToken)
    {
        var productBacklog = await _backlogRepository.GetByTeamIdAsync(request.TeamId, cancellationToken);
        
        if (productBacklog == null)
        {
            return null;
        }

        // Apply filtering if needed
        var filteredItems = productBacklog.Items.AsQueryable();
        
        if (!string.IsNullOrEmpty(request.Status))
        {
            filteredItems = filteredItems.Where(item => item.Status.ToString().Equals(request.Status, StringComparison.OrdinalIgnoreCase));
        }
        
        if (!string.IsNullOrEmpty(request.Type))
        {
            filteredItems = filteredItems.Where(item => item.Type.ToString().Equals(request.Type, StringComparison.OrdinalIgnoreCase));
        }

        var totalCount = filteredItems.Count();
        var pagedItems = filteredItems.Skip(request.Offset).Take(request.Limit).ToList();

        return new GetBacklogResponse
        {
            Backlog = new ProductBacklogDto
            {
                Id = productBacklog.Id.Value.GetHashCode(), // Convert Guid to int for now
                TeamId = productBacklog.TeamId.Value.GetHashCode(), // Convert Guid to int for now
                LastRefinedDate = productBacklog.LastRefinedDate
            },
            Items = pagedItems.Select(item => new BacklogItemDto
            {
                Id = item.Id.Value.GetHashCode(), // Convert Guid to int for now
                Title = item.Title.Value,
                Description = item.Description.Value,
                AcceptanceCriteria = item.AcceptanceCriteria?.Value ?? string.Empty,
                Priority = item.Priority.Value,
                StoryPoints = item.StoryPoints?.Value,
                Status = item.Status.ToString(),
                Type = item.Type.ToString(),
                CreatedBy = item.CreatedBy.Value,
                CreatedDate = item.CreatedDate,
                IsInCurrentSprint = false, // TODO: Implement sprint check
                SprintId = null // TODO: Implement sprint check
            }).ToList(),
            TotalCount = totalCount,
            HasNext = request.Offset + request.Limit < totalCount
        };
    }
}

/// <summary>
/// Handler for getting a product backlog by its ID.
/// </summary>
public class GetProductBacklogByIdQueryHandler : IRequestHandler<GetProductBacklogByIdQuery, ProductBacklogDto?>
{
    private readonly IProductBacklogRepository _backlogRepository;

    public GetProductBacklogByIdQueryHandler(IProductBacklogRepository backlogRepository)
    {
        _backlogRepository = backlogRepository;
    }

    public async Task<ProductBacklogDto?> Handle(GetProductBacklogByIdQuery request, CancellationToken cancellationToken)
    {
        var productBacklog = await _backlogRepository.GetByIdAsync(request.BacklogId, cancellationToken);
        
        if (productBacklog == null)
        {
            return null;
        }

        return new ProductBacklogDto
        {
            Id = productBacklog.Id.Value.GetHashCode(), // Convert Guid to int for now
            TeamId = productBacklog.TeamId.Value.GetHashCode(), // Convert Guid to int for now
            LastRefinedDate = productBacklog.LastRefinedDate
        };
    }
}