using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Application.ProductBacklog.Commands;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using ItemTitle = ScrumOps.Domain.ProductBacklog.ValueObjects.ItemTitle;
using ItemDescription = ScrumOps.Domain.ProductBacklog.ValueObjects.ItemDescription;
using AcceptanceCriteria = ScrumOps.Domain.ProductBacklog.ValueObjects.AcceptanceCriteria;
using Priority = ScrumOps.Domain.ProductBacklog.ValueObjects.Priority;
using StoryPoints = ScrumOps.Domain.ProductBacklog.ValueObjects.StoryPoints;
using BacklogItemType = ScrumOps.Domain.ProductBacklog.ValueObjects.BacklogItemType;
using UserName = ScrumOps.Domain.ProductBacklog.ValueObjects.UserName;

namespace ScrumOps.Application.ProductBacklog.Handlers.CommandHandlers;

/// <summary>
/// Handler for adding a new item to a product backlog.
/// </summary>
public class AddBacklogItemCommandHandler : IRequestHandler<AddBacklogItemCommand, ProductBacklogItemId>
{
    private readonly IProductBacklogRepository _backlogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddBacklogItemCommandHandler(IProductBacklogRepository backlogRepository, IUnitOfWork unitOfWork)
    {
        _backlogRepository = backlogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductBacklogItemId> Handle(AddBacklogItemCommand request, CancellationToken cancellationToken)
    {
        // Get the product backlog
        var productBacklog = await _backlogRepository.GetByIdAsync(request.BacklogId, cancellationToken);
        if (productBacklog == null)
        {
            throw new InvalidOperationException($"Product backlog with ID {request.BacklogId} not found");
        }

        // Create value objects
        var title = ItemTitle.Create(request.Title);
        var description = ItemDescription.Create(request.Description);
        var acceptanceCriteria = AcceptanceCriteria.Create(request.AcceptanceCriteria);
        var priority = Priority.Create(request.Priority);
        var storyPoints = request.StoryPoints.HasValue ? StoryPoints.Create(request.StoryPoints.Value) : null;
        var itemType = Enum.Parse<BacklogItemType>(request.BacklogItemType);
        var createdBy = UserName.Create("System"); // TODO: Get from current user context

        // Create the backlog item
        var itemId = ProductBacklogItemId.New();
        var backlogItem = new ProductBacklogItem(
            itemId,
            productBacklog.Id,
            title,
            description,
            itemType,
            createdBy);

        // Set additional properties if provided
        if (storyPoints != null)
            backlogItem.EstimateStoryPoints(storyPoints);
        
        if (!string.IsNullOrEmpty(request.AcceptanceCriteria))
            backlogItem.SetAcceptanceCriteria(acceptanceCriteria);

        // Add item to backlog
        productBacklog.AddItem(backlogItem);

        // Save changes
        await _backlogRepository.UpdateAsync(productBacklog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return itemId;
    }
}