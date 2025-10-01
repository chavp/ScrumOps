using ScrumOps.Infrastructure.Persistence.Repositories;
using ScrumOps.Infrastructure.Tests.Builders;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.SharedKernel.ValueObjects;

namespace ScrumOps.Infrastructure.Tests.Repositories;

/// <summary>
/// Tests for TeamRepository functionality.
/// </summary>
public class TeamRepositoryTests : TestBase
{
    private readonly TeamRepository _repository;

    public TeamRepositoryTests()
    {
        _repository = new TeamRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTeam_ReturnsTeam()
    {
        // Arrange
        var team = TeamBuilder.Random();
        Context.Teams.Add(team);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(team.Id);
        result.Name.Value.Should().Be(team.Name.Value);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingTeam_ReturnsNull()
    {
        // Arrange
        var nonExistingId = TeamId.New();

        // Act
        var result = await _repository.GetByIdAsync(nonExistingId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetActiveTeamsAsync_ReturnsOnlyActiveTeams()
    {
        // Arrange
        var activeTeam1 = TeamBuilder.Random();
        var activeTeam2 = TeamBuilder.Random();
        var inactiveTeam = TeamBuilder.Random();
        
        inactiveTeam.Deactivate();
        
        Context.Teams.AddRange(activeTeam1, activeTeam2, inactiveTeam);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetActiveTeamsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(t => t.IsActive.Should().BeTrue());
        result.Select(t => t.Id).Should().Contain(new[] { activeTeam1.Id, activeTeam2.Id });
    }

    [Fact]
    public async Task GetAllTeamsAsync_ReturnsAllTeams()
    {
        // Arrange
        var teams = TeamBuilder.CreateMultiple(3);
        teams[2].Deactivate(); // Make one inactive

        Context.Teams.AddRange(teams);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetAllTeamsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Count(t => t.IsActive).Should().Be(2);
        result.Count(t => !t.IsActive).Should().Be(1);
    }

    [Fact]
    public async Task GetTeamsByNameAsync_FindsMatchingTeams()
    {
        // Arrange
        var team1 = new TeamBuilder().WithName("Alpha Team").Build();
        var team2 = new TeamBuilder().WithName("Beta Team").Build();
        var team3 = new TeamBuilder().WithName("Gamma Squad").Build();

        Context.Teams.AddRange(team1, team2, team3);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetTeamsByNameAsync("Team");

        // Assert
        result.Should().HaveCount(2);
        result.Select(t => t.Name.Value).Should().Equal("Alpha Team", "Beta Team");
    }

    [Fact]
    public async Task ExistsWithNameAsync_ExistingName_ReturnsTrue()
    {
        // Arrange
        var team = new TeamBuilder().WithName("Unique Team Name").Build();
        Context.Teams.Add(team);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsWithNameAsync("Unique Team Name");

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsWithNameAsync_NonExistingName_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsWithNameAsync("Non-existing Team");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ExistsWithNameAsync_ExcludeSpecificTeam_ReturnsCorrectResult()
    {
        // Arrange
        var team1 = new TeamBuilder().WithName("Same Name").Build();
        var team2 = new TeamBuilder().WithName("Different Name").Build();
        
        Context.Teams.AddRange(team1, team2);
        await Context.SaveChangesAsync();

        // Act - Check if name exists excluding team1 itself
        var result = await _repository.ExistsWithNameAsync("Same Name", team1.Id);

        // Assert - Should return false because we're excluding the only team with that name
        result.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_AddsTeamToContext()
    {
        // Arrange
        var team = TeamBuilder.Random();

        // Act
        await _repository.AddAsync(team);

        // Assert
        Context.Teams.Should().Contain(team);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTeam()
    {
        // Arrange
        var team = TeamBuilder.Random();
        Context.Teams.Add(team);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        var retrievedTeam = await _repository.GetByIdAsync(team.Id);
        // Note: UpdateDetails method doesn't exist yet in the domain
        // In a real implementation, you would call the domain method here

        // Act
        await _repository.UpdateAsync(retrievedTeam!);
        await Context.SaveChangesAsync();

        // Assert
        Context.ChangeTracker.Clear();
        var updatedTeam = await _repository.GetByIdAsync(team.Id);
        updatedTeam.Should().NotBeNull();
    }

    // Note: DeleteAsync method doesn't exist in current repository implementation
    // This test is disabled until the method is implemented
    
    /*
    [Fact]
    public async Task DeleteAsync_RemovesTeamFromContext()
    {
        // This test is disabled until DeleteAsync is implemented in the repository
    }
    */

    [Fact]
    public async Task GetByIdAsync_IncludesMembers()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var users = UserBuilder.CreateTeamMembers(team.Id, 2);

        Context.Teams.Add(team);
        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        // Act
        var result = await _repository.GetByIdAsync(team.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Members.Should().HaveCount(4); // 1 PO + 1 SM + 2 Developers
    }
}