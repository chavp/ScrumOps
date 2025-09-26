using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrumOps.Application.ProductBacklog.Queries;
using ScrumOps.Domain.ProductBacklog.Repositories;

namespace ScrumOps.Application.ProductBacklog.Handlers.QueryHandlers;

/// <summary>
/// Handler for getting a product backlog by team ID.
/// </summary>
public class GetProductBacklogQueryHandler : IRequestHandler<GetProductBacklogQuery, ProductBacklogDto?>
{
    private readonly IProductBacklogRepository _backlogRepository;

    public GetProductBacklogQueryHandler(IProductBacklogRepository backlogRepository)
    {
        _backlogRepository = backlogRepository;
    }

    public async Task<ProductBacklogDto?> Handle(GetProductBacklogQuery request, CancellationToken cancellationToken)
    {
        var productBacklog = await _backlogRepository.GetByTeamIdAsync(request.TeamId, cancellationToken);
        
        if (productBacklog == null)
        {
            return null;
        }

        return new ProductBacklogDto
        {
            Id = productBacklog.Id.Value.ToString(),
            TeamId = productBacklog.TeamId.Value.ToString(),
            CreatedDate = productBacklog.CreatedDate,
            LastRefinedDate = productBacklog.LastRefinedDate,
            Notes = productBacklog.Notes.Value,
            Items = productBacklog.Items.Select(item => new ProductBacklogItemDto
            {
                Id = item.Id.Value.ToString(),
                Title = item.Title.Value,
                Description = item.Description.Value,
                AcceptanceCriteria = item.AcceptanceCriteria?.Value ?? string.Empty,
                Priority = item.Priority.Value,
                StoryPoints = item.StoryPoints?.Value,
                Status = item.Status.ToString(),
                Type = item.Type.ToString(),
                CreatedBy = item.CreatedBy.Value,
                CreatedDate = item.CreatedDate
            }).ToList()
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
            Id = productBacklog.Id.Value.ToString(),
            TeamId = productBacklog.TeamId.Value.ToString(),
            CreatedDate = productBacklog.CreatedDate,
            LastRefinedDate = productBacklog.LastRefinedDate,
            Notes = productBacklog.Notes.Value,
            Items = productBacklog.Items.Select(item => new ProductBacklogItemDto
            {
                Id = item.Id.Value.ToString(),
                Title = item.Title.Value,
                Description = item.Description.Value,
                AcceptanceCriteria = item.AcceptanceCriteria?.Value ?? string.Empty,
                Priority = item.Priority.Value,
                StoryPoints = item.StoryPoints?.Value,
                Status = item.Status.ToString(),
                Type = item.Type.ToString(),
                CreatedBy = item.CreatedBy.Value,
                CreatedDate = item.CreatedDate
            }).ToList()
        };
    }
}