using Microsoft.EntityFrameworkCore;
using ScrumOps.Infrastructure.Tests.Builders;
using ScrumOps.Domain.TeamManagement.Entities;
using ScrumOps.Domain.ProductBacklog.Entities;

namespace ScrumOps.Infrastructure.Tests.Persistence;

/// <summary>
/// Tests for ScrumOpsDbContext entity mappings and configurations.
/// </summary>
public class ScrumOpsDbContextTests : TestBase
{
    [Fact]
    public async Task CanSaveAndRetrieveTeam()
    {
        // Arrange
        var team = TeamBuilder.Random();

        // Act
        Context.Teams.Add(team);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var retrievedTeam = await Context.Teams.FirstOrDefaultAsync(t => t.Id == team.Id);

        // Assert
        retrievedTeam.Should().NotBeNull();
        retrievedTeam!.Id.Should().Be(team.Id);
        retrievedTeam.Name.Value.Should().Be(team.Name.Value);
        retrievedTeam.Description.Value.Should().Be(team.Description.Value);
        retrievedTeam.SprintLength.Weeks.Should().Be(team.SprintLength.Weeks);
        retrievedTeam.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CanSaveAndRetrieveUser()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var user = UserBuilder.Random(team.Id);

        // Act
        Context.Teams.Add(team);
        Context.Users.Add(user);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var retrievedUser = await Context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);

        // Assert
        retrievedUser.Should().NotBeNull();
        retrievedUser!.Id.Should().Be(user.Id);
        retrievedUser.TeamId.Should().Be(user.TeamId);
        retrievedUser.Name.Value.Should().Be(user.Name.Value);
        retrievedUser.Email.Value.Should().Be(user.Email.Value);
        retrievedUser.Role.Name.Should().Be(user.Role.Name);
        retrievedUser.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CanSaveAndRetrieveProductBacklog()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var productBacklog = ProductBacklogBuilder.Random(team.Id);

        // Act
        Context.Teams.Add(team);
        Context.ProductBacklogs.Add(productBacklog);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var retrievedBacklog = await Context.ProductBacklogs.FirstOrDefaultAsync(pb => pb.Id == productBacklog.Id);

        // Assert
        retrievedBacklog.Should().NotBeNull();
        retrievedBacklog!.Id.Should().Be(productBacklog.Id);
        retrievedBacklog.TeamId.Should().Be(productBacklog.TeamId);
        retrievedBacklog.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    [Fact]
    public async Task CanSaveAndRetrieveProductBacklogItem()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var productBacklog = ProductBacklogBuilder.Random(team.Id);
        var item = ProductBacklogItemBuilder.Random(productBacklog.Id);

        // Act
        Context.Teams.Add(team);
        Context.ProductBacklogs.Add(productBacklog);
        Context.ProductBacklogItems.Add(item);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();
        var retrievedItem = await Context.ProductBacklogItems
            .FirstOrDefaultAsync(pbi => pbi.Id == item.Id);

        // Assert
        retrievedItem.Should().NotBeNull();
        retrievedItem!.Id.Should().Be(item.Id);
        retrievedItem.ProductBacklogId.Should().Be(item.ProductBacklogId);
        retrievedItem.Title.Value.Should().Be(item.Title.Value);
        retrievedItem.Description.Value.Should().Be(item.Description.Value);
        retrievedItem.Type.Should().Be(item.Type);
        retrievedItem.Status.Should().Be(item.Status);
    }

    [Fact]
    public async Task TeamNameUniqueConstraintIsEnforced()
    {
        // Arrange
        var team1 = new TeamBuilder().WithName("Unique Team").Build();
        var team2 = new TeamBuilder().WithName("Unique Team").Build();

        // Act & Assert
        Context.Teams.Add(team1);
        Context.Teams.Add(team2);

        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UserEmailUniqueConstraintIsEnforced()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var user1 = new UserBuilder().WithTeamId(team.Id).WithEmail("same@example.com").Build();
        var user2 = new UserBuilder().WithTeamId(team.Id).WithEmail("same@example.com").Build();

        // Act & Assert
        Context.Teams.Add(team);
        Context.Users.Add(user1);
        Context.Users.Add(user2);

        var act = async () => await Context.SaveChangesAsync();
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task CanQueryTeamsWithMembers()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var users = UserBuilder.CreateTeamMembers(team.Id, 3);

        Context.Teams.Add(team);
        Context.Users.AddRange(users);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        // Act
        var teamWithMembers = await Context.Teams
            .Include(t => t.Members)
            .FirstOrDefaultAsync(t => t.Id == team.Id);

        // Assert
        teamWithMembers.Should().NotBeNull();
        teamWithMembers!.Members.Should().HaveCount(5); // 1 PO + 1 SM + 3 Developers
        teamWithMembers.Members.Should().Contain(u => u.Role.Name == "Product Owner");
        teamWithMembers.Members.Should().Contain(u => u.Role.Name == "Scrum Master");
        teamWithMembers.Members.Count(u => u.Role.Name == "Developer").Should().Be(3);
    }

    [Fact]
    public async Task CanQueryProductBacklogWithItems()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var productBacklog = ProductBacklogBuilder.Random(team.Id);
        var items = ProductBacklogItemBuilder.CreateMultiple(productBacklog.Id, 5);

        Context.Teams.Add(team);
        Context.ProductBacklogs.Add(productBacklog);
        Context.ProductBacklogItems.AddRange(items);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        // Act
        var backlogWithItems = await Context.ProductBacklogs
            .Include(pb => pb.Items)
            .FirstOrDefaultAsync(pb => pb.Id == productBacklog.Id);

        // Assert
        backlogWithItems.Should().NotBeNull();
        backlogWithItems!.Items.Should().HaveCount(5);
        backlogWithItems.Items.Should().AllSatisfy(item => item.ProductBacklogId.Should().Be(productBacklog.Id));
    }

    [Fact]
    public async Task ValueObjectsAreMappedCorrectly()
    {
        // Arrange
        var team = new TeamBuilder()
            .WithName("Test Team Name")
            .WithDescription("Test Team Description")
            .WithSprintLength(3)
            .Build();

        // Act
        Context.Teams.Add(team);
        await Context.SaveChangesAsync();

        Context.ChangeTracker.Clear();

        // Use raw SQL to verify the database structure
        var result = await Context.Database.SqlQueryRaw<TeamDbRecord>(
            "SELECT \"Id\", \"Name\", \"Description\", \"SprintLengthWeeks\", \"CurrentVelocityValue\" FROM \"TeamManagement\".\"Teams\" WHERE \"Id\" = {0}",
            team.Id.Value).FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Test Team Name");
        result.Description.Should().Be("Test Team Description");
        result.SprintLengthWeeks.Should().Be(3);
        result.CurrentVelocityValue.Should().Be(0);
    }

    [Fact]
    public async Task DatabaseSchemasAreCreatedCorrectly()
    {
        // Arrange & Act
        var schemas = await Context.Database.SqlQueryRaw<string>(
            "SELECT schema_name FROM information_schema.schemata WHERE schema_name IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')")
            .ToListAsync();

        // Assert
        schemas.Should().Contain("TeamManagement");
        schemas.Should().Contain("ProductBacklog");
        schemas.Should().Contain("SprintManagement");
    }

    private sealed record TeamDbRecord(Guid Id, string Name, string Description, int SprintLengthWeeks, decimal CurrentVelocityValue);
}