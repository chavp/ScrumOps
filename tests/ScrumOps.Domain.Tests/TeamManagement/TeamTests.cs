using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using ScrumOps.Domain.TeamManagement.Events;

namespace ScrumOps.Domain.Tests.TeamManagement;

/// <summary>
/// Domain tests for Team aggregate root.
/// These tests MUST FAIL initially to follow TDD principles.
/// </summary>
public class TeamTests
{
    [Fact]
    public void Team_Create_ShouldRaiseTeamCreatedEvent()
    {
        // Arrange
        var teamId = TeamId.New();
        var teamName = TeamName.Create("Alpha Team");
        var description = TeamDescription.Create("Mobile development team");
        var sprintLength = SprintLength.Create(2);

        // Act
        var team = new Team(teamId, teamName, description, sprintLength);

        // Assert
        Assert.Single(team.DomainEvents);
        Assert.IsType<TeamCreatedEvent>(team.DomainEvents.First());
        
        var teamCreatedEvent = (TeamCreatedEvent)team.DomainEvents.First();
        Assert.Equal(teamId, teamCreatedEvent.TeamId);
        Assert.Equal("Alpha Team", teamCreatedEvent.TeamName);
    }

    [Fact]
    public void Team_AddMember_ShouldAddMemberSuccessfully()
    {
        // Arrange
        var team = CreateValidTeam();
        var user = CreateValidUser(team.Id, ScrumRole.Developer);

        // Act
        team.AddMember(user);

        // Assert
        Assert.Single(team.Members);
        Assert.Contains(user, team.Members);
        Assert.Contains(team.DomainEvents, e => e is MemberAddedToTeamEvent);
    }

    [Fact]
    public void Team_AddMember_WithDuplicateEmail_ShouldThrowDomainException()
    {
        // Arrange
        var team = CreateValidTeam();
        var email = Email.Create("john.doe@example.com");
        var user1 = CreateValidUser(team.Id, ScrumRole.Developer, email);
        var user2 = CreateValidUser(team.Id, ScrumRole.Developer, email);
        
        team.AddMember(user1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => team.AddMember(user2));
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void Team_AddMember_WithDuplicateSingletonRole_ShouldThrowDomainException()
    {
        // Arrange
        var team = CreateValidTeam();
        var productOwner1 = CreateValidUser(team.Id, ScrumRole.ProductOwner);
        var productOwner2 = CreateValidUser(team.Id, ScrumRole.ProductOwner, Email.Create("po2@example.com"));
        
        team.AddMember(productOwner1);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => team.AddMember(productOwner2));
        Assert.Contains("already has a", exception.Message);
    }

    [Fact]
    public void Team_UpdateVelocity_ShouldRaiseVelocityUpdatedEvent()
    {
        // Arrange
        var team = CreateValidTeam();
        var newVelocity = Velocity.Create(25);

        // Act
        team.UpdateVelocity(newVelocity);

        // Assert
        Assert.Equal(newVelocity, team.CurrentVelocity);
        Assert.Contains(team.DomainEvents, e => e is TeamVelocityUpdatedEvent);
    }

    [Fact]
    public void Team_AddMember_ToInactiveTeam_ShouldThrowDomainException()
    {
        // Arrange
        var team = CreateValidTeam();
        team.Deactivate(); // This method doesn't exist yet - should fail
        var user = CreateValidUser(team.Id, ScrumRole.Developer);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => team.AddMember(user));
        Assert.Contains("inactive team", exception.Message);
    }

    private static Team CreateValidTeam()
    {
        return new Team(
            TeamId.New(),
            TeamName.Create("Test Team"),
            TeamDescription.Create("Test Description"), 
            SprintLength.Create(2)
        );
    }

    private static User CreateValidUser(TeamId teamId, ScrumRole role, Email? email = null)
    {
        return new User(
            UserId.New(),
            teamId,
            UserName.Create("John Doe"),
            email ?? Email.Create($"user{Guid.NewGuid()}@example.com"),
            role
        );
    }
}