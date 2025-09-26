using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ScrumOps.Application.Common.Interfaces;
using ScrumOps.Application.TeamManagement.Commands;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.Repositories;
using TeamName = ScrumOps.Domain.TeamManagement.ValueObjects.TeamName;
using TeamDescription = ScrumOps.Domain.TeamManagement.ValueObjects.TeamDescription;
using SprintLength = ScrumOps.Domain.TeamManagement.ValueObjects.SprintLength;
using UserName = ScrumOps.Domain.TeamManagement.ValueObjects.UserName;
using ScrumRole = ScrumOps.Domain.TeamManagement.ValueObjects.ScrumRole;

namespace ScrumOps.Application.TeamManagement.Handlers.CommandHandlers;

/// <summary>
/// Handler for creating a new team.
/// </summary>
public class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand, TeamId>
{
    private readonly ITeamRepository _teamRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTeamCommandHandler(ITeamRepository teamRepository, IUnitOfWork unitOfWork)
    {
        _teamRepository = teamRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TeamId> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
    {
        // Create value objects
        var teamName = TeamName.Create(request.Name);
        var description = !string.IsNullOrEmpty(request.Description) 
            ? TeamDescription.Create(request.Description) 
            : TeamDescription.Create(""); // Default empty description if null
        var sprintLength = SprintLength.Create(request.SprintLengthWeeks);
        
        // Create team ID
        var teamId = TeamId.New();
        
        // Create the team using constructor
        var team = new Team(teamId, teamName, description, sprintLength);
        
        // Add Product Owner and Scrum Master (simplified - in real app would get users from repository)
        var productOwnerEmail = Email.Create(request.ProductOwnerEmail);
        var scrumMasterEmail = Email.Create(request.ScrumMasterEmail);
        
        var productOwner = new User(
            UserId.New(),
            teamId,
            UserName.Create("Product Owner"), 
            productOwnerEmail, 
            ScrumRole.ProductOwner
        );
        
        var scrumMaster = new User(
            UserId.New(),
            teamId,
            UserName.Create("Scrum Master"), 
            scrumMasterEmail, 
            ScrumRole.ScrumMaster
        );
        
        team.AddMember(productOwner);
        team.AddMember(scrumMaster);
        
        // Save to repository
        await _teamRepository.AddAsync(team, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return team.Id;
    }
}