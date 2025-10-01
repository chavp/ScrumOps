using Microsoft.EntityFrameworkCore;
using ScrumOps.Infrastructure.Persistence;
using ScrumOps.Infrastructure.Tests.Builders;
using ScrumOps.Domain.SharedKernel.ValueObjects;
using Testcontainers.PostgreSql;

namespace ScrumOps.Infrastructure.Tests.Integration;

/// <summary>
/// Integration tests using real PostgreSQL database with Testcontainers.
/// </summary>
public class PostgreSqlIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private ScrumOpsDbContext _context = null!;

    public PostgreSqlIntegrationTests()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("scrumops_test")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
        
        var connectionString = _postgresContainer.GetConnectionString();
        var options = new DbContextOptionsBuilder<ScrumOpsDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        _context = new ScrumOpsDbContext(options);
        await _context.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanCreateDatabaseSchema()
    {
        // Act
        var schemas = await _context.Database.SqlQueryRaw<string>(
            "SELECT schema_name FROM information_schema.schemata WHERE schema_name IN ('TeamManagement', 'ProductBacklog', 'SprintManagement')")
            .ToListAsync();

        // Assert
        schemas.Should().Contain("TeamManagement");
        schemas.Should().Contain("ProductBacklog");
        schemas.Should().Contain("SprintManagement");
    }

    [Fact]
    public async Task CanCreateAndQueryTeams()
    {
        // Arrange
        var teams = TeamBuilder.CreateMultiple(3);

        // Act
        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();
        var retrievedTeams = await _context.Teams.ToListAsync();

        // Assert
        retrievedTeams.Should().HaveCount(3);
        retrievedTeams.Should().AllSatisfy(t => t.Id.Should().NotBe(Guid.Empty));
    }

    [Fact]
    public async Task CanCreateComplexEntityGraph()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var users = UserBuilder.CreateTeamMembers(team.Id, 2);
        var backlog = ProductBacklogBuilder.Random(team.Id);
        var items = ProductBacklogItemBuilder.CreateMultiple(backlog.Id, 5);

        // Act
        _context.Teams.Add(team);
        _context.Users.AddRange(users);
        _context.ProductBacklogs.Add(backlog);
        _context.ProductBacklogItems.AddRange(items);

        await _context.SaveChangesAsync();

        // Assert
        _context.ChangeTracker.Clear();

        var retrievedTeam = await _context.Teams
            .Include(t => t.Members)
            .FirstAsync(t => t.Id == team.Id);

        var retrievedBacklog = await _context.ProductBacklogs
            .Include(pb => pb.Items)
            .FirstAsync(pb => pb.Id == backlog.Id);

        retrievedTeam.Members.Should().HaveCount(4); // 1 PO + 1 SM + 2 Developers
        retrievedBacklog.Items.Should().HaveCount(5);
    }

    [Fact]
    public async Task UniqueConstraintsAreEnforced()
    {
        // Arrange
        var team1 = new TeamBuilder().WithName("Unique Team").Build();
        var team2 = new TeamBuilder().WithName("Unique Team").Build();

        // Act & Assert
        _context.Teams.Add(team1);
        await _context.SaveChangesAsync();

        _context.Teams.Add(team2);
        var act = async () => await _context.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task ForeignKeyConstraintsAreEnforced()
    {
        // Arrange
        var nonExistentTeamId = TeamId.New();
        var user = UserBuilder.Random(nonExistentTeamId);

        // Act & Assert
        _context.Users.Add(user);
        var act = async () => await _context.SaveChangesAsync();

        await act.Should().ThrowAsync<DbUpdateException>();
    }

    [Fact]
    public async Task CascadeDeleteWorks()
    {
        // Arrange
        var team = TeamBuilder.Random();
        var backlog = ProductBacklogBuilder.Random(team.Id);
        var items = ProductBacklogItemBuilder.CreateMultiple(backlog.Id, 3);

        _context.Teams.Add(team);
        _context.ProductBacklogs.Add(backlog);
        _context.ProductBacklogItems.AddRange(items);
        await _context.SaveChangesAsync();

        var itemCount = await _context.ProductBacklogItems.CountAsync();
        itemCount.Should().Be(3);

        // Act
        _context.ProductBacklogs.Remove(backlog);
        await _context.SaveChangesAsync();

        // Assert
        var remainingItems = await _context.ProductBacklogItems.CountAsync();
        remainingItems.Should().Be(0);
    }

    [Fact]
    public async Task IndexesImproveQueryPerformance()
    {
        // Arrange - Create many teams to test index performance
        var teams = TeamBuilder.CreateMultiple(1000);

        _context.Teams.AddRange(teams);
        await _context.SaveChangesAsync();

        // Act - Query that should use the Name index
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = await _context.Teams
            .Where(t => t.Name.Value == "Team 500")
            .FirstOrDefaultAsync();
        stopwatch.Stop();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Value.Should().Be("Team 500");
        
        // Performance should be good with index (this is more of a sanity check)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(100);
    }

    [Fact]
    public async Task ValueObjectMappingWorksCorrectly()
    {
        // Arrange
        var team = new TeamBuilder()
            .WithName("Integration Test Team")
            .WithDescription("This is a detailed description for integration testing")
            .WithSprintLength(3)
            .Build();

        // Act
        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Query with raw SQL to verify database structure
        var result = await _context.Database.SqlQueryRaw<TeamDbRecord>(
            """
            SELECT "Id", "Name", "Description", "SprintLengthWeeks", "CurrentVelocityValue", "IsActive"
            FROM "TeamManagement"."Teams" 
            WHERE "Id" = {0}
            """,
            team.Id.Value).FirstOrDefaultAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Integration Test Team");
        result.Description.Should().Be("This is a detailed description for integration testing");
        result.SprintLengthWeeks.Should().Be(3);
        result.CurrentVelocityValue.Should().Be(0);
        result.IsActive.Should().BeTrue();
    }

    private sealed record TeamDbRecord(Guid Id, string Name, string Description, int SprintLengthWeeks, decimal CurrentVelocityValue, bool IsActive);
}