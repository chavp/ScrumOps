using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Application.ProductBacklog.Commands;
using ScrumOps.Domain.ProductBacklog.Entities;
using ScrumOps.Domain.ProductBacklog.Repositories;
using ScrumOps.Domain.ProductBacklog.ValueObjects;
using BacklogNotes = ScrumOps.Domain.ProductBacklog.ValueObjects.BacklogNotes;
using ProductBacklogEntity = ScrumOps.Domain.ProductBacklog.Entities.ProductBacklog;

namespace ScrumOps.Application.ProductBacklog.Handlers.CommandHandlers;

/// <summary>
/// Handler for creating a new product backlog.
/// </summary>
public class CreateProductBacklogCommandHandler : IRequestHandler<CreateProductBacklogCommand, ProductBacklogId>
{
    private readonly IProductBacklogRepository _backlogRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProductBacklogCommandHandler(IProductBacklogRepository backlogRepository, IUnitOfWork unitOfWork)
    {
        _backlogRepository = backlogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProductBacklogId> Handle(CreateProductBacklogCommand request, CancellationToken cancellationToken)
    {
        // Check if team already has a product backlog
        var existingBacklog = await _backlogRepository.GetByTeamIdAsync(request.TeamId, cancellationToken);
        if (existingBacklog != null)
        {
            return existingBacklog.Id;
        }

        // Create new product backlog
        var backlogId = ProductBacklogId.New();
        var notes = !string.IsNullOrEmpty(request.Notes) 
            ? BacklogNotes.Create(request.Notes) 
            : BacklogNotes.Create("");

        var productBacklog = new ProductBacklogEntity(backlogId, request.TeamId);
        
        // Set notes if provided
        if (!string.IsNullOrEmpty(request.Notes))
        {
            productBacklog.MarkAsRefined(DateTime.UtcNow, notes);
        }

        // Save to repository
        await _backlogRepository.AddAsync(productBacklog, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return productBacklog.Id;
    }
}