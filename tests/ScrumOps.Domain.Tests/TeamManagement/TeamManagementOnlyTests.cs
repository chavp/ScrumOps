using ScrumOps.Domain.SharedKernel.Exceptions;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.TeamManagement.ValueObjects;
using ScrumOps.Domain.TeamManagement.Events;

namespace ScrumOps.Domain.Tests.TeamManagement;

/// <summary>
/// Isolated tests for Team Management bounded context only.
/// These tests verify that our Team Management implementation works correctly.
/// </summary>
public class TeamManagementOnlyTests
{
    [Fact]
    public void TeamName_Create_WithValidName_ShouldSucceed()
    {
        // Arrange
        var name = "Alpha Team";

        // Act
        var teamName = TeamName.Create(name);

        // Assert
        Assert.Equal("Alpha Team", teamName.Value);
    }

    [Fact]
    public void ScrumRole_ProductOwner_ShouldBeSingletonRole()
    {
        // Arrange
        var productOwner = ScrumRole.ProductOwner;

        // Act & Assert
        Assert.True(productOwner.IsSingletonRole());
        Assert.Equal("Product Owner", productOwner.Name);
    }

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
    public void Team_Deactivate_ShouldPreventAddingMembers()
    {
        // Arrange
        var team = CreateValidTeam();
        team.Deactivate("Testing deactivation");
        var user = CreateValidUser(team.Id, ScrumRole.Developer);

        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => team.AddMember(user));
        Assert.Contains("inactive team", exception.Message);
    }

    [Fact]
    public void Velocity_Create_WithValidValue_ShouldSucceed()
    {
        // Arrange & Act
        var velocity = Velocity.Create(25.5m);

        // Assert
        Assert.Equal(25.5m, velocity.Value);
    }

    [Fact]
    public void Velocity_Create_WithNegativeValue_ShouldThrowDomainException()
    {
        // Act & Assert
        var exception = Assert.Throws<DomainException>(() => Velocity.Create(-1));
        Assert.Contains("cannot be negative", exception.Message);
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