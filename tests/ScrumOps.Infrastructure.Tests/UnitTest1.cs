using ScrumOps.Infrastructure.Tests.Builders;

namespace ScrumOps.Infrastructure.Tests;

/// <summary>
/// Smoke tests to verify basic infrastructure functionality.
/// </summary>
public class InfrastructureSmokeTests : TestBase
{
    [Fact]
    public void CanCreateTestBuilders()
    {
        // Act
        var team = TeamBuilder.Random();
        var user = UserBuilder.Random();
        var backlog = ProductBacklogBuilder.Random();
        var item = ProductBacklogItemBuilder.Random();

        // Assert
        team.Should().NotBeNull();
        team.Name.Value.Should().NotBeNullOrWhiteSpace();
        
        user.Should().NotBeNull();
        user.Email.Value.Should().NotBeNullOrWhiteSpace();
        
        backlog.Should().NotBeNull();
        backlog.Id.Value.Should().NotBe(Guid.Empty);
        
        item.Should().NotBeNull();
        item.Title.Value.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void DbContextCanBeCreated()
    {
        // Act & Assert
        Context.Should().NotBeNull();
        Context.Teams.Should().NotBeNull();
        Context.Users.Should().NotBeNull();
        Context.ProductBacklogs.Should().NotBeNull();
        Context.ProductBacklogItems.Should().NotBeNull();
    }

    [Fact]
    public async Task CanSaveBasicEntity()
    {
        // Arrange
        var team = TeamBuilder.Random();

        // Act
        Context.Teams.Add(team);
        var result = await Context.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0); // At least 1 entity saved (may include related value objects)
    }
}
